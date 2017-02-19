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

using Elmah;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ApplicationModels;
using OpenIZAdmin.Models.ApplicationModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
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
		public ActionResult Activate(Guid id)
		{
			try
			{
				var securityApplicationInfo = this.AmiClient.GetApplication(id.ToString());

				if (securityApplicationInfo == null)
				{
					TempData["error"] = Locale.Policy + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				securityApplicationInfo.Id = id;
				securityApplicationInfo.Application.ObsoletedBy = null;
				securityApplicationInfo.Application.ObsoletionTime = null;
				securityApplicationInfo.Application.ObsoletionTimeXml = null;

				this.AmiClient.UpdateApplication(id.ToString(), securityApplicationInfo);

				TempData["success"] = Locale.Policy + " " + Locale.Activated + " " + Locale.Successfully;

				return RedirectToAction("ViewApplication", new { id = securityApplicationInfo.Id });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
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

					TempData["success"] = Locale.Application + " " + Locale.Created + " " + Locale.Successfully;

					return RedirectToAction("ViewApplication", new { id = application.Id.ToString() });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
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
		public ActionResult Delete(Guid id)
		{
			try
			{
				this.AmiClient.DeleteApplication(id.ToString());

				TempData["success"] = Locale.Application + " " + Locale.Deleted + " " + Locale.Successfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToDelete + " " + Locale.Application;

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
					TempData["error"] = Locale.Application + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var model = new EditApplicationModel(securityApplicationInfo);

				model.PoliciesList.Add(new SelectListItem { Text = string.Empty, Value = string.Empty });
				model.PoliciesList.AddRange(CommonUtil.GetAllPolicies(this.AmiClient).Select(p => new SelectListItem { Text = p.Name, Value = p.Key.ToString() }));

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.Application + " " + Locale.NotFound;

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
						TempData["error"] = Locale.Application + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					var appInfo = ApplicationUtil.ToSecurityApplicationInfo(this.AmiClient, model, appEntity);

					this.AmiClient.UpdateApplication(model.Id.ToString(), appInfo);

					TempData["success"] = Locale.Application + " " + Locale.Updated + " " + Locale.Successfully;

					return RedirectToAction("ViewApplication", new { id = appInfo.Id.ToString() });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
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
			TempData["searchType"] = "Application";
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
					var collection = this.AmiClient.GetApplications(a => a.Name.Contains(searchTerm));

					TempData["searchTerm"] = searchTerm;

					return PartialView("_ApplicationsPartial", collection.CollectionItem.Select(a => new ApplicationViewModel(a)));
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
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
					TempData["error"] = Locale.Application + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(new ApplicationViewModel(result));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.Application + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}
	}
}