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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.ConceptSetModels;
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
			try
			{
				var exists = this.ImsiClient.Query<ConceptSet>(c => c.Oid == model.Oid).Item.OfType<ConceptSet>().Any();

                if (exists) ModelState.AddModelError("Oid", Locale.OidMustBeUnique);


                var duplicate = this.ImsiClient.Query<ConceptSet>(c => c.Mnemonic == model.Mnemonic).Item.OfType<ConceptSet>().Any();

                if (duplicate) ModelState.AddModelError("Mnemonic", Locale.MnemonicMustBeUnique);


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
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to create concept set: {e}");
			}

			TempData["error"] = Locale.UnableToCreateConceptSet;

			return View(model);
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
					TempData["error"] = Locale.ConceptSetNotFound;

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
				Trace.TraceError($"Unable to delete concept from concept set: {e}");
			}

			TempData["error"] = Locale.UnableToUpdateConceptSet;

			return RedirectToAction("Edit", new { id = setId });
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="id">The identifier of the ConceptSet</param>
		/// <returns>An <see cref="ActionResult"/> instance</returns>
		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			try
			{
				var conceptSet = this.GetConceptSet(id);

				if (conceptSet == null)
				{
					this.TempData["error"] = Locale.ConceptSetNotFound;
					return RedirectToAction("Index");
				}

				var model = new EditConceptSetModel(conceptSet);

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve concept set: {e}");
				this.TempData["error"] = Locale.ConceptSetNotFound;
			}

			return RedirectToAction("ViewConceptSet", new { id = id });
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
			try
			{
                var conceptSet = this.GetConceptSet(model.Id);

                if (conceptSet == null)
                {
                    TempData["error"] = Locale.ConceptSetNotFound;

                    return RedirectToAction("Index");
                }

                //check oid
                if (!conceptSet.Oid.Equals(model.Oid))
                {
                    var exists = this.ImsiClient.Query<ConceptSet>(c => c.Oid == model.Oid).Item.OfType<ConceptSet>().Any();

                    if (exists) ModelState.AddModelError("Oid", Locale.OidMustBeUnique);
                }

                //check mnemonic
                if (!string.Equals(conceptSet.Mnemonic, model.Mnemonic))
                {
                    var duplicate = DoesConceptSetExist(model.Mnemonic);
                    if(duplicate) ModelState.AddModelError("Mnemonic", Locale.MnemonicMustBeUnique);                    
                }

                if (ModelState.IsValid)
				{									                        				
                    conceptSet = model.ToConceptSet(conceptSet);

                    var result = this.ImsiClient.Update<ConceptSet>(conceptSet);

                    TempData["success"] = Locale.ConceptSetUpdatedSuccessfully;

                    return RedirectToAction("ViewConceptSet", new { id = result.Key });
                    					
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to update concept set: {e}");
				this.TempData["error"] = Locale.UnableToUpdateConceptSet;
			}

            TempData["error"] = Locale.UnableToUpdateConceptSet;
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
            TempData["searchTerm"] = "*";
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
			var results = new List<ConceptSetViewModel>();

			try
			{
				if (this.IsValidId(searchTerm))
				{					
					var bundle = searchTerm == "*" ? this.ImsiClient.Query<ConceptSet>(c => c.ObsoletionTime == null) : this.ImsiClient.Query<ConceptSet>(c => c.Mnemonic.Contains(searchTerm) && c.ObsoletionTime == null);

                    results = bundle.Item.OfType<ConceptSet>().Select(p => new ConceptSetViewModel(p)).ToList();

                    TempData["searchTerm"] = searchTerm;

					return PartialView("_ConceptSetSearchResultsPartial", results.OrderBy(c => c.Mnemonic));
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to search for concept sets: { e }");
			}

			this.TempData["error"] = Locale.InvalidSearch;
			this.TempData["searchTerm"] = searchTerm;

			return PartialView("_ConceptSetSearchResultsPartial", results);
		}

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
					TempData["error"] = Locale.ConceptSetNotFound;

					return RedirectToAction("Index");
				}

				var viewModel = new ConceptSetViewModel(conceptSet, true);

				return View(viewModel);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				this.TempData["error"] = Locale.ConceptSetNotFound;
			}

			return RedirectToAction("Index");
		}
	}
}