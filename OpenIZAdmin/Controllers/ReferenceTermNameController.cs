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
using System.Diagnostics;
using System.Linq;
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
		/// <param name="id">The reference term identifier.</param>
		/// <returns>Returns the create view.</returns>
		[HttpGet]
        public ActionResult Create(Guid id)
        {
            var referenceTerm = ImsiClient.Get<ReferenceTerm>(id, null) as ReferenceTerm;

            if (referenceTerm == null)
            {
                TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

                return RedirectToAction("Index", "ReferenceTerm");
            }

            var model = new CreateReferenceTermNameViewModel(referenceTerm)
            {
                LanguageList = LanguageUtil.GetLanguageList().ToSelectList("DisplayName", "TwoLetterCountryCode").ToList(),
                TwoLetterCountryCode = Locale.EN,
                ReferenceTermNameList = referenceTerm.DisplayNames.Select(n=> new ReferenceTermNameViewModel(n)).ToList()
            };

            return View(model);
        }

        /// <summary>
		/// Adds the new reference term.
		/// </summary>
		/// <param name="model">The <see cref="ReferenceTermViewModel"/> instance.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateReferenceTermNameViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var referenceTerm = ImsiClient.Get<ReferenceTerm>(model.ReferenceTermId.Value, null) as ReferenceTerm;

                    if (referenceTerm == null)
                    {
                        TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

                        return RedirectToAction("Index", "ReferenceTerm");
                    }

                    referenceTerm.DisplayNames.Add(model.ToReferenceTermName());

                    var result = this.ImsiClient.Update<ReferenceTerm>(referenceTerm);

                    TempData["success"] = Locale.ReferenceTermName + " " + Locale.Created + " " + Locale.Successfully;

                    return RedirectToAction("Edit", "ReferenceTerm", new { id = result.Key });
                }
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to retrieve entity: { e }");
            }

            TempData["error"] = Locale.UnableToCreate + " " + Locale.ReferenceTermName;            
            model.LanguageList = LanguageUtil.GetLanguageList().ToSelectList("DisplayName", "TwoLetterCountryCode").ToList();

            return View(model);
        }

        /// <summary>
		/// Deletes a reference term name from a reference term.
		/// </summary>
		/// <param name="id">The reference term name identifier</param>
		/// <param name="refTermId">The identifier of the reference term instance.</param>		
		/// <returns>Returns the index view.</returns>
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id, Guid refTermId)
        {
            try
            {
                var referenceTerm = ImsiClient.Get<ReferenceTerm>(refTermId, null) as ReferenceTerm;

                if (referenceTerm == null)
                {
                    TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

                    return RedirectToAction("Index", "ReferenceTerm");
                }

                var index = referenceTerm.DisplayNames.FindIndex(c => c.Key == id);

                if (index == -1)
                {
                    TempData["error"] = Locale.ReferenceTermName + " " + Locale.NotFound;

                    return RedirectToAction("Edit", "ReferenceTerm", new { referenceTerm.Key });
                }

                referenceTerm.DisplayNames.RemoveAt(index);

                var result = this.ImsiClient.Update<ReferenceTerm>(referenceTerm);

                TempData["success"] = Locale.ReferenceTermName + " " + Locale.Deleted + " " + Locale.Successfully;

                return RedirectToAction("Edit", "ReferenceTerm", new { id = result.Key });

            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to retrieve entity: { e }");
            }

            TempData["error"] = Locale.ReferenceTermName + " " + Locale.NotFound;

            return RedirectToAction("Index", "ReferenceTerm");
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

                    return RedirectToAction("Index", "ReferenceTerm");
                }
                                
                var index = referenceTerm.DisplayNames.FindIndex(c => c.Key == id);

                if (index == -1)
                {
                    TempData["error"] = Locale.ReferenceTermName + " " + Locale.NotFound;

                    return RedirectToAction("Edit", "ReferenceTerm", new { referenceTerm.Key });
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
		/// Updates the reference term name associated with the reference term.
		/// </summary>
		/// <param name="model">The <see cref="EditReferenceTermNameViewModel"/> instance.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditReferenceTermNameViewModel model)
        {
            try
            {
                var referenceTerm = ImsiClient.Get<ReferenceTerm>(model.ReferenceTermId.Value, null) as ReferenceTerm;

                if (referenceTerm == null)
                {
                    TempData["error"] = Locale.ReferenceTerm + " " + Locale.NotFound;

                    return RedirectToAction("Index", "ReferenceTerm");
                }

                var index = referenceTerm.DisplayNames.FindIndex(c => c.Key == model.Id);                

                if (index == -1)
                {
                    TempData["error"] = Locale.ReferenceTermName + " " + Locale.NotFound;

                    return RedirectToAction("Edit", "ReferenceTerm", new { referenceTerm.Key });
                }
                
                referenceTerm.DisplayNames[index].Language = model.TwoLetterCountryCode;
                referenceTerm.DisplayNames[index].Name = model.Name;

                var result = this.ImsiClient.Update<ReferenceTerm>(referenceTerm);

                TempData["success"] = Locale.ReferenceTermName + " " + Locale.Updated + " " + Locale.Successfully;

                return RedirectToAction("Edit", "ReferenceTerm", new { id = result.Key });
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
                Trace.TraceError($"Unable to retrieve entity: { e }");
            }

            TempData["error"] = Locale.UnableToUpdate + " " + Locale.ReferenceTerm;

            return RedirectToAction("ViewReferenceTerm", "ReferenceTerm", new { id = model.ReferenceTermId });
        } 
    }
}