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
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.ConceptModels.ViewModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
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
		/// The internal reference to the <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient client;

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
			var restClient = new RestClientService("AMI");

			restClient.Accept = "application/xml";
			restClient.Credentials = new AmiCredentials(this.User, HttpContext.Request);

			this.client = new AmiServiceClient(restClient);

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

				return PartialView("_ConceptSearchResultsPartial", viewModels);
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve concepts", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve concepts", e.Message);
			}

			TempData["error"] = "Unable to retrieve concepts";
			return View(model);
		}

		private AmiCollection<Concept> SearchConcepts(SearchConceptModel model)
		{
			List<KeyValuePair<string, object>> query = new List<KeyValuePair<string, object>>();

			if (!string.IsNullOrEmpty(model.Mnemonic) && !string.IsNullOrWhiteSpace(model.Mnemonic))
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Mnemonic == model.Mnemonic));
			}

			if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrWhiteSpace(model.Name))
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.ConceptNames.Any(cn => cn.Name == model.Name)));
			}

			if (query.Count == 0)
			{
				throw new ArgumentException(string.Format("{0} must not be empty", nameof(query)));
			}

			return this.client.GetConcepts(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));
		}

		private AmiCollection<ConceptSet> SearchConceptSets(SearchConceptModel model)
		{
			List<KeyValuePair<string, object>> query = new List<KeyValuePair<string, object>>();

			if (!string.IsNullOrEmpty(model.Mnemonic) && !string.IsNullOrWhiteSpace(model.Mnemonic))
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<ConceptSet>(c => c.Mnemonic == model.Mnemonic));
			}

			if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrWhiteSpace(model.Name))
			{
				query.AddRange(QueryExpressionBuilder.BuildQuery<ConceptSet>(c => c.Name == model.Name));
			}

			if (query.Count == 0)
			{
				throw new ArgumentException(string.Format("{0} must not be empty", nameof(query)));
			}

			return this.client.GetConceptSets(QueryExpressionParser.BuildLinqExpression<ConceptSet>(new NameValueCollection(query.ToArray())));
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

					var concept = this.client.GetConcepts(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray()))).CollectionItem.SingleOrDefault();

					if (concept == null)
					{
						TempData["error"] = "Concept not found";
						return RedirectToAction("Index");
					}

					return View(new ConceptViewModel(concept));

				}
			}

			TempData["error"] = "Concept not found";
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

					var conceptSet = this.client.GetConceptSets(QueryExpressionParser.BuildLinqExpression<ConceptSet>(new NameValueCollection(query.ToArray()))).CollectionItem.SingleOrDefault();

					if (conceptSet == null)
					{
						TempData["error"] = "Concept Set not found";
						return RedirectToAction("Index");
					}

					return View("ViewConcept", new ConceptViewModel(conceptSet));

				}
			}

			TempData["error"] = "Concept not found";
			return RedirectToAction("Index");
		}
	}
}