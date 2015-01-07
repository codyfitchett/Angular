using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.Domain
{
	public class CheckinStats
	{
		public string BeginDate { get; set; }
		public string EndDate { get; set; }
		public int Count { get; set; }

		public CheckinStats Clone()
		{
			return (CheckinStats)this.MemberwiseClone();
		}
	}
}