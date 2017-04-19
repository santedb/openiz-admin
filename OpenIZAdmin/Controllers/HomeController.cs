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
using Microsoft.AspNet.Identity;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.AppletModels;
using OpenIZAdmin.Models.CertificateModels;
using OpenIZAdmin.Models.DebugModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OpenIZAdmin.Models.DeviceModels;

namespace OpenIZAdmin.Controllers
{
    /// <summary>
    /// Provides operations for generic use.
    /// </summary>
    [TokenAuthorize]
	public class HomeController : BaseController
	{
		/// <summary>
		/// Gets all devices.
		/// </summary>
		/// <returns>Returns a list of all devices as device view model instances.</returns>
		private IEnumerable<DeviceViewModel> GetAllDevices()
		{
			return this.AmiClient.GetDevices(d => d.Name != string.Empty).CollectionItem.Select(d => new DeviceViewModel(d));
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		public ActionResult Index()
		{
			if (!RealmConfig.IsJoinedToRealm())
			{
				return RedirectToAction("JoinRealm", "Realm");
			}

			var viewModel = new DashboardViewModel
			{
				Applets = this.AmiClient.GetApplets().CollectionItem.Select(a => new AppletViewModel(a)),
				CertificateRequests = new List<CertificateSigningRequestViewModel>(), //CertificateUtil.GetAllCertificateSigningRequests(this.client),
				Devices = this.GetAllDevices().OrderBy(d => d.CreationTime).ThenBy(d => d.Name).Take(15),
				Roles = this.GetAllRoles().OrderBy(r => r.Name).Take(15)
			};

			return View(viewModel);
		}

		/// <summary>
		/// Gets the current user info and initiates the bug report page
		/// </summary>
		/// <returns>Returns the SubmitBugReport view.</returns>
		[HttpGet]
		public ActionResult SubmitBugReport()
		{
			try
			{
				var userId = Guid.Parse(User.Identity.GetUserId());

				var securityUserInfo = this.AmiClient.GetUser(userId.ToString());

				if (securityUserInfo == null)
				{
					TempData["error"] = Locale.User + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var model = new SubmitBugReportModel(securityUserInfo);

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.User + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the create view.
		/// </summary>
		/// <param name="model">The model containing the bug report information.</param>
		/// <returns>Returns the Index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SubmitBugReport(SubmitBugReportModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var report = model.ToDiagnosticReport(this.GetUserEntityBySecurityUserKey(Guid.Parse(this.User.Identity.GetUserId())));
					report = AmiClient.SubmitDiagnosticReport(report);
					model.TransactionMessage = report.CorrelationId;
					model.Success = true;
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			return View(model);
		}

		/// <summary>
		/// Gets the version information of the current application.
		/// </summary>
		/// <returns>Returns the version information.</returns>
		[HttpGet]
		public ActionResult VersionInformation()
		{
			var viewModel = new VersionViewModel(typeof(MvcApplication).Assembly);

			try
			{
				viewModel.Assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Select(a => new AssemblyInfoViewModel(a)).Where(a => a.Title != null).OrderBy(a => a.Title));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			return View(viewModel);
		}
	}
}