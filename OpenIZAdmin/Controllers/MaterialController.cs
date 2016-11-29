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
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.ViewModels.MaterialModels;

namespace OpenIZAdmin.Controllers
{
    /// <summary>
    /// Provides operations for managing materials.
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
        /// Creates a material.
        /// </summary>
        /// <param name="model">The model containing the information to create a material.</param>
        /// <returns>Returns the created material.</returns>
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
                    var result = this.ImsiClient.Create<Material>(material);
                    TempData["success"] = Locale.Material + " " + Locale.CreatedSuccessfully;

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.TraceError("Unable to create material: {0}", e.StackTrace);
#endif
                    Trace.TraceError("Unable to create material: {0}", e.Message);
                }
            }

            TempData["error"] = Locale.UnableToCreate + " " + Locale.Material;

            return View(model);
        }


        /// <summary>
        /// Displays the create view.
        /// </summary>
        /// <returns>Returns the create view.</returns>
        [HttpGet]
        public ActionResult Edit(Guid key)
        {

            var query = new List<KeyValuePair<string, object>>();

            if (key != Guid.Empty)
            {
                query.AddRange(QueryExpressionBuilder.BuildQuery<Material>(c => c.Key == key));

                var bundle = this.ImsiClient.Query<Material>(QueryExpressionParser.BuildLinqExpression<Material>(new NameValueCollection(query.ToArray())));
                bundle.Reconstitute();
                EditMaterialModel model = new EditMaterialModel()
                {
                    Name = (bundle.Item.FirstOrDefault() as Material).Names[0].Component[0].Value
                };
                return View(model);
        }
            
            return View("Index");
        }

        /// <summary>
        /// Edits a material.
        /// </summary>
        /// <param name="model">The model containing the information to edit a material.</param>
        /// <returns>Returns the edited material.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditMaterialModel model)
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
                    var result = this.ImsiClient.Update<Material>(material);
                    TempData["success"] = Locale.Material + " " + Locale.CreatedSuccessfully;

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.TraceError("Unable to edit material: {0}", e.StackTrace);
#endif
                    Trace.TraceError("Unable to edit material: {0}", e.Message);
                }
            }

            TempData["error"] = Locale.UnableToCreate + " " + Locale.Material;

            return View(model);
        }

        public ActionResult Index()
        {
			TempData["searchType"] = "Material";
			return View();
        }


        /// <summary>
        /// Searches for a user.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <returns>Returns a list of materials which match the search term.</returns>
        [HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<MaterialSearchResultViewModel> users = new List<MaterialSearchResultViewModel>();

			if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
			{
				var bundle = this.ImsiClient.Query<Material>(m => m.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))));

				TempData["searchTerm"] = searchTerm;

				return PartialView("_MaterialSearchResultsPartial", bundle.Item.OfType<Material>().Select(MaterialUtil.ToMaterialSearchResultViewModel));
			}

			TempData["error"] = Locale.Material + " " + Locale.NotFound;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_MaterialSearchResultsPartial", users);
		}

        /// <summary>
        /// Displays the material view.
        /// </summary>
        /// <returns>Returns the material view.</returns>
        [HttpGet]
        public ActionResult ViewMaterial(Guid key)
        {

            var query = new List<KeyValuePair<string, object>>();

            if (key != Guid.Empty)
            {
                query.AddRange(QueryExpressionBuilder.BuildQuery<Material>(c => c.Key == key));

                var bundle = this.ImsiClient.Query<Material>(QueryExpressionParser.BuildLinqExpression<Material>(new NameValueCollection(query.ToArray())));
                bundle.Reconstitute();
                ViewMaterialModel model = new ViewMaterialModel()
                {
                    Name = (bundle.Item.FirstOrDefault() as Material).Names[0].Component[0].Value
                };
                return View(model);
            }

            return View("Index");
        }

    }
}