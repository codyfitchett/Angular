using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.DTO
{
	[Route("/quests/{QuestId}/checkins", "POST")]
	public class CheckinDto : IReturn<CheckinResponse>
	{
		public long QuestId { get; set; }
		public string Comment { get; set; }
		public bool CheckpointWin { get; set; }
		public bool VisibleInTimeline { get; set; }
		public int? Progress { get; set; }
        public long? CheckInType { get; set; }
	}

	public class CheckinResponse
	{
		public static readonly CheckinResponse NoPendingCheckpoints = new CheckinResponse()
		{
			ResponseStatus = new ResponseStatus() { Message = "All checkpoints have been completed" }
		};

		public static readonly CheckinResponse OK = new CheckinResponse() { Success = true };

		public ResponseStatus ResponseStatus { get; set; }
		public bool Success { get; set; }
	}
}