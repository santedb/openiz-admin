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

using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AssigningAuthorityModels;
using OpenIZAdmin.Models.AssigningAuthorityModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing assigning authorities.
	/// </summary>
	[TokenAuthorize]
	public class AssigningAuthorityController : BaseController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AssigningAuthorityController"/> class.
		/// </summary>
		public AssigningAuthorityController()
		{
		}

		/// <summary>
		/// Returns an action result create view
		/// </summary>
		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// Displays the create view.
		/// </summary>
		/// <returns>Returns the create view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateAssigningAuthorityModel model)
		{
			if (ModelState.IsValid)
			{
				this.AmiClient.CreateAssigningAuthority(AssigningAuthorityUtil.ToCreateAssigningAuthorityModel(model));

				TempData["success"] = Locale.AssigningAuthority + " " + Locale.Created + " " + Locale.Successfully;

				return RedirectToAction("Index");
			}

			TempData["error"] = Locale.UnableToCreate + " " + Locale.AssigningAuthority;

			return View(model);
		}

		/// <summary>
		/// Deletes an assigning authority.
		/// </summary>
		/// <param name="key">The id of the assigning authority to be deleted.</param>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Delete(Guid key)
		{
			var assigningAuthority = this.AmiClient.GetAssigningAuthorities(m => m.Key == key).CollectionItem.FirstOrDefault();

			if (assigningAuthority == null)
			{
				TempData["error"] = Locale.AssigningAuthority + " " + Locale.NotFound;
				return RedirectToAction("Index");
			}

			this.AmiClient.DeleteAssigningAuthority(key.ToString());

			TempData["success"] = Locale.AssigningAuthority + " " + Locale.Deleted + " " + Locale.Successfully;

			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Edit(Guid key)
		{
			var assigningAuthority = this.AmiClient.GetAssigningAuthorities(m => m.Key == key).CollectionItem.FirstOrDefault();

			if (assigningAuthority == null)
			{
				TempData["error"] = Locale.AssigningAuthority + " " + Locale.NotFound;
				return RedirectToAction("Index");
			}

			return View(AssigningAuthorityUtil.ToEditAssigningAuthorityModel(assigningAuthority));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditAssigningAuthorityModel model)
		{
			if (ModelState.IsValid)
			{
				this.AmiClient.UpdateAssigningAuthority(model.Key.ToString(), AssigningAuthorityUtil.ToAssigningAuthorityInfo(model));

				TempData["success"] = Locale.AssigningAuthority + " " + Locale.Edited + " " + Locale.Successfully;

				return RedirectToAction("Index");
			}

			TempData["error"] = Locale.AssigningAuthority + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Returns an action result index view
		/// </summary>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "AssigningAuthority";
			return View();
		}

		/// <summary>
		/// Displays the search view.
		/// </summary>
		/// <returns>Returns the search view.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			var assigningAuthorities = new List<AssigningAuthorityViewModel>();

			if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
			{
				var results = this.AmiClient.GetAssigningAuthorities(p => p.Name.Contains(searchTerm)).CollectionItem.Where(a => a.AssigningAuthority.ObsoletionTime == null);

				TempData["searchTerm"] = searchTerm;

				return PartialView("_AssigningAuthoritySearchResultsPartial", results.Select(AssigningAuthorityUtil.ToAssigningAuthorityViewModel));
			}

			TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_AssigningAuthoritySearchResultsPartial", assigningAuthorities);
		}

		[HttpGet]
		public ActionResult ViewAssigningAuthority(Guid key)
		{
			var assigningAuthority = this.AmiClient.GetAssigningAuthorities(m => m.Key == key).CollectionItem.FirstOrDefault();

			if (assigningAuthority == null)
			{
				TempData["error"] = Locale.AssigningAuthority + " " + Locale.NotFound;
				return RedirectToAction("Index");
			}

			return View(AssigningAuthorityUtil.ToAssigningAuthorityViewModel(assigningAuthority));
		}
	}
}