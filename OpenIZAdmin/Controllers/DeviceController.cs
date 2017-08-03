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

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.DeviceModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Services.Http.Security;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing devices.
	/// </summary>
	[TokenAuthorize(Constants.CreateDevice)]
	public class DeviceController : SecurityBaseController
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
					TempData["error"] = Locale.DeviceNotFound;

					return RedirectToAction("Index");
				}

				securityDeviceInfo.Id = id;
				securityDeviceInfo.Device.ObsoletedBy = null;
				securityDeviceInfo.Device.ObsoletionTime = null;

				this.AmiClient.UpdateDevice(id.ToString(), securityDeviceInfo);

				TempData["success"] = Locale.DeviceActivatedSuccessfully;

				return RedirectToAction("ViewDevice", new { id = securityDeviceInfo.Id });
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to activate device: {e}");
			}

			TempData["error"] = Locale.UnableToActivateDevice;

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
					var synchronizersRole = this.AmiClient.GetRoles(r => r.Name == "SYNCHRONIZERS").CollectionItem.FirstOrDefault();
					var deviceRole = this.AmiClient.GetRoles(r => r.Name == "DEVICE").CollectionItem.FirstOrDefault();

					var device = this.AmiClient.CreateDevice(model.ToSecurityDeviceInfo());

					var securityUserInfo = new SecurityUserInfo
					{
						Password = model.DeviceSecret,
						Roles = new List<SecurityRoleInfo>
						{
							deviceRole,
							synchronizersRole
						},
						UserName = model.Name,
						User = new SecurityUser
						{
							Key = Guid.NewGuid(),
							UserClass = UserClassKeys.ApplicationUser,
							UserName = model.Name,
							SecurityHash = Guid.NewGuid().ToString()
						},
					};

					this.AmiClient.CreateUser(securityUserInfo);

					TempData["success"] = Locale.DeviceCreatedSuccessfully;

					return RedirectToAction("ViewDevice", new { id = device.Id });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to create device: {e}");
			}

			TempData["error"] = Locale.UnableToCreateDevice;

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

				TempData["success"] = Locale.DeviceDeactivatedSuccessfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to delete device: {e}");
			}

			TempData["error"] = Locale.UnableToDeleteDevice;

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
					TempData["error"] = Locale.DeviceNotFound;

					return RedirectToAction("Index");
				}

				var model = new EditDeviceModel(securityDeviceInfo);

				model.PoliciesList.AddRange(this.GetAllPolicies().ToSelectList("Name", "Id", null, true));

				return View(model);
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
				Trace.TraceError($"Unable to retrieve device: {e}");
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Updates a device.
		/// </summary>
		/// <param name="model">The model containing the device information.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Exclude = nameof(EditDeviceModel.CreationTime))] EditDeviceModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var securityDeviceInfo = this.AmiClient.GetDevice(model.Id.ToString());

					if (securityDeviceInfo == null)
					{
						TempData["error"] = Locale.DeviceNotFound;

						return RedirectToAction("Index");
					}

					this.AmiClient.UpdateDevice(model.Id.ToString(), this.ToSecurityDeviceInfo(model, securityDeviceInfo));

					TempData["success"] = Locale.DeviceUpdatedSuccessfully;

					return RedirectToAction("ViewDevice", new { id = securityDeviceInfo.Id.ToString() });
				}

                model.PoliciesList.AddRange(this.GetAllPolicies().ToSelectList("Name", "Id", null, true));
            }
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update device: {e}");
			}

			TempData["error"] = Locale.UnableToUpdateDevice;            

            return View(model);
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		public ActionResult Index()
		{
			TempData["searchType"] = "Device";
            TempData["searchTerm"] = "*";
            return View();
		}

		/// <summary>
		/// Gets a list of devices based on a search term.
		/// </summary>
		/// <param name="searchTerm">The search term to use to find the devices.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		[ValidateInput(false)]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<DeviceViewModel> devices = new List<DeviceViewModel>();

			try
			{
				if (this.IsValidId(searchTerm))
				{
					var results = new List<SecurityDeviceInfo>();

					results.AddRange(searchTerm == "*" ? this.AmiClient.GetDevices(a => a.Id != null).CollectionItem : this.AmiClient.GetDevices(a => a.Name.Contains(searchTerm)).CollectionItem);

					TempData["searchTerm"] = searchTerm;

					return PartialView("_DevicesPartial", results.Select(d => new DeviceViewModel(d)).OrderBy(a => a.Name));
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve devices: {e}");
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
					TempData["error"] = Locale.DeviceNotFound;

					return RedirectToAction("Index");
				}

				return View(new DeviceViewModel(result));
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
				Trace.TraceError($"Unable to retrieve device: {e}");
			}

			return RedirectToAction("Index");
		}
	}
}