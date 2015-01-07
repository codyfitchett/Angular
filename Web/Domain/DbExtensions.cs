using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using System.Data;
using RevHR.Web.DTO;

using RevHR.Web.Util;

namespace RevHR.Web.Domain
{
	public static class DbExtensions
	{
		public static QuestDto GetQuest(this IDbConnection db, long questId, int userId = 0)
		{
			var quest = db.GetByIdOrDefault<Quest>(questId);

			if (quest == null)
				return null;

			var fields = db.SelectParam<QuestField>(f => f.QuestId == quest.Id).Sort(f => f.SortOrder);
			var checkpoints = db.SelectParam<Checkpoint>(p => p.QuestId == quest.Id).Sort(c => c.SortOrder);
			var activities = db.SelectParam<QuestActivity>(a => a.QuestId == quest.Id);
			var allies = db.SelectParam<Ally>(a => a.QuestId == quest.Id && a.Status == AllyStatus.Accepted)
							.SortDescending(a => a.AcceptDate.Value);
			var checkins = activities.Where(a => a.Type == QuestActivityType.CheckIn).ToList().Sort(a => a.Date);

			int percentCompleted = checkpoints
									.TakeWhile(c => c.CompletionDate != null)
									.Aggregate(0, (total, checkpoint) => total + checkpoint.Percent);
			int daysSinceLastCheckin = -1;
			if (checkins.Count > 0)
				daysSinceLastCheckin = (int)(DateTime.UtcNow - checkins.Max(c => c.Date)).TotalDays;
			else if (quest.StartDate != null)
				daysSinceLastCheckin = (int)(DateTime.UtcNow - quest.StartDate.Value).TotalDays;

			var pointsWon = checkins.Aggregate(0, (points, checkin) =>
			{
				var checkpoint = checkpoints.FirstOrDefault(p => p.Id == checkin.CheckpointId);
				return checkpoint == null
					|| (checkpoint.NeedsVerification && checkin.CheckinVerifyDate == null) ? 0 : points + quest.CheckinPoints;
			});

			return new QuestDto()
			{
				Id = quest.Id,
				CategoryId = quest.CategoryId,
				Name = quest.Name,
				Description = quest.Description,
				ImageUrl = quest.ImageUrl,
				IsTemplate = quest.IsTemplate,
				Timeframe = quest.Timeframe,
				EstimatedDate = quest.EstimatedDate != null ? quest.EstimatedDate.Value.ToString("MM/dd/yyyy") : null,
				TotalPoints = quest.TotalPoints,

				Checkpoints = checkpoints.ToDto(),
				Fields = fields.ToDto(),

				DaysLeft = quest.DaysLeft(DateTime.UtcNow),
				PercentCompleted = percentCompleted,
				AssistCount = activities.Count(a => a.Type == QuestActivityType.Assist),
				FeedbackCount = activities.Count(a => a.Type == QuestActivityType.Feedback),
				PointsWon = pointsWon,
				DaysSinceLastCheckin = daysSinceLastCheckin,
				CheckinFrequency = quest.CheckinFrequency,
				CheckinsExpected = quest.CheckinsExpected,
				CheckinsActual = quest.GetCheckinCountForThisPeriod(checkins, verifiedOnly: false),
				Checkins = quest.GetCheckinStats(checkins, verifiedOnly: false),
			};
		}

		public static QuestDto SaveQuest(this IDbConnection db, QuestDto request, int userId = 0)
		{
			if (request.CheckinsExpected == 0)
				request.CheckinsExpected = 1;

			// TODO: auth & validation
			QuestDto success = request, failure = request.Clone();

			var quest = request.Id > 0 ? db.GetByIdOrDefault<Quest>(request.Id) : new Quest() { UserId = userId };

			if (quest == null)
				return failure; // TODO: populate error

			quest.PopulateWith(request);

			try
			{
				using (var trans = db.OpenTransaction())
				{
					try
					{
						bool isNewQuest = quest.Id <= 0;

						if (isNewQuest)
						{
							db.Insert(quest);
							quest.Id = db.GetLastInsertId();
						}
						else
						{
							db.Update(quest);
						}

						db.SaveFields(quest.Id, isNewQuest, request);
						db.SaveCheckpoints(quest.Id, isNewQuest, request);

						trans.Commit();

						return success;
					}
					catch (Exception x)
					{
						return failure; // TODO: populate error
					}
				}
			}
			catch (Exception x)
			{
				return failure; // TODO: populate error
			}
		}

