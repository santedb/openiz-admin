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
 * User: Nityan
 * Date: 2016-8-15
 */

using OpenIZ.Core.Model.Roles;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models.ProviderModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using OpenIZAdmin.Util;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	[TokenAuthorize]
	public class ProviderController : Controller
	{
		/// <summary>
		/// The internal reference to the <see cref="OpenIZ.Messaging.IMSI.Client.ImsiServiceClient"/> instance.
		/// </summary>
		private ImsiServiceClient client;

		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateProviderModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var provider = this.client.Create<Provider>(ProviderUtil.ToProvider(model));

					TempData["success"] = "Provider created successfully";
					return RedirectToAction("ViewProvider", new { key = provider.Key, versionKey = provider.VersionKey });
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to create provider: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to create provider: {0}", e.Message);
				}
			}

			TempData["error"] = "Unable to create provider";
			return View(model);
		}

		[HttpGet]
		public ActionResult Edit()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditProviderModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var provider = this.client.Update<Provider>(ProviderUtil.ToProvider(model));

					TempData["success"] = "Provider updated successfully";
					return RedirectToAction("ViewProvider", new { key = provider.Key, versionKey = provider.VersionKey });
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to update provider: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to update provider: {0}", e.Message);
				}
			}

			TempData["error"] = "Unable to update provider";
			return View(model);
		}

		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "Provider";
			return View();
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var restClient = new RestClientService("IMSI");

			restClient.Accept = "application/xml";
			restClient.Credentials = new ImsCredentials(this.User, HttpContext.Request);

			this.client = new ImsiServiceClient(restClient);

			base.OnActionExecuting(filterContext);
		}

		[HttpGet]
		public ActionResult ViewProvider(string key, string versionKey)
		{
			Guid providerKey = Guid.Empty;
			Guid providerVersioKey = Guid.Empty;

			if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key) && Guid.TryParse(key, out providerKey) &&
				!string.IsNullOrEmpty(versionKey) && !string.IsNullOrWhiteSpace(versionKey) && Guid.TryParse(versionKey, out providerVersioKey))
			{
				try
				{
					var provider = this.client.Get<Provider>(providerKey, providerVersioKey);

					object model = null;

					return View(model);
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to update provider: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to update provider: {0}", e.Message);
				}
			}

			TempData["error"] = "Provider not found";
			return RedirectToAction("Index");
		}
	}
}