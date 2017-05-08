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
using OpenIZ.Core.Model.AMI.DataTypes;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AssigningAuthorityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OpenIZ.Core.Model.DataTypes;

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
		/// Displays the create view.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// Displays the create assigning authority view.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Returns an <see cref="ActionResult" /> instance.</returns>
		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateAssigningAuthorityModel model)
		{
			try
			{   
                //refactor at later time             
                var exists = this.AmiClient.GetAssigningAuthorities(m => m.Oid == model.Oid).CollectionItem.Any();
                if (exists) ModelState.AddModelError("Oid", Locale.OidMustBeUnique);

                var duplicateName = this.AmiClient.GetAssigningAuthorities(m => m.Name == model.Name).CollectionItem.Any();
                if (duplicateName) ModelState.AddModelError("Name", Locale.NameMustBeUnique);

                var duplicateDomainName = this.AmiClient.GetAssigningAuthorities(m => m.DomainName == model.DomainName).CollectionItem.Any();
                if (duplicateDomainName) ModelState.AddModelError("DomainName", Locale.DomainNameMustBeUnique);

                if (ModelState.IsValid)
				{
					var assigningAuthority = this.AmiClient.CreateAssigningAuthority(model.ToAssigningAuthorityInfo());

					TempData["success"] = Locale.AssigningAuthorityCreatedSuccessfully;

					return RedirectToAction("ViewAssigningAuthority", new { id = assigningAuthority.Id });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToCreateAssigningAuthority;

			return View(model);
		}

		/// <summary>
		/// Deletes an assigning authority.
		/// </summary>
		/// <param name="id">The id of the assigning authority to delete.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var assigningAuthority = this.AmiClient.GetAssigningAuthorities(m => m.Key == id).CollectionItem.FirstOrDefault();

				if (assigningAuthority == null)
				{
					TempData["error"] = Locale.AssigningAuthorityNotFound;
					return RedirectToAction("Index");
				}

				this.AmiClient.DeleteAssigningAuthority(id.ToString());

				TempData["success"] = Locale.AssigningAuthorityDeletedSuccessfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToDeleteAssigningAuthority;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the edit assigning authority view.
		/// </summary>
		/// <param name="id">The id of the assigning authority to edit.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			try
			{
				var assigningAuthority = this.AmiClient.GetAssigningAuthorities(m => m.Key == id).CollectionItem.FirstOrDefault();

				if (assigningAuthority == null)
				{
					TempData["error"] = Locale.AssigningAuthorityNotFound;
					return RedirectToAction("Index");
				}

				return View(new EditAssigningAuthorityModel(assigningAuthority));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.AssigningAuthorityNotFound;
			return RedirectToAction("Index");
		}

		/// <summary>
		/// Updates an assigning authority.
		/// </summary>
		/// <param name="model">The model containing the assigning authority information.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditAssigningAuthorityModel model)
		{
			try
			{
                var assigningAuthorityInfo = this.AmiClient.GetAssigningAuthorities(m => m.Key == model.Id).CollectionItem.FirstOrDefault();

                if (assigningAuthorityInfo == null)
                {
                    TempData["error"] = Locale.AssigningAuthorityNotFound;
                    return RedirectToAction("Index");
                }
                
                if (!assigningAuthorityInfo.AssigningAuthority.Oid.Equals(model.Oid))
                {
                    var exists = this.AmiClient.GetAssigningAuthorities(m => m.Oid == model.Oid).CollectionItem.FirstOrDefault();
                    if (exists?.AssigningAuthority != null) ModelState.AddModelError("Oid", Locale.OidMustBeUnique);
                }

			    if (!assigningAuthorityInfo.AssigningAuthority.Name.Equals(model.Name))
			    {
			        var duplicateName = this.AmiClient.GetAssigningAuthorities(m => m.Name == model.Name).CollectionItem.FirstOrDefault();
			        if (duplicateName?.AssigningAuthority != null) ModelState.AddModelError("Name", Locale.NameMustBeUnique);
			    }

			    if (!assigningAuthorityInfo.AssigningAuthority.DomainName.Equals(model.DomainName))
			    {
			        var duplicateDomainName = this.AmiClient.GetAssigningAuthorities(m => m.DomainName == model.DomainName).CollectionItem.FirstOrDefault();
			        if (duplicateDomainName?.AssigningAuthority != null) ModelState.AddModelError("DomainName", Locale.DomainNameMustBeUnique);
			    }



			    if (ModelState.IsValid)
				{
					this.AmiClient.UpdateAssigningAuthority(model.Id.ToString(), model.ToAssigningAuthorityInfo());

					TempData["success"] = Locale.AssigningAuthorityUpdatedSuccessfully;
					return RedirectToAction("ViewAssigningAuthority", new { id = model.Id });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToUpdateAssigningAuthority;

			return View(model);
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "AssigningAuthority";
            TempData["searchTerm"] = "*";
            return View();
		}

		/// <summary>
		/// Searches for an assigning authority which matches the given search term.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		[ValidateInput(false)]
		public ActionResult Search(string searchTerm)
		{
			var assigningAuthorities = new List<AssigningAuthorityViewModel>();

			if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
			{
				var results = new List<AssigningAuthorityInfo>();

				results.AddRange(searchTerm == "*" ? this.AmiClient.GetAssigningAuthorities(a => a.Key != null).CollectionItem : this.AmiClient.GetAssigningAuthorities(a => a.Name.Contains(searchTerm)).CollectionItem);

				TempData["searchTerm"] = searchTerm;

				return PartialView("_AssigningAuthoritiesPartial", results.Select(a => new AssigningAuthorityViewModel(a)).OrderBy(a => a.Name));
			}

			TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_AssigningAuthoritiesPartial", assigningAuthorities);
		}

		/// <summary>
		/// Displays the view assigning authority view.
		/// </summary>
		/// <param name="id">The id of the assigning authority to view.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult ViewAssigningAuthority(Guid id)
		{
			try
			{
				var assigningAuthority = this.AmiClient.GetAssigningAuthorities(m => m.Key == id).CollectionItem.FirstOrDefault();

				if (assigningAuthority == null)
				{
					TempData["error"] = Locale.AssigningAuthorityNotFound;
					return RedirectToAction("Index");
				}

				return View(new AssigningAuthorityViewModel(assigningAuthority));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.AssigningAuthorityNotFound;

			return RedirectToAction("Index");
		}
	}
}