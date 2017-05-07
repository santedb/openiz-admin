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
 * Date: 2016-7-30
 */

using Elmah;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.PolicyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OpenIZ.Core.Model.Security;

namespace OpenIZAdmin.Controllers
{
    /// <summary>
    /// Provides operations for administering policies.
    /// </summary>
    [TokenAuthorize]
	public class PolicyController : BaseController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Controllers.PolicyController"/> class.
		/// </summary>
		public PolicyController()
		{
		}

		/// <summary>
		/// Displays the create policy view.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			var model = new CreatePolicyModel();

			model.GrantsList.Add(new SelectListItem { Text = Locale.Select, Value = "" });
			model.GrantsList.Add(new SelectListItem { Text = Locale.Deny, Value = "0" });
			model.GrantsList.Add(new SelectListItem { Text = Locale.Elevate, Value = "1" });
			model.GrantsList.Add(new SelectListItem { Text = Locale.Grant, Value = "2" });

			return View(model);
		}

		/// <summary>
		/// Creates a policy.
		/// </summary>
		/// <param name="model">The model containing the policy information.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreatePolicyModel model)
		{
			try
			{
				var exists = this.AmiClient.GetPolicies(c => c.Oid == model.Oid).CollectionItem.Any();

                if (exists) ModelState.AddModelError("Oid", Locale.OidMustBeUnique);

                if (this.ModelState.IsValid)
				{
					var policy = this.AmiClient.CreatePolicy(model.ToSecurityPolicyInfo());

					TempData["success"] = Locale.Policy + " " + Locale.Created + " " + Locale.Successfully;

					return RedirectToAction("ViewPolicy", new { id = policy.Policy.Key.ToString() });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			model.GrantsList.Add(new SelectListItem { Text = Locale.Select, Value = "" });
			model.GrantsList.Add(new SelectListItem { Text = Locale.Deny, Value = "0" });
			model.GrantsList.Add(new SelectListItem { Text = Locale.Elevate, Value = "1" });
			model.GrantsList.Add(new SelectListItem { Text = Locale.Grant, Value = "2" });

			if (!string.IsNullOrEmpty(model.Grant) && !string.IsNullOrWhiteSpace(model.Grant))
			{
				model.GrantsList = model.GrantsList.Select(g => new SelectListItem { Selected = model.Grant == g.Value, Text = g.Text, Value = g.Value}).ToList();
			}

			TempData["error"] = Locale.UnableToCreatePolicy;

			return View(model);
		}

		/// <summary>
		/// Deletes a policy.
		/// </summary>
		/// <param name="id">The id of the policy to delete.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
				this.AmiClient.DeletePolicy(id.ToString());
				TempData["success"] = Locale.Policy + " " + Locale.Deleted + " " + Locale.Successfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToDelete + " " + Locale.Policy;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "Policy";
            TempData["searchTerm"] = "*";
            return View();
		}

		/// <summary>
		/// Searches for a policy which matches the given search term.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<PolicyViewModel> policies = new List<PolicyViewModel>();

			try
			{
				if (this.IsValidId(searchTerm))
				{
					var results = new List<SecurityPolicyInfo>();

					results.AddRange(searchTerm == "*" ? this.AmiClient.GetPolicies(a => a.Key != null).CollectionItem : this.AmiClient.GetPolicies(a => a.Name.Contains(searchTerm)).CollectionItem);

					TempData["searchTerm"] = searchTerm;

					return PartialView("_PolicySearchResultsPartial", results.Select(p => new PolicyViewModel(p)).OrderBy(a => a.Name));
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_PolicySearchResultsPartial", policies);
		}

		/// <summary>
		/// Displays the view policy view.
		/// </summary>
		/// <param name="id">The id of the policy to view.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult ViewPolicy(Guid id)
		{
			try
			{
				var result = this.AmiClient.GetPolicies(r => r.Key == id);

				if (!result.CollectionItem.Any())
				{
					TempData["error"] = Locale.Policy + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}			    

                return View(new PolicyViewModel(result.CollectionItem.First()));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.Policy + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}
	}
}