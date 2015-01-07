using ServiceStack.DataAnnotations;
using ServiceStack.ServiceInterface.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.Domain
{
	public class Ally
	{
		[AutoIncrement]
		public long Id { get; set; }
		[References(typeof(Quest))]
		public long QuestId { get; set; }
		[References(typeof(UserAuth))]
		public int UserId { get; set; }
		public AllyStatus Status { get; set; }
		public DateTime? InviteDate { get; set; }
		public DateTime? AcceptDate { get; set; }
	}
}