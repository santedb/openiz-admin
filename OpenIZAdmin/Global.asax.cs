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

using Elmah;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OpenIZAdmin.Logging;

namespace OpenIZAdmin
{
	/// <summary>
	/// Represents global configuration for the application.
	/// </summary>
	public class MvcApplication : System.Web.HttpApplication
	{
		/// <summary>
		/// The event source for the application.
		/// </summary>
		public const string EventSource = "OpenIZAdmin";

		/// <summary>
		/// The memory cache for the application.
		/// </summary>
		public static readonly MemoryCache MemoryCache = MemoryCache.Default;

		/// <summary>
		/// Called when the application encounters an unexpected error.
		/// </summary>
		/// <param name="sender">The sender of the error.</param>
		/// <param name="e">The event arguments.</param>
		protected void Application_Error(object sender, EventArgs e)
		{
			try
			{
				ErrorLog.GetDefault(HttpContext.Current).Log(new Error(this.Server.GetLastError(), HttpContext.Current));
			}
			catch
			{
				// ignored
			}
			finally
			{
				Trace.TraceError($"Application error: { e }");
			}
	}

		/// <summary>
		/// Called when the application starts.
		/// </summary>
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			// realm initialization
			RealmConfig.Initialize();

			// quartz initialization
			QuartzConfig.Initialize();

			Trace.TraceInformation("Application started");
		}
	}
}