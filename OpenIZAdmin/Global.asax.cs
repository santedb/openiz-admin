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
using System.Configuration;
using System.Diagnostics;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CacheItemPriority = System.Runtime.Caching.CacheItemPriority;

namespace OpenIZAdmin
{
	/// <summary>
	/// Represents global configuration for the application.
	/// </summary>
	public class MvcApplication : System.Web.HttpApplication
	{
		/// <summary>
		/// The memory cache for the application.
		/// </summary>
		public static readonly MemoryCache MemoryCache = MemoryCache.Default;

		/// <summary>
		/// The cache item policy.
		/// </summary>
		public static readonly CacheItemPolicy CacheItemPolicy = new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 0, 10, 0), Priority = CacheItemPriority.Default, RemovedCallback = CacheItemRemoved };

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

			var enableCaching = Convert.ToBoolean(ConfigurationManager.AppSettings["enableCaching"]);

			if (enableCaching)
			{
				Trace.TraceInformation("Enabling caching");
				QuartzConfig.Initialize();
			}

			Trace.TraceInformation("Application started");
		}

		private static void CacheItemRemoved(CacheEntryRemovedArguments arguments)
		{
			Trace.TraceInformation($"Cache item removed key: { arguments.CacheItem.Key } value: { arguments.CacheItem.Value }");
		}
	}
}