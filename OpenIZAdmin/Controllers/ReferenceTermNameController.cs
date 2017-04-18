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
 * User: Andrew
 * Date: 2017-4-17
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ReferenceTermModels;
using OpenIZAdmin.Models.ReferenceTermNameModels;
using OpenIZAdmin.Util;

namespace OpenIZAdmin.Controllers
{
    /// <summary>
    /// Provides operations for managing reference term names.
    /// </summary>
    [TokenAuthorize]
    public class ReferenceTermNameController : BaseController
    {
        /// <summary>
		/// Displays the create view.
		/// </summary>
		/// <returns>Returns the create view.</returns>
		[HttpGet]
        public ActionResult Create()
        {
            var model = new CreateReferenceTermNameViewModel
            {
                LanguageList = LanguageUtil.GetLanguageList().ToSelectList("DisplayName", "TwoLetterCountryCode").ToList(),
                TwoLetterCountryCode = Locale.EN
            };

            return View(model);
        }

        /// <summary>
        /// Retrieves the names and metadata associated with the reference term to edit
        /// </summary>
        /// <param name="id">The reference term identifier</param>
        /// <param name="refTermId">The Reference Term identifier</param>
        /// <returns>An ActionResult instance</returns>
        [HttpGet]
        public ActionResult Edit(Guid id, Guid refTermId)
        {
            try
            {
                var referenceTerm = ImsiClient.Get<ReferenceTerm>(refTermId, null) as ReferenceTerm;                                

                if (referenceTerm == null)
                {
                    TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

                    return RedirectToAction("Index");
                }
                                
                var index = referenceTerm.DisplayNames.FindIndex(c => c.Key == id);

                if (index == -1)
                {
                    TempData["error"] = Locale.ReferenceTermName + " " + Locale.NotFound;

                    return RedirectToAction("Index");
                }

                var name = referenceTerm.DisplayNames[index];

                return View(new EditReferenceTermNameViewModel(referenceTerm, name)
                {
                    LanguageList = LanguageUtil.GetLanguageList().ToSelectList("DisplayName", "TwoLetterCountryCode").ToList(),
                    Name = name.Name,
                    TwoLetterCountryCode = name.Language
                });
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to retrieve entity: { e }");
            }

            TempData["error"] = Locale.UnableToUpdate + " " + Locale.ReferenceTerm;

            return RedirectToAction("ViewReferenceTerm", "ReferenceTerm", new { id });

        }

        /// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}