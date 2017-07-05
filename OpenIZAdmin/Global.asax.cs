/*
 * Copyright 2016-2017 Mohawk College of Applied Arts and Technology
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you
 * may not use this file except in compliance with the License. You may
 * obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 *
 * User: khannan
 * Date: 2016-5-31
 */

using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZAdmin.Audit;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Models.Audit;
using OpenIZAdmin.Services.Http.Security;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Caching;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OpenIZAdmin
{
	/// <summary>
	/// Represents global configuration for the application.
	/// </summary>
	public class MvcApplication : System.Web.HttpApplication
	{
		/// <summary>
		/// The cache item policy.
		/// </summary>
		public static readonly CacheItemPolicy CacheItemPolicy = new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 0, 10, 0), Priority = CacheItemPriority.Default };

		/// <summary>
		/// The memory cache for the application.
		/// </summary>
		public static readonly MemoryCache MemoryCache = MemoryCache.Default;

		/// <summary>
		/// Handles the End event of the Application control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void Application_End(object sender, EventArgs e)
		{
			try
			{
				if (RealmConfig.IsJoinedToRealm())
				{
					var deviceIdentity = ApplicationSignInManager.LoginAsDevice();

					var auditHelper = new GlobalAuditHelper(new AmiCredentials(this.User, deviceIdentity.AccessToken), this.Context);

					auditHelper.AuditApplicationStop(OutcomeIndicator.Success);
				}
			}
			catch (Exception exception)
			{
				Trace.TraceError("****************************************************");
				Trace.TraceError($"Unable to send application stop audit: {exception}");
				Trace.TraceError("****************************************************");
			}

			Trace.TraceInformation("Application stopped");
		}

		/// <summary>
		/// Handles the EndRequest event of the Application control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void Application_EndRequest(object sender, EventArgs e)
		{
			try
			{
				if (!RealmConfig.IsJoinedToRealm())
				{
					return;
				}

				// if the response status code is not a client level error code, we don't want to attempt to audit the access
				if (this.Response.StatusCode != 401 && this.Response.StatusCode != 403 && this.Response.StatusCode != 404)
				{
					return;
				}

				var deviceIdentity = ApplicationSignInManager.LoginAsDevice();

				var auditHelper = new GlobalAuditHelper(new AmiCredentials(this.User, deviceIdentity.AccessToken), this.Context);

				switch (this.Response.StatusCode)
				{
					case 401:
						if (this.Request.Headers["Authorization"] != null)
							auditHelper.AuditUnauthorizedAccess();
						break;
					case 403:
						auditHelper.AuditForbiddenAccess();
						break;
					case 404:
						auditHelper.AuditResourceNotFoundAccess();
						break;
				}
			}
			catch (Exception exception)
			{
				Trace.TraceError($"Unable to audit application end request: {exception}");
			}
		}

		/// <summary>
		/// Called when the application encounters an unexpected error.
		/// </summary>
		/// <param name="sender">The sender of the error.</param>
		/// <param name="e">The event arguments.</param>
		protected void Application_Error(object sender, EventArgs e)
		{
			Trace.TraceError($"Unexpected application error: {Server.GetLastError()}");

			try
			{
				if (!RealmConfig.IsJoinedToRealm())
				{
					return;
				}

				var deviceIdentity = ApplicationSignInManager.LoginAsDevice();

				var auditHelper = new GlobalAuditHelper(new AmiCredentials(this.User, deviceIdentity.AccessToken), this.Context);

				auditHelper.AuditGenericError(OutcomeIndicator.EpicFail, EventTypeCode.ApplicationActivity, EventIdentifierType.ApplicationActivity, this.Server.GetLastError());
			}
			catch (Exception exception)
			{
				Trace.TraceError($"Unable to audit application generic error: {exception}");
			}
		}

		/// <summary>
		/// Called when the application starts.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void Application_Start(object sender, EventArgs e)
		{
			AreaRegistration.RegisterAllAreas();

			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			// realm initialization
			RealmConfig.Initialize();

			if (!Directory.Exists(this.Server.MapPath("~/Manuals")))
			{
				Directory.CreateDirectory(this.Server.MapPath("~/Manuals"));
			}

			if (RealmConfig.IsJoinedToRealm())
			{
				try
				{
					var deviceIdentity = ApplicationSignInManager.LoginAsDevice();

					var auditHelper = new GlobalAuditHelper(new AmiCredentials(this.User, deviceIdentity.AccessToken), this.Context);

					auditHelper.AuditApplicationStart(OutcomeIndicator.Success);
				}
				catch (Exception exception)
				{
					Trace.TraceError("****************************************************");
					Trace.TraceError($"Unable to send application start audit: {exception}");
					Trace.TraceError("****************************************************");
				}
			}

			Trace.TraceInformation("Application started");
		}
	}
}