		private static void SaveFields(this IDbConnection db, long questId, bool isNewQuest, QuestDto request)
		{
			var fields = new Dictionary<long, QuestField>();

			if (!isNewQuest)
			{
				fields = db.SelectParam<QuestField>(f => f.QuestId == request.Id).ToDictionary(f => f.Id);
				request.Fields.RemoveAll(f => f.Id != 0 && !fields.ContainsKey(f.Id));
			}
			else
			{
				// new quest, clear out any ids
				request.Fields.ForEach(f => f.Id = 0);
			}

			request.Fields.ForEachWithIndex((dto, sortOrder) =>
			{
				if (dto.Deleted)
				{
					if (fields.ContainsKey(dto.Id))
						db.Delete(fields[dto.Id]);
					return;
				}

				var field = dto.Id > 0 ? fields[dto.Id] : new QuestField() { QuestId = questId };

				field.PopulateWith(dto);
				field.SortOrder = sortOrder;

				if (field.Id == 0)
				{
					db.Insert(field);
					field.Id = dto.Id = db.GetLastInsertId();
				}
				else
				{
					db.Update(field);
				}
			});

			request.Fields.RemoveAll(f => f.Deleted);
		}

		private static void SaveCheckpoints(this IDbConnection db, long questId, bool isNewQuest, QuestDto request)
		{
			var checkpoints = new Dictionary<long, Checkpoint>();

			if (!isNewQuest)
			{
				checkpoints = db.SelectParam<Checkpoint>(p => p.QuestId == request.Id).ToDictionary(c => c.Id);
				request.Checkpoints.RemoveAll(c => c.Id != 0 && !checkpoints.ContainsKey(c.Id));
			}
			else
			{
				// new quest, clear out any ids
				request.Checkpoints.ForEach(c => c.Id = 0);
			}

			request.Checkpoints.ForEachWithIndex((dto, sortOrder) =>
			{
				if (dto.Deleted)
				{
					if (checkpoints.ContainsKey(dto.Id))
						db.Delete(checkpoints[dto.Id]);
					return;
				}

				var checkpoint = dto.Id > 0 ? checkpoints[dto.Id] : new Checkpoint() { QuestId = questId };

				checkpoint.PopulateWith(dto);
				checkpoint.SortOrder = sortOrder;

				if (checkpoint.Id == 0)
				{
					db.Insert(checkpoint);
					checkpoint.Id = dto.Id = db.GetLastInsertId();
				}
				else
				{
					db.Update(checkpoint);
				}
			});

			request.Checkpoints.RemoveAll(c => c.Deleted);
		}

