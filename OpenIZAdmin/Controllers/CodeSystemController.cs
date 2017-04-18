using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.CodeSystem;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Util;

namespace OpenIZAdmin.Controllers
{
    /// <summary>
	/// Provides operations for managing code systems.
	/// </summary>
	[TokenAuthorize]
    public class CodeSystemController : BaseController
    {
        /// <summary>
        /// Displays the create view.
        /// </summary>
        /// <returns>Returns the create view.</returns>
        [HttpGet]
        public ActionResult Create()
        {
            return View(new CreateCodeSystemViewModel());
        }

        /// <summary>
		/// Creates a code system.
		/// </summary>
		/// <param name="model">The model containing the information to create a code system.</param>
		/// <returns>Returns the created concept.</returns>
		[HttpPost]
        [ValidateAntiForgeryToken]        
        public ActionResult Create(CreateCodeSystemViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = this.ImsiClient.Create<CodeSystem>(model.ToCodeSystem());

                    TempData["success"] = Locale.CodeSystem + " " + Locale.Created + " " + Locale.Successfully;

                    return RedirectToAction("ViewCodeSystem", new { id = result.Key });
                }
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
            }

            TempData["error"] = Locale.UnableToCreate + " " + Locale.CodeSystem;            

            return View(model);
        }

        /// <summary>
        /// Deletes a concept.
        /// </summary>
        /// <param name="id">The id of the code system to delete.</param>		
        /// <returns>Returns the index view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid? id)
        {
            try
            {
                var codeSystem = CodeSystemUtil.GetCodeSystem(ImsiClient, id);

                if (codeSystem == null)
                {
                    TempData["error"] = Locale.CodeSystem + " " + Locale.NotFound;

                    return RedirectToAction("Index");
                }

                this.ImsiClient.Obsolete<CodeSystem>(codeSystem);

                TempData["success"] = Locale.CodeSystem + " " + Locale.Deactivated + " " + Locale.Successfully;

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
            }

            TempData["error"] = Locale.CodeSystem + " " + Locale.NotFound;

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Edits the specified concept.
        /// </summary>
        /// <param name="id">The identifier.</param>        
        /// <returns>ActionResult.</returns>
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            var codeSystem = CodeSystemUtil.GetCodeSystem(ImsiClient, id);

            if (codeSystem == null)
            {
                TempData["error"] = Locale.CodeSystem + " " + Locale.NotFound;

                return RedirectToAction("Index");
            }

            return View(new EditCodeSystemViewModel(codeSystem));
        }

        /// <summary>
		/// Updates a Concept from the <see cref="EditConceptModel"/> instance.
		/// </summary>
		/// <param name="model">The EditCodeSystemViewModel instance</param>
		/// <returns></returns>
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditCodeSystemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var codeSystem = CodeSystemUtil.GetCodeSystem(ImsiClient, model.Id);

                if (codeSystem == null)
                {
                    TempData["error"] = Locale.CodeSystem + " " + Locale.NotFound;

                    return RedirectToAction("Index");
                }                

                codeSystem = model.ToCodeSystem(codeSystem);

                var result = this.ImsiClient.Update<CodeSystem>(codeSystem);

                TempData["success"] = Locale.CodeSystem + " " + Locale.Updated + " " + Locale.Successfully;

                return RedirectToAction("Edit", new { id = result.Key });
            }

            TempData["error"] = Locale.UnableToUpdate + " " + Locale.CodeSystem;

            return View(model);
        }

        /// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		public ActionResult Index()
        {
            TempData["searchType"] = "CodeSystem";
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
            var results = new List<CodeSystemSearchResultsViewModel>();

            if (this.IsValidId(searchTerm))
            {
                Bundle bundle;

                if (searchTerm == "*")
                {
                    bundle = this.ImsiClient.Query<CodeSystem>(c => c.ObsoletionTime == null);
                    results = bundle.Item.OfType<CodeSystem>().Select(p => new CodeSystemSearchResultsViewModel(p)).ToList();
                }
                else
                {
                    bundle = this.ImsiClient.Query<CodeSystem>(c => c.Name.Contains(searchTerm) && c.ObsoletionTime == null);
                    results = bundle.Item.OfType<CodeSystem>().Where(c => c.Name.Contains(searchTerm) && c.ObsoletionTime == null).Select(p => new CodeSystemSearchResultsViewModel(p)).ToList();
                }

                TempData["searchTerm"] = searchTerm;

                return PartialView("_CodeSystemSearchResultsPartial", results.OrderBy(c => c.Name));
            }

            TempData["error"] = Locale.InvalidSearch;
            TempData["searchTerm"] = searchTerm;

            return PartialView("_CodeSystemSearchResultsPartial", results);
        }

        /// <summary>
		/// Retrieves the Concept by identifier
		/// </summary>
		/// <param name="id">The identifier of the Concept</param>		
		/// <returns>Returns the concept view.</returns>
		[HttpGet]
        public ActionResult ViewCodeSystem(Guid? id)
        {
            try
            {
                var codeSystem = CodeSystemUtil.GetCodeSystem(ImsiClient, id);

                if (codeSystem == null)
                {
                    TempData["error"] = Locale.CodeSystem + " " + Locale.NotFound;

                    return RedirectToAction("Index");
                }                

                return View(new CodeSystemViewModel(codeSystem));
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
