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
                //TwoLetterCountryCodeList = {[0] = Locale.EN}
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
        public ActionResult Create(LanguageModel model)
        {
            //var model = new LanguageModel();

            try
            {
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

        /// <summary>
        /// Displays the Create view.
        /// </summary>
        /// <returns>Returns the Create view.</returns>
        [HttpGet]
        public ActionResult Edit(Guid? id, string langCode, string displayName )
        {
            var bundle = this.ImsiClient.Query<Concept>(c => c.Key == id && c.ObsoletionTime == null);

            bundle.Reconstitute();

            var concept = bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Key == id && c.ObsoletionTime == null);

            if (concept == null)
            {
                TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                return RedirectToAction("Index", "Concept");
            }

            var model = new LanguageModel(langCode, displayName, concept)
            {
                LanguageList = LanguageUtil.GetSelectListItemLanguageList().ToList()
            };

            //model.TwoLetterCountryCode = Locale.EN;

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
            //var model = new LanguageModel();

            try
            {
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