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

using Elmah;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.RoleModels;
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
	public class RoleController : BaseController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RoleController"/> class.
		/// </summary>
		public RoleController()
		{
		}

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
                    TempData["error"] = Locale.Role + " " + Locale.NotFound;

                    return RedirectToAction("Index");
                }

                securityRoleInfo.Role.CreationTime = DateTimeOffset.Now;
                securityRoleInfo.Role.ObsoletedByKey = null;
                securityRoleInfo.Role.ObsoletionTime = null;

                var result = this.AmiClient.UpdateRole(id.ToString(), securityRoleInfo);

                TempData["success"] = Locale.Role + " " + Locale.Activated + " " + Locale.Successfully;

                return RedirectToAction("ViewRole", new { id = result.Id });
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to activate role: { e }");
            }

            TempData["error"] = Locale.UnableToActivate + " " + Locale.Role;

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

					TempData["success"] = Locale.Role + " " + Locale.Created + " " + Locale.Successfully;

					return RedirectToAction("ViewRole", new { id = role.Id.ToString() });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToCreate + " " + Locale.Role;

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

				TempData["success"] = Locale.Role + " " + Locale.Deleted + " " + Locale.Successfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToDelete + " " + Locale.Role;

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
					TempData["error"] = Locale.Role + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(RoleUtil.ToEditRoleModel(this.AmiClient, securityRoleInfo));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
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
					var roleInfo = this.AmiClient.GetRole(model.Id);

					if (roleInfo == null)
					{
						TempData["error"] = Locale.Role + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					this.AmiClient.UpdateRole(roleInfo.Id.ToString(), RoleUtil.ToSecurityRoleInfo(this.AmiClient, model, roleInfo));

					TempData["success"] = Locale.Role + " " + Locale.Updated + " " + Locale.Successfully;

					return RedirectToAction("ViewRole", new { id = roleInfo.Id.ToString() });
				}
				catch (Exception e)
				{
					ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				}
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Role;

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
			return View(new List<RoleViewModel>());
		}

		/// <summary>
		/// Searches for a role.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of roles which match the search term.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<RoleViewModel> roles = new List<RoleViewModel>();

			try
			{
				if (CommonUtil.IsValidString(searchTerm))
				{
					var collection = this.AmiClient.GetRoles(r => r.Name.Contains(searchTerm));

					TempData["searchTerm"] = searchTerm;

					return PartialView("_RolesPartial", collection.CollectionItem.Select(r => new RoleViewModel(r)));
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
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
					TempData["error"] = Locale.Role + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(new RoleViewModel(securityRoleInfo));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.Role + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}
	}
}