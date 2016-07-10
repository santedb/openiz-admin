/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
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
		/// Called when the application starts.
		/// </summary>
		protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

			// default .net configuration
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

			// message handler configuration
			MessageHandlerConfig.Register(GlobalConfiguration.Configuration);

			Trace.TraceInformation("Application started");
        }

		/// <summary>
		/// Called when the application encounters an unexpected error.
		/// </summary>
		/// <param name="sender">The sender of the error.</param>
		/// <param name="e">The event.</param>
		protected void Application_Error(object sender, EventArgs e)
		{
#if DEBUG
			Trace.TraceError("Application error: {0}", Server.GetLastError());
#endif
			Trace.TraceError("Application error: {0}", Server.GetLastError().Message);
		}
	}
}
