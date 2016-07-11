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
 * Date: 2016-7-10
 */
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.DeviceModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	[TokenAuthorize]
	public class SearchController : Controller
	{
		public SearchController()
		{

		}

		[HttpPost]
		[ActionName("Search")]
		public async Task<ActionResult> SearchAsync(SearchType searchType, SearchModel model)
		{
			string partialViewName = null;

			switch (searchType)
			{
				case SearchType.Applet:
					partialViewName = "_AppletsPartial";
					break;
				case SearchType.Certificate:
					partialViewName = "_CertificateRequestsPartial";
					break;
				case SearchType.Device:
					partialViewName = "_DevicesPartial";
					break;
				case SearchType.Role:
					partialViewName = "_UserRolesPartial";
					break;
				case SearchType.User:
					partialViewName = "_UsersPartial";
					break;
				default:
					return Redirect(Request.UrlReferrer.ToString());
			}

			if (ModelState.IsValid)
			{
				List<DeviceViewModel> viewModels = new List<DeviceViewModel>
				{
					new DeviceViewModel(DateTime.Now, "Nexus 5", null),
					new DeviceViewModel(DateTime.Now, "Nexus 7", null)
				};

				return PartialView(partialViewName, viewModels);
			}

			return Redirect(Request.UrlReferrer.ToString());
		}
	}
}