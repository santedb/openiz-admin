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

using System;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.CertificateModels.ViewModels;
using OpenIZAdmin.Util;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OpenIZAdmin.Models.DebugModels;
using OpenIZAdmin.Models.DebugModels.ViewModels;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for generic use.
	/// </summary>
	[TokenAuthorize]
	public class HomeController : BaseController
	{
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

			DashboardViewModel viewModel = new DashboardViewModel
			{
				Applets = AppletUtil.GetApplets(this.AmiClient),
				CertificateRequests = new List<CertificateSigningRequestViewModel>(), //CertificateUtil.GetAllCertificateSigningRequests(this.client),
				Devices = DeviceUtil.GetAllDevices(this.AmiClient).OrderBy(d => d.CreationTime).ThenBy(d => d.Name).Take(15),
				Roles = RoleUtil.GetAllRoles(this.AmiClient).OrderBy(r => r.Name).Take(15),
				Users = UserUtil.GetAllUsers(this.AmiClient).OrderBy(u => u.Username).Take(15)
			};

			return View(viewModel);
		}

		/// <summary>
		/// Gets the version information of the current application.
		/// </summary>
		/// <returns>Returns the version information.</returns>
		[HttpGet]
		[TokenAuthorize]
		public ActionResult VersionInformation()
		{
			var viewModel = new VersionViewModel(typeof(MvcApplication).Assembly);

			viewModel.Assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Select(a => new AssemblyInfoViewModel(a)).Where(a => a.Title != null).OrderBy(a => a.Title));

			return View(viewModel);
		}
	}
}