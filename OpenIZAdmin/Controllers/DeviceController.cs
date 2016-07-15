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

using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models.DeviceModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	[TokenAuthorize]
	public class DeviceController : Controller
	{
		public ActionResult Index()
		{
			TempData["searchType"] = "Device";

			List<DeviceViewModel> viewModels = new List<DeviceViewModel>
			{
				new DeviceViewModel(DateTime.Now, "Nexus 5", null),
				new DeviceViewModel(DateTime.Now, "Nexus 7", null),
				new DeviceViewModel(new DateTime(DateTime.UtcNow.Year -1, DateTime.UtcNow.Month, DateTime.UtcNow.Day), "Samsung Galaxy 3", DateTime.UtcNow)
			};

			return View(viewModels);
		}

		[HttpGet]
		[ActionName("Search")]
		public async Task<ActionResult> SearchAsync(string searchTerm)
		{
			if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
			{
				List<DeviceViewModel> viewModels = new List<DeviceViewModel>
				{
					new DeviceViewModel(DateTime.Now, "Nexus 5", null),
					new DeviceViewModel(DateTime.Now, "Nexus 7", null)
				};

				return View("Index", viewModels);
			}

			TempData["error"] = "Invalid search, please check your search criteria";
			return View("Index", searchTerm);
		}
	}
}