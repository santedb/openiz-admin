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
using System.Web.Mvc;
using Elmah;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Util;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptModels;
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
        /// Displays the create view.
        /// </summary>
        /// <returns>Returns the create view.</returns>
        [HttpGet]
		public ActionResult Create()
		{		                          
            var model = new CreateReferenceTermModel
			{
                CodeSystemList = this.GetCodeSystems().CollectionItem.ToSelectList("Oid", "Key", null, true).ToList(),
                LanguageList = LanguageUtil.GetLanguageList().ToSelectList("DisplayName", "TwoLetterCountryCode", null, true).ToList(),
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
		public ActionResult Create(CreateReferenceTermModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
                    var codeSystem = this.AmiClient.GetCodeSystem(model.CodeSystem);

                    if (codeSystem == null)
                    {
                        TempData["error"] = Locale.CodeSystemNotFound;

                        return RedirectToAction("Index");
                    }

				    var referenceTerm = model.ToReferenceTerm();
				    referenceTerm.CodeSystem = codeSystem;

                    var result = this.ImsiClient.Create(referenceTerm);

					TempData["success"] = Locale.ReferenceTermCreatedSuccessfully;

					return RedirectToAction("ViewReferenceTerm", new { id = result.Key });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve entity: { e }");
			}

			TempData["error"] = Locale.UnableToCreateReferenceTerm;

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
                    TempData["error"] = Locale.ReferenceTermNotFound;

                    return RedirectToAction("Index");
                }

                this.ImsiClient.Obsolete(referenceTerm);

                TempData["success"] = Locale.ReferenceTermDeactivatedSuccessfully;

                return RedirectToAction("Index");
            }
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve entity: { e }");
			}

			TempData["error"] = Locale.ReferenceTermNotFound;

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
				var bundle = this.ImsiClient.Query<ReferenceTerm>(r => r.Key == id && r.ObsoletionTime == null, 0, null, true);

				bundle.Reconstitute();

				var referenceTerm = bundle.Item.OfType<ReferenceTerm>().FirstOrDefault(r => r.Key == id && r.ObsoletionTime == null);

				if (referenceTerm == null)
				{
					TempData["error"] = Locale.ReferenceTermNotFound;

					return RedirectToAction("Index");
				}

				return View(new EditReferenceTermModel(referenceTerm));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve entity: { e }");
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

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
					TempData["error"] = Locale.ReferenceTermNotFound;

					return RedirectToAction("Index");
				}

				referenceTerm.CreationTime = DateTimeOffset.Now;
				referenceTerm.Mnemonic = model.Mnemonic;

				var result = this.ImsiClient.Update(referenceTerm);

				TempData["success"] = Locale.ReferenceTermUpdatedSuccessfully;

				return RedirectToAction("ViewReferenceTerm", "ReferenceTerm", new { id = result.Key });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve entity: { e }");
			}

			TempData["error"] = Locale.UnableToUpdateReferenceTerm;

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
            TempData["searchTerm"] = "*";
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
				var bundle = searchTerm == "*" ? this.ImsiClient.Query<ReferenceTerm>(c => c.ObsoletionTime == null, 0, null, new [] { "name" }) : this.ImsiClient.Query<ReferenceTerm>(c => c.Mnemonic.Contains(searchTerm) && c.ObsoletionTime == null, 0, null, new[] { "name" });

				//foreach (var referenceTerm in bundle.Item.OfType<ReferenceTerm>())
				//{
				//	referenceTerm.LoadCollection<ReferenceTermName>(nameof(ReferenceTerm.DisplayNames));
				//}

                results = bundle.Item.OfType<ReferenceTerm>().Select(p => new ReferenceTermViewModel(p)).ToList();

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
				var bundle = this.ImsiClient.Query<ReferenceTerm>(r => r.Key == id && r.ObsoletionTime == null, 0, null, true);

				bundle.Reconstitute();

				var referenceTerm = bundle.Item.OfType<ReferenceTerm>().FirstOrDefault(r => r.Key == id && r.ObsoletionTime == null);

				if (referenceTerm == null)
				{
					TempData["error"] = Locale.ReferenceTermNotFound;

					return RedirectToAction("Index");
				}

				var conceptsBundle = this.ImsiClient.Query<Concept>(c => c.ReferenceTerms.Any(r => r.ReferenceTermKey == referenceTerm.Key) && c.ObsoletionTime == null);

				conceptsBundle.Reconstitute();

				var concepts = conceptsBundle.Item.OfType<Concept>().Where(c => c.ReferenceTerms.Any(r => r.ReferenceTermKey == referenceTerm.Key) && c.ObsoletionTime == null);

				foreach (var concept in concepts)
				{
					concept.LoadProperty<ConceptClass>(nameof(Concept.Class));
				}

				var viewModel = new ReferenceTermViewModel(referenceTerm)
				{
					Concepts = concepts.Select(c => new ConceptViewModel(c)).ToList()
				};


				return View(viewModel);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

			return RedirectToAction("Index");
		}
	}
}