using Funq;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.OrmLite;
using ServiceStack.Razor;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

using ServiceStack.Common;
using ServiceStack.Configuration;
using ServiceStack.WebHost.Endpoints.Support;
using ServiceStack.Text;
using ServiceStack.ServiceInterface.Validation;
using ServiceStack.WebHost.Endpoints.Formats;
using RevHR.Web.Services;
using RevHR.Web.Domain;

namespace RevHR.Web
{
	public class AppHost : AppHostBase
	{
		public AppHost() : base("MaximusLife", typeof(AppHost).Assembly) { }

		public override void Configure(Container container)
		{
			var appSettings = new AppSettings();

			var connectionString = appSettings.GetString("DB:ConnectionString");

			LogManager.LogFactory = new ConsoleLogFactory();

			// Plugins.Add(new RazorFormat());

			container.Register<IDbConnectionFactory>(
				new OrmLiteConnectionFactory(connectionString, false, SqlServer2008Dialect.Provider));

			container.Register<ICacheClient>(new MemoryCacheClient());

			JsConfig.EmitCamelCaseNames = true;
			JsConfig.IncludeNullValues = true;

			SetConfig(new EndpointHostConfig
			{
				DebugMode = true,

				//AllowFileExtensions = { { "map" } },

				//CustomHttpHandlers = {
				//	{ HttpStatusCode.NotFound, new RazorHandler("/notfound") },
				//	{ HttpStatusCode.Unauthorized, new RazorHandler("/login") },
				//},
			});

			AddAuthentication(container);
			CreateDomainTables(container);
			SeedData(container);
		}

		private bool IsRequestForAllowedStaticFile(IHttpRequest request)
		{
			var filePath = request.GetPhysicalPath();
			var fileExt = System.IO.Path.GetExtension(filePath);
			if (string.IsNullOrEmpty(fileExt)) return false;
			return Config.AllowFileExtensions.Contains(fileExt.Substring(1));
		}

		private void AddAuthentication(Container container)
		{
			container.Register<IUserAuthRepository>(c =>
				new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>()));

			var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>();
			authRepo.CreateMissingTables();

			Plugins.Add(new AuthFeature(() => new AuthUserSession(),
				new IAuthProvider[] {
					new CredentialsAuthProvider(),
				}
			));

			Plugins.Add(new RegistrationFeature());
		}

		private void CreateDomainTables(Container container)
		{
			using (var db = container.Resolve<IDbConnectionFactory>().OpenDbConnection())
			{
				var tableTypes = new[] {
					typeof(QuestCategory),
					typeof(Quest),
					typeof(QuestField),
					typeof(Checkpoint),
					typeof(QuestActivity),
					typeof(Ally),
				};

				db.DropTables(tableTypes);
				db.CreateTables(true, tableTypes);
			}
		}

		private void SeedData(Container container)
		{
			var authRepo = container.Resolve<IUserAuthRepository>();

			var user = authRepo.GetUserAuthByUserName("testuser");

			if (user == null)
			{
				user = authRepo.CreateUserAuth(new UserAuth()
				{
					UserName = "testuser",
					Email = "testuser@maximuslife.com"
				}, "testuser123");
			}

			using (var db = container.Resolve<IDbConnectionFactory>().OpenDbConnection())
				db.SeedData(user.Id);
		}
	}
}