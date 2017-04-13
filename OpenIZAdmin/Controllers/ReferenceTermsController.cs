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
        public ActionResult Create()
        {            
            var model = new CreateReferenceTermViewModel()
            {
                LanguageList = LanguageUtil.GetSelectListItemLanguageList().ToList(),
                TwoLetterCountryCode = Locale.EN
            };

            return View(model);
        }

  //      /// <summary>
		///// Displays the create view.
		///// </summary>
		///// <returns>Returns the create view.</returns>
		//[HttpGet]
  //      public ActionResult Create(Guid id)
  //      {

  //          var referenceTerm = ImsiClient.Get<ReferenceTerm>(id, null) as ReferenceTerm;

  //          if (referenceTerm == null)
  //          {
  //              TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

  //              return RedirectToAction("Index");
  //          }

  //          var model = new CreateReferenceTermNameViewModel(referenceTerm)
  //          {
  //              LanguageList = LanguageUtil.GetSelectListItemLanguageList().ToList(),
  //              TwoLetterCountryCode = Locale.EN
  //          };

  //          return View(model);            
  //      }

        
  //      /// <summary>
		///// Creates a concept.
		///// </summary>
		///// <param name="model">The model containing the information to create a reference term name.</param>
		///// <returns>Returns the created concept.</returns>
		//[HttpPost]
  //      [ValidateAntiForgeryToken]
  //      public ActionResult Create(CreateReferenceTermNameViewModel model)
  //      {
  //          try
  //          {
  //              if (ModelState.IsValid)
  //              {
  //                  var referenceTerm = ImsiClient.Get<ReferenceTerm>(model.ReferenceTermId, null) as ReferenceTerm;

  //                  if (referenceTerm == null)
  //                  {
  //                      TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

  //                      return RedirectToAction("Index");
  //                  }

  //                  var name = new ReferenceTermName()
  //                  {
  //                      Language = model.Language,
  //                      Name = model.Name
  //                  };

  //                  referenceTerm.DisplayNames.Add(name);

  //                  var result = this.ImsiClient.Update<ReferenceTerm>(referenceTerm);

  //                  TempData["success"] = Locale.ReferenceTermName + " " + Locale.Created + " " + Locale.Successfully;

  //                  return RedirectToAction("Edit", new { id = result.Key });                    
  //              }
  //          }
  //          catch (Exception e)
  //          {
  //              ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
  //          }

  //          TempData["error"] = Locale.UnableToCreate + " " + Locale.ReferenceTermName;

  //          //model.LanguageList = LanguageUtil.GetSelectListItemLanguageList().ToList();

  //          return View(model);
  //      }


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
                    var referenceTerm = this.ImsiClient.Create<ReferenceTerm>(model.ToReferenceTerm());

                    TempData["success"] = Locale.ReferenceTerm + " " + Locale.Created + " " + Locale.Successfully;

                    return RedirectToAction("ViewReferenceTerm", new { id = referenceTerm.Key });
                }
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to retrieve entity: { e }");
            }

            TempData["error"] = Locale.UnableToCreate + " " + Locale.ReferenceTerm;

            model.LanguageList = LanguageUtil.GetSelectListItemLanguageList().ToList();

            return View(model);
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
                ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to retrieve entity: { e }");
            }

            TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

            return RedirectToAction("Index", "ReferenceTerms");
            //return View();
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

            return RedirectToAction("ViewReferenceTerm", "ReferenceTerms", new { id });

        }

        ///// <summary>
        ///// Retrieves the languages associated with the Concept to edit
        ///// </summary>
        ///// <param name="id">The concept Guid id</param>
        ///// <param name="versionId">The version identifier of the Concept instance.</param>
        ///// <param name="termId">The identifier of the reference term</param>        
        ///// <returns>An ActionResult instance</returns>
        //[HttpGet]
        //public ActionResult Edit(Guid? id, Guid? versionId, Guid? termId)
        //{
        //    try
        //    {
        //        var concept = ConceptUtil.GetConcept(ImsiClient, id, versionId);

        //        if (concept == null)
        //        {
        //            TempData["error"] = Locale.Concept + " " + Locale.NotFound;
        //            return RedirectToAction("Index", "ReferenceTerms");
        //        }                

        //        return View(new ReferenceTermViewModel(ReferenceTermUtil.GetConceptReferenceTerm(ImsiClient, concept, termId), concept));
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
        //        Trace.TraceError($"Unable to retrieve entity: { e }");
        //    }

        //    TempData["error"] = Locale.UnableToUpdate + " " + Locale.ReferenceTerm;

        //    return RedirectToAction("ViewReferenceTerm", "ReferenceTerms", new { id });

        //}

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

                var result = this.ImsiClient.Update<ReferenceTerm>(referenceTerm);

                TempData["success"] = Locale.ReferenceTerm + " " + Locale.Updated + " " + Locale.Successfully;

                return RedirectToAction("ViewReferenceTerm", "ReferenceTerms", new { id = result.Key });
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to retrieve entity: { e }");
            }

            TempData["error"] = Locale.UnableToUpdate + " " + Locale.ReferenceTerm;

            return RedirectToAction("ViewReferenceTerm", "ReferenceTerms", new { id = model.Id });            
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