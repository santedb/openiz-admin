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
 * User: Nityan
 * Date: 2016-7-23
 */

using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.ConceptModels.ViewModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
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
	public class ConceptController : Controller
	{
		/// <summary>
		/// The internal reference to the <see cref="AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient amiClient;

		/// <summary>
		/// The internal reference to the <see cref="ImsiServiceClient"/> instance.
		/// </summary>
		private ImsiServiceClient imsiClient;

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
					var result = this.imsiClient.Create(ConceptUtil.ToConcept(model));
                    TempData["success"] = Locale.ConceptCreatedSuccessfully;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to create concept: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to create concept: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.UnableToCreateConcept;

			var languages = LanguageUtil.GetLanguageList();

			model.LanguageList = languages.Select(l => new SelectListItem { Text = l.DisplayName, Value = l.TwoLetterCountryCode }).ToList();

			return View(model);
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		public ActionResult Index()
		{
			return View();
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var amiRestClient = new RestClientService(Constants.AMI);

			amiRestClient.Accept = "application/xml";
			amiRestClient.Credentials = new AmiCredentials(this.User, HttpContext.Request);

			this.amiClient = new AmiServiceClient(amiRestClient);

			var imsiRestClient = new RestClientService(Constants.IMSI);

			imsiRestClient.Accept = "application/xml";
			imsiRestClient.Credentials = new ImsCredentials(this.User, HttpContext.Request);

			this.imsiClient = new ImsiServiceClient(imsiRestClient);

			base.OnActionExecuting(filterContext);
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
			List<ConceptSearchResultViewModel> viewModels = new List<ConceptSearchResultViewModel>();

			AmiCollection<Concept> concepts = new AmiCollection<Concept>();
			AmiCollection<ConceptSet> conceptSets = new AmiCollection<ConceptSet>();

			try
			{
				if (model.SearchType == ConceptSearchType.Concept)
				{
					concepts = this.SearchConcepts(model);
					viewModels.AddRange(concepts.CollectionItem.Select(c => new ConceptSearchResultViewModel(c)));
				}
				else if (model.SearchType == ConceptSearchType.ConceptSet)
				{
					conceptSets = this.SearchConceptSets(model);
					viewModels.AddRange(conceptSets.CollectionItem.Select(c => new ConceptSearchResultViewModel(c)));
				}
				else
				{
					TempData["error"] = "Unable to retrieve concept sets";
					return RedirectToAction("Index");
				}

				return PartialView("_ConceptSearchResultsPartial", viewModels.OrderBy(c => c.Mnemonic).ToList());
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve concepts", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve concepts", e.Message);
			}

			TempData["error"] = "Unable to retrieve concepts";

			return PartialView("_ConceptSearchResultsPartial", viewModels.OrderBy(c => c.Mnemonic).ToList());
		}

		private AmiCollection<Concept> SearchConcepts(SearchConceptModel model)
		{
			List<KeyValuePair<string, object>> query = new List<KeyValuePair<string, object>>();

			if (!string.IsNullOrEmpty(model.Mnemonic) && !string.IsNullOrWhiteSpace(model.Mnemonic))
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Mnemonic.Contains(model.Mnemonic)));
			}

			if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrWhiteSpace(model.Name))
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.ConceptNames.Any(cn => cn.Name.Contains(model.Name))));
			}

			if (query.Count == 0)
			{
				throw new ArgumentException(string.Format("{0} must not be empty", nameof(query)));
			}

			return this.amiClient.GetConcepts(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));
		}

		private AmiCollection<ConceptSet> SearchConceptSets(SearchConceptModel model)
		{
			List<KeyValuePair<string, object>> query = new List<KeyValuePair<string, object>>();

			if (!string.IsNullOrEmpty(model.Mnemonic) && !string.IsNullOrWhiteSpace(model.Mnemonic))
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<ConceptSet>(c => c.Mnemonic.Contains(model.Mnemonic)));
			}

			if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrWhiteSpace(model.Name))
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<ConceptSet>(c => c.Name.Contains(model.Name)));
			}

			if (query.Count == 0)
			{
				throw new ArgumentException(string.Format("{0} must not be empty", nameof(query)));
			}

			return this.amiClient.GetConceptSets(QueryExpressionParser.BuildLinqExpression<ConceptSet>(new NameValueCollection(query.ToArray())));
		}

		[HttpGet]
		public ActionResult ViewConcept(string key, string versionKey)
		{
			if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
			{
				Guid conceptId = Guid.Empty;
				Guid conceptVersion = Guid.Empty;

				if (Guid.TryParse(key, out conceptId) && Guid.TryParse(versionKey, out conceptVersion))
				{
					List<KeyValuePair<string, object>> query = new List<KeyValuePair<string, object>>();

					if (conceptVersion != Guid.Empty)
					{
						query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == conceptId && c.VersionKey == conceptVersion));
					}
					else
					{
						query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == conceptId));
					}

					var concept = this.amiClient.GetConcepts(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray()))).CollectionItem.SingleOrDefault();

					if (concept == null)
					{
						TempData["error"] = Locale.ConceptNotFound;

						return RedirectToAction("Index");
					}

					return View(new ConceptViewModel(concept));
				}
			}

			TempData["error"] = Locale.ConceptNotFound;

			return RedirectToAction("Index");
		}

        [HttpGet]
        public ActionResult Edit(string key, string versionKey)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
            {
                Guid conceptId = Guid.Empty;
                Guid conceptVersion = Guid.Empty;

                if (Guid.TryParse(key, out conceptId) && Guid.TryParse(versionKey, out conceptVersion))
                {
                    List<KeyValuePair<string, object>> query = new List<KeyValuePair<string, object>>();

                    if (conceptVersion != Guid.Empty)
                    {
                        query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == conceptId && c.VersionKey == conceptVersion));
                    }
                    else
                    {
                        query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == conceptId));
                    }

                    var concept = this.amiClient.GetConcepts(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray()))).CollectionItem.SingleOrDefault();

                    if (concept == null)
                    {
                        TempData["error"] = Locale.ConceptNotFound;

                        return RedirectToAction("Index");
                    }
                    var editConceptModel = new EditConceptModel()
                    {
                        Mnemonic = concept.Mnemonic,
                        Name = concept.ConceptNames.Select(c => c.Name).Aggregate((a, b) => (a + ", " + b)),
                        Language = concept.ConceptNames.Select(c => c.Language).FirstOrDefault()
                };
                    return View(editConceptModel);
                }
            }

            TempData["error"] = Locale.ConceptNotFound;

            return RedirectToAction("Index");
        }

        [HttpGet]
		public ActionResult ViewConceptSet(string key)
		{
			if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
			{
				Guid conceptSetId = Guid.Empty;

				if (Guid.TryParse(key, out conceptSetId))
				{
					List<KeyValuePair<string, object>> query = new List<KeyValuePair<string, object>>();

					query.AddRange(QueryExpressionBuilder.BuildQuery<ConceptSet>(c => c.Key == conceptSetId));

					var conceptSet = this.amiClient.GetConceptSets(QueryExpressionParser.BuildLinqExpression<ConceptSet>(new NameValueCollection(query.ToArray()))).CollectionItem.SingleOrDefault();

					if (conceptSet == null)
					{
						TempData["error"] = Locale.ConceptNotFound;

						return RedirectToAction("Index");
					}

					ConceptViewModel viewModel = new ConceptViewModel(conceptSet);

					viewModel.Details.Add(new DetailedConceptViewModel
					{
						Oid = conceptSet.Oid,
						Concepts = conceptSet.Concepts.SelectMany(c => c.ConceptNames).Select(c => c.Name).OrderBy(c => c).ToList(),
						Url = conceptSet.Url,
					});

					return View("ViewConcept", viewModel);
				}
			}

			TempData["error"] = Locale.ConceptNotFound;

			return RedirectToAction("Index");
		}
	}
}