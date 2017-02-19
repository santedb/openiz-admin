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

using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptModels.ViewModels;
using OpenIZAdmin.Models.ConceptSetModels;
using OpenIZAdmin.Models.ConceptSetModels.ViewModels;
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
	public class ConceptSetController : BaseController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptSetController"/> class.
		/// </summary>
		public ConceptSetController()
		{
		}

		[HttpPost]
		public ActionResult Add(EditConceptSetModel model)
		{
			var query = new List<KeyValuePair<string, object>>();

			query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == model.ConceptToAdd));

			var bundle = this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));

			bundle.Reconstitute();

			var concept = bundle.Item.OfType<Concept>().FirstOrDefault();

			model.Concepts.Add(concept);

			if (model.ConceptDeletion == null)
			{
				model.ConceptDeletion = new List<bool>();
			}

			model.ConceptDeletion.Add(false);

			return PartialView("_ConceptList", model);
		}

		/// <summary>
		/// Displays the create view.
		/// </summary>
		/// <returns>Returns the create view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// Creates a concept.
		/// </summary>
		/// <param name="model">The model containing the information to create a concept.</param>
		/// <returns>Returns the created concept.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateConceptSetModel model)
		{
			if (ModelState.IsValid)
			{
				var conceptSet = new ConceptSet
				{
					Mnemonic = model.Mnemonic,
					Name = model.Name,
					Url = model.Url,
					Oid = model.Oid,
					Key = Guid.NewGuid()
				};

				var result = this.ImsiClient.Create<ConceptSet>(conceptSet);

				TempData["success"] = Locale.ConceptSet + " " + Locale.Created + " " + Locale.Successfully;

				return RedirectToAction("ViewConceptSet", new { id = result.Key });
			}

			TempData["error"] = Locale.UnableToCreate + " " + Locale.ConceptSet;

			return View(model);
		}

		[HttpGet]
		public ActionResult Delete(Guid id)
		{
			var conceptSet = this.ImsiClient.Get<ConceptSet>(id, null) as ConceptSet;

			if (conceptSet == null)
			{
				TempData["error"] = Locale.ConceptSet + " " + Locale.NotFound;
				return RedirectToAction("Index", "Concept");
			}

			this.ImsiClient.Obsolete<ConceptSet>(conceptSet);

			TempData["success"] = Locale.ConceptSet + " " + Locale.Deleted + " " + Locale.Successfully;
			return RedirectToAction("Index", "Concept");
		}

		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			var bundle = this.ImsiClient.Query<ConceptSet>(c => c.Key == id && c.ObsoletionTime == null);

			bundle.Reconstitute();

			var conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Key == id && c.ObsoletionTime == null);

			if (conceptSet == null)
			{
				TempData["error"] = Locale.Concept + " " + Locale.NotFound;
				return RedirectToAction("Index", "Concept");
			}

			var model = new EditConceptSetModel(conceptSet);

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditConceptSetModel model)
		{
			if (ModelState.IsValid)
			{
				var bundle = this.ImsiClient.Query<ConceptSet>(c => c.Key == model.Id && c.ObsoletionTime == null);

				bundle.Reconstitute();

				var conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault();

				if (conceptSet == null)
				{
					TempData["error"] = Locale.ConceptSet + " " + Locale.NotFound;

					return RedirectToAction("Index", "Concept");
				}

				conceptSet = model.ToConceptSet();

				for (var i = 0; i < model.ConceptDeletion.Count; i++)
				{
					if (conceptSet.ConceptsXml.Contains(model.Concepts[i].Key.Value) && model.ConceptDeletion[i])
					{
						conceptSet.ConceptsXml.RemoveAt(i);
					}
					else if (!conceptSet.ConceptsXml.Contains(model.Concepts[i].Key.Value) && !model.ConceptDeletion[i])
					{
						conceptSet.ConceptsXml.Add(model.Concepts[i].Key.Value);
					}
				}

				var result = this.ImsiClient.Update<ConceptSet>(conceptSet);

				TempData["success"] = Locale.ConceptSet + " " + Locale.Updated + " " + Locale.Successfully;

				return RedirectToAction("ViewConceptSet", new { id = result.Key });
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.ConceptSet;

			return View(model);
		}

		/// <summary>
		/// Displays the search view.
		/// </summary>
		/// <returns>Returns the search view.</returns>
		[HttpPost]
		public ActionResult Search(EditConceptSetModel model)
		{
			var viewModels = new List<ConceptSearchResultViewModel>();

			var query = new List<KeyValuePair<string, object>>();

			if (!string.IsNullOrEmpty(model.ConceptMnemonic) && !string.IsNullOrWhiteSpace(model.ConceptMnemonic))
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Mnemonic.Contains(model.ConceptMnemonic)));
			};

			if (!string.IsNullOrEmpty(model.ConceptName) && !string.IsNullOrWhiteSpace(model.ConceptName))
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Mnemonic.Contains(model.ConceptName)));
			};

			query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.ObsoletionTime == null));

			var bundle = this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));

			viewModels.AddRange(bundle.Item.OfType<Concept>().Select(c => new ConceptSearchResultViewModel(c)));

			var keys = model.Concepts.Select(m => m.Key).Distinct();

			viewModels = viewModels.Where(m => !keys.Any(n => n.Value == m.Id)).ToList();

			return PartialView("_ConceptSetConceptSearchResultsPartial", viewModels.OrderBy(c => c.Mnemonic).ToList());
		}

		[HttpGet]
		public ActionResult ViewConceptSet(Guid id)
		{
			var bundle = this.ImsiClient.Query<ConceptSet>(c => c.Key == id && c.ObsoletionTime == null);

			bundle.Reconstitute();

			var conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Key == id && c.ObsoletionTime == null);

			if (conceptSet == null)
			{
				TempData["error"] = Locale.ConceptSet + " " + Locale.NotFound;

				return RedirectToAction("Index");
			}

			var viewModel = new ConceptSetViewModel(conceptSet);

			return View(viewModel);
		}
	}
}