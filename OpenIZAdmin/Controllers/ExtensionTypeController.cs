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
 * Date: 2017-5-19
 */

using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ExtensionTypeModels;
using OpenIZAdmin.Services.Metadata.ExtensionTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing extension types.
	/// </summary>
	[TokenAuthorize(Constants.UnrestrictedMetadata)]
	public class ExtensionTypeController : Controller
	{
		/// <summary>
		/// The extension type service.
		/// </summary>
		private readonly IExtensionTypeService extensionTypeService;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionTypeController" /> class.
		/// </summary>
		/// <param name="extensionTypeService">The extension type service.</param>
		public ExtensionTypeController(IExtensionTypeService extensionTypeService)
		{
			this.extensionTypeService = extensionTypeService;
		}

		/// <summary>
		/// Displays the create extension type view.
		/// </summary>
		/// <returns>Returns the create extension type view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			return View(new CreateExtensionTypeModel());
		}

		/// <summary>
		/// Creates an extension type.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Returns an action result instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateExtensionTypeModel model)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					var created = this.extensionTypeService.Create(model.ToExtensionType());

					return RedirectToAction("ViewExtensionType", new { id = created.Key.Value });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to create extension type: {e}");
			}

			return View(model);
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "ExtensionType";
			TempData["searchTerm"] = "*";
			return View();
		}

		/// <summary>
		/// Searches for an extension type which matches the given search term.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		[ValidateInput(false)]
		public ActionResult Search(string searchTerm)
		{
			var extensionTypes = new List<ExtensionTypeViewModel>();

			if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
			{
				var results = this.extensionTypeService.Search(searchTerm);

				TempData["searchTerm"] = searchTerm;

				return PartialView("_ExtensionTypesPartial", results.Select(a => new ExtensionTypeViewModel(a)).OrderBy(a => a.Name));
			}

			TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_ExtensionTypesPartial", extensionTypes);
		}

		/// <summary>
		/// Displays the view extension type view.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the extension type view.</returns>
		public ActionResult ViewExtensionType(Guid id)
		{
			try
			{
				var extensionType = this.extensionTypeService.Get(id);

				if (extensionType == null)
				{
					TempData["error"] = Locale.ExtensionTypeNotFound;
					return RedirectToAction("Index");
				}

				return View(new ExtensionTypeViewModel(extensionType));
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve extension type: { e }");
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

			return RedirectToAction("Index");
		}
	}
}