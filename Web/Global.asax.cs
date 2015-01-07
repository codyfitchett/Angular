using ServiceStack.MiniProfiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace RevHR.Web
{
	public class Global : System.Web.HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			new AppHost().Init();
		}

		protected void Application_BeginRequest(object src, EventArgs e)
		{
			if (Request.IsLocal)
				Profiler.Start();
		}

		protected void Application_EndRequest(object src, EventArgs e)
		{
			Profiler.Stop();
		}
	}
}