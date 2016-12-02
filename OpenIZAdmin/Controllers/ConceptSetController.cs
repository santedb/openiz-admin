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

using Microsoft.AspNet.Identity;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.ConceptModels.ViewModels;
using OpenIZAdmin.Models.ConceptSetModels;
using OpenIZAdmin.Models.ConceptSetModels.ViewModels;
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
	public class ConceptSetController : BaseController
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
			CreateConceptSetModel model = new CreateConceptSetModel();

			return View(model);
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
				try
				{
                    ConceptSet conceptSet = new ConceptSet();

					conceptSet.CreatedBy = new OpenIZ.Core.Model.Security.SecurityUser
					{
						Key = Guid.Parse(User.Identity.GetUserId())
					};
                    conceptSet.Mnemonic = model.Mnemonic;
					var result = this.ImsiClient.Create<ConceptSet>(conceptSet);

					TempData["success"] = Locale.ConceptSet + " " + Locale.CreatedSuccessfully;

                    return RedirectToAction("ViewConceptSet", new { key = result.Key});
                }
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to create concept set: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to create concept set: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.UnableToCreate + " " + Locale.ConceptSet;
            
			return View(model);
		}

		[HttpGet]
		public ActionResult Delete(Guid key, Guid versionKey)
		{
			var conceptSet = this.ImsiClient.Get<ConceptSet>(key, versionKey) as ConceptSet;

			if (conceptSet == null)
			{
				TempData["error"] = Locale.ConceptSet + " " + Locale.NotFound;
				return RedirectToAction("Index");
			}


			this.ImsiClient.Obsolete<ConceptSet>(conceptSet);

			TempData["success"] = Locale.ConceptSet + " " + Locale.DeletedSuccessfully;
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
            query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.ObsoletionTime==null));
            if (model.SearchType == 0)
            {


                var bundle = this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));

                viewModels.AddRange(bundle.Item.OfType<Concept>().Select(c => new ConceptSearchResultViewModel(c)));

                return PartialView("_ConceptSearchResultsPartial", viewModels.OrderBy(c => c.Mnemonic).ToList());
            }
            else
            {
                var bundle = this.ImsiClient.Query<ConceptSet>(QueryExpressionParser.BuildLinqExpression<ConceptSet>(new NameValueCollection(query.ToArray())));

                viewModels.AddRange(bundle.Item.OfType<ConceptSet>().Select(c => new ConceptSearchResultViewModel(c)));

                return PartialView("_ConceptSearchResultsPartial", viewModels.OrderBy(c => c.Mnemonic).ToList());
            }
		}

		[HttpGet]
		public ActionResult ViewConceptSet(Guid key)
		{
			var query = new List<KeyValuePair<string, object>>();

			query.AddRange(QueryExpressionBuilder.BuildQuery<ConceptSet>(c => c.Key == key));
			var bundle = this.ImsiClient.Query<ConceptSet>(QueryExpressionParser.BuildLinqExpression<ConceptSet>(new NameValueCollection(query.ToArray())));

			bundle.Reconstitute();

			var conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault();

			if (conceptSet == null)
			{
				TempData["error"] = Locale.Concept + " " + Locale.NotFound;
				return RedirectToAction("Index");
			}

            conceptSet.SetDelayLoad(true);

            ConceptSetViewModel model = ConceptSetUtil.ToConceptSetViewModel(conceptSet);


			return View(model);
		}

		[HttpGet]
		public ActionResult Edit(Guid key)
		{
            var query = new List<KeyValuePair<string, object>>();

            query.AddRange(QueryExpressionBuilder.BuildQuery<ConceptSet>(c => c.Key == key));
            var bundle = this.ImsiClient.Query<ConceptSet>(QueryExpressionParser.BuildLinqExpression<ConceptSet>(new NameValueCollection(query.ToArray())));

            bundle.Reconstitute();

            var conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault();

            if (conceptSet == null)
            {
                TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                return RedirectToAction("Index");
            }

            conceptSet.SetDelayLoad(true);

            EditConceptSetModel model = ConceptSetUtil.ToEditConceptSetModel(conceptSet);


            return View(model);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditConceptSetModel model)
		{
            if (ModelState.IsValid)
            {
                var query = new List<KeyValuePair<string, object>>();

                query.AddRange(QueryExpressionBuilder.BuildQuery<ConceptSet>(c => c.Key == model.Key));

                var bundle = this.ImsiClient.Query<ConceptSet>(QueryExpressionParser.BuildLinqExpression<ConceptSet>(new NameValueCollection(query.ToArray())));

                bundle.Reconstitute();

                var conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault();
                for(var i = 0; i<model.ConceptDeletion.Count(); i++)
                {
                    if(model.ConceptDeletion[i])
                        conceptSet.ConceptsXml.RemoveAt(i);

                }
                this.ImsiClient.Update<ConceptSet>(conceptSet);
                TempData["success"] = Locale.UpdatedSuccessfully + " " + Locale.ConceptSet;
                return RedirectToAction("ViewConceptSet",new { key = conceptSet.Key });
            }

                TempData["error"] = Locale.UnableToUpdate + " " + Locale.ConceptSet;
            
			return View(model);
		}
	}
}