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
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models.RoleModels;
using OpenIZAdmin.Models.RoleModels.ViewModels;
using OpenIZAdmin.Services;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
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
		/// The internal reference to the <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient client;

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
		[ValidateAntiForgeryToken]
		public ActionResult CreateRole(CreateRoleModel model)
		{
			if (ModelState.IsValid)
			{
				SecurityRoleInfo role = model.ToSecurityRoleInfo();

				try
				{
					var result = this.client.CreateRole(role);

					TempData["success"] = "Role created successfully";

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to create role: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to create role: {0}", e.Message);
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
			//if (id != Guid.Empty)
			//{
			//	var result = await this.client.DeleteAsync(string.Format("/role/{0}", id));

			//	if (result.IsSuccessStatusCode)
			//	{
			//		TempData["success"] = "User deleted successfully";

			//		return RedirectToAction("Index");
			//	}
			//}

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
		public ActionResult GetRole(string id)
		{
			Guid roleId = Guid.Empty;

			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out roleId))
			{
				var result = this.client.GetRoles(r => r.Key == roleId);

				if (result.CollectionItem.Count == 0)
				{
					TempData["error"] = Localization.Resources.RoleNotFound;

					return RedirectToAction("Index");
				}

				return View(new RoleViewModel(result.CollectionItem.Single()));
			}

			TempData["error"] = Localization.Resources.RoleNotFound;

			return RedirectToAction("Index");
		}

		[HttpGet]
		[ActionName("Roles")]
		public ActionResult GetRoles()
		{
			try
			{
				// HACK
				var roles = this.client.GetRoles(r => r.Name != null);

				return View(roles.CollectionItem.Select(r => new RoleViewModel(r)));
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve roles: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve roles: {0}", e.Message);
			}

			TempData["error"] = "Unable to retrieve role list";

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public ActionResult Index()
		{
			try
			{
				// HACK
				var roles = this.client.GetRoles(r => r.Name != null);

				return View(roles.CollectionItem.Select(r => new RoleViewModel(r)));
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve roles: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve roles: {0}", e.Message);
			}

			TempData["error"] = "Unable to retrieve role list";

			return RedirectToAction("Index", "Home");
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