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
 * User: Nityan
 * Date: 2016-7-8
 */

using Elmah;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.DeviceModels;
using OpenIZAdmin.Models.DeviceModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
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
		/// Activate a device.
		/// </summary>
		/// <param name="id">The id of the device to activate.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Activate(Guid id)
		{
			try
			{
				var securityDeviceInfo = this.AmiClient.GetDevice(id.ToString());

				if (securityDeviceInfo == null)
				{
					TempData["error"] = Locale.Device + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				securityDeviceInfo.Id = id;
				securityDeviceInfo.Device.ObsoletedBy = null;
				securityDeviceInfo.Device.ObsoletionTime = null;

				this.AmiClient.UpdateDevice(id.ToString(), securityDeviceInfo);

				TempData["success"] = Locale.Device + " " + Locale.Activated + " " + Locale.Successfully;

				return RedirectToAction("ViewDevice", new { id = securityDeviceInfo.Id });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToActivate + " " + Locale.Device;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the create device view.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			var viewModel = new CreateDeviceModel
			{
				DeviceSecret = Guid.NewGuid().ToString().ToUpper()
			};

			return View(viewModel);
		}

		/// <summary>
		/// Creates a device.
		/// </summary>
		/// <param name="model">The model containing the device information.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateDeviceModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var device = this.AmiClient.CreateDevice(model.ToSecurityDeviceInfo());

					TempData["success"] = Locale.Device + " " + Locale.Created + " " + Locale.Successfully;

					return RedirectToAction("ViewDevice", new { id = device.Id });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToCreate + " " + Locale.Device;

			return View(model);
		}

		/// <summary>
		/// Deletes a device.
		/// </summary>
		/// <param name="id">The id of the device to delete.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
				this.AmiClient.DeleteDevice(id.ToString());
				TempData["success"] = Locale.Device + " " + Locale.Deleted + " " + Locale.Successfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToDelete + " " + Locale.Device;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the edit device view.
		/// </summary>
		/// <param name="id">The id of the device to edit.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			try
			{
				var securityDeviceInfo = this.AmiClient.GetDevice(id.ToString());

				if (securityDeviceInfo == null)
				{
					TempData["error"] = Locale.Device + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var model = new EditDeviceModel(securityDeviceInfo);

				model.PoliciesList.Add(new SelectListItem { Text = string.Empty, Value = string.Empty });
				model.PoliciesList.AddRange(CommonUtil.GetAllPolicies(this.AmiClient).Select(r => new SelectListItem { Text = r.Name, Value = r.Key.ToString() }).OrderBy(q => q.Text));

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.Device + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Updates a device.
		/// </summary>
		/// <param name="model">The model containing the device information.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Exclude = "CreationTime")] EditDeviceModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var securityDeviceInfo = this.AmiClient.GetDevice(model.Id.ToString());

					if (securityDeviceInfo == null)
					{
						TempData["error"] = Locale.Device + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					var deviceInfo = DeviceUtil.ToSecurityDeviceInfo(this.AmiClient, model, securityDeviceInfo);

					this.AmiClient.UpdateDevice(model.Id.ToString(), deviceInfo);

					TempData["success"] = Locale.Device + " " + Locale.Updated + " " + Locale.Successfully;

					return RedirectToAction("ViewDevice", new { id = securityDeviceInfo.Id.ToString() });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Device;

			return View(model);
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		public ActionResult Index()
		{
			TempData["searchType"] = "Device";
			return View(new List<DeviceViewModel>());
		}

		/// <summary>
		/// Gets a list of devices based on a search term.
		/// </summary>
		/// <param name="searchTerm">The search term to use to find the devices.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<DeviceViewModel> devices = new List<DeviceViewModel>();

			try
			{
				if (CommonUtil.IsValidString(searchTerm))
				{
					var collection = this.AmiClient.GetDevices(d => d.Name.Contains(searchTerm));

					TempData["searchTerm"] = searchTerm;

					return PartialView("_DevicesPartial", collection.CollectionItem.Select(d => new DeviceViewModel(d)));
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_DevicesPartial", devices);
		}

		/// <summary>
		/// Displays the view device view.
		/// </summary>
		/// <param name="id">The id of the device to view.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult ViewDevice(Guid id)
		{
			try
			{
				var result = this.AmiClient.GetDevice(id.ToString());

				if (result == null)
				{
					TempData["error"] = Locale.Device + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(new DeviceViewModel(result));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.Device + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}
	}
}