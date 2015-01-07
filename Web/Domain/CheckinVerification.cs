using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.Domain
{
	public enum CheckinVerificationType
	{
		Photo,
		Allies,
		App
	}

	public class CheckinVerification
	{
		public CheckinVerificationType Type { get; set; }
		public string PhotoUrl { get; set; }
	}
}