using RevHR.Web.Domain;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.DTO.Mobile
{
	[Route("/builder/categories", "GET")]
	public class ListBuilderCategories : IReturn<ListBuilderCategoriesResponse>
	{
	}

	public class ListBuilderCategoriesResponse
	{
		public ResponseStatus ResponseStatus { get; set; }

		private List<QuestCategory> _categories;
		public List<QuestCategory> Categories
		{
			get { return _categories ?? (_categories = new List<QuestCategory>()); }
			set { _categories = value; }
		}
	}
}