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
using System.Runtime.Caching;
using System.Web;
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
			ErrorLog.GetDefault(HttpContext.Current).Log(new Error(Server.GetLastError(), HttpContext.Current));
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

			//ThreadPool.QueueUserWorkItem(state =>
			//{
			//	var accessToken = this.SignIn();

			//	if (accessToken != null)
			//	{
			//		var restClientService = new RestClientService(Constants.Ami)
			//		{
			//			Credentials = new AmiCredentials(null, accessToken)
			//		};

			//		var client = new AmiServiceClient(restClientService);

			//		var policies = client.GetPolicies(p => p.ObsoletionTime == null);

			//		foreach (var securityPolicyInfo in policies.CollectionItem)
			//		{
			//			MvcApplication.MemoryCache.Set(securityPolicyInfo.Policy.Key.Value.ToString(), securityPolicyInfo.Policy, ObjectCache.InfiniteAbsoluteExpiration);
			//		}
			//	}
			//});

			Trace.TraceInformation("Application started");
		}

		//private string SignIn()
		//{
		//	string accessToken = null;

		//	using (var client = new HttpClient())
		//	{
		//		var currentRealm = RealmConfig.GetCurrentRealm();

		//		if (RealmConfig.IsJoinedToRealm())
		//		{
		//			client.DefaultRequestHeaders.Add("Authorization", "BASIC " + Convert.ToBase64String(Encoding.UTF8.GetBytes(currentRealm.ApplicationId + ":" + currentRealm.ApplicationSecret)));

		//			var content = new StringContent($"grant_type=password&username={currentRealm.ApplicationId}&password={currentRealm.ApplicationSecret}&scope={currentRealm.Address}/imsi");

		//			// HACK: have to remove the headers before adding them...
		//			content.Headers.Remove("Content-Type");
		//			content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

		//			var result = client.PostAsync($"{currentRealm.Address}/auth/oauth2_token", content).Result;

		//			if (result.IsSuccessStatusCode)
		//			{
		//				var response = JObject.Parse(result.Content.ReadAsStringAsync().Result);

		//				accessToken = response.GetValue("access_token").ToString();
		//			}
		//		}

		//		return accessToken;
		//	}
		//}
	}
}