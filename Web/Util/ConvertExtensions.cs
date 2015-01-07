using RevHR.Web.Domain;
using RevHR.Web.DTO.Mobile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using ServiceStack;
using ServiceStack.Common;
using ServiceStack.Text;
using ServiceStack.OrmLite;
using RevHR.Web.Util;
using RevHR.Web.DTO;

namespace RevHR.Web.Util
{
	public static class ConvertExtensions
	{
		public static string GetTimelineString(this Quest quest)
		{
			switch (quest.Timeframe)
			{
				case CompletionTimeframe.NinetyDays:
					return "90";
				case CompletionTimeframe.OneYear:
					return "One";
				case CompletionTimeframe.OwnDate:
					return quest.EstimatedDate != null ? quest.EstimatedDate.Value.ToString("MM/dd/yyyy") : "Unknown";
				case CompletionTimeframe.SixMonths:
					return "Six";
				case CompletionTimeframe.UnderNinetyDays:
					return "Under 90";
				default:
					return "Unknown";
			}
		}

		public static string GetTimelineTypeString(this Quest quest)
		{
			switch (quest.Timeframe)
			{
				case CompletionTimeframe.NinetyDays: return "Days";
				case CompletionTimeframe.OneYear: return "Year";
				case CompletionTimeframe.OwnDate: return "Custom Date";
				case CompletionTimeframe.SixMonths: return "Months";
				case CompletionTimeframe.UnderNinetyDays: return "Days";
				default: return "Unknown";
			}
		}

		public static string GetCheckinFrequencyString(this Quest quest)
		{
			var formatString =
				  quest.CheckinsExpected == 1 ? "{0}"
				: quest.CheckinsExpected == 2 ? "Twice {0}"
				: quest.CheckinsExpected == 3 ? "Thrice {0}"
				: quest.CheckinsExpected + " times {0}";

			return formatString.Fmt(quest.CheckinFrequency == CheckinFrequency.BiWeekly
										? " every two weeks" : quest.CheckinFrequency.ToString());
		}

		public static CheckpointDto ToDto(this Checkpoint checkpoint)
		{
			var dto = checkpoint.TranslateTo<CheckpointDto>();
			dto.EstimatedDate = checkpoint.EstimatedDate.ToString("MM/dd/yyyy");
			dto.CompletionDate = checkpoint.CompletionDate != null ? checkpoint.CompletionDate.Value.ToString("MM/dd/yyyy") : "";
			return dto;
		}

		public static List<CheckpointDto> ToDto(this IEnumerable<Checkpoint> checkpoints)
		{
			var checkpointsDto = checkpoints.Select(c => c.ToDto()).ToList();
			var currentCheckpoint = checkpointsDto.FirstOrDefault(c => c.CompletionDate.IsNullOrEmpty());
			if (currentCheckpoint == null)
				currentCheckpoint = checkpointsDto.LastOrDefault();
			if (currentCheckpoint != null)
				currentCheckpoint.IsCurrent = true;
			return checkpointsDto;
		}

		public static List<QuestFieldDto> ToDto(this IEnumerable<QuestField> fields)
		{
			return fields.Select(f => f.TranslateTo<QuestFieldDto>()).ToList();
		}
	}
}