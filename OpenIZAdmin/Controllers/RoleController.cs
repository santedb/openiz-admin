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
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.RoleModels;
using OpenIZAdmin.Models.RoleModels.ViewModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for administering roles.
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
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateRoleModel model)
		{
			if (ModelState.IsValid)
			{
				SecurityRoleInfo role = RoleUtil.ToSecurityRoleInfo(model);

				try
				{
					var result = this.client.CreateRole(role);

                    TempData["success"] = Locale.Role + " " + Locale.CreatedSuccessfully;

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

            TempData["error"] = Locale.UnableToCreate + " " + Locale.Role;

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(string id)
		{
			Guid userKey = Guid.Empty;

			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
			{
				try
				{
					this.client.DeleteRole(id);
                    TempData["success"] = Locale.Role + " " + Locale.DeletedSuccessfully;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to delete role: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to delete role: {0}", e.Message);
				}
			}

            TempData["error"] = Locale.UnableToDelete + " " + Locale.Role;

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

		/// <summary>
		/// Displays the edit view.
		/// </summary>
		/// <param name="id">The id of the role to edit.</param>
		/// <returns>Returns the edit view.</returns>
		[HttpGet]
		public ActionResult Edit(string id)
		{
			Guid roleId = Guid.Empty;

			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out roleId))
			{
				try
				{
					var role = this.client.GetRole(roleId.ToString());

					if (role == null)
					{
						TempData["error"] = Locale.Role + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					EditRoleModel model = new EditRoleModel();

					model.Description = role.Role.Description;
					model.Id = role.Id.ToString();
					model.Name = role.Name;

					return View(model);
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to find role: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to find role: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.Role + " " + Locale.NotFound;

            return RedirectToAction("Index");
		}

		/// <summary>
		/// Updates a role.
		/// </summary>
		/// <param name="model">The model containing the updated role information.</param>
		/// <returns>Returns the edit view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditRoleModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var role = this.client.GetRole(model.Id);

					if (role == null)
					{
						TempData["error"] = Locale.Role + " " + Locale.NotFound;

                        return RedirectToAction("Index");
					}

					role.Role.Description = model.Description;
					role.Name = model.Name;

					this.client.UpdateRole(role.Id.ToString(), role);

					TempData["success"] = Locale.Role + " " + Locale.UpdatedSuccessfully;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to update role: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to update role: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Role;

			return View(model);
		}

		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "Role";
			return View(RoleUtil.GetAllRoles(this.client));
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var restClient = new RestClientService(Constants.AMI);

			restClient.Accept = "application/xml";
			restClient.Credentials = new AmiCredentials(this.User, HttpContext.Request);

			this.client = new AmiServiceClient(restClient);

			base.OnActionExecuting(filterContext);
		}

		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<RoleViewModel> roles = new List<RoleViewModel>();

			try
			{
				if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
				{
					var collection = this.client.GetRoles(r => r.Name.Contains(searchTerm));

					TempData["searchTerm"] = searchTerm;

					return PartialView("_RolesPartial", collection.CollectionItem.Select(r => RoleUtil.ToRoleViewModel(r)));
				}
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to search roles: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to search roles: {0}", e.Message);
			}

            TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_RolesPartial", roles);
		}

		[HttpGet]
		public ActionResult ViewRole(string id)
		{
			Guid roleId = Guid.Empty;

			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out roleId))
			{
				var result = this.client.GetRoles(r => r.Key == roleId);

				if (result.CollectionItem.Count == 0)
				{
					TempData["error"] = Locale.Role + " " + Locale.NotFound;

                    return RedirectToAction("Index");
				}

				return View(RoleUtil.ToRoleViewModel(result.CollectionItem.Single()));
			}

			TempData["error"] = Locale.Role + " " + Locale.NotFound;

            return RedirectToAction("Index");
		}
	}
}