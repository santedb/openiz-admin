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

using Microsoft.AspNet.Identity;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.AppletModels;
using OpenIZAdmin.Models.DebugModels;
using OpenIZAdmin.Models.DeviceModels;
using OpenIZAdmin.Models.RoleModels;
using OpenIZAdmin.Services.Applets;
using OpenIZAdmin.Services.Security;
using OpenIZAdmin.Services.Security.Devices;
using OpenIZAdmin.Services.Security.Roles;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using OpenIZAdmin.Core.Engine;
using OpenIZAdmin.Models.DebugModels.ServerInformationViewModels;
using OpenIZAdmin.Services.Server;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for generic use.
	/// </summary>
	[TokenAuthorize]
	public class HomeController : BaseController
	{
		/// <summary>
		/// The AMI server information service.
		/// </summary>
		private readonly IAmiServerInformationService amiServerInformationService;

		/// <summary>
		/// The applet service.
		/// </summary>
		private readonly IAppletService appletService;

		/// <summary>
		/// The IMSI server information service.
		/// </summary>
		private readonly IImsiServerInformationService imsiServerInformationService;

		/// <summary>
		/// The security device service.
		/// </summary>
		private readonly ISecurityDeviceService securityDeviceService;

		/// <summary>
		/// The security role service.
		/// </summary>
		private readonly ISecurityRoleService securityRoleService;

		/// <summary>
		/// Initializes a new instance of the <see cref="HomeController" /> class.
		/// </summary>
		/// <param name="appletService">The applet service.</param>
		/// <param name="securityDeviceService">The security device service.</param>
		/// <param name="securityRoleService">The security role service.</param>
		/// <param name="amiServerInformationService">The ami server information service.</param>
		/// <param name="imsiServerInformationService">The imsi server information service.</param>
		public HomeController(IAppletService appletService, ISecurityDeviceService securityDeviceService, ISecurityRoleService securityRoleService, IAmiServerInformationService amiServerInformationService, IImsiServerInformationService imsiServerInformationService)
		{
			this.appletService = appletService;
			this.securityDeviceService = securityDeviceService;
			this.securityRoleService = securityRoleService;
			this.amiServerInformationService = amiServerInformationService;
			this.imsiServerInformationService = imsiServerInformationService;
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

			var viewModel = new DashboardViewModel();

			try
			{
				if (PolicyPermission.TryDemand(this.User, Constants.UnrestrictedMetadata))
				{
					viewModel.Applets = appletService.GetAllApplets().Select(a => new AppletViewModel(a));
				}

				if (PolicyPermission.TryDemand(this.User, Constants.CreateDevice))
				{
					viewModel.Devices = securityDeviceService.GetAllDevices().Select(d => new DeviceViewModel(d)).OrderBy(d => d.CreationTime).ThenBy(d => d.Name).Take(15);
				}

				if (PolicyPermission.TryDemand(this.User, Constants.AlterRoles))
				{
					viewModel.Roles = securityRoleService.GetAllRoles().OrderBy(r => r.Name).Take(15).Select(r => new RoleViewModel(r));
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to create dashboard view model: {e}");
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

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
					TempData["error"] = Locale.UserNotFound;

					return RedirectToAction("Index");
				}

				var model = new SubmitBugReportModel(securityUserInfo);

				return View(model);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to submit bug report: {e}");
			}

			TempData["error"] = Locale.UserNotFound;

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
				Trace.TraceError($"Unable to submit bug report: {e}");
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
				var amiServerInformation = new ServerInformationViewModel(this.amiServerInformationService.GetServerInformation(), amiServerInformationService.Name);
				var imsiServerInformation = new ServerInformationViewModel(this.imsiServerInformationService.GetServerInformation(), imsiServerInformationService.Name);

				viewModel.ServerInformation.Add(amiServerInformation);
				viewModel.ServerInformation.Add(imsiServerInformation);

				viewModel.Assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Select(a => new AssemblyInfoViewModel(a)).Where(a => a.Title != null).OrderBy(a => a.Title));
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve version information: {e}");
			}

			return View(viewModel);
		}
	}
}