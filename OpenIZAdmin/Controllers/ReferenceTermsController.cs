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
    public class ReferenceTermsController : BaseController
    {
        /// <summary>
		/// Displays the create view.
		/// </summary>
		/// <returns>Returns the create view.</returns>
		[HttpGet]
        public ActionResult Create(Guid id, Guid? versionId)
        {

            var concept = ConceptUtil.GetConcept(ImsiClient, id, versionId);

            if (concept == null)
            {
                TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                return RedirectToAction("Index", "Concept");
            }

            var model = new ReferenceTermViewModel(concept)
            {
                LanguageList = LanguageUtil.GetSelectListItemLanguageList().ToList(),
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
        public ActionResult Create(ReferenceTermViewModel model)
        {
            try
            {
                //var concept = ConceptUtil.GetConcept(ImsiClient, model.ConceptId, model.ConceptVersionKey);

                //if (concept == null)
                //{
                //    TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                //    return RedirectToAction("Index", "Concept");
                //}

                //concept.ConceptNames.Add(new ConceptName
                //{
                //    Language = model.TwoLetterCountryCode,
                //    Name = model.DisplayName,
                //});

                //var result = this.ImsiClient.Update<Concept>(concept);

                //TempData["success"] = Locale.Language + " " + Locale.Updated + " " + Locale.Successfully;

                //return RedirectToAction("Edit", "Concept", new { id = result.Key, versionKey = result.VersionKey });
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to retrieve entity: { e }");
            }

            //return View(model);
            return View();
        }

        /// <summary>
        /// Deletes a reference term from a Concept.
        /// </summary>
        /// <param name="id">The Concept Guid id</param>
        /// <param name="versionId">The verion identifier of the Concept instance.</param>
        /// <param name="mnemonic">The mnemonic of the reference term</param>
        /// <param name="name">The text name representation of the reference term</param>
        /// <returns>Returns the index view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid? id, Guid? versionId, string mnemonic, string name)
        {
            try
            {
                //var concept = ConceptUtil.GetConcept(ImsiClient, id, versionId);

                //if (concept == null)
                //{
                //    TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                //    return RedirectToAction("Index", "Concept");
                //}

                //var index = concept.ConceptNames.FindIndex(c => c.Language == langCode && c.Name == displayName);
                //if (index < 0)
                //{
                //    TempData["error"] = Locale.LanguageCode + " " + Locale.NotFound;
                //    return RedirectToAction("ViewConcept", "Concept", new { id, versionKey = versionId });
                //}

                //concept.ConceptNames.RemoveAt(index);

                //var result = this.ImsiClient.Update<Concept>(concept);

                //TempData["success"] = Locale.Language + " " + Locale.Deleted + " " + Locale.Successfully;

                //return RedirectToAction("Edit", "Concept", new { id = result.Key, versionId = result.VersionKey });
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
            }

            //TempData["error"] = Locale.Concept + " " + Locale.NotFound;

            //return RedirectToAction("Index", "Concept");
            return View();
        }


        /// <summary>
        /// Retrieves the languages associated with the Concept to edit
        /// </summary>
        /// <param name="id">The concept Guid id</param>
        /// <param name="versionId">The version identifier of the Concept instance.</param>
        /// <param name="termId">The identifier of the reference term</param>        
        /// <returns>An ActionResult instance</returns>
        [HttpGet]
        public ActionResult Edit(Guid? id, Guid? versionId, Guid? termId)
        {
            try
            {
                var concept = ConceptUtil.GetConcept(ImsiClient, id, versionId);

                if (concept == null)
                {
                    TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                    return RedirectToAction("Index", "Concept");
                }                

                return View(new ReferenceTermViewModel(ReferenceTermUtil.GetConceptReferenceTerm(ImsiClient, concept, termId), concept));
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to retrieve entity: { e }");
            }

            TempData["error"] = Locale.UnableToUpdate + " " + Locale.Concept + " " + Locale.ReferenceTerm;

            return RedirectToAction("ViewConcept", "Concept", new { id, versionId });

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
                //var concept = ConceptUtil.GetConcept(this.ImsiClient, model.ConceptId, model.ConceptVersionKey);

                //if (concept == null)
                //{
                //    TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                //    return RedirectToAction("Index", "Concept");
                //}

                //var index = concept.ConceptNames.FindIndex(c => c.Language == model.Language && c.Name == model.Name);

                //if (index < 0)
                //{
                //    TempData["error"] = Locale.LanguageCode + " " + Locale.NotFound;
                //    return RedirectToAction("Edit", "Concept", new { id = model.ConceptId, versionKey = model.ConceptVersionKey });
                //}

                //concept.ConceptNames[index].Language = model.TwoLetterCountryCode;
                //concept.ConceptNames[index].Name = model.DisplayName;

                //var result = this.ImsiClient.Update<Concept>(concept);

                //TempData["success"] = Locale.Concept + " " + Locale.Updated + " " + Locale.Successfully;

                //return RedirectToAction("Edit", "Concept", new { id = result.Key, versionKey = result.VersionKey });
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to retrieve entity: { e }");
            }

            //TempData["error"] = Locale.UnableToUpdate + " " + Locale.Concept;

            //return RedirectToAction("ViewConcept", "Concept", new { id = model.ConceptId, model.ConceptVersionKey });

            return View();
        }

        /// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
        public ActionResult Index()
        {
            TempData["searchType"] = "ReferenceTerms";
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
            var results = new List<ReferenceTermSearchResultsViewModel>();

            if (CommonUtil.IsValidString(searchTerm))
            {
                Bundle bundle;

                if (searchTerm == "*")
                {
                    bundle = this.ImsiClient.Query<ReferenceTerm>(c => c.ObsoletionTime == null);
                    results = bundle.Item.OfType<ReferenceTerm>().Select(p => new ReferenceTermSearchResultsViewModel(p)).ToList();
                }
                else
                {
                    bundle = this.ImsiClient.Query<ReferenceTerm>(c => c.Mnemonic.Contains(searchTerm) && c.ObsoletionTime == null);
                    results = bundle.Item.OfType<ReferenceTerm>().Where(c => c.Mnemonic.Contains(searchTerm) && c.ObsoletionTime == null).Select(p => new ReferenceTermSearchResultsViewModel(p)).ToList();
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
        public ActionResult View(Guid id)
        {
            try
            {
                //var concept = ConceptUtil.GetConcept(ImsiClient, id, versionId);
                var referenceTerm = ImsiClient.Get<ReferenceTerm>(id, null) as ReferenceTerm;

                if (referenceTerm == null)
                {
                    TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

                    return RedirectToAction("Index");
                }

                var model = new ReferenceTermViewModel(referenceTerm)
                {
                    //ReferenceTerms = ReferenceTermUtil.GetConceptReferenceTermsList(ImsiClient, concept)
                };

                //for (var i = 0; i < concept.ReferenceTerms.Count(r => r.ReferenceTerm == null && r.RelationshipTypeKey.HasValue); i++)
                //{
                //	concept.ReferenceTerms[i].ReferenceTerm = this.ImsiClient.Get<ReferenceTerm>(concept.ReferenceTerms[i].ReferenceTermKey.Value, null) as ReferenceTerm;
                //}

                //conceptViewModel.ReferenceTerms.AddRange(concept.ReferenceTerms.Select(r => new ReferenceTermModel
                //{
                //	Mnemonic = r.ReferenceTerm?.Mnemonic,
                //	Name = string.Join(", ", r.ReferenceTerm.DisplayNames.Select(d => d.Name)),
                //	Id = r.Key.Value
                //}));

                return View(model);
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
            }

            TempData["error"] = Locale.Concept + " " + Locale.NotFound;

            return RedirectToAction("Index");
        }
    }
}