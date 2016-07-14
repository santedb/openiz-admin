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

using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.AppletModels.ViewModels;
using OpenIZAdmin.Models.CertificateModels.ViewModels;
using OpenIZAdmin.Models.DeviceModels.ViewModels;
using OpenIZAdmin.Models.UserAdministration.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	[TokenAuthorize]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			DashboardViewModel viewModel = new DashboardViewModel
			{
				Applets = new List<AppletViewModel>
				{
					new AppletViewModel("org.openiz.core", Guid.NewGuid(), "org.openiz.authentication", "0.5.0.0"),
					new AppletViewModel("org.openiz.core", Guid.NewGuid(), "org.openiz.patientAdministration", "0.5.0.0"),
					new AppletViewModel("org.openiz.core", Guid.NewGuid(), "org.openiz.patientEncounters", "0.5.0.0"),
				},
				CertificateRequests = new List<CertificateSigningRequestViewModel>
				{
					new CertificateSigningRequestViewModel("demo.openiz.org", DateTime.UtcNow, Guid.NewGuid()),
					new CertificateSigningRequestViewModel("arusha.openiz.org", DateTime.UtcNow, Guid.NewGuid()),
					new CertificateSigningRequestViewModel("zanzibar.openiz.org", DateTime.UtcNow, Guid.NewGuid()),
				},
				Devices = new List<DeviceViewModel>
				{
					new DeviceViewModel(DateTime.Now, "Nexus 5", null),
					new DeviceViewModel(DateTime.Now, "Nexus 7", null),
					new DeviceViewModel(new DateTime(DateTime.UtcNow.Year -1, DateTime.UtcNow.Month, DateTime.UtcNow.Day), "Samsung Galaxy 3", DateTime.UtcNow)
				},
				UserRoles = new List<UserRoleViewModel>(),
				Users = new List<UserViewModel>
				{
					new UserViewModel(Guid.NewGuid().ToString(), "nityan", "nityan@example.com", false),
					new UserViewModel(Guid.NewGuid().ToString(), "mo", "mo@example.com", false),
					new UserViewModel(Guid.NewGuid().ToString(), "justin", "justin@example.com", false),
					new UserViewModel(Guid.NewGuid().ToString(), "lockedout", "lockedout@example.com", true)
				}
			};

			return View(viewModel);
		}
	}
}