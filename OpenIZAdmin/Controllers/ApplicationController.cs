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
 * User: khannan
 * Date: 2016-11-14
 */

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Services.Http.Security;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing security applications.
	/// </summary>
	[TokenAuthorize]
	public class ApplicationController : SecurityBaseController
	{
		/// <summary>
		/// Activates an application.
		/// </summary>
		/// <param name="id">The id of the application to be activated.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Activate(Guid id)
		{
			try
			{
				var securityApplicationInfo = this.AmiClient.GetApplication(id.ToString());

				if (securityApplicationInfo == null)
				{
					TempData["error"] = Locale.ApplicationNotFound;

					return RedirectToAction("Index");
				}

				securityApplicationInfo.Id = id;
				securityApplicationInfo.Application.ObsoletedBy = null;
				securityApplicationInfo.Application.ObsoletionTime = null;
				securityApplicationInfo.Application.ObsoletionTimeXml = null;

				this.AmiClient.UpdateApplication(id.ToString(), securityApplicationInfo);

				TempData["success"] = Locale.ApplicationActivatedSuccessfully;

				return RedirectToAction("ViewApplication", new { id = securityApplicationInfo.Id });
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to activate security application: {e}");
			}

			TempData["error"] = Locale.UnableToActivateApplication;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationController"/> class.
		/// </summary>
		[HttpGet]
		public ActionResult Create()
		{
			var viewModel = new CreateApplicationModel
			{
				ApplicationSecret = Guid.NewGuid().ToString().ToUpper()
			};

			return View(viewModel);
		}

		/// <summary>
		/// Creates a new application instance
		/// </summary>
		/// <param name="model">The model containing the information of the application to be created.</param>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateApplicationModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var application = this.AmiClient.CreateApplication(model.ToSecurityApplication());

					TempData["success"] = Locale.ApplicationCreatedSuccessfully;

					return RedirectToAction("ViewApplication", new { id = application.Id.ToString() });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to create security application: {e}");
			}

			TempData["error"] = Locale.UnableToCreateApplication;

			return View(model);
		}

		/// <summary>
		/// Deletes an application.
		/// </summary>
		/// <param name="id">The id of the application to delete.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
				this.AmiClient.DeleteApplication(id.ToString());

				TempData["success"] = Locale.ApplicationDeactivatedSuccessfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to delete security application: {e}");
			}

			TempData["error"] = Locale.UnableToDeleteApplication;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the edit application view.
		/// </summary>
		/// <param name="id">The id of the application to be edit.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			try
			{
				var securityApplicationInfo = this.AmiClient.GetApplication(id.ToString());

				if (securityApplicationInfo == null)
				{
					TempData["error"] = Locale.ApplicationNotFound;

					return RedirectToAction("Index");
				}

				var model = new EditApplicationModel(securityApplicationInfo);

				model.PoliciesList.AddRange(this.GetAllPolicies().ToSelectList("Name", "Id", null, true));

				return View(model);
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
				Trace.TraceError($"Unable to retrieve security application: {e}");
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Updates a security application.
		/// </summary>
		/// <param name="model">The model containing the updated application information.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditApplicationModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var appEntity = this.AmiClient.GetApplication(model.Id.ToString());

					if (appEntity == null)
					{
						TempData["error"] = Locale.ApplicationNotFound;

						return RedirectToAction("Index");
					}

					var appInfo = this.ToSecurityApplicationInfo(model, appEntity);

					this.AmiClient.UpdateApplication(model.Id.ToString(), appInfo);

					TempData["success"] = Locale.ApplicationUpdatedSuccessfully;

					return RedirectToAction("ViewApplication", new { id = appInfo.Id.ToString() });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update security application: {e}");
			}

			TempData["error"] = Locale.UnableToUpdateApplication;

			return View(model);
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "Application";
            TempData["searchTerm"] = "*";
            return View();
		}

		/// <summary>
		/// Gets an application list based on the search parameter applied to the SoftwareName field
		/// </summary>
		/// <param name="searchTerm">The search parameter to apply to the query.</param>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		[ValidateInput(false)]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<ApplicationViewModel> applications = new List<ApplicationViewModel>();

			try
			{
				if (this.IsValidId(searchTerm))
				{
					var results = new List<SecurityApplicationInfo>();

					results.AddRange(searchTerm == "*" ? this.AmiClient.GetApplications(a => a.Id != null).CollectionItem : this.AmiClient.GetApplications(a => a.Name.Contains(searchTerm)).CollectionItem);

					TempData["searchTerm"] = searchTerm;

					return PartialView("_ApplicationsPartial", results.Select(a => new ApplicationViewModel(a)).OrderBy(a => a.ApplicationName));
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to query for security applications: {e}");
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
		public ActionResult ViewApplication(Guid id)
		{
			try
			{
				var result = this.AmiClient.GetApplication(id.ToString());

				if (result == null)
				{
					TempData["error"] = Locale.ApplicationNotFound;

					return RedirectToAction("Index");
				}

				return View(new ApplicationViewModel(result));
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve security application: {e}");

				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

			return RedirectToAction("Index");
		}
	}
}