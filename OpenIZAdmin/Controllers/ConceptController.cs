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

using OpenIZ.Core.Model.Collection;
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

            TempData["error"] = Locale.UnableToCreate + " " + Locale.Concept;

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

            Bundle concepts = new Bundle();
            //AmiCollection<ConceptSet> conceptSets = new AmiCollection<ConceptSet>();

            try
            {
                if (model.SearchType == ConceptSearchType.Concept)
                {
                    concepts = this.SearchConcepts(model);
                    viewModels.AddRange(concepts.Item.OfType<Concept>().Select(c => new ConceptSearchResultViewModel(c)));
                }
				else if (model.SearchType == ConceptSearchType.ConceptSet)
				{
					concepts = this.SearchConceptSets(model);
					viewModels.AddRange(concepts.Item.OfType<ConceptSet>().Select(c => new ConceptSearchResultViewModel(c)));
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

        private Bundle SearchConcepts(SearchConceptModel model)
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

            return this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));
        }

        private Bundle SearchConceptSets(SearchConceptModel model)
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

            return this.ImsiClient.Query<ConceptSet>(QueryExpressionParser.BuildLinqExpression<ConceptSet>(new NameValueCollection(query.ToArray())));
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
                    List<KeyValuePair<string, object>> referenceTermQuery = new List<KeyValuePair<string, object>>();

                    if (conceptVersion != Guid.Empty)
                    {
                        query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == conceptId && c.VersionKey == conceptVersion));
                    }
                    else
                    {
                        query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == conceptId));
                    }
                    var bundle = this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));

                    bundle.Reconstitute();

                    var concept = bundle.Item.OfType<Concept>().FirstOrDefault();

                    concept.SetDelayLoad(true);

                    for (var i = 0; i < concept.ReferenceTerms.Count; i++)
                    {
                        var referenceTermKey = concept.ReferenceTerms[0].ReferenceTerm.Key;
                        referenceTermQuery.AddRange(QueryExpressionBuilder.BuildQuery<ReferenceTerm>(c => c.Key == referenceTermKey));
                    }
                    var referenceTerms = this.ImsiClient.Query<ReferenceTerm>(QueryExpressionParser.BuildLinqExpression<ReferenceTerm>(new NameValueCollection(referenceTermQuery.ToArray()))).Item.OfType<ReferenceTerm>();
                    if (concept == null)
                    {
                        TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                        return RedirectToAction("Index");
                    }
                    var conceptViewModel = ConceptUtil.ToConceptViewModel(concept as Concept);
                    conceptViewModel.ReferenceTerms = new List<ReferenceTermModel>();
                    for (var i = 0; i < referenceTerms.Count(); i++)
                    {

                        conceptViewModel.ReferenceTerms.Add(new ReferenceTermModel()
                        {
                            Mnemonic = referenceTerms.ElementAt(i).Mnemonic,
                            Name = referenceTerms.ElementAt(i).DisplayNames[0].Name,
                            Key = referenceTerms.ElementAt(i).Key.Value

                        });

                    }
                    return View(conceptViewModel);
                }
            }

            TempData["error"] = Locale.Concept + " " + Locale.NotFound;

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
                    List<KeyValuePair<string, object>> referenceTermQuery = new List<KeyValuePair<string, object>>();

                    if (conceptVersion != Guid.Empty)
                    {
                        query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == conceptId && c.VersionKey == conceptVersion));
                    }
                    else
                    {
                        query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == conceptId));
                    }
                    var bundle = this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));

                    bundle.Reconstitute();

                    var concept = bundle.Item.OfType<Concept>().FirstOrDefault();

                    concept.SetDelayLoad(true);

                    for (var i = 0; i < concept.ReferenceTerms.Count; i++)
                    {
                        var referenceTermKey = concept.ReferenceTerms[0].ReferenceTerm.Key;
                        referenceTermQuery.AddRange(QueryExpressionBuilder.BuildQuery<ReferenceTerm>(c => c.Key == referenceTermKey));
                    }
                    var referenceTerms = this.ImsiClient.Query<ReferenceTerm>(QueryExpressionParser.BuildLinqExpression<ReferenceTerm>(new NameValueCollection(referenceTermQuery.ToArray()))).Item.OfType<ReferenceTerm>();
                    if (concept == null)
                    {
                        TempData["error"] = Locale.Concept + " " + Locale.NotFound;
                        return RedirectToAction("Index");
                    }
                    var editConceptModel = ConceptUtil.ToEditConceptModel(concept as Concept);
                    editConceptModel.ReferenceTerms = new List<ReferenceTermModel>();
                    for (var i = 0; i < referenceTerms.Count(); i++)
                    {

                        editConceptModel.ReferenceTerms.Add(new ReferenceTermModel()
                        {
                            Mnemonic = referenceTerms.ElementAt(i).Mnemonic,
                            Name = referenceTerms.ElementAt(i).DisplayNames[0].Name,
                            Key = referenceTerms.ElementAt(i).Key.Value

                        });

                    }


                    var languages = LanguageUtil.GetLanguageList();

                    editConceptModel.LanguageList = languages.Select(l => new SelectListItem { Text = l.DisplayName, Value = l.TwoLetterCountryCode }).ToList();
                    
                    return View(editConceptModel);
                }
            }

            TempData["error"] = Locale.Concept + " " + Locale.NotFound;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(EditConceptModel model)
        {
            if (ModelState.IsValid)
            {
                List<KeyValuePair<string, object>> query = new List<KeyValuePair<string, object>>();

                query.AddRange(QueryExpressionBuilder.BuildQuery<Concept>(c => c.Key == model.Key));

                var bundle = this.ImsiClient.Query<Concept>(QueryExpressionParser.BuildLinqExpression<Concept>(new NameValueCollection(query.ToArray())));

                bundle.Reconstitute();

                var concept = bundle.Item.OfType<Concept>().FirstOrDefault();

                concept.SetDelayLoad(true);

                for(var i = 0; i<model.Languages.Count(); i++)
                {
                    if (model.Name[i] != null)
                    {
                        if ((concept.ConceptNames.Count() - 1) < i)
                        {
                            ConceptName conceptName = new ConceptName
                            {
                                Name = model.Name[i],
                                Language = model.Languages[i],
                                SourceEntityKey = model.Key,
                                Key = Guid.NewGuid()
                            };
                            this.ImsiClient.Create<ConceptName>(conceptName);
                        }
                    }
                }
                

            }
            return RedirectToAction("Index");
        }
    }
}