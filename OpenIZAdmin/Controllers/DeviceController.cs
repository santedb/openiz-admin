﻿/*
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

using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.DeviceModels;
using OpenIZAdmin.Models.DeviceModels.ViewModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing devices.
	/// </summary>
	[TokenAuthorize]
	public class DeviceController : BaseController
	{
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
					var device = this.AmiClient.CreateDevice(DeviceUtil.ToSecurityDevice(model));

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

            TempData["error"] = Locale.UnableToCreate + " " + Locale.Device;
			return View(model);
		}

		[HttpGet]
		public ActionResult Edit(string id)
		{
			Guid deviceKey = Guid.Empty;

			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out deviceKey))
			{
				var result = this.AmiClient.GetDevices(r => r.Key == deviceKey);

				if (result.CollectionItem.Count == 0)
				{
					TempData["error"] = Locale.Device + " "  + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(DeviceUtil.ToDeviceViewModel(result.CollectionItem.Single()));
			}

			TempData["error"] = Localization.Locale.Device + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditDeviceModel model)
		{
			throw new NotImplementedException();
		}

		public ActionResult Index()
		{
			TempData["searchType"] = "Device";
			return View(DeviceUtil.GetAllDevices(this.AmiClient));
		}

		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<DeviceViewModel> devices = new List<DeviceViewModel>();

			try
			{
				if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
				{
					var collection = this.AmiClient.GetDevices(d => d.Name.Contains(searchTerm));

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

            TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_DevicesPartial", devices);
		}

		[HttpGet]
		public ActionResult ViewDevice(string id)
		{
			Guid deviceKey = Guid.Empty;

			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out deviceKey))
			{
				var result = this.AmiClient.GetDevices(r => r.Key == deviceKey);

				if (result.CollectionItem.Count == 0)
				{
					TempData["error"] = Locale.Device + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(DeviceUtil.ToDeviceViewModel(result.CollectionItem.Single()));
			}

			TempData["error"] = Locale.Device + " " + Locale.NotFound;

            return RedirectToAction("Index");
		}
	}
}