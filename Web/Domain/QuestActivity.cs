using ServiceStack.DataAnnotations;
using ServiceStack.ServiceInterface.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.Domain
{
	public class Like
	{
		public long LikedByUserId { get; set; }
		public DateTime? LikeDate { get; set; }
	}

	public class QuestActivity
	{
		[AutoIncrement]
		public long Id { get; set; }
		[References(typeof(QuestActivity))]
		public long? ParentId { get; set; }
		[References(typeof(QuestActivity))]
		public long? RootId { get; set; }
		[References(typeof(Quest))]
		public long QuestId { get; set; }
		public QuestActivityType Type { get; set; }
		[References(typeof(UserAuth))]
		public int UserId { get; set; }
		[References(typeof(UserAuth))]
		public int? AllyId { get; set; }
		public DateTime Date { get; set; }
		[References(typeof(Checkpoint))]
		public long? CheckpointId { get; set; }
		public string InviteEmail { get; set; }
		public string Subject { get; set; }
		public string Comment { get; set; }
		[References(typeof(QuestField))]
		public long? CheckinType { get; set; }
		public int? CheckpointProgress { get; set; }
		public bool CheckpointWin { get; set; }
		public string FeedbackType { get; set; }
		public string FeedbackBadge { get; set; }
		public string FeedbackCorporateValue { get; set; }
		public AssistType AssistType { get; set; }
		public int? AssistImpactRating { get; set; }
		public DateTime? ViewDate { get; set; }
		public bool VisibleInTimeline { get; set; }
		public DateTime? CheckinVerifyDate { get; set; }
		public CheckinVerification CheckinVerification { get; set; }
		private List<Like> _likes;
		public List<Like> Likes
		{
			get { return _likes ?? (_likes = new List<Like>()); }
			set { _likes = value; }
		}
	}
}