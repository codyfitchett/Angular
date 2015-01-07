using RevHR.Web.Domain;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.DTO.Mobile
{
	[Route("/builder/categories/{CategoryId}")]
	public class GetBuilderCategory : IReturn<GetBuilderCategoryResponse>
	{
		public long CategoryId { get; set; }
	}

	public class GetBuilderCategoryResponse
	{
		public ResponseStatus ResponseStatus { get; set; }

		public long Id { get; set; }
		public long? ParentId { get; set; }
		public int SortOrder { get; set; }
		public bool IsTemplate { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }

		private List<QuestCategory> _childCategories;
		public List<QuestCategory> ChildCategories
		{
			get { return _childCategories ?? (_childCategories = new List<QuestCategory>()); }
			set { _childCategories = value; }
		}

		private List<Quest> _quests;
		public List<Quest> Quests
		{
			get { return _quests ?? (_quests = new List<Quest>()); }
			set { _quests = value; }
		}

		public class Quest
		{
			public long Id { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
            public string ImageUrl { get; set; }
            public CheckinFrequency CheckinFrequency { get; set; }
            public CompletionTimeframe Timeframe { get; set; }
		}
	}
}