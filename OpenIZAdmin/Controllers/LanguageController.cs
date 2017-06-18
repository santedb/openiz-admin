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
 * Date: 2017-4-19
 */

using Elmah;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.LanguageModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using OpenIZAdmin.Extensions;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing languages.
	/// </summary>
	public class LanguageController : MetadataController
	{
		/// <summary>
		/// Displays the Create view.
		/// </summary>
		/// <returns>Returns the Create view.</returns>
		[HttpGet]
		public ActionResult Create(Guid id, Guid? versionId)
		{
			try
			{
				var concept = this.GetConcept(id, versionId);

				if (concept == null)
				{
					TempData["error"] = Locale.ConceptNotFound;
					return RedirectToAction("Index", "Concept");
				}

				var model = new LanguageViewModel(concept)
				{
					LanguageList = LanguageUtil.GetLanguageList().ToSelectList("DisplayName", "TwoLetterCountryCode").ToList(),
					TwoLetterCountryCode = Locale.EN
				};

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve entity: { e }");
			}

			TempData["error"] = Locale.ConceptNotFound;

			return RedirectToAction("Index", "Concept");
		}

		/// <summary>
		/// Adds the new language.
		/// </summary>
		/// <param name="model">The <see cref="LanguageViewModel"/> instance.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(LanguageViewModel model)
		{
			try
			{
				var concept = this.GetConcept(model.ConceptId.Value, model.ConceptVersionKey);

				if (concept == null)
				{
					TempData["error"] = Locale.ConceptNotFound;
					return RedirectToAction("Index", "Concept");
				}

				concept.ConceptSetsXml = this.LoadConceptSets(model.ConceptId.Value);

				concept.CreationTime = DateTimeOffset.Now;
				concept.VersionKey = null;

				concept.ConceptNames.Add(new ConceptName
				{
					Language = model.TwoLetterCountryCode,
					Name = model.DisplayName
				});

				var result = this.ImsiClient.Update<Concept>(concept);

				TempData["success"] = Locale.ConceptNameUpdatedSuccessfully;

				return RedirectToAction("Edit", "Concept", new { id = result.Key, versionId = result.VersionKey });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve entity: { e }");
			}

			model.LanguageList = LanguageUtil.GetLanguageList().ToSelectList("DisplayName", "TwoLetterCountryCode").ToList();

			TempData["error"] = Locale.UnableToAddConceptName;

			return RedirectToAction("Index", "Concept");
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
		public ActionResult Delete(Guid id, Guid versionId, string langCode, string displayName)
		{
			try
			{
				var concept = this.GetConcept(id, versionId);

				if (concept == null)
				{
					TempData["error"] = Locale.ConceptNotFound;
					return RedirectToAction("Index", "Concept");
				}

				concept.ConceptSetsXml = this.LoadConceptSets(id);

				var index = concept.ConceptNames.FindIndex(c => c.Language == langCode && c.Name == displayName);
				if (index < 0)
				{
					TempData["error"] = Locale.LanguageCodeNotFound;
					return RedirectToAction("ViewConcept", "Concept", new { id, versionKey = versionId });
				}

				concept.CreationTime = DateTimeOffset.Now;
				concept.VersionKey = null;

				concept.ConceptNames.RemoveAt(index);

				var result = this.ImsiClient.Update<Concept>(concept);

				TempData["success"] = Locale.LanguageDeletedSuccessfully;

				return RedirectToAction("Edit", "Concept", new { id = result.Key, versionId = result.VersionKey });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.ConceptNotFound;

			return RedirectToAction("Index", "Concept");
		}

		/// <summary>
		/// Retrieves the languages associated with the Concept to edit
		/// </summary>
		/// <param name="id">The concept Guid id</param>
		/// <param name="versionId">The version identifier of the Concept instance.</param>
		/// <param name="langId">The language identifier.</param>
		/// <param name="langCode">The language two character code identifier</param>
		/// <param name="displayName">The text name representation of the Concept</param>
		/// <returns>An ActionResult instance</returns>
		[HttpGet]
		public ActionResult Edit(Guid id, Guid versionId, Guid? langId, string langCode, string displayName)
		{
			Concept concept = null;
			try
			{
				concept = this.GetConcept(id, versionId);

				if (concept == null)
				{
					TempData["error"] = Locale.ConceptNotFound;
					return RedirectToAction("Index", "Concept");
				}

				var model = new LanguageViewModel(langCode, displayName, concept)
				{
					LanguageList = LanguageUtil.GetLanguageList().ToSelectList("DisplayName", "TwoLetterCountryCode").ToList()
				};

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToUpdateLanguage;

			if (concept != null)
			{
				return RedirectToAction("ViewConcept", "Concept", new { id = concept.Key, concept.VersionKey });
			}

			return RedirectToAction("Index", "Concept");
		}

		/// <summary>
		/// Updates the language associated with the Concept.
		/// </summary>
		/// <param name="model">The <see cref="LanguageViewModel"/> instance.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(LanguageViewModel model)
		{
			try
			{
				var concept = this.GetConcept(model.ConceptId.Value, model.ConceptVersionKey.Value);

				if (concept == null)
				{
					TempData["error"] = Locale.ConceptNotFound;
					return RedirectToAction("Index", "Concept");
				}

				concept.ConceptSetsXml = this.LoadConceptSets(model.ConceptId.Value);

				var index = concept.ConceptNames.FindIndex(c => c.Language == model.Language && c.Name == model.Name);

				if (index < 0)
				{
					TempData["error"] = Locale.LanguageCodeNotFound;
					return RedirectToAction("Edit", "Concept", new { id = model.ConceptId, versionKey = model.ConceptVersionKey });
				}

				concept.ConceptNames[index].Language = model.TwoLetterCountryCode;
				concept.ConceptNames[index].Name = model.DisplayName;

				concept.CreationTime = DateTimeOffset.Now;
				concept.VersionKey = null;

				var result = this.ImsiClient.Update<Concept>(concept);

				TempData["success"] = Locale.ConceptUpdatedSuccessfully;

				return RedirectToAction("Edit", "Concept", new { id = result.Key, versionId = result.VersionKey });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve concept: { e }");
			}

			TempData["error"] = Locale.UnableToUpdateConcept;

			return RedirectToAction("ViewConcept", "Concept", new { id = model.ConceptId, model.ConceptVersionKey });
		}
	}
}