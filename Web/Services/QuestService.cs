using RevHR.Web.DTO.Mobile;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ServiceStack;
using ServiceStack.Text;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using RevHR.Web.Domain;
using RevHR.Web.Util;
using ServiceStack.Common.Web;
using RevHR.Web.DTO;
using ServiceStack.ServiceInterface.Auth;

using ServiceStack.ServiceInterface.ServiceModel;

namespace RevHR.Web.Services
{
	public class QuestService : Service
	{
		public IUserAuthRepository AuthRepo { get; set; }

		public object Get(ListBuilderCategories request)
		{
			return new ListBuilderCategoriesResponse()
			{
				Categories = Db.SelectParam<QuestCategory>(c => c.IsTemplate && c.ParentId == null).Sort(c => c.SortOrder)
			};
		}

		public object Get(GetBuilderCategory request)
		{
			var categories = Db.SelectParam<QuestCategory>(c => c.Id == request.CategoryId || c.ParentId == request.CategoryId)
									.Sort(c => c.SortOrder);
			var quests = Db.SelectParam<Quest>(q => q.IsTemplate && q.CategoryId == request.CategoryId)
									.Sort(q => q.SortOrder);

			var selectedCategory = categories.FirstOrDefault(c => c.Id == request.CategoryId);
			var childCategories = categories.Where(c => c.Id != request.CategoryId);

			var response = new GetBuilderCategoryResponse()
			{
				ChildCategories = childCategories.ToList(),
				Quests = quests.ConvertAll(q => q.TranslateTo<GetBuilderCategoryResponse.Quest>())
			};

			if (selectedCategory != null)
				response.PopulateWith(selectedCategory);

			return response;
		}

		public object Get(GetQuest request)
		{
			// TODO: auth
			var quest = Db.GetQuest(request.Id);

			if (quest == null)
				throw HttpError.NotFound("Quest with id {0} not found".Fmt(request.Id));

			return quest;
		}

		public object Post(QuestDto quest)
		{
			var user = AuthRepo.GetUserAuthByUserName("testuser");
			return Db.SaveQuest(quest, user.Id);
		}

		public object Post(CheckinDto checkin)
		{
			// TODO: auth
			var user = AuthRepo.GetUserAuthByUserName("testuser");

			var quest = Db.GetByIdOrDefault<Quest>(checkin.QuestId);

			if (quest == null)
				throw HttpError.NotFound("Quest with id {0} not found".Fmt(checkin.QuestId));

			var checkpoints = Db.SelectParam<Checkpoint>(p => p.QuestId == quest.Id).Sort(c => c.SortOrder);
			var currentCheckpoint = quest.GetCurrentCheckpoint(checkpoints);

			if (currentCheckpoint == null)
				return CheckinResponse.NoPendingCheckpoints;

			Db.Insert(new QuestActivity()
			{
				QuestId = quest.Id,
				UserId = user.Id,
				Date = DateTime.UtcNow,
				Type = QuestActivityType.CheckIn,
				CheckpointId = currentCheckpoint.Id,
				CheckpointWin = checkin.CheckpointWin,
				CheckpointProgress = checkin.Progress,
				Comment = checkin.Comment,
				VisibleInTimeline = checkin.VisibleInTimeline,
                CheckinType = checkin.CheckInType
			});

			if (checkin.CheckpointWin)
			{
				currentCheckpoint.CompletionDate = DateTime.UtcNow;
				Db.UpdateOnly(currentCheckpoint, c => c.CompletionDate);
			}

			return CheckinResponse.OK;
		}
	}
}