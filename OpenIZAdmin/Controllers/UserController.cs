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
using OpenIZ.Core.Model.Query;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models.UserModels;
using OpenIZAdmin.Models.UserModels.ViewModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for administering users.
	/// </summary>
	[TokenAuthorize]
	public class UserController : Controller
	{
		/// <summary>
		/// The internal reference to the <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient client;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Controllers.UserController"/> class.
		/// </summary>
		public UserController()
		{
		}

		/// <summary>
		/// Displays the create user view.
		/// </summary>
		/// <returns>Returns the create user view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			CreateUserModel model = new CreateUserModel();
			model.RolesList.Add(new SelectListItem { Text = "", Value = "" });

			model.RolesList.AddRange(RoleUtil.GetAllRoles(this.client).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }));

			return View(model);
		}

		/// <summary>
		/// Creates a user.
		/// </summary>
		/// <param name="model">The model containing the information about the user.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateUserModel model)
		{
			if (ModelState.IsValid)
			{
				SecurityUserInfo user = UserUtil.ToSecurityUserInfo(model);

				try
				{
					var result = this.client.CreateUser(user);

					TempData["success"] = "User created successfully";

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to create user: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to create user: {0}", e.Message);
				}
			}

			TempData["error"] = "Unable to create user";

			return View(model);
		}

		/// <summary>
		/// Deletes a user.
		/// </summary>
		/// <param name="id">The id of the user to be deleted.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(string id)
		{
			Guid userKey = Guid.Empty;

			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
			{
				try
				{
					this.client.DeleteUser(id);
					TempData["success"] = "User deleted successfully";

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to delete user: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to delete user: {0}", e.Message);
				}
			}

			TempData["error"] = "Unable to delete user";

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected override void Dispose(bool disposing)
		{
			Trace.TraceInformation("{0} disposing", nameof(UserController));

			this.client?.Dispose();

			base.Dispose(disposing);
		}

		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "User";
			return View(UserUtil.GetAllUsers(this.client));
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var restClient = new RestClientService("AMI");

			restClient.Accept = "application/xml";
			restClient.Credentials = new AmiCredentials(this.User, HttpContext.Request);

			this.client = new AmiServiceClient(restClient);

			base.OnActionExecuting(filterContext);
		}

		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<UserViewModel> users = new List<UserViewModel>();

			try
			{
				if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
				{
					var collection = this.client.GetUsers(u => u.UserName.Contains(searchTerm));

					TempData["searchTerm"] = searchTerm;

					return PartialView("_UsersPartial", collection.CollectionItem.Select(u => UserUtil.ToUserViewModel(u)));
				}
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to search roles: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to search roles: {0}", e.Message);
			}

			TempData["error"] = "Invalid search, please check your search criteria";
			TempData["searchTerm"] = searchTerm;

			return PartialView("_UsersPartial", users);
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

		[HttpGet]
		public ActionResult ViewUser(string id)
		{
			Guid userId = Guid.Empty;

			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out userId))
			{
				var result = this.client.GetUsers(u => u.Key == userId);

				if (result.CollectionItem.Count == 0)
				{
					TempData["error"] = Localization.Resources.UserNotFound;

					return RedirectToAction("Index");
				}

				return View(UserUtil.ToUserViewModel(result.CollectionItem.Single()));
			}

			TempData["error"] = Localization.Resources.UserNotFound;

			return RedirectToAction("Index");
		}
	}
}