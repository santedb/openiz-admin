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
 * User: yendtr
 * Date: 2016-7-23
 */

using Elmah;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing concepts.
	/// </summary>
	[TokenAuthorize]
	public class ConceptController : BaseController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptController"/> class.
		/// </summary>
		public ConceptController()
		{
		}

		/// <summary>
		/// Displays the create view.
		/// </summary>
		/// <returns>Returns the create view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			var model = new CreateConceptModel();

			var languages = LanguageUtil.GetLanguageList();

			var bundle = this.ImsiClient.Query<ConceptClass>(c => c.ObsoletionTime == null);

			bundle.Reconstitute();

			var conceptClasses = bundle.Item.OfType<ConceptClass>();

			model.ConceptClassList.AddRange(conceptClasses.ToSelectList().OrderBy(c => c.Text));

			model.LanguageList = languages.Select(l => new SelectListItem { Text = l.DisplayName, Value = l.TwoLetterCountryCode, Selected = l.TwoLetterCountryCode == Locale.EN }).OrderBy(l => l.Text).ToList();

			return View(model);
		}

		/// <summary>
		/// Creates a concept.
		/// </summary>
		/// <param name="model">The model containing the information to create a concept.</param>
		/// <returns>Returns the created concept.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateConceptModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var concept = this.ImsiClient.Create<Concept>(model.ToConcept());

					TempData["success"] = Locale.Concept + " " + Locale.Created + " " + Locale.Successfully;

					return RedirectToAction("ViewConcept", new { id = concept.Key });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToCreate + " " + Locale.Concept;

			var languages = LanguageUtil.GetLanguageList();

			model.LanguageList = languages.Select(l => new SelectListItem { Text = l.DisplayName, Value = l.TwoLetterCountryCode }).ToList();

			return View(model);
		}

		/// <summary>
		/// Deletes a concept.
		/// </summary>
		/// <param name="id">The id of the concept to delete.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var concept = this.ImsiClient.Get<Concept>(id, null) as Concept;

				if (concept == null)
				{
					TempData["error"] = Locale.Concept + " " + Locale.NotFound;
					return RedirectToAction("Index");
				}

				this.ImsiClient.Obsolete<Concept>(concept);

				TempData["success"] = Locale.Concept + " " + Locale.Deleted + " " + Locale.Successfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.Concept + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			var bundle = this.ImsiClient.Query<Concept>(c => c.Key == id && c.ObsoletionTime == null);

			bundle.Reconstitute();

			var concept = bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Key == id && c.ObsoletionTime == null);

			if (concept == null)
			{
				TempData["error"] = Locale.Concept + " " + Locale.NotFound;
				return RedirectToAction("Index");
			}

			var referenceTermQuery = new List<KeyValuePair<string, object>>();

			foreach (var conceptReferenceTerm in concept.ReferenceTerms)
			{
				referenceTermQuery.AddRange(QueryExpressionBuilder.BuildQuery<ReferenceTerm>(c => c.Key == conceptReferenceTerm.ReferenceTerm.Key));
			}

			var referenceTerms = this.ImsiClient.Query<ReferenceTerm>(QueryExpressionParser.BuildLinqExpression<ReferenceTerm>(new NameValueCollection(referenceTermQuery.ToArray()))).Item.OfType<ReferenceTerm>();

			var editConceptModel = new EditConceptModel(concept);

			editConceptModel.ReferenceTerms.AddRange(referenceTerms.Select(r => new ReferenceTermModel
			{
				Mnemonic = r.Mnemonic,
				Name = string.Join(" ", r.DisplayNames.Select(d => d.Name)),
				Id = r.Key.Value
			}));

			var conceptClasses = this.ImsiClient.Query<ConceptClass>(c => c.ObsoletionTime == null);

			for (var i = 0; i < conceptClasses.Count; i++)
			{
				if (conceptClasses.Item[i].Type == concept.Class.Type)
				{
					var selected = concept.Class.Key == (conceptClasses.Item[i] as ConceptClass).Key;

					editConceptModel.ConceptClassList.Add(new SelectListItem()
					{
						Text = (conceptClasses.Item[i] as ConceptClass)?.Mnemonic,
						Value = (conceptClasses.Item[i] as ConceptClass)?.Key.Value.ToString(),
						Selected = selected
					});
				}
			}

			var languages = LanguageUtil.GetLanguageList();

			editConceptModel.LanguageList = languages.Select(l => new SelectListItem { Text = l.DisplayName, Value = l.TwoLetterCountryCode }).ToList();

			return View(editConceptModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditConceptModel model)
		{
			if (ModelState.IsValid)
			{
				var bundle = this.ImsiClient.Query<Concept>(c => c.Key == model.Id && c.ObsoletionTime == null);

				bundle.Reconstitute();

				var concept = bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Key == model.Id && c.ObsoletionTime == null);

				if (concept == null)
				{
					TempData["error"] = Locale.Concept + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				for (var i = 0; i < model.Languages.Count; i++)
				{
					if (model.Name[i] != string.Empty)
					{
						if (concept.ConceptNames.Count > i)
						{
							if (concept.ConceptNames[i].Language == model.Languages[i])
							{
								concept.ConceptNames[i].Name = model.Name[i];
							}
						}
						else
						{
							concept.ConceptNames.Add(new ConceptName
							{
								Language = model.Languages[i],
								Name = model.Name[i]
							});
						}
					}
				}

				var conceptClassBundle = this.ImsiClient.Query<ConceptClass>(c => c.ObsoletionTime == null);

				conceptClassBundle.Reconstitute();

				var conceptClasses = conceptClassBundle.Item.OfType<ConceptClass>().Select(c => new ConceptClass { Mnemonic = c.Mnemonic, Name = c.Name, Key = c.Key }).ToList();

				var conceptClass = conceptClasses.FirstOrDefault(c => c.Name == model.ConceptClass);

				concept.Class = conceptClass;

				var result = this.ImsiClient.Update<Concept>(concept);

				TempData["success"] = Locale.Concept + " " + Locale.Updated + " " + Locale.Successfully;

				return RedirectToAction("Edit", new { id = result.Key });
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Concept;

			return View(model);
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		public ActionResult Index()
		{
			TempData["searchType"] = "Concept";
			return View();
		}

		/// <summary>
		/// Displays the search view.
		/// </summary>
		/// <returns>Returns the search view.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			var viewModels = new List<ConceptSearchResultViewModel>();

			if (CommonUtil.IsValidString(searchTerm))
			{
				var conceptBundle = this.ImsiClient.Query<Concept>(c => c.Mnemonic.Contains(searchTerm) && c.ObsoletionTime == null);

				viewModels.AddRange(conceptBundle.Item.OfType<Concept>().Select(c => new ConceptSearchResultViewModel(c)));

				var conceptSetBundle = this.ImsiClient.Query<ConceptSet>(c => c.Mnemonic.Contains(searchTerm) && c.ObsoletionTime == null);

				viewModels.AddRange(conceptSetBundle.Item.OfType<ConceptSet>().Select(c => new ConceptSearchResultViewModel(c)));

				TempData["searchTerm"] = searchTerm;

				return PartialView("_ConceptSearchResultsPartial", viewModels.OrderBy(c => c.Mnemonic));
			}
			else
			{
				TempData["error"] = Locale.InvalidSearch;
			}

			TempData["searchTerm"] = searchTerm;

			return PartialView("_ConceptSearchResultsPartial", viewModels);
		}

		[HttpGet]
		public ActionResult ViewConcept(Guid id)
		{
			try
			{
				var bundle = this.ImsiClient.Query<Concept>(c => c.Key == id && c.ObsoletionTime == null);

				bundle.Reconstitute();

				var concept = bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Key == id && c.ObsoletionTime == null);

				if (concept == null)
				{
					TempData["error"] = Locale.Concept + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var conceptViewModel = new ConceptViewModel(concept);

				conceptViewModel.ReferenceTerms.AddRange(concept.ReferenceTerms.Select(r => new ReferenceTermModel
				{
					Mnemonic = r.ReferenceTerm.Mnemonic,
					Name = string.Join(", ", r.ReferenceTerm.DisplayNames.Select(d => d.Name)),
					Id = r.Key.Value
				}));

				return View(conceptViewModel);
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