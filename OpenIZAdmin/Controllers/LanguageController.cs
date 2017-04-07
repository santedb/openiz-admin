using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.LanguageModels;
using OpenIZAdmin.Util;

namespace OpenIZAdmin.Controllers
{
    /// <summary>
    /// Provides operations for managing languages.
    /// </summary>
    public class LanguageController : BaseController
    {        
        /// <summary>
        /// Displays the Create view.
        /// </summary>
        /// <returns>Returns the Create view.</returns>
        [HttpGet]
        public ActionResult Create(Guid id)
        {                        
            var concept = ConceptUtil.GetConcept(ImsiClient, id);

            if (concept == null)
            {
                TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                return RedirectToAction("Index", "Concept");
            }

            var model = new LanguageModel(concept)
            {
                LanguageList = LanguageUtil.GetSelectListItemLanguageList().ToList(),                
            };
            
            return View(model);
        }

        /// <summary>
		/// Adds the new language.
		/// </summary>		
		/// <param name="model">The Edit LanguageModel instance.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LanguageModel model)
        {            
            try
            {                
                var concept = ConceptUtil.GetConcept(ImsiClient, model.ConceptId);

                if (concept == null)
                {
                    TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                    return RedirectToAction("Index", "Concept");
                }

                concept.ConceptNames.Add(new ConceptName
                {
                    Language = model.TwoLetterCountryCode,
                    Name = model.DisplayName,
                    //Key = Guid.NewGuid(),
                    //EffectiveVersionSequenceId = concept.VersionSequence
                });

                var result = this.ImsiClient.Update<Concept>(concept);

                TempData["success"] = Locale.Language + " " + Locale.Updated + " " + Locale.Successfully;

                return RedirectToAction("ViewConcept", "Concept", new { id = result.Key });               
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to retrieve entity: { e }");
            }

            return View(model);
        }

        /// <summary>
        /// Deletes a language from a Concept.
        /// </summary>        
        /// <param name="id">The Concept Guid id</param>
        /// <param name="langCode">The language two character code identifier</param>
        /// <param name="displayName">The text name representation of the Concept</param>
        /// <returns>Returns the index view.</returns>
        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid? id, string langCode, string displayName)
        {
            try
            {                
                var concept = ConceptUtil.GetConcept(ImsiClient, id);

                if (concept == null)
                {
                    TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                    return RedirectToAction("Index", "Concept");
                }
                    
                var index = concept.ConceptNames.FindIndex(c => c.Language == langCode && c.Name == displayName);
                if (index < 0)
                {
                    TempData["error"] = Locale.LanguageCode + " " + Locale.NotFound;
                    return RedirectToAction("ViewConcept", "Concept", new { id = id });
                }

                concept.ConceptNames.RemoveAt(index);                

                var result = this.ImsiClient.Update<Concept>(concept);

                TempData["success"] = Locale.Language + " " + Locale.Deleted + " " + Locale.Successfully;

                return RedirectToAction("ViewConcept", "Concept", new { id = result.Key });             
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
            }

            TempData["error"] = Locale.Concept + " " + Locale.NotFound;

            return RedirectToAction("Index", "Concept");
        }



        /// <summary>
        /// Retrieves the languages associated with the Concept to edit
        /// </summary>
        /// <param name="id">The concept Guid id</param>
        /// <param name="langCode">The language two character code identifier</param>
        /// <param name="displayName">The text name representation of the Concept</param>
        /// <returns>An ActionResult instance</returns>
        [HttpGet]
        public ActionResult Edit(Guid? id, string langCode, string displayName )
        {            
            var concept = ConceptUtil.GetConcept(this.ImsiClient, id);

            if (concept == null)
            {
                TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                return RedirectToAction("Index", "Concept");
            }            

            var model = new LanguageModel(langCode, displayName, concept)
            {
                LanguageList = LanguageUtil.GetSelectListItemLanguageList().ToList()
            };            

            return View(model);
        }

        /// <summary>
        /// Adds the new identifier.
        /// </summary>
        /// <param name="id">The entity identifier for which to add a new identifier.</param>
        /// <param name="type">The type.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LanguageModel model)
        {            
            try
            {
                var concept = ConceptUtil.GetConcept(this.ImsiClient, model.ConceptId);

                if (concept == null)
                {
                    TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                    return RedirectToAction("Index", "Concept");
                }

                var index = concept.ConceptNames.FindIndex(c => c.Language == model.Language && c.Name == model.Name);

                if (index < 0)
                {
                    TempData["error"] = Locale.LanguageCode + " " + Locale.NotFound;
                    return RedirectToAction("ViewConcept", "Concept", new { id = model.ConceptId });
                }
                
                concept.ConceptNames[index].Language = model.TwoLetterCountryCode;
                concept.ConceptNames[index].Name = model.DisplayName;                    

                var result = this.ImsiClient.Update<Concept>(concept);

                TempData["success"] = Locale.Concept + " " + Locale.Updated + " " + Locale.Successfully;

                return RedirectToAction("Edit", "Concept", new { id = result.Key });
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to retrieve entity: { e }");
            }

            TempData["error"] = Locale.UnableToUpdate + " " + Locale.Concept;

            return RedirectToAction("ViewConcept", "Concept", new { id = model.ConceptId });
        }
        
    }
}