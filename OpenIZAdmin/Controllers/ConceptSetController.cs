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
 * User: khannan
 * Date: 2016-7-23
 */

using Elmah;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.ConceptSetModels;
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
	public class ConceptSetController : MetadataController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptSetController"/> class.
		/// </summary>
		public ConceptSetController()
		{
		}

		///// <summary>
		///// Adds the specified model.
		///// </summary>
		///// <param name="model">The model.</param>
		///// <returns>ActionResult.</returns>
		//[HttpPost]
		//public ActionResult Add(EditConceptSetModel model)
		//{
		//	var query = new List<KeyValuePair<string, object>>();

		//	query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == model.ConceptToAdd));

		//	var bundle = this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));

		//	bundle.Reconstitute();

		//	var concept = bundle.Item.OfType<Concept>().FirstOrDefault();

		//	//model.Concepts.Add(concept);

		//	//if (model.ConceptDeletion == null)
		//	//{
		//	//	model.ConceptDeletion = new List<bool>();
		//	//}

		//	//model.ConceptDeletion.Add(false);

		//	return PartialView("_ConceptList", model);
		//}

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

		/// <summary>
		/// Removes the selected Concept from the Concept Set
		/// </summary>
		/// <param name="setId"> The Guid of the Concept</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid setId)
		{
			var conceptSet = this.ImsiClient.Get<ConceptSet>(setId, null) as ConceptSet;

			if (conceptSet == null)
			{
				TempData["error"] = Locale.ConceptSet + " " + Locale.NotFound;
				return RedirectToAction("Index");
			}

			if (conceptSet.Concepts.Any())
			{
				TempData["error"] = Locale.UnableTo + " " + Locale.Delete + ". " + Locale.ConceptSet + " " + Locale.ContainsConcepts;
				return RedirectToAction("ViewConceptSet", "ConceptSet", new { id = setId });
			}

			this.ImsiClient.Obsolete<ConceptSet>(conceptSet);

			TempData["success"] = Locale.ConceptSet + " " + Locale.Deleted + " " + Locale.Successfully;
			return RedirectToAction("Index");
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="setId"></param>
		/// <param name="conceptId"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConceptFromSet(Guid setId, Guid conceptId)
		{
			try
			{
				var conceptSet = this.GetConceptSet(setId);

				if (conceptSet == null)
				{
					TempData["error"] = Locale.ConceptSet + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var index = conceptSet.ConceptsXml.FindIndex(a => a.Equals(conceptId));
				if (index != -1) conceptSet.ConceptsXml.RemoveAt(index);

				var result = this.ImsiClient.Update<ConceptSet>(conceptSet);

				TempData["success"] = Locale.Concept + " " + Locale.Deleted + " " + Locale.Successfully;

				return RedirectToAction("ViewConceptSet", new { id = result.Key });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.ConceptSet;
			return View();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="id">The identifier of the ConceptSet</param>
		/// <returns>An <see cref="ActionResult"/> instance</returns>
		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			var conceptSet = this.GetConceptSet(id);

			if (conceptSet == null)
			{
				TempData["error"] = Locale.Concept + " " + Locale.NotFound;
				return RedirectToAction("Index");
			}

			var model = new EditConceptSetModel(conceptSet);

			return View(model);
		}

		/// <summary>
		/// Updates the ConceptSet
		/// </summary>
		/// <param name="model">The <see cref="EditConceptSetModel"/> instance</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditConceptSetModel model)
		{
			if (ModelState.IsValid)
			{
				var conceptSet = this.GetConceptSet(model.Id);

				if (conceptSet == null)
				{
					TempData["error"] = Locale.ConceptSet + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				if (!string.Equals(conceptSet.Mnemonic, model.Mnemonic) && !DoesConceptSetExist(model.Mnemonic))
				{
					TempData["error"] = Locale.Mnemonic + " " + Locale.MustBeUnique;
					return View(model);
				}

				conceptSet = model.ToConceptSet(conceptSet);

				var result = this.ImsiClient.Update<ConceptSet>(conceptSet);

				TempData["success"] = Locale.ConceptSet + " " + Locale.Updated + " " + Locale.Successfully;

				return RedirectToAction("ViewConceptSet", new { id = result.Key });
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.ConceptSet;

			return View(model);
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "ConceptSet";
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
			var results = new List<ConceptSetSearchResultViewModel>();

			try
			{
				if (this.IsValidId(searchTerm))
				{
					Bundle bundle;

					if (searchTerm == "*")
					{
						bundle = this.ImsiClient.Query<ConceptSet>(c => c.ObsoletionTime == null);
						results = bundle.Item.OfType<ConceptSet>().Select(p => new ConceptSetSearchResultViewModel(p)).ToList();
					}
					else
					{
						bundle = this.ImsiClient.Query<ConceptSet>(c => c.Mnemonic.Contains(searchTerm) && c.ObsoletionTime == null);
						results = bundle.Item.OfType<ConceptSet>().Where(c => c.Mnemonic.Contains(searchTerm) && c.ObsoletionTime == null).Select(p => new ConceptSetSearchResultViewModel(p)).ToList();
					}

					TempData["searchTerm"] = searchTerm;

					return PartialView("_ConceptSetSearchResultsPartial", results.OrderBy(c => c.Mnemonic));
				}

				TempData["error"] = Locale.InvalidSearch;
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to search for concept sets: { e }");
			}

			this.TempData["searchTerm"] = searchTerm;

			return PartialView("_ConceptSetSearchResultsPartial", results);
		}

		//      /// <summary>
		//      /// Displays the search view.
		//      /// </summary>
		//      /// <returns>Returns the search view.</returns>
		//      [HttpPost]
		//public ActionResult Search(EditConceptSetModel model)
		//{
		//	var viewModels = new List<ConceptSearchResultViewModel>();

		//	var query = new List<KeyValuePair<string, object>>();

		//	if (!string.IsNullOrEmpty(model.ConceptMnemonic) && !string.IsNullOrWhiteSpace(model.ConceptMnemonic))
		//	{
		//		query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Mnemonic.Contains(model.ConceptMnemonic)));
		//	};

		//	if (!string.IsNullOrEmpty(model.ConceptName) && !string.IsNullOrWhiteSpace(model.ConceptName))
		//	{
		//		query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Mnemonic.Contains(model.ConceptName)));
		//	};

		//	query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.ObsoletionTime == null));

		//	var bundle = this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));

		//	viewModels.AddRange(bundle.Item.OfType<Concept>().Select(c => new ConceptSearchResultViewModel(c)));

		//	//var keys = model.Concepts.Select(m => m.Key).Distinct();

		//	//viewModels = viewModels.Where(m => !keys.Any(n => n.Value == m.Id)).ToList();

		//	return PartialView("_ConceptSetConceptSearchResultsPartial", viewModels.OrderBy(c => c.Mnemonic).ToList());
		//}

		/// <summary>
		/// Searches for a user.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of users which match the search term.</returns>
		[HttpGet]
		public ActionResult SearchAjax(string searchTerm)
		{
			var viewModels = new List<ConceptViewModel>();

			var query = new List<KeyValuePair<string, object>>();

			query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Mnemonic.Contains(searchTerm)));
			query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.ObsoletionTime == null));

			var bundle = this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));

			viewModels.AddRange(bundle.Item.OfType<Concept>().Select(c => new ConceptViewModel(c)));

			return Json(viewModels.OrderBy(c => c.Mnemonic).ToList(), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Views the concept set.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>ActionResult.</returns>
		[HttpGet]
		public ActionResult ViewConceptSet(Guid id)
		{
			try
			{
				var conceptSet = this.GetConceptSet(id);

				if (conceptSet == null)
				{
					TempData["error"] = Locale.ConceptSet + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var viewModel = new ConceptSetViewModel(conceptSet);

				return View(viewModel);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.ConceptSet + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}
	}
}