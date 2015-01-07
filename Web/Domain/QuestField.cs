using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.Domain
{
	public class QuestField
	{
		[AutoIncrement]
		public long Id { get; set; }
		[References(typeof(Quest))]
		public long QuestId { get; set; }
		public int SortOrder { get; set; }
		public QuestFieldType Type { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal? Value { get; set; }
		public bool Active { get; set; }
	}
}