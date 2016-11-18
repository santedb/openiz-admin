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

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
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

        /// <summary>
		/// Activates a device.
		/// </summary>
		/// <param name="id">The id of the device to be activated.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Activate(EditDeviceModel model)
        {
            Guid deviceKey = Guid.Empty;
            SecurityDevice device = null;

            if (PolicyUtil.IsValidString(model.Id) && Guid.TryParse(model.Id, out deviceKey))
            {
                try
                {
                    device = DeviceUtil.GetDevice(this.client, deviceKey);

                    if (device == null)
                    {
                        TempData["error"] = Locale.Device + " " + Locale.NotFound;

                        return RedirectToAction("Index");
                    }                                       
                                        
                    SecurityDeviceInfo deviceInfo = DeviceUtil.ToSecurityDeviceInfo(model, device);

                    this.client.UpdateDevice(model.Id, deviceInfo);

                    TempData["success"] = Locale.Device + " " + Locale.ActivatedSuccessfully;

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.TraceError("Unable to activate device: {0}", e.StackTrace);
#endif
                    Trace.TraceError("Unable to activate device: {0}", e.Message);
                }
            }

            TempData["error"] = Locale.UnableToActivate + " " + Locale.Device;

            return RedirectToAction("Index");
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

            TempData["error"] = Locale.UnableToCreate + " " + Locale.Device;
			return View(model);
		}

        /// <summary>
		/// Deletes a device.
		/// </summary>
		/// <param name="id">The id of the device to be deleted.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            if (DeviceUtil.IsValidString(id))
            {
                try
                {
                    this.client.DeleteDevice(id);
                    TempData["success"] = Locale.Device + " " + Locale.DeletedSuccessfully;

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.TraceError("Unable to delete device: {0}", e.StackTrace);
#endif
                    Trace.TraceError("Unable to delete device: {0}", e.Message);
                }
            }

            TempData["error"] = Locale.UnableToDelete + " " + Locale.Device;


            return RedirectToAction("Index");
        }

        [HttpGet]
		public ActionResult Edit(string key)
		{
			Guid deviceKey = Guid.Empty;
            SecurityDevice device = null;

			if (DeviceUtil.IsValidString(key) && Guid.TryParse(key, out deviceKey))
			{
                device = DeviceUtil.GetDevice(this.client, deviceKey);

                if (device == null)
                {
                    TempData["error"] = Locale.Device + " "  + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(DeviceUtil.ToEditDeviceModel(device));
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
			return View(DeviceUtil.GetAllDevices(this.client));
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var restClient = new RestClientService(Constants.AMI);

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
				if (DeviceUtil.IsValidString(searchTerm))
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

            TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_DevicesPartial", devices);
		}

		[HttpGet]
		public ActionResult ViewDevice(string id)
		{
			Guid deviceKey = Guid.Empty;

			if (DeviceUtil.IsValidString(id) && Guid.TryParse(id, out deviceKey))
			{                
                var result = DeviceUtil.GetDevice(this.client, deviceKey);

                if(result == null)
                { 
                    TempData["error"] = Locale.Device + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(DeviceUtil.ToDeviceViewModel(result));
			}

			TempData["error"] = Locale.Device + " " + Locale.NotFound;

            return RedirectToAction("Index");
		}
	}
}