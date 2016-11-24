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
 * Date: 2016-7-30
 */

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.PolicyModels;
using OpenIZAdmin.Models.PolicyModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			Guid userKey = Guid.Empty;
			SecurityPolicyInfo policyInfo = null;

			if (PolicyUtil.IsValidString(id) && Guid.TryParse(id, out userKey))
			{
				try
				{
					policyInfo = PolicyUtil.GetPolicy(this.AmiClient, userKey);

					if (policyInfo == null)
					{
						TempData["error"] = Locale.Policy + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					policyInfo.Policy.ObsoletedBy = null;
					policyInfo.Policy.ObsoletionTime = null;

					this.AmiClient.UpdatePolicy(id, policyInfo);

					TempData["success"] = Locale.Policy + " " + Locale.ActivatedSuccessfully;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to delete policy: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to delete policy: {0}", e.Message);
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
			CreatePolicyModel model = new CreatePolicyModel();

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
				SecurityPolicyInfo policy = PolicyUtil.ToSecurityPolicy(model);

				try
				{
					this.AmiClient.CreatePolicy(policy);

					TempData["success"] = Locale.Policy + " " + Locale.CreatedSuccessfully;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to create policy: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to create policy: {0}", e.Message);
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
		public ActionResult Delete(string id)
		{
			if (PolicyUtil.IsValidString(id))
			{
				try
				{
					this.AmiClient.DeletePolicy(id);
					TempData["success"] = Locale.Policy + " " + Locale.DeletedSuccessfully;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to delete policy: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to delete policy: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.UnableToDelete + " " + Locale.Policy;

			return RedirectToAction("Index");
		}

        /// <summary>
		/// Retrieves the policy entity by id
		/// </summary>
		/// <param name="key">The policy identifier.</param>
		/// <returns>Returns the policy edit view.</returns>
		[HttpGet]
		public ActionResult Edit(string key)
		{
			Guid policyId = Guid.Empty;
			SecurityPolicyInfo policyInfo = null;

			if (PolicyUtil.IsValidString(key) && Guid.TryParse(key, out policyId))
			{
				policyInfo = PolicyUtil.GetPolicy(this.AmiClient, policyId);

				if (policyInfo == null)
				{
					TempData["error"] = Locale.Policy + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(PolicyUtil.ToEditPolicyModel(policyInfo));
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
					SecurityPolicyInfo policy = PolicyUtil.GetPolicy(this.AmiClient, model.Key);

					if (policy == null)
					{
						TempData["error"] = Locale.Policy + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					SecurityPolicyInfo policyInfo = PolicyUtil.ToSecurityPolicy(model, policy);

					this.AmiClient.UpdatePolicy(model.Key.ToString(), policyInfo);

					TempData["success"] = Locale.Policy + " " + Locale.UpdatedSuccessfully;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to edit policy: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to edit policy: {0}", e.Message);
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
				if (PolicyUtil.IsValidString(searchTerm))
				{
					var collection = this.AmiClient.GetPolicies(p => p.Name.Contains(searchTerm));

					TempData["searchTerm"] = searchTerm;

					return PartialView("_PolicySearchResultsPartial", collection.CollectionItem.Select(p => PolicyUtil.ToPolicyViewModel(p)));
				}
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to search policies: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to search policies: {0}", e.Message);
			}

			TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_PoliciesPartial", policies);
		}

        /// <summary>
		/// Searches for a policy to view details.
		/// </summary>
		/// <param name="key">The policy identifier search string.</param>
		/// <returns>Returns a policy view that matches the search term.</returns>
		[HttpGet]
		public ActionResult ViewPolicy(string key)
		{
			Guid policyId = Guid.Empty;

			if (PolicyUtil.IsValidString(key) && Guid.TryParse(key, out policyId))
			{
				var result = this.AmiClient.GetPolicies(r => r.Key == policyId);

				if (result.CollectionItem.Count == 0)
				{
					TempData["error"] = Locale.Policy + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(PolicyUtil.ToPolicyViewModel(result.CollectionItem.Single()));
			}

			TempData["error"] = Locale.Policy + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}
	}
}