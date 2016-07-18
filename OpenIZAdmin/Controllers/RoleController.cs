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
 * Date: 2016-7-17
 */

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models.RoleModels;
using OpenIZAdmin.Models.RoleModels.ViewModels;
using OpenIZAdmin.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for administering users.
	/// </summary>
	[TokenAuthorize]
	public class RoleController : Controller
	{
		/// <summary>
		/// The internal reference to the administrative interface endpoint.
		/// </summary>
		private static readonly Uri amiEndpoint = new Uri(RealmConfig.GetCurrentRealm().AmiEndpoint);

		/// <summary>
		/// The internal reference to the <see cref="OpenIZAdmin.Services.RestClient"/> instance.
		/// </summary>
		private RestClient client;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Controllers.RoleController"/> class.
		/// </summary>
		public RoleController()
		{
		}

		[HttpGet]
		public ActionResult CreateRole()
		{
			return View();
		}

		[HttpPost]
		[ActionName("CreateRole")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateRoleAsync(CreateRoleModel model)
		{
			if (ModelState.IsValid)
			{
				SecurityRoleInfo role = model.ToSecurityRoleInfo();

				var result = await this.client.PostAsync("/role/", role);

				if (result.IsSuccessStatusCode)
				{
					TempData["success"] = "Role created successfully";

					return RedirectToAction("Index");
				}
			}

			TempData["error"] = "Unable to create role";

			return View(model);
		}

		[HttpPost]
		[ActionName("DeleteRole")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteRoleAsync(Guid id)
		{
			if (id != Guid.Empty)
			{
				var result = await this.client.DeleteAsync(string.Format("/role/{0}", id));

				if (result.IsSuccessStatusCode)
				{
					TempData["success"] = "User deleted successfully";

					return RedirectToAction("Index");
				}
			}

			TempData["error"] = "Unable to delete role";

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected override void Dispose(bool disposing)
		{
			Trace.TraceInformation("{0} disposing", nameof(RoleController));

			this.client?.Dispose();

			base.Dispose(disposing);
		}

		[HttpGet]
		[ActionName("Role")]
		public async Task<ActionResult> GetRoleAsync(Guid id)
		{
			if (id != Guid.Empty)
			{
				var result = await this.client.GetAsync<SecurityRoleInfo>(string.Format("/role/{0}", id));

				if (result == null)
				{
					TempData["error"] = "Role not found";

					return RedirectToAction("Index");
				}

				return View(new RoleViewModel(result));
			}

			TempData["error"] = "Role not found";

			return RedirectToAction("Index");
		}

		[HttpGet]
		[ActionName("Roles")]
		public async Task<ActionResult> GetRolesAsync()
		{
			var result = await this.client.GetAsync(string.Format("/roles/"));

			if (result.IsSuccessStatusCode)
			{
				var content = await result.Content.ReadAsAsync<AmiCollection<SecurityRoleInfo>>();

				return View(content.CollectionItem.Select(r => new RoleViewModel(r)));
			}

			TempData["error"] = "Unable to retrieve role list";

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[ActionName("Index")]
		public async Task<ActionResult> IndexAsync()
		{
			var result = await this.client.GetAsync(string.Format("/roles/"));

			if (result.IsSuccessStatusCode)
			{
				var content = await result.Content.ReadAsAsync<AmiCollection<SecurityRoleInfo>>();

				return View(content.CollectionItem.Select(r => new RoleViewModel(r)));
			}

			TempData["error"] = "Unable to retrieve role list";

			return RedirectToAction("Index", "Home");
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			this.client = new RestClient(amiEndpoint, new Credentials(HttpContext.Request));

			base.OnActionExecuting(filterContext);
		}
	}
}