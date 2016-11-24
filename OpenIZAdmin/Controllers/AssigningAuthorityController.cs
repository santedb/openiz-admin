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
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for administering policies.
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
		/// Returns an action result index view
		/// </summary>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "AssigningAuthority";
			return View();
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
				try
				{
					var test = "";

					var results = this.AmiClient.CreateAssigningAuthority(AssigningAuthorityUtil.ToCreateAssigningAuthorityModel(model));
					TempData["success"] = Locale.AssigningAuthority + " " + Locale.CreatedSuccessfully;
					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to delete assigning authority: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to delete assigning authority: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.AssigningAuthority + " " + Locale.NotFound;
			return RedirectToAction("Index");
		}

        /// <summary>
        /// Displays the search view.
        /// </summary>
        /// <returns>Returns the search view.</returns>
        [HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<AssigningAuthorityViewModel> assigningAuthorities = new List<AssigningAuthorityViewModel>();
			try
			{
				if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
				{
					var collection = this.AmiClient.GetAssigningAuthorities(p => p.Name.Contains(searchTerm) && p.ObsoletionTime == null);
					var filtered = collection.CollectionItem.FindAll(p => p.AssigningAuthority.ObsoletionTime == null);//TEMP: Until the obsoletion time is taken as query parameter

					TempData["searchTerm"] = searchTerm;

					return PartialView("_AssigningAuthoritySearchResultsPartial", filtered.Select(p => AssigningAuthorityUtil.ToAssigningAuthorityViewModel(p)));
				}
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to search assigning authorities: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to search assigning authorities: {0}", e.Message);
			}

			TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_AssigningAuthoritySearchResultsPartial", assigningAuthorities);
		}

        /// <summary>
		/// Deletes an assigning authority.
		/// </summary>
		/// <param name="key">The id of the assigning authority to be deleted.</param>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Delete(string key)
		{
			Guid assigningAuthorityKey = Guid.Empty;
			if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key) && Guid.TryParse(key, out assigningAuthorityKey))
			{
				try
				{
					var assigningAuthority = this.AmiClient.GetAssigningAuthorities(m => m.Key == assigningAuthorityKey);
					var singleAssigningAuthority = assigningAuthority.CollectionItem.SingleOrDefault();

					singleAssigningAuthority.AssigningAuthority.ObsoletionTime = new DateTimeOffset(DateTime.Now);
					this.AmiClient.DeleteAssigningAuthority(key);
					TempData["success"] = Locale.AssigningAuthority + " " + Locale.DeletedSuccessfully;
					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to delete assigning authority: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to delete assigning authority: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.AssigningAuthority + " " + Locale.NotFound;
			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult ViewAssigningAuthority(string key)
		{
			Guid assigningAuthorityKey = Guid.Empty;

			if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key) && Guid.TryParse(key, out assigningAuthorityKey))
			{
				try
				{
					var assigningAuthority = this.AmiClient.GetAssigningAuthorities(m => m.Key == assigningAuthorityKey);

					object model = null;

					return View(assigningAuthority.CollectionItem.Select(p => AssigningAuthorityUtil.ToAssigningAuthorityViewModel(p)).SingleOrDefault());
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to find assigning authority: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to find assigning authority: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.AssigningAuthority + " " + Locale.NotFound;
			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Edit(string key)
		{
			Guid assigningAuthorityKey = Guid.Empty;

			if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key) && Guid.TryParse(key, out assigningAuthorityKey))
			{
				try
				{
					var assigningAuthority = this.AmiClient.GetAssigningAuthorities(m => m.Key == assigningAuthorityKey);

					object model = null;

					return View(assigningAuthority.CollectionItem.Select(p => AssigningAuthorityUtil.ToEditAssigningAuthorityModel(p)).SingleOrDefault());
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to find assigning authority: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to find assigning authority: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.AssigningAuthority + " " + Locale.NotFound;
			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Edit(EditAssigningAuthorityModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var assigningAuthority = this.AmiClient.GetAssigningAuthorities(m => m.Key == model.Key).CollectionItem.SingleOrDefault();

					assigningAuthority.AssigningAuthority.Url = model.Url;
					assigningAuthority.AssigningAuthority.DomainName = model.DomainName;
					assigningAuthority.AssigningAuthority.Description = model.Description;
					assigningAuthority.AssigningAuthority.Oid = model.Oid;
					assigningAuthority.AssigningAuthority.Name = model.Name;

					var key = assigningAuthority.AssigningAuthority.Key.Value.ToString();
					this.AmiClient.UpdateAssigningAuthority(key, assigningAuthority);
					TempData["success"] = Locale.AssigningAuthority + " " + Locale.EditedSuccessfully;
					return View("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to find assigning authority: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to find assigning authority: {0}", e.Message);
				}
			}
			TempData["error"] = Locale.AssigningAuthority + " " + Locale.NotFound;
			return RedirectToAction("Index");
		}
	}
}