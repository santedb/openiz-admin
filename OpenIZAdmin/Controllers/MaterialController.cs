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
			TempData["searchType"] = "Material";
			return View();
        }


		/// <summary>
		/// Searches for a user.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of users which match the search term.</returns>
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

	}
}