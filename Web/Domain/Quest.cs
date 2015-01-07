using ServiceStack.DataAnnotations;
using ServiceStack.ServiceInterface.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.Domain
{
	public class Quest
	{
		public Quest()
		{
			CheckinsExpected = 1;
		}

		[AutoIncrement]
		public long Id { get; set; }
		[References(typeof(Quest))]
		public long? ParentId { get; set; }
		[References(typeof(UserAuth))]
		public int UserId { get; set; }
		[References(typeof(Quest))]
		public long? TemplateId { get; set; }
		public bool IsTemplate { get; set; }
		public int SortOrder { get; set; }
		[References(typeof(QuestCategory))]
		public long CategoryId { get; set; }

		public QuestType Type { get; set; }
		public QuestStatus Status { get; set; }
		public CompletionTimeframe Timeframe { get; set; }
		public SuccessLikelihood SuccessLikelihood { get; set; }

		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
		public bool SequentialCheckpoints { get; set; }
		public DateTime? EstimatedDate { get; set; }

		public CheckinFrequency CheckinFrequency { get; set; }
		public int CheckinsExpected { get; set; }

		public DateTime? StartDate { get; set; }
		public DateTime? CompletionDate { get; set; }
		public string Feedback { get; set; }

		public int TotalPoints { get; set; }
		public int FeedbackPoints { get; set; }
		public int AssistPoints { get; set; }
		public int RecognitionPoints { get; set; }
		public int CheckinPoints { get; set; }

		public int DaysLeft(DateTime now)
		{
			now = now.ToUniversalTime().Date;

			if(StartDate == null)
				return -1;

			if(CompletionDate != null)
				return 0;

			DateTime dueDate;
			if (EstimatedDate != null)
				dueDate = EstimatedDate.Value;

			if (Timeframe == CompletionTimeframe.NinetyDays)
				dueDate = StartDate.Value.AddDays(90);
			else if (Timeframe == CompletionTimeframe.SixMonths)
				dueDate = StartDate.Value.AddMonths(6);
			else if (Timeframe == CompletionTimeframe.OneYear)
				dueDate = StartDate.Value.AddYears(1);
			else
				return -1;

			return (int) (dueDate - now).TotalDays;
		}

		public Checkpoint GetCurrentCheckpoint(IEnumerable<Checkpoint> checkpoints)
		{
			return checkpoints.FirstOrDefault(c => c.CompletionDate == null);
		}

		public int GetCheckinCountForThisPeriod(List<QuestActivity> activities, bool verifiedOnly = false)
		{
			var startDate = DateTime.UtcNow.Date;

			if (CheckinFrequency == Domain.CheckinFrequency.Daily)
				startDate = startDate.AddDays(-1);
			else if (CheckinFrequency == Domain.CheckinFrequency.Weekly)
				startDate = startDate.AddDays(-7);
			else if (CheckinFrequency == Domain.CheckinFrequency.BiWeekly)
				startDate = startDate.AddDays(-14);
			else if (CheckinFrequency == Domain.CheckinFrequency.Monthly)
				startDate = startDate.AddMonths(-1);

			var checkins = activities.Where(a => a.Type == QuestActivityType.CheckIn && a.Date >= startDate);
			if (verifiedOnly)
				checkins = checkins.Where(c => c.CheckinVerifyDate != null);

			return checkins.Count();
		}

		public List<CheckinStats> GetCheckinStats(List<QuestActivity> activities, bool verifiedOnly = false)
		{
			if (StartDate == null)
				return new List<CheckinStats>();

			var checkins = activities.Where(a => a.Type == QuestActivityType.CheckIn);
			if(verifiedOnly)
				checkins = checkins.Where(c => c.CheckinVerifyDate != null);

			var stats = new List<CheckinStats>();

			var now = DateTime.UtcNow;
			for (var date = this.StartDate.Value; date < now; date = date.AddDays(7))
			{
				DateTime beginDate = date, endDate = date.AddDays(7);

				stats.Add(new CheckinStats() {
					BeginDate = beginDate.ToString("MM/dd/yyyy"),
					EndDate = endDate.ToString("MM/dd/yyyy"),
					Count = checkins.Count(c => c.Date >= beginDate && c.Date < endDate)
				});
			}

			return stats;
		}
	}
}