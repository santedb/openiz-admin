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

using OpenIZAdmin.Models;
using OpenIZAdmin.Models.AppletModels.ViewModels;
using OpenIZAdmin.Models.CertificateModels.ViewModels;
using OpenIZAdmin.Models.DeviceModels.ViewModels;
using OpenIZAdmin.Models.RoleModels.ViewModels;
using OpenIZAdmin.Models.UserAdministration.ViewModels;
using OpenIZAdmin.Models.UserModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	[AllowAnonymous]
	public class HomeController : Controller
	{
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
					CertificateRequests = new List<CertificateSigningRequestViewModel>
					{
						new CertificateSigningRequestViewModel
						{
							AdministrativeContactEmail = "nityan.khanna@mohawkcollege.ca",
							AdministrativeContactName = "Nityan Khanna",
							DistinguishedName = "demo.openiz.org",
							SubmissionTime = DateTime.Now
						},
						new CertificateSigningRequestViewModel
						{
							AdministrativeContactEmail = "justin.fyfe1@mohawkcollege.ca",
							AdministrativeContactName = "Justin Fyfe",
							DistinguishedName = "arusha.openiz.org",
							SubmissionTime = DateTime.Now
						},
						new CertificateSigningRequestViewModel
						{
							AdministrativeContactEmail = "mohamed.ibrahim1@mohawkcollege.ca",
							AdministrativeContactName = "Mohamed Ibrahim",
							DistinguishedName = "zanzibar@openiz.org",
							SubmissionTime = DateTime.Now
						}
					},
					Devices = new List<DeviceViewModel>
					{
						new DeviceViewModel(DateTime.Now, "Nexus 5", null),
						new DeviceViewModel(DateTime.Now, "Nexus 7", null),
						new DeviceViewModel(new DateTime(DateTime.UtcNow.Year -1, DateTime.UtcNow.Month, DateTime.UtcNow.Day), "Samsung Galaxy 3", DateTime.UtcNow)
					},
					Roles = new List<RoleViewModel>
					{
						new RoleViewModel
						{
							Description = "Group for users who have administrative access",
							Id = Guid.Parse("f6d2ba1d-5bb5-41e3-b7fb-2ec32418b2e1"),
							Name = "ADMINISTRATORS"
						}
					},
					UserRoles = new List<UserRoleViewModel>(),
					Users = new List<UserViewModel>
					{
						new UserViewModel
						{
							Email = "administrator@openiz.org",
							IsLockedOut = false,
							UserId = Guid.Parse("f8f4449f-6b4c-e611-bf27-f46d0450b928"),
							Username = "Administrator"
						},
						new UserViewModel
						{
							Email = "nityan@example.com",
							UserId = Guid.NewGuid(),
							IsLockedOut = false,
							Username = "nityan"
						},
						new UserViewModel
						{
							Email = "mo@example.com",
							UserId = Guid.NewGuid(),
							IsLockedOut = false,
							Username = "mo"
						},
						new UserViewModel
						{
							Email = "justin@example.com",
							UserId = Guid.NewGuid(),
							IsLockedOut = false,
							Username = "justin"
						},
						new UserViewModel
						{
							Email = "lockedout@example.com",
							UserId = Guid.NewGuid(),
							IsLockedOut = false,
							Username = "lockedout"
						}
					}
				};

				return View(viewModel);
			}

			return RedirectToAction("Login", "Account", new { returnUrl = Request.UrlReferrer?.ToString() });
		}
	}
}