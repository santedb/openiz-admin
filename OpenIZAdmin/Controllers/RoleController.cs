/*
 * Copyright 2016-2017 Mohawk College of Applied Arts and Technology
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
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.PolicyModels;
using OpenIZAdmin.Models.RoleModels;
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
	[TokenAuthorize(Constants.AlterRoles)]
	public class RoleController : SecurityBaseController
	{
		/// <summary>
		/// Activates the specified Role.
		/// </summary>
		/// <param name="id">The role identifier.</param>
		/// <returns>ActionResult.</returns>
		public ActionResult Activate(Guid id)
		{
			try
			{
				var securityRoleInfo = this.AmiClient.GetRole(id.ToString());

				if (securityRoleInfo == null)
				{
					TempData["error"] = Locale.RoleNotFound;

					return RedirectToAction("Index");
				}

				securityRoleInfo.Role.CreationTime = DateTimeOffset.Now;
				securityRoleInfo.Role.ObsoletedByKey = null;
				securityRoleInfo.Role.ObsoletionTime = null;

				var result = this.AmiClient.UpdateRole(id.ToString(), securityRoleInfo);

				TempData["success"] = Locale.RoleActivatedSuccessfully;

				return RedirectToAction("ViewRole", new { id = result.Id });
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to activate role: { e }");
			}

			TempData["error"] = Locale.UnableToActivateRole;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the create role view.
		/// </summary>
		/// <returns>Returns the create role view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// Displays the create view.
		/// </summary>
		/// <param name="model">The model containing the new role information.</param>
		/// <returns>Returns the Index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateRoleModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var role = this.AmiClient.CreateRole(model.ToSecurityRoleInfo());

					TempData["success"] = Locale.RoleCreatedSuccessfully;

					return RedirectToAction("ViewRole", new { id = role.Id.ToString() });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to create role: {e}");
			}

			TempData["error"] = Locale.UnableToCreateRole;

			return View(model);
		}

		/// <summary>
		/// Displays the edit view.
		/// </summary>
		/// <param name="id">The id of the role to delete.</param>
		/// <returns>Returns the Index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
				this.AmiClient.DeleteRole(id.ToString());

				TempData["success"] = Locale.RoleDeactivatedSuccessfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to delete role: {e}");
			}

			TempData["error"] = Locale.UnableToDeleteRole;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the edit view.
		/// </summary>
		/// <param name="id">The id of the role to edit.</param>
		/// <returns>Returns the edit view.</returns>
		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			try
			{
				var securityRoleInfo = this.AmiClient.GetRole(id.ToString());

				if (securityRoleInfo == null)
				{
					TempData["error"] = Locale.RoleNotFound;

					return RedirectToAction("Index");
				}

				var model = new EditRoleModel(securityRoleInfo)
				{
					PoliciesList = new List<SelectListItem>()
				};

				model.PoliciesList.AddRange(this.GetAllPolicies().ToSelectList("Name", "Id", null, true));

				return View(model);
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
				Trace.TraceError($"Unable to retrieve role: {e}");
			}

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
			SecurityRoleInfo roleInfo = null;

			if (ModelState.IsValid)
			{
				try
				{
					roleInfo = this.AmiClient.GetRole(model.Id.ToString());

					if (roleInfo == null)
					{
						TempData["error"] = Locale.RoleNotFound;

						return RedirectToAction("Index");
					}

					this.AmiClient.UpdateRole(roleInfo.Id.ToString(), this.ToSecurityRoleInfo(model, roleInfo));

					TempData["success"] = Locale.RoleUpdatedSuccessfully;

					return RedirectToAction("ViewRole", new { id = roleInfo.Id.ToString() });
				}
				catch (Exception e)
				{
					Trace.TraceError($"Unable to update role: {e}");
				}
			}

			if (roleInfo != null)
			{
				model.RolePolicies = roleInfo.Policies.Select(p => new PolicyViewModel(p)).OrderBy(q => q.Name).ToList();
			}

			model.PoliciesList = new List<SelectListItem>();
			model.PoliciesList.AddRange(this.GetAllPolicies().ToSelectList("Name", "Id", null, true));
			model.Policies = model.RolePolicies?.Select(p => p.Id.ToString()).ToList();

			TempData["error"] = Locale.UnableToUpdateRole;

			return View(model);
		}

		/// <summary>
		/// Displays the Index view
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "Role";
			TempData["searchTerm"] = "*";
			return View();
		}

		/// <summary>
		/// Searches for a role.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of roles which match the search term.</returns>
		[HttpGet]
		[ValidateInput(false)]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<RoleViewModel> roles = new List<RoleViewModel>();

			try
			{
				if (this.IsValidId(searchTerm))
				{
					var results = new List<SecurityRoleInfo>();

					results.AddRange(searchTerm == "*" ? this.AmiClient.GetRoles(a => a.Name != null).CollectionItem : this.AmiClient.GetRoles(a => a.Name.Contains(searchTerm)).CollectionItem);

					TempData["searchTerm"] = searchTerm;

					return PartialView("_RolesPartial", results.Select(r => new RoleViewModel(r)).OrderBy(a => a.Name));
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve roles: {e}");
			}

			TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_RolesPartial", roles);
		}

		/// <summary>
		/// Retrieves the selected role
		/// </summary>
		/// <param name="id">The identifier of the role object</param>
		/// <returns>Returns the ViewRole view.</returns>
		[HttpGet]
		public ActionResult ViewRole(Guid id)
		{
			try
			{
				var securityRoleInfo = this.AmiClient.GetRole(id.ToString());

				if (securityRoleInfo == null)
				{
					TempData["error"] = Locale.RoleNotFound;

					return RedirectToAction("Index");
				}

				return View(new RoleViewModel(securityRoleInfo));
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
				Trace.TraceError($"Unable to retrieve role: {e}");
			}

			return RedirectToAction("Index");
		}
	}
}