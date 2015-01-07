using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.Domain
{
	public enum CompletionTimeframe
	{
		NinetyDays,
		SixMonths,
		OneYear,
		UnderNinetyDays,
		OwnDate
	}

	public enum QuestType
	{
		Personal,
		Team
	}

	public enum SuccessLikelihood
	{
		Likely,
		HighlyLikely,
		SlamDunk,
		LongShot,
	}

	public enum QuestStatus
	{
		Draft,
		NotStarted,
		Started,
		Completed,
		Aborted,
	}

	public enum CheckinFrequency
	{
		Weekly,
		Daily,
		BiWeekly,
		Monthly,
	}

	public enum QuestActivityType
	{
		CheckIn,
		Assist,
		Feedback,
		Recognition,
		Like,
		Invite,
		InviteAccept,
		CheckinVerification
	}

	public enum AssistType
	{
		CheckpointHelp,
		ProgressHelp
	}

	public enum QuestFieldType
	{
		Purpose,
		Timeframe,
		Metric,
		CheckinType,
		CheckinFrequency,
		Pitfall,
		Asset,
		Wishlist,
		SuccessLikelihood,
		Device,
		App
	}

	public enum AllyStatus
	{
		None,
		Invited,
		Accepted
	}
}