using RevHR.Web.Domain;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.DTO
{
	[Route("/quests", "POST")]
	[Route("/quests/{Id}", "POST,PUT")]
	public class QuestDto : IReturn<QuestDto>
	{
		public ResponseStatus ResponseStatus { get; set; }

		public long Id { get; set; }
		public long CategoryId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
		public bool IsTemplate { get; set; }
		private List<CheckpointDto> _checkpoints;
		public List<CheckpointDto> Checkpoints
		{
			get { return _checkpoints ?? (_checkpoints = new List<CheckpointDto>()); }
			set { _checkpoints = value; }
		}

		public CompletionTimeframe Timeframe { get; set; }
		public string EstimatedDate { get; set; }
		public int TotalPoints { get; set; }

		private List<QuestFieldDto> _fields;
		public List<QuestFieldDto> Fields
		{
			get { return _fields ?? (_fields = new List<QuestFieldDto>()); }
			set { _fields = value; }
		}

		public CheckinFrequency CheckinFrequency { get; set; }
		public int CheckinsExpected { get; set; }

		// User Data
		public int DaysLeft { get; set; }
		public int PercentCompleted { get; set; }
		public int AssistCount { get; set; }
		public int FeedbackCount { get; set; }
		public int PointsWon { get; set; }
		public int DaysSinceLastCheckin { get; set; }
		public int CheckinsActual { get; set; }

		private List<CheckinStats> _checkins;
		public List<CheckinStats> Checkins
		{
			get { return _checkins ?? (_checkins = new List<CheckinStats>()); }
			set { _checkins = value; }
		}

		public QuestDto Clone()
		{
			var copy = (QuestDto)this.MemberwiseClone();

			copy.Checkpoints = this.Checkpoints.Select(c => c.Clone()).ToList();
			copy.Fields = this.Fields.Select(f => f.Clone()).ToList();
			copy.Checkins = this.Checkins.Select(c => c.Clone()).ToList();

			return copy;
		}
	}
}