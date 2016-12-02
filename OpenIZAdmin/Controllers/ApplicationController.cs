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
 * User: khannan
 * Date: 2016-11-14
 */

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ApplicationModels;
using OpenIZAdmin.Models.ApplicationModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing security applications.
	/// </summary>
	[TokenAuthorize]
	public class ApplicationController : BaseController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationController"/> class.
		/// </summary>
		public ApplicationController()
		{
		}

		/// <summary>
		/// Activates an application.
		/// </summary>
		/// <param name="id">The id of the application to be activated.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Activate(string id)
		{
			Guid appKey = Guid.Empty;
			SecurityApplicationInfo appInfo = null;

			if (CommonUtil.IsValidString(id) && Guid.TryParse(id, out appKey))
			{
				appInfo = ApplicationUtil.GetApplication(this.AmiClient, appKey);

				if (appInfo == null)
				{
					TempData["error"] = Locale.Policy + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				appInfo.Id = appKey;
				appInfo.Application.ObsoletedBy = null;
				appInfo.Application.ObsoletionTime = null;
				appInfo.Application.ObsoletionTimeXml = null;

				this.AmiClient.UpdateApplication(id, appInfo);

				TempData["success"] = Locale.Policy + " " + Locale.ActivatedSuccessfully;

				return RedirectToAction("Index");
			}

			TempData["error"] = Locale.UnableToActivate + " " + Locale.Application;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationController"/> class.
		/// </summary>
		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

        /// <summary>
		/// Creates a new application instance
		/// </summary>
        /// <param name="model">The model containing the information of the application to be created.</param>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateApplicationModel model)
		{
			if (ModelState.IsValid)
			{
				var application = this.AmiClient.CreateApplication(ApplicationUtil.ToSecurityApplication(model));

				TempData["success"] = Locale.Application + " " + Locale.CreatedSuccessfully;

				return RedirectToAction("ViewApplication", new { key = application.Id.ToString() });
			}

			TempData["error"] = Locale.UnableToCreate + " " + Locale.Application;

			return View(model);
		}

		/// <summary>
		/// Deletes an application.
		/// </summary>
		/// <param name="id">The id of the application to be deleted.</param>
		/// <returns>Returns the Index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(string id)
		{
			if (CommonUtil.IsValidString(id))
			{
				this.AmiClient.DeleteApplication(id);

				TempData["success"] = Locale.Application + " " + Locale.DeletedSuccessfully;

				return RedirectToAction("Index");
			}

			TempData["error"] = Locale.UnableToDelete + " " + Locale.Application;

			return RedirectToAction("Index");
		}

        /// <summary>
        /// Deletes a policy associated to an application.
        /// </summary>
        /// <param name="appId">The application id string of the application.</param>
        /// <param name="key">The policy guid key of the policy to be deleted.</param>
        /// <returns>Returns the Index view.</returns>
        [HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeletePolicy(string appId, Guid key)
		{					
			if (CommonUtil.IsValidString(appId) && CommonUtil.IsGuid(key))
			{
				try
				{
                    var appEntity = ApplicationUtil.GetApplication(this.AmiClient, appId);

                    if (appEntity == null)
                    {
                        TempData["error"] = Locale.Application + " " + Locale.NotFound;

                        return RedirectToAction("Index");
                    }

                    appEntity.Policies.RemoveAll(a => a.Policy.Key == key);

                    this.AmiClient.UpdateApplication(appId, appEntity);

					TempData["success"] = Locale.Application + " " + Locale.UpdatedSuccessfully;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to delete policy from application: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to delete policy from application: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.UnableToDelete + " " + Locale.Policy;

			return RedirectToAction("Index");
		}

        /// <summary>
        /// Gets the application object to edit
        /// </summary>
        /// <param name="key">The id of the application to be edited.</param>
        /// <returns>Returns the Edit view.</returns>
        [HttpGet]
		public ActionResult Edit(string key)
		{
			Guid appKey = Guid.Empty;
			SecurityApplicationInfo application = null;

			if (CommonUtil.IsValidString(key) && Guid.TryParse(key, out appKey))
			{
				application = ApplicationUtil.GetApplication(this.AmiClient, appKey);

				if (application == null)
				{
					TempData["error"] = Locale.Application + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(ApplicationUtil.ToEditApplicationModel(this.AmiClient, application));
			}

			TempData["error"] = Localization.Locale.Device + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Appies the changes to the application object
		/// </summary>
		/// <param name="model">The model containing the updated application information.</param>
		/// <returns>Returns the application view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditApplicationModel model)
		{            
			if (ModelState.IsValid)
			{				
				var appEntity = ApplicationUtil.GetApplication(this.AmiClient, model.Id);

				if (appEntity == null)
				{
					TempData["error"] = Locale.Application + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

                model.Policies = appEntity.Policies;
                model.AddPoliciesList = CommonUtil.GetNewPolicies(this.AmiClient, model.AddPolicies);                

                SecurityApplicationInfo appInfo = ApplicationUtil.ToSecurityApplicationInfo(model, appEntity);

				this.AmiClient.UpdateApplication(model.Id.ToString(), appInfo);

				TempData["success"] = Locale.Application + " " + Locale.UpdatedSuccessfully;				

				return Redirect("Index");
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Application;

			return View(model);
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = Locale.Application;
			return View();
		}

		/// <summary>
		/// Gets an application list based on the search parameter applied to the SoftwareName field
		/// </summary>
		/// <param name="searchTerm">The search parameter to apply to the query.</param>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<ApplicationViewModel> applications = new List<ApplicationViewModel>();

			try
			{
				if (CommonUtil.IsValidString(searchTerm))
				{
					var collection = this.AmiClient.GetApplications(d => d.Name.Contains(searchTerm));

					TempData["searchTerm"] = searchTerm;

					return PartialView("_ApplicationsPartial", collection.CollectionItem.Select(ApplicationUtil.ToApplicationViewModel));
				}
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to search applications: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to search applications: {0}", e.Message);
			}

			TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_ApplicationsPartial", applications);
		}

		/// <summary>
		/// Searches for an application object with the supplied id.
		/// </summary>
		/// <param name="id">The application identifier search parameter to apply to the query.</param>
		/// <returns>Returns the ViewApplication view.</returns>
		[HttpGet]
		public ActionResult ViewApplication(string id)
		{
			if (CommonUtil.IsValidString(id))
			{
				var result = ApplicationUtil.GetApplication(this.AmiClient, id);

				if (result == null)
				{
					TempData["error"] = Locale.Application + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(ApplicationUtil.ToApplicationViewModel(result));
			}

			TempData["error"] = Locale.Application + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}
	}
}