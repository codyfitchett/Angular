using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.DTO
{
	public class CheckpointDto
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public int Percent { get; set; }
		public string EstimatedDate { get; set; }
		public string CompletionDate { get; set; }
		public bool NeedsVerification { get; set; }
		public bool IsCurrent { get; set; }
		public bool Deleted { get; set; }

		public CheckpointDto Clone()
		{
			return (CheckpointDto)this.MemberwiseClone();
		}
	}
}