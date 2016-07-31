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
 * Date: 2016-7-8
 */

using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models.DeviceModels.ViewModels;
using System;
using System.Collections.Generic;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Mvc;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Util;
using System.Diagnostics;
using OpenIZAdmin.Models.DeviceModels;

namespace OpenIZAdmin.Controllers
{
	[TokenAuthorize]
	public class DeviceController : Controller
	{
		/// <summary>
		/// The internal reference to the <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient client;

		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateDeviceModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var device = this.client.CreateDevice(DeviceUtil.ToSecurityDevice(model));

					return RedirectToAction("ViewDevice", new { key = device.Key });
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to create device: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to create device: {0}", e.Message);
				}

			}

			TempData["error"] = "Unable to create device";
			return View(model);
		}

		public ActionResult Index()
		{
			TempData["searchType"] = "Device";
			return View(DeviceUtil.GetAllDevices(this.client));
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var restClient = new RestClientService("AMI");

			restClient.Accept = "application/xml";
			restClient.Credentials = new AmiCredentials(this.User, HttpContext.Request);

			this.client = new AmiServiceClient(restClient);

			base.OnActionExecuting(filterContext);
		}

		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<DeviceViewModel> devices = new List<DeviceViewModel>();

			try
			{
				if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
				{

					var collection = this.client.GetDevices(d => d.Name.Contains(searchTerm));

					TempData["searchTerm"] = searchTerm;

					return PartialView("_DevicesPartial", collection.CollectionItem.Select(d => DeviceUtil.ToDeviceViewModel(d)));
				}
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to search devices: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to search devices: {0}", e.Message);
			}

			TempData["error"] = "Invalid search, please check your search criteria";
			TempData["searchTerm"] = searchTerm;

			return PartialView("_DevicesPartial", devices);
		}

		[HttpGet]
		public ActionResult ViewDevice(string key)
		{
			Guid deviceId = Guid.Empty;

			if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key) && Guid.TryParse(key, out deviceId))
			{
				var result = this.client.GetDevices(r => r.Key == deviceId);

				if (result.CollectionItem.Count == 0)
				{
					TempData["error"] = Localization.Resources.Devices;

					return RedirectToAction("Index");
				}

				return View(DeviceUtil.ToDeviceViewModel(result.CollectionItem.Single()));
			}

			TempData["error"] = Localization.Resources.Devices;

			return RedirectToAction("Index");
		}
	}
}