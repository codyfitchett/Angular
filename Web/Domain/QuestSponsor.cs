using ServiceStack.DataAnnotations;
using ServiceStack.ServiceInterface.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.Domain
{
	public class QuestSponsor
	{
		[AutoIncrement]
		public long Id { get; set; }
		[References(typeof(UserAuth))]
		public int UserId { get; set; }
		[References(typeof(Quest))]
		public long QuestId { get; set; }
	}
}