		#region Seed Data
		public static void SeedData(this IDbConnection db, int testUserId)
		{
			using (var trans = db.OpenTransaction())
			{
				db.Insert(
					new QuestCategory() {
                        Name = "Work & Career",
                        IsTemplate = true,
                        SortOrder = 1,
                        Description = "Redefine Greatness at Work. Take Wins With You Wherever. Grow, Track & Share Your Wa",
                        ImageUrl = "./images/xhdpi/categories/workcareer.png"
                    },
                    new QuestCategory() {
                        Name = "Health & Fitness", 
                        IsTemplate = true, 
                        SortOrder = 2,
                        ImageUrl = "./images/xhdpi/categories/healthfitness.png" 
                    },
                    new QuestCategory() {
                        Name = "Personal Greatness", 
                        IsTemplate = true, 
                        SortOrder = 3,
                        ImageUrl = "./images/xhdpi/categories/personalgreateness.png" 
                    },
                    new QuestCategory() {
                        Name = "Just For Fun", 
                        IsTemplate = true, 
                        SortOrder = 4,
                        ImageUrl = "./images/xhdpi/categories/justforfun.png" 
                    },
                    new QuestCategory() {
                        Name = "Sponsored Quests", 
                        IsTemplate = true, 
                        SortOrder = 5,
                        ImageUrl = "./images/xhdpi/categories/sponsoredquest.png" 
                    },
					new QuestCategory() { 
                        Name = "Build Your Own", 
                        IsTemplate = true, 
                        SortOrder = 6,
                    }
				);

				var parent = db.First<QuestCategory>(c => c.Name == "Work & Career");

				db.Insert(
                    new QuestCategory()
                    {
                        ParentId = parent.Id,
                        Name = "Certifications",
                        IsTemplate = true,
                        SortOrder = 1,
                        Description = "Certification refers to the confirmation of certain characteristics of an object, person, or organization. This confirmation is often, but not always, provided by some form of external review, education, assessment, or audit",
                        ImageUrl = "./images/emblem_s.png"
                    },
					new QuestCategory() { 
                        ParentId = parent.Id, 
                        Name = "Presentations", 
                        IsTemplate = true, 
                        SortOrder = 2,
						Description = "Establish Yourself as An Expert. Certifications NOT Only Grow You. They Can Grow Your Career Too",
                        ImageUrl = "./images/emblem_s.png"
                    },
					new QuestCategory() { 
                        ParentId = parent.Id, 
                        Name = "Training", 
                        IsTemplate = true, 
                        SortOrder = 3,
                        Description = "Training is the acquisition of knowledge, skills, and competencies as a result of the teaching of vocational or practical skills and knowledge that relate to specific useful competencies",
                        ImageUrl = "./images/emblem_s.png"
                    },

					new QuestCategory() { 
                        ParentId = parent.Id, 
                        Name = "Public Speaking", 
                        IsTemplate = true, 
                        SortOrder = 4 
                    },
					new QuestCategory() { ParentId = parent.Id, Name = "Skill Mastery", IsTemplate = true, SortOrder = 5 },
					new QuestCategory() { ParentId = parent.Id, Name = "Time Management", IsTemplate = true, SortOrder = 6 },
					new QuestCategory() { ParentId = parent.Id, Name = "Get Coaching", IsTemplate = true, SortOrder = 7 },
					new QuestCategory() { ParentId = parent.Id, Name = "Customer", IsTemplate = true, SortOrder = 8 },
					new QuestCategory() { ParentId = parent.Id, Name = "Product", IsTemplate = true, SortOrder = 9 }
				);

				var category = db.First<QuestCategory>(c => c.Name == "Certifications");

				var startDate = DateTime.UtcNow.Date.AddDays(-18);
				var endDate = startDate.AddDays(90);

				db.Insert(new Quest()
				{
					UserId = testUserId, 
					IsTemplate = true,
					Name = "HCHB: Pointcare",
					Description = "Pointcare certifcations are specific to ACI roles at HCHB. The certification is a key part of the individuals career growth while in the ACI role levels 1 & 2.",
					CategoryId = category.Id,
					CheckinFrequency = CheckinFrequency.Weekly,
					CheckinsExpected = 3,
					Timeframe = CompletionTimeframe.NinetyDays,
					TotalPoints = 1000,
					CheckinPoints = 50,
					StartDate = startDate,
					Status = QuestStatus.Started,
                    ImageUrl = "./images/xhdpi/pushups.jpg"
				});

				var questId = db.GetLastInsertId();

				var estimatedDate = startDate;

				// checkpoints & checkins
				for(int i = 1; i <= 5; ++i) 
				{
					estimatedDate = estimatedDate.AddDays(7);
					var checkpoint = new Checkpoint()
					{
						Name = "Checkpoint " + i,
						QuestId = questId,
						SortOrder = i,
						Percent = 20,
						EstimatedDate = i == 5 ? endDate : estimatedDate
					};

					if (i < 3) // 1'st 2 checkpoints completed
						checkpoint.CompletionDate = estimatedDate;

					db.Insert(checkpoint);

					var checkpointId = db.GetLastInsertId();

					if (i <= 3)
					{
						for (int checkin = 1; checkin <= 3; ++checkin)
						{
							var checkinDate = estimatedDate.AddDays(-6);
							var activity = new QuestActivity()
							{
								Date = checkinDate,
								Type = QuestActivityType.CheckIn,
								QuestId = questId,
								CheckpointId = checkpointId,
								UserId = testUserId,
								CheckpointWin = (checkin == 3),
								CheckinVerifyDate = checkinDate,
							};

							db.Insert(activity);

							if(i == 3)
								break; // just add 1 checkin for current period
						}
					}
				}

				// feedback and assists
				for (int i = 0; i < 4; ++i)
				{
					db.Insert(new QuestActivity()
					{
						Date = startDate.AddDays(1),
						Type = QuestActivityType.Feedback,
						QuestId = questId,
						UserId = testUserId,
					});

					if(i < 2)
						db.Insert(new QuestActivity()
						{
							Date = startDate.AddDays(1),
							Type = QuestActivityType.Assist,
							QuestId = questId,
							UserId = testUserId,
						});
				}

				var fields = new[] {
					new { Type = QuestFieldType.Purpose, Name = "Pointcare Certification" },
					new { Type = QuestFieldType.Purpose, Name = "One more certification" },

					new { Type = QuestFieldType.CheckinType, Name = "Wins" },
					new { Type = QuestFieldType.CheckinType, Name = "Meetings" },
					new { Type = QuestFieldType.CheckinType, Name = "Progress" },

					new { Type = QuestFieldType.Metric, Name = "Sessions" },
					new { Type = QuestFieldType.Metric, Name = "Review Completion" },

					new { Type = QuestFieldType.Pitfall, Name = "Busyness" },
					new { Type = QuestFieldType.Pitfall, Name = "No Accountability" },

					new { Type = QuestFieldType.Asset, Name = "Required-ACI Level 1" },
					new { Type = QuestFieldType.Asset, Name = "Mgr. Approval" },

					new { Type = QuestFieldType.App, Name = "Withings" },
					new { Type = QuestFieldType.App, Name = "RunKeeper" },

					new { Type = QuestFieldType.Device, Name = "Withings Scale" },
					new { Type = QuestFieldType.Device, Name = "Withings Heart-rate monitor" },
				};

				for (int i = 0; i < fields.Length; ++i)
				{
					var f = fields[i];

					bool active = true;

					if (f.Type == QuestFieldType.Purpose && f.Name != "Pointcare Certification")
						active = false;

					db.Insert(new QuestField()
					{
						QuestId = questId,
						Type = f.Type,
						Active = active,
						SortOrder = i,
						Name = f.Name,
					});
				}

				db.Insert(new Quest()
				{
					IsTemplate = true,
					UserId = testUserId,
					Name = "Home Health Aid (HHA)",
					Description = "Home health aides make a remarkable difference in the lives of the sick, disabled and elderly",
					CategoryId = category.Id,
					CheckinFrequency = CheckinFrequency.Weekly,
					Timeframe = CompletionTimeframe.SixMonths,
					TotalPoints = 1000,
                    ImageUrl = "./images/xhdpi/pushups.jpg"
				}, new Quest()
				{
					IsTemplate = true,
					UserId = testUserId,
					Name = "Pediatric Advanced Life Support",
					Description = "Pediatric advanced life support (PALS) usually takes place in the setting of an organized response in an advanced healthcare environment.",
					CategoryId = category.Id,
					CheckinFrequency = CheckinFrequency.Weekly,
					Timeframe = CompletionTimeframe.UnderNinetyDays,
					TotalPoints = 1000,
                    ImageUrl = "./images/xhdpi/pushups.jpg"
				});

				trans.Commit();
			}
		}
		#endregion
	}
}