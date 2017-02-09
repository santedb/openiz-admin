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
using OpenIZAdmin.Models.PolicyModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
		/// Activates a policy.
		/// </summary>
		/// <param name="id">The id of the policy to be activated.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Activate(string id)
		{
			Guid policyKey = Guid.Empty;
			SecurityPolicyInfo policyInfo = null;

			if (CommonUtil.IsValidString(id) && Guid.TryParse(id, out policyKey))
			{
				try
				{
					policyInfo = PolicyUtil.GetPolicy(this.AmiClient, policyKey);

					if (policyInfo == null)
					{
						TempData["error"] = Locale.Policy + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					policyInfo.Policy.ObsoletedBy = null;
					policyInfo.Policy.ObsoletionTime = null;

					this.AmiClient.UpdatePolicy(id, policyInfo);

					TempData["success"] = Locale.Policy + " " + Locale.Activated + " " + Locale.Successfully;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
					ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				}
			}

			TempData["error"] = Locale.UnableToActivate + " " + Locale.Policy;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the create policy view.
		/// </summary>
		/// <returns>Returns the create policy view.</returns>
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
		/// <param name="model">The model containing the information about the policy.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreatePolicyModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
                    var policy = this.AmiClient.CreatePolicy(model.ToSecurityPolicyInfo());

					TempData["success"] = Locale.Policy + " " + Locale.Created + " " + Locale.Successfully;
					return RedirectToAction("ViewPolicy", new { id = policy.Policy.Key.ToString() });
				}
				catch (Exception e)
				{
					ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				}
			}

			TempData["error"] = "Unable to create policy";
			return View(model);
		}

		/// <summary>
		/// Deletes a policy.
		/// </summary>
		/// <param name="id">The id of the policy to be deleted.</param>
		/// <returns>Returns the index view.</returns>
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
		/// Retrieves the policy entity by id.
		/// </summary>
		/// <param name="id">The policy identifier.</param>
		/// <returns>Returns the policy edit view.</returns>
		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			try
			{
				var policyInfo = PolicyUtil.GetPolicy(this.AmiClient, id);

				if (policyInfo == null)
				{
					TempData["error"] = Locale.Policy + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(new EditPolicyModel(policyInfo));

			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.Policy + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Updates a policy.
		/// </summary>
		/// <param name="model">The model containing the updated policy information.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditPolicyModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var policy = PolicyUtil.GetPolicy(this.AmiClient, model.Key);

					if (policy == null)
					{
						TempData["error"] = Locale.Policy + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					SecurityPolicyInfo policyInfo = model.ToSecurityPolicyInfo(policy);

					this.AmiClient.UpdatePolicy(model.Key.ToString(), policyInfo);

					TempData["success"] = Locale.Policy + " " + Locale.Updated + " " + Locale.Successfully;
					return RedirectToAction("Edit", new { key = policyInfo.Policy.Key.ToString() });
				}
				catch (Exception e)
				{
					ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				}
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Policy;
			return View(model);
		}

		/// <summary>
		/// Displays the Index view
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "Policy";
			return View(PolicyUtil.GetAllPolicies(this.AmiClient));
		}

		/// <summary>
		/// Searches for a policy.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of policies which match the search term.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<PolicyViewModel> policies = new List<PolicyViewModel>();

			try
			{
				if (CommonUtil.IsValidString(searchTerm))
				{
					var collection = this.AmiClient.GetPolicies(p => p.Name.Contains(searchTerm));

					TempData["searchTerm"] = searchTerm;

					return PartialView("_PolicySearchResultsPartial", collection.CollectionItem.Select(p => new PolicyViewModel(p)));
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
		/// Searches for a policy to view details.
		/// </summary>
		/// <param name="key">The policy identifier search string.</param>
		/// <returns>Returns a policy view that matches the search term.</returns>
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