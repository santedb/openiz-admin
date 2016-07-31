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
 * User: khannan
 * Date: 2016-5-31
 */

using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.AppletModels.ViewModels;
using OpenIZAdmin.Models.CertificateModels.ViewModels;
using OpenIZAdmin.Models.DeviceModels.ViewModels;
using OpenIZAdmin.Models.RoleModels.ViewModels;
using OpenIZAdmin.Models.UserModels.ViewModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	[AllowAnonymous]
	public class HomeController : Controller
	{
		/// <summary>
		/// The internal reference to the <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient client;

		public ActionResult Index()
		{
			if (!RealmConfig.IsJoinedToRealm())
			{
				return RedirectToAction("JoinRealm", "Realm");
			}

			if (User.Identity.IsAuthenticated)
			{
				DashboardViewModel viewModel = new DashboardViewModel
				{
					Applets = new List<AppletViewModel>
					{
						new AppletViewModel("org.openiz.core", Guid.NewGuid(), "org.openiz.authentication", "0.5.0.0"),
						new AppletViewModel("org.openiz.core", Guid.NewGuid(), "org.openiz.patientAdministration", "0.5.0.0"),
						new AppletViewModel("org.openiz.core", Guid.NewGuid(), "org.openiz.patientEncounters", "0.5.0.0"),
					},
					CertificateRequests = new List<CertificateSigningRequestViewModel>(),//CertificateUtil.GetAllCertificateSigningRequests(this.client),
					Devices = DeviceUtil.GetAllDevices(this.client).OrderBy(d => d.CreationTime).ThenBy(d => d.Name),
					Roles = RoleUtil.GetAllRoles(this.client).OrderBy(r => r.Name),
					Users = UserUtil.GetAllUsers(this.client).OrderBy(u => u.Username)
				};

				return View(viewModel);
			}

			return RedirectToAction("Login", "Account", new { returnUrl = Request.UrlReferrer?.ToString() });
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var restClient = new RestClientService("AMI");

			restClient.Accept = "application/xml";
			restClient.Credentials = new AmiCredentials(this.User, HttpContext.Request);

			this.client = new AmiServiceClient(restClient);

			base.OnActionExecuting(filterContext);
		}
	}
}