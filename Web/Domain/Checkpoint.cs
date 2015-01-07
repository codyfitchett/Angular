using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.Domain
{
	public class Checkpoint
	{
		[AutoIncrement]
		public long Id { get; set; }
		[References(typeof(Quest))]
		public long QuestId { get; set; }
		public int SortOrder { get; set; }
		public string Name { get; set; }
		public int Percent { get; set; }
		public int DurationDays { get; set; }
		public DateTime EstimatedDate { get; set; }
		public DateTime? CompletionDate { get; set; }
		public bool NeedsVerification { get; set; }
		public int TotalPoints { get; set; }
		public int CheckinPoints { get; set; }
	}
}