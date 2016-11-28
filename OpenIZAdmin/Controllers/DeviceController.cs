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
		/// <summary>
		/// Activates a device.
		/// </summary>
		/// <param name="model">The edit device model of the device to be activated.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Activate(string id)
		{
			var deviceKey = Guid.Empty;

			try
			{
				if (DeviceUtil.IsValidString(id) && Guid.TryParse(id, out deviceKey))
				{

					var deviceInfo = DeviceUtil.GetDevice(this.AmiClient, deviceKey);

					if (deviceInfo == null)
					{
						TempData["error"] = Locale.Device + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					deviceInfo.Device.ObsoletedBy = null;
					deviceInfo.Device.ObsoletionTime = null;

					this.AmiClient.UpdateDevice(id, deviceInfo);

					TempData["success"] = Locale.Device + " " + Locale.ActivatedSuccessfully;

					return RedirectToAction("Index");
				}
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to activate device: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to activate device: {0}", e.Message);
			}

			TempData["error"] = Locale.UnableToActivate + " " + Locale.Device;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Initialization method for Create action.
		/// </summary>        
		/// <returns>Returns the Create view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			var viewModel = new CreateDeviceModel
			{
				DeviceSecret = Guid.NewGuid().ToString()
			};

			return View(viewModel);
		}

		/// <summary>
		/// Creates a new device.
		/// </summary>
		/// <param name="model">The model with the name and device secret of the device to be created.</param>
		/// <returns>Returns the ViewDevice view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateDeviceModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var device = this.AmiClient.CreateDevice(DeviceUtil.ToSecurityDevice(model));

					TempData["success"] = Locale.Device + " " + Locale.CreatedSuccessfully;

					//return RedirectToAction("ViewDevice", new { key = device.Id.ToString() });
                    return RedirectToAction("Index");
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
		/// <returns>Returns the ViewDevice view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(string id)
		{
			if (DeviceUtil.IsValidString(id))
			{
				try
				{
					this.AmiClient.DeleteDevice(id);
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

		/// <summary>
		/// Deletes a policy associate to a device.
		/// </summary>
		/// <param name="key">The policy guid key of the policy to be deleted.</param>
		/// <returns>Returns the ViewDevice view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeletePolicy(Guid key)
		{
			//---TO DO!!!
			//apply and update to the device with the policies removed from the property
			//string id = string.Empty;
			string id = string.Empty;
			if (DeviceUtil.IsValidString(id))
			{
				try
				{
					this.AmiClient.DeleteDevice(id);
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

		/// <summary>
		/// Gets the device object to edit
		/// </summary>
		/// <param name="key">The id of the device to be edited.</param>
		/// <returns>Returns the Edit view.</returns>
		[HttpGet]
		public ActionResult Edit(string key)
		{
			var deviceKey = Guid.Empty;

			if (DeviceUtil.IsValidString(key) && Guid.TryParse(key, out deviceKey))
			{
				var device = DeviceUtil.GetDevice(this.AmiClient, deviceKey);

				if (device == null)
				{
					TempData["error"] = Locale.Device + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(DeviceUtil.ToEditDeviceModel(this.AmiClient, device));
			}

			TempData["error"] = Localization.Locale.Device + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Appies the changes to the device object
		/// </summary>
		/// <param name="model">The model containing the updated device information.</param>
		/// <returns>Returns the ViewDevice view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditDeviceModel model)
		{
			if (ModelState.IsValid)
			{
				var deviceEntity = DeviceUtil.GetDevice(this.AmiClient, model.Id);

				if (deviceEntity == null)
				{
					TempData["error"] = Locale.Device + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var addPolicies = new List<SecurityPolicy>();

				if (model.AddPoliciesList != null && model.AddPoliciesList.Any())
				{
					addPolicies = DeviceUtil.GetNewPolicies(this.AmiClient, model.AddPoliciesList);
				}

				var deviceInfo = DeviceUtil.ToSecurityDeviceInfo(model, deviceEntity, addPolicies);

				this.AmiClient.UpdateDevice(model.Id.ToString(), deviceInfo);

				TempData["success"] = Locale.Device + " " + Locale.UpdatedSuccessfully;

				//return RedirectToAction("ViewDevice", new { key = model.Id });

				return Redirect("Index");
			}
			else
			{
				return View(model);
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Device;

			return View(model);
		}

		/// <summary>
		/// Gets all device objects
		/// </summary>        
		/// <returns>Returns the index view.</returns>
		public ActionResult Index()
		{
			TempData["searchType"] = "Device";
			return View(DeviceUtil.GetAllDevices(this.AmiClient));
		}

		/// <summary>
		/// Gets a device list based on the search parameter applied to the Name field
		/// </summary>
		/// <param name="searchTerm">The search parameter to apply to the query.</param>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<DeviceViewModel> devices = new List<DeviceViewModel>();

			try
			{
				if (DeviceUtil.IsValidString(searchTerm))
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

		/// <summary>
		/// Searches for a device object with the supplied device id.
		/// </summary>
		/// <param name="id">The device identifier search parameter to apply to the query.</param>
		/// <returns>Returns the ViewDevice view.</returns>
		[HttpGet]
		public ActionResult ViewDevice(string id)
		{
			Guid deviceKey = Guid.Empty;

			if (DeviceUtil.IsValidString(id) && Guid.TryParse(id, out deviceKey))
			{
				var result = DeviceUtil.GetDevice(this.AmiClient, deviceKey);

				if (result == null)
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