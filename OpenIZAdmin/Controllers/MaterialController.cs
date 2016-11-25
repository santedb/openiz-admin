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
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Query;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.MaterialModels;
using OpenIZAdmin.Models.MaterialModels.ViewModels;
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
    public class MaterialController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialController"/> class.
        /// </summary>
        public MaterialController()
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
        public ActionResult Create(CreateMaterialModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Material material = new Material()
                    {
                        Names = new List<EntityName>()
                        {
                            new EntityName()
                            {
                                Component = new List<EntityNameComponent>()
                                {
                                    new EntityNameComponent()
                                    {
                                        Value = model.Name
                                    }
                                }
                            }
                        }
                    };
                    var result = this.ImsiClient.Create(material);
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

            return View(model);
        }
        
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
        public ActionResult Search(SearchMaterialModel model)
        {
            List<MaterialSearchResultViewModel> viewModels = new List<MaterialSearchResultViewModel>();

            Bundle materials = new Bundle();

            try
            {
                    materials = this.ImsiClient.Query<Material>(m => m.ObsoletionTime != null);
                    viewModels.AddRange(MaterialUtil.ToMaterialList(materials));


                return PartialView("_ConceptSearchResultsPartial", viewModels.OrderBy(c => c.Name).ToList());
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.TraceError("Unable to retrieve concepts", e.StackTrace);
#endif
                Trace.TraceError("Unable to retrieve concepts", e.Message);
            }

            TempData["error"] = "Unable to retrieve concepts";

            return PartialView("_ConceptSearchResultsPartial", viewModels.OrderBy(c => c.Name).ToList());
        }

    }
}