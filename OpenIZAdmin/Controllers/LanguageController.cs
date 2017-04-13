using Elmah;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.LanguageModels;
using OpenIZAdmin.Util;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

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
		public ActionResult Create(Guid id, Guid? versionId)
		{
			var concept = ConceptUtil.GetConcept(ImsiClient, id, versionId);

			if (concept == null)
			{
				TempData["error"] = Locale.Concept + " " + Locale.NotFound;
				return RedirectToAction("Index", "Concept");
			}

			var model = new LanguageModel(concept)
			{
				LanguageList = LanguageUtil.GetSelectListItemLanguageList().ToList(),
				TwoLetterCountryCode = Locale.EN
			};

			return View(model);
		}

		/// <summary>
		/// Adds the new language.
		/// </summary>
		/// <param name="model">The <see cref="LanguageModel"/> instance.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(LanguageModel model)
		{
			try
			{
				var concept = ConceptUtil.GetConcept(ImsiClient, model.ConceptId, model.ConceptVersionKey);

				if (concept == null)
				{
					TempData["error"] = Locale.Concept + " " + Locale.NotFound;
					return RedirectToAction("Index", "Concept");
				}

				concept.ConceptNames.Add(new ConceptName
				{
					Language = model.TwoLetterCountryCode,
					Name = model.DisplayName,
				});

				var result = this.ImsiClient.Update<Concept>(concept);

				TempData["success"] = Locale.Language + " " + Locale.Updated + " " + Locale.Successfully;

				return RedirectToAction("Edit", "Concept", new { id = result.Key, versionKey = result.VersionKey });
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
        /// <param name="versionId">The verion identifier of the Concept instance.</param>
        /// <param name="langCode">The language two character code identifier</param>
        /// <param name="displayName">The text name representation of the Concept</param>
        /// <returns>Returns the index view.</returns>
        [HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid? id, Guid? versionId, string langCode, string displayName)
		{
			try
			{
				var concept = ConceptUtil.GetConcept(ImsiClient, id, versionId);

				if (concept == null)
				{
					TempData["error"] = Locale.Concept + " " + Locale.NotFound;
					return RedirectToAction("Index", "Concept");
				}

				var index = concept.ConceptNames.FindIndex(c => c.Language == langCode && c.Name == displayName);
				if (index < 0)
				{
					TempData["error"] = Locale.LanguageCode + " " + Locale.NotFound;
					return RedirectToAction("ViewConcept", "Concept", new { id, versionKey = versionId });
				}

				concept.ConceptNames.RemoveAt(index);

				var result = this.ImsiClient.Update<Concept>(concept);

				TempData["success"] = Locale.Language + " " + Locale.Deleted + " " + Locale.Successfully;

				return RedirectToAction("Edit", "Concept", new { id = result.Key, versionId = result.VersionKey });
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
        /// <param name="versionId">The version identifier of the Concept instance.</param>
        /// <param name="langCode">The language two character code identifier</param>
        /// <param name="displayName">The text name representation of the Concept</param>
        /// <returns>An ActionResult instance</returns>
        [HttpGet]
		public ActionResult Edit(Guid? id, Guid? versionId, string langCode, string displayName)
		{
			var concept = ConceptUtil.GetConcept(ImsiClient, id, versionId);

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
		/// Updates the language associated with the Concept.
		/// </summary>
		/// <param name="model">The <see cref="LanguageModel"/> instance.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(LanguageModel model)
		{
			try
			{
				var concept = ConceptUtil.GetConcept(this.ImsiClient, model.ConceptId, model.ConceptVersionKey);

				if (concept == null)
				{
					TempData["error"] = Locale.Concept + " " + Locale.NotFound;
					return RedirectToAction("Index", "Concept");
				}

				var index = concept.ConceptNames.FindIndex(c => c.Language == model.Language && c.Name == model.Name);

				if (index < 0)
				{
					TempData["error"] = Locale.LanguageCode + " " + Locale.NotFound;
					return RedirectToAction("Edit", "Concept", new { id = model.ConceptId, versionKey = model.ConceptVersionKey });
				}

				concept.ConceptNames[index].Language = model.TwoLetterCountryCode;
				concept.ConceptNames[index].Name = model.DisplayName;

				var result = this.ImsiClient.Update<Concept>(concept);

				TempData["success"] = Locale.Concept + " " + Locale.Updated + " " + Locale.Successfully;

				return RedirectToAction("Edit", "Concept", new { id = result.Key, versionKey = result.VersionKey });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve entity: { e }");
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Concept;

			return RedirectToAction("ViewConcept", "Concept", new { id = model.ConceptId, model.ConceptVersionKey });
		}
	}
}