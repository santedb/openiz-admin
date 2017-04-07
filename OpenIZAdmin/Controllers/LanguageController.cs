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
            var bundle = this.ImsiClient.Query<Concept>(c => c.Key == id && c.ObsoletionTime == null);

            bundle.Reconstitute();

            var concept = bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Key == id && c.ObsoletionTime == null);

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

                var bundle = this.ImsiClient.Query<Concept>(c => c.Key == model.ConceptId && c.ObsoletionTime == null);

                bundle.Reconstitute();

                var concept = bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Key == model.ConceptId && c.ObsoletionTime == null);

                if (concept == null)
                {
                    TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                    return RedirectToAction("Index", "Concept");
                }

                concept.ConceptNames.Add(new ConceptName
                {
                    Language = model.Language,
                    Name = model.Name,
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
		/// Deletes a concept.
		/// </summary>
		/// <param name="id">The id of the concept to delete.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult Delete(LanguageModel model)
        {
            try
            {
                if (model.ConceptId != null)
                {
                    var concept = this.ImsiClient.Get<Concept>((Guid)model.ConceptId, null) as Concept;

                    if (concept == null)
                    {
                        TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                        return RedirectToAction("Index", "Concept");
                    }
                    
                    var index = concept.ConceptNames.FindIndex(c => c.Language == model.Language && c.Name == model.Name);
                    concept.ConceptNames.RemoveAt(index);                

                    var result = this.ImsiClient.Update<Concept>(concept);

                    TempData["success"] = Locale.Language + " " + Locale.Deleted + " " + Locale.Successfully;

                    return RedirectToAction("ViewConcept", "Concept", new { id = result.Key });
                }
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
            }

            TempData["error"] = Locale.Concept + " " + Locale.NotFound;

            return RedirectToAction("Index", "Concept");
        }


        /// <summary>
        /// Displays the Create view.
        /// </summary>
        /// <returns>Returns the Create view.</returns>
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
                //if (model.Name[i] != string.Empty)
                //{
                //	if (concept.ConceptNames.Count > i)
                //	{
                //		if (concept.ConceptNames[i].Language == model.Languages[i])
                //		{
                //			concept.ConceptNames[i].Name = model.Name[i];
                //		}
                //	}
                //	else
                //	{
                //		concept.ConceptNames.Add(new ConceptName
                //		{
                //			Language = model.Languages[i],
                //			Name = model.Name[i]
                //		});
                //	}
                //}


                //var modelType = this.GetModelType(type);
                //var entity = this.GetEntity(model.EntityId, modelType);

                //model.ExistingIdentifiers = entity.Identifiers.Select(i => new EntityIdentifierViewModel(i)).ToList();
                //model.ModelType = type;
                //model.Types = RemoveExistingIdentifiers(this.GetAssigningAuthorities().ToSelectList("Name", "Key").ToList(), entity.Identifiers);
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to retrieve entity: { e }");
            }

            return View(model);
        }

        // GET: Language
        //public ActionResult Index()
        //{
        //    return View();
        //}
    }
}