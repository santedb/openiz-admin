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
 * User: Andrew
 * Date: 2017-4-12
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Util;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.LanguageModels;
using OpenIZAdmin.Models.ReferenceTermModels;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing reference terms.
	/// </summary>
	[TokenAuthorize]
	public class ReferenceTermController : MetadataController
    {
        /// <summary>
		/// Activates the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>ActionResult.</returns>
		public ActionResult Activate(Guid id)
        {
            try
            {
                var referenceTerm = ImsiClient.Get<ReferenceTerm>(id, null) as ReferenceTerm;

                if (referenceTerm == null)
                {
                    TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

                    return RedirectToAction("Index");
                }
                                
                referenceTerm.ObsoletedByKey = null;
                referenceTerm.ObsoletionTime = null;

                var result = this.ImsiClient.Update(referenceTerm);

                TempData["success"] = Locale.ReferenceTerm + " " + Locale.Activated + " " + Locale.Successfully;

                return RedirectToAction("ViewReferenceTerm", new { id = result.Key });
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to activate reference term: { e }");
            }

            TempData["error"] = Locale.UnableToActivate + " " + Locale.ReferenceTerm;

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays the create view.
        /// </summary>
        /// <returns>Returns the create view.</returns>
        [HttpGet]
		public ActionResult Create()
		{		                          
            var model = new CreateReferenceTermViewModel
			{   CodeSystemList = this.GetCodeSystems().CollectionItem.ToSelectList("Oid", "Key").ToList(),
                LanguageList = LanguageUtil.GetLanguageList().ToSelectList("DisplayName", "TwoLetterCountryCode").ToList(),
				TwoLetterCountryCode = Locale.EN
			};

			return View(model);
		}

		/// <summary>
		/// Adds the new reference term.
		/// </summary>
		/// <param name="model">The <see cref="ReferenceTermViewModel"/> instance.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateReferenceTermViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
                    var codeSystem = this.AmiClient.GetCodeSystem(model.CodeSystem.ToString());

                    if (codeSystem == null)
                    {
                        TempData["error"] = Locale.CodeSystem + " " + Locale.NotFound;

                        return RedirectToAction("Index");
                    }

				    var referenceTerm = model.ToReferenceTerm();
				    referenceTerm.CodeSystem = codeSystem;

                    var result = this.ImsiClient.Create(referenceTerm);

					TempData["success"] = Locale.ReferenceTerm + " " + Locale.Created + " " + Locale.Successfully;

					return RedirectToAction("ViewReferenceTerm", new { id = result.Key });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve entity: { e }");
			}

			TempData["error"] = Locale.UnableToCreate + " " + Locale.ReferenceTerm;

		    model.CodeSystemList = this.GetCodeSystems().CollectionItem.ToSelectList("Oid", "Key").ToList();
			model.LanguageList = LanguageUtil.GetLanguageList().ToSelectList("DisplayName", "TwoLetterCountryCode").ToList();

			return View(model);
		}

		/// <summary>
		/// Deactivates a reference term.
		/// </summary>
		/// <param name="id">The Reference Term identifier</param>		
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
                var referenceTerm = ImsiClient.Get<ReferenceTerm>(id, null) as ReferenceTerm;

                if (referenceTerm == null)
                {
                    TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

                    return RedirectToAction("Index");
                }

                this.ImsiClient.Obsolete(referenceTerm);

                TempData["success"] = Locale.ReferenceTerm + " " + Locale.Deactivated + " " + Locale.Successfully;

                return RedirectToAction("Index");
            }
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve entity: { e }");
			}

			TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

			return RedirectToAction("Index", "ReferenceTerm");
		}

		/// <summary>
		/// Retrieves the names and metadata associated with the reference term to edit
		/// </summary>
		/// <param name="id">The reference term identifier</param>        
		/// <returns>An ActionResult instance</returns>
		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			try
			{
				var referenceTerm = ImsiClient.Get<ReferenceTerm>(id, null) as ReferenceTerm;

				if (referenceTerm == null)
				{
					TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(new EditReferenceTermViewModel(referenceTerm));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve entity: { e }");
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.ReferenceTerm;

			return RedirectToAction("ViewReferenceTerm", "ReferenceTerm", new { id });

		}

		/// <summary>
		/// Updates the reference term associated with the Concept.
		/// </summary>
		/// <param name="model">The <see cref="ReferenceTermViewModel"/> instance.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(ReferenceTermViewModel model)
		{
			try
			{
				var referenceTerm = ImsiClient.Get<ReferenceTerm>(model.Id, null) as ReferenceTerm;

				if (referenceTerm == null)
				{
					TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				referenceTerm.Mnemonic = model.Mnemonic;

				var result = this.ImsiClient.Update(referenceTerm);

				TempData["success"] = Locale.ReferenceTerm + " " + Locale.Updated + " " + Locale.Successfully;

				return RedirectToAction("ViewReferenceTerm", "ReferenceTerm", new { id = result.Key });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve entity: { e }");
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.ReferenceTerm;

			return RedirectToAction("ViewReferenceTerm", "ReferenceTerm", new { id = model.Id });
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "ReferenceTerm";
			return View();
		}

		/// <summary>
		/// Displays the search view.
		/// </summary>
		/// <returns>Returns the search view.</returns>
		[HttpGet]
		[ValidateInput(false)]
		public ActionResult Search(string searchTerm)
		{
			var results = new List<ReferenceTermViewModel>();

			if (this.IsValidId(searchTerm))
			{
				Bundle bundle;

				if (searchTerm == "*")
				{
                    //bundle = this.ImsiClient.Query<ReferenceTerm>(c => c.ObsoletionTime == null);
                    //results = bundle.Item.OfType<ReferenceTerm>().Select(p => new ReferenceTermSearchResultsViewModel(p)).ToList();
                    bundle = this.ImsiClient.Query<ReferenceTerm>(c => c.Key != null);
                    results = bundle.Item.OfType<ReferenceTerm>().Select(p => new ReferenceTermViewModel(p)).ToList();
                }
				else
				{
                    //bundle = this.ImsiClient.Query<ReferenceTerm>(c => c.Mnemonic.Contains(searchTerm) && c.ObsoletionTime == null);
                    //results = bundle.Item.OfType<ReferenceTerm>().Where(c => c.Mnemonic.Contains(searchTerm) && c.ObsoletionTime == null).Select(p => new ReferenceTermSearchResultsViewModel(p)).ToList();
                    bundle = this.ImsiClient.Query<ReferenceTerm>(c => c.Mnemonic.Contains(searchTerm) && c.Key != null);
                    results = bundle.Item.OfType<ReferenceTerm>().Where(c => c.Mnemonic.Contains(searchTerm) && c.ObsoletionTime == null).Select(p => new ReferenceTermViewModel(p)).ToList();
                }

				TempData["searchTerm"] = searchTerm;

				return PartialView("_ReferenceTermSearchResultsPartial", results.OrderBy(c => c.Mnemonic));
			}

			TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_ReferenceTermSearchResultsPartial", results);
		}

		/// <summary>
		/// Retrieves the Reference Term by identifier
		/// </summary>
		/// <param name="id">The identifier of the Reference Term</param>		
		/// <returns>Returns the Reference Term ActionResult</returns>
		[HttpGet]
		public ActionResult ViewReferenceTerm(Guid id)
		{
			try
			{
				var referenceTerm = ImsiClient.Get<ReferenceTerm>(id, null) as ReferenceTerm;

				if (referenceTerm == null)
				{
					TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(new ReferenceTermViewModel(referenceTerm));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}
	}
}