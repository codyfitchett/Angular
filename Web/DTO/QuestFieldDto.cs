using RevHR.Web.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.DTO
{
	public class QuestFieldDto
	{
		public long Id { get; set; }
		public QuestFieldType Type { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal? Value { get; set; }
		public bool Active { get; set; }
		public string Error { get; set; }
		public bool Deleted { get; set; }


		public QuestFieldDto Clone()
		{
			return (QuestFieldDto)this.MemberwiseClone();
		}
	}
}