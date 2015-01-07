using RevHR.Web.Domain;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.DTO.Mobile
{
	[Route("/quests/{Id}", "GET")]
	public class GetQuest : IReturn<QuestDto>
	{
		public long Id { get; set; }
	}
}