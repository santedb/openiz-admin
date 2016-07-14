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
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for administering users.
	/// </summary>
	[TokenAuthorize]
	public class UserAdministrationController : Controller
	{
		/// <summary>
		/// The internal reference to the administrative interface endpoint.
		/// </summary>
		private static readonly Uri amiEndpoint = AmiConfig.AmiEndpoint;

		/// <summary>
		/// The internal reference to the <see cref="System.Net.Http.HttpClient"/> instance.
		/// </summary>
		private HttpClient client;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Controllers.UserAdministrationController"/> class.
		/// </summary>
		public UserAdministrationController()
		{
		}

		[HttpGet]
		public ActionResult CreateRole()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateRole(object model)
		{
			if (ModelState.IsValid)
			{
			}

			TempData["error"] = "Unable to create role";

			return View(model);
		}

		[HttpGet]
		public ActionResult CreateUser()
		{
			return View();
		}

		[HttpPost]
		public ActionResult CreateUser(object model)
		{
			if (ModelState.IsValid)
			{
			}

			TempData["error"] = "Unable to create user";

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteRole(object model)
		{
			if (ModelState.IsValid)
			{
			}

			TempData["error"] = "Unable to delete role";

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteUser(object model)
		{
			if (ModelState.IsValid)
			{
			}

			TempData["error"] = "Unable to delete user";

			return View(model);
		}

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected override void Dispose(bool disposing)
		{
			Trace.TraceInformation("{0} disposing", nameof(CertificateController));

			this.client?.Dispose();

			base.Dispose(disposing);
		}

		[HttpGet]
		[ActionName("Role")]
		public ActionResult GetRole(string id)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
			{
			}

			TempData["error"] = "Role not found";

			return RedirectToAction("Index");
		}

		[HttpGet]
		[ActionName("Roles")]
		public ActionResult GetRoles()
		{
			return View();
		}

		[HttpGet]
		[ActionName("User")]
		public async Task<ActionResult> GetUserAsync(string id)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
			{
				var result = await this.client.GetAsync(string.Format("{0}/user/{1}", amiEndpoint, id));

				if (result.IsSuccessStatusCode)
				{
					var content = await result.Content.ReadAsStringAsync();

					Trace.TraceInformation(content);

					return View();
				}
			}

			TempData["error"] = "User not found";

			return RedirectToAction("Index");
		}

		[HttpGet]
		[ActionName("Users")]
		public async Task<ActionResult> GetUsersAsync()
		{
			var result = await this.client.GetAsync(string.Format("{0}/user/", amiEndpoint));

			if (result.IsSuccessStatusCode)
			{
			}

			TempData["error"] = "Unable to retrieve user list";

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[ActionName("Index")]
		public async Task<ActionResult> IndexAsync()
		{
			var result = await this.client.GetAsync(string.Format("{0}/user/", amiEndpoint));

			if (result.IsSuccessStatusCode)
			{
				var content = await result.Content.ReadAsStringAsync();

				Trace.TraceInformation(content);

				return View();
			}

			TempData["error"] = "Unable to retrieve user list";

			return RedirectToAction("Index", "Home");
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			this.client = new HttpClient();
			this.client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", HttpContext.Request.Cookies["access_token"].Value));

			base.OnActionExecuting(filterContext);
		}

		[HttpGet]
		public ActionResult UpdateUser()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UpdateUser(object model)
		{
			if (ModelState.IsValid)
			{
			}

			TempData["error"] = "Unable to update user";

			return View(model);
		}
	}
}