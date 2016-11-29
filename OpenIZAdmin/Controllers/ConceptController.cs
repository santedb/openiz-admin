/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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

using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.ConceptModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			CreateConceptModel model = new CreateConceptModel();

			var languages = LanguageUtil.GetLanguageList();

			model.LanguageList = languages.Select(l => new SelectListItem { Text = l.DisplayName, Value = l.TwoLetterCountryCode }).ToList();

			var conceptClasses = this.ImsiClient.Query<ConceptClass>(c => c.ObsoletionTime == null);

			for (var i = 0; i < conceptClasses.Count; i++)
			{
				model.ConceptClassList.Add(new SelectListItem()
				{
					Text = (conceptClasses.Item[i] as ConceptClass).Mnemonic,
					Value = (conceptClasses.Item[i] as ConceptClass).Key.Value.ToString(),
				});
			}

			model.LanguageList = languages.Select(l => new SelectListItem { Text = l.DisplayName, Value = l.TwoLetterCountryCode }).ToList();

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
			if (ModelState.IsValid)
			{
				try
				{
					var result = this.ImsiClient.Create(ConceptUtil.ToConcept(model));
					TempData["success"] = Locale.Concept + " " + Locale.CreatedSuccessfully;

                    return RedirectToAction("ViewConcept", new { key = result.Key, versionKey = result.VersionKey });
                }
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to create concept: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to create concept: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.UnableToCreate + " " + Locale.Concept;

			var languages = LanguageUtil.GetLanguageList();

			model.LanguageList = languages.Select(l => new SelectListItem { Text = l.DisplayName, Value = l.TwoLetterCountryCode }).ToList();
            
			return View(model);
		}

		[HttpGet]
		public ActionResult Delete(Guid key, Guid versionKey)
		{
			var concept = this.ImsiClient.Get<Concept>(key, versionKey) as Concept;

			if (concept == null)
			{
				TempData["error"] = Locale.Concept + " " + Locale.NotFound;
				return RedirectToAction("Index");
			}

			concept.ObsoletionTime = new DateTimeOffset(DateTime.Now);

			this.ImsiClient.Obsolete<Concept>(concept);

			TempData["success"] = Locale.Concept + " " + Locale.DeletedSuccessfully;
			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		public ActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// Displays the search view.
		/// </summary>
		/// <returns>Returns the search view.</returns>
		[HttpGet]
		public ActionResult Search()
		{
			return View();
		}

		/// <summary>
		/// Searches the IMS for a concept.
		/// </summary>
		/// <param name="model">The search model containing the search parameters.</param>
		/// <returns>Returns a list of concepts matching the specified query.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Search(SearchConceptModel model)
		{
			var viewModels = new List<ConceptSearchResultViewModel>();

			var query = new List<KeyValuePair<string, object>>();

			if (!string.IsNullOrEmpty(model.Mnemonic) && !string.IsNullOrWhiteSpace(model.Mnemonic))
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Mnemonic.Contains(model.Mnemonic)));
			}

			if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrWhiteSpace(model.Name))
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.ConceptNames.Any(cn => cn.Name.Contains(model.Name))));
			}

			var bundle = this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));

			viewModels.AddRange(bundle.Item.OfType<Concept>().Select(c => new ConceptSearchResultViewModel(c)));

			return PartialView("_ConceptSearchResultsPartial", viewModels.OrderBy(c => c.Mnemonic).ToList());
		}

		[HttpGet]
		public ActionResult ViewConcept(Guid key, Guid versionKey)
		{
			var query = new List<KeyValuePair<string, object>>();
			var referenceTermQuery = new List<KeyValuePair<string, object>>();

			if (versionKey != Guid.Empty)
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == key && c.VersionKey == versionKey));
			}
			else
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == key));
			}

			var bundle = this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));

			bundle.Reconstitute();

			var concept = bundle.Item.OfType<Concept>().FirstOrDefault();

			if (concept == null)
			{
				TempData["error"] = Locale.Concept + " " + Locale.NotFound;
				return RedirectToAction("Index");
			}

			concept.SetDelayLoad(true);

			foreach (var conceptReferenceTerm in concept.ReferenceTerms)
			{
				referenceTermQuery.AddRange(QueryExpressionBuilder.BuildQuery<ReferenceTerm>(c => c.Key == conceptReferenceTerm.ReferenceTerm.Key));
			}

			var referenceTerms = this.ImsiClient.Query<ReferenceTerm>(QueryExpressionParser.BuildLinqExpression<ReferenceTerm>(new NameValueCollection(referenceTermQuery.ToArray()))).Item.OfType<ReferenceTerm>();

			var conceptViewModel = ConceptUtil.ToConceptViewModel(concept);

			conceptViewModel.ReferenceTerms = new List<ReferenceTermModel>();

			conceptViewModel.ReferenceTerms.AddRange(referenceTerms.Select(r => new ReferenceTermModel
			{
				Mnemonic = r.Mnemonic,
				Name = string.Join(" ", r.DisplayNames.Select(d => d.Name)),
				Key = r.Key.Value
			}));

			return View(conceptViewModel);
		}

		[HttpGet]
		public ActionResult Edit(Guid key, Guid versionKey)
		{
			var query = new List<KeyValuePair<string, object>>();
			var referenceTermQuery = new List<KeyValuePair<string, object>>();

			if (versionKey != Guid.Empty)
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == key && c.VersionKey == versionKey));
			}
			else
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == key));
			}
			var bundle = this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));

			bundle.Reconstitute();

			var concept = bundle.Item.OfType<Concept>().FirstOrDefault();

			if (concept == null)
			{
				TempData["error"] = Locale.Concept + " " + Locale.NotFound;
				return RedirectToAction("Index");
			}

			concept.SetDelayLoad(true);

			foreach (var conceptReferenceTerm in concept.ReferenceTerms)
			{
				referenceTermQuery.AddRange(QueryExpressionBuilder.BuildQuery<ReferenceTerm>(c => c.Key == conceptReferenceTerm.ReferenceTerm.Key));
			}

			var referenceTerms = this.ImsiClient.Query<ReferenceTerm>(QueryExpressionParser.BuildLinqExpression<ReferenceTerm>(new NameValueCollection(referenceTermQuery.ToArray()))).Item.OfType<ReferenceTerm>();

			var editConceptModel = ConceptUtil.ToEditConceptModel(concept);

			editConceptModel.ReferenceTerms = new List<ReferenceTermModel>();

			editConceptModel.ReferenceTerms.AddRange(referenceTerms.Select(r => new ReferenceTermModel
			{
				Mnemonic = r.Mnemonic,
				Name = string.Join(" ", r.DisplayNames.Select(d => d.Name)),
				Key = r.Key.Value
			}));

			var conceptClasses = this.ImsiClient.Query<ConceptClass>(c => c.ObsoletionTime == null);

			for (var i = 0; i < conceptClasses.Count; i++)
			{
				var selected = concept.Class.Key == (conceptClasses.Item[i] as ConceptClass).Key;

				editConceptModel.ConceptClassList.Add(new SelectListItem()
				{
					Text = (conceptClasses.Item[i] as ConceptClass)?.Mnemonic,
					Value = (conceptClasses.Item[i] as ConceptClass)?.Key.Value.ToString(),
					Selected = selected
				});
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
				var query = new List<KeyValuePair<string, object>>();

				query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == model.Key));

				var bundle = this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));

				bundle.Reconstitute();

				var concept = bundle.Item.OfType<Concept>().FirstOrDefault();

				if (concept == null)
				{
					TempData["error"] = Locale.Concept + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				concept.SetDelayLoad(true);

				for (var i = 0; i < model.Languages.Count(); i++)
				{
					if (model.Name[i] != "")
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
							concept.ConceptNames.Add(new ConceptName()
							{
								Language = model.Languages[i],
								Name = model.Name[i]
							});
						}
					}
				}

				var conceptClasses = this.ImsiClient.Query<ConceptClass>(c => c.ObsoletionTime == null);
				var conceptClassList = new List<SelectListItem>();

				for (var i = 0; i < conceptClasses.Count; i++)
				{
					conceptClassList.Add(new SelectListItem()
					{
						Text = (conceptClasses.Item[i] as ConceptClass).Mnemonic,
						Value = (conceptClasses.Item[i] as ConceptClass).Key.Value.ToString(),
					});
				}

				var conceptClass = conceptClassList.Find(c => c.Value == model.ConceptClass);

				concept.Class = new ConceptClass
				{
					Mnemonic = conceptClass.Text,
					Key = Guid.Parse(conceptClass.Value)
				};

				this.ImsiClient.Update<Concept>(concept);

				TempData["success"] = Locale.Concept + " " + Locale.UpdatedSuccessfully;
				return RedirectToAction("Index");
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Concept;

			return View(model);
		}
	}
}