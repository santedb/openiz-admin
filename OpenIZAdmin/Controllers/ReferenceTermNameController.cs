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

using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ReferenceTermModels;
using OpenIZAdmin.Models.ReferenceTermNameModels;
using OpenIZAdmin.Util;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing reference term names.
	/// </summary>
	[TokenAuthorize(Constants.AdministerConceptDictionary)]
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
			try
			{
				var bundle = this.ImsiClient.Query<ReferenceTerm>(r => r.Key == id && r.ObsoletionTime == null, 0, null, true);

				bundle.Reconstitute();

				var referenceTerm = bundle.Item.OfType<ReferenceTerm>().FirstOrDefault(r => r.Key == id && r.ObsoletionTime == null);

				if (referenceTerm == null)
				{
					TempData["error"] = Locale.ReferenceTermNotFound;

					return RedirectToAction("Index", "ReferenceTerm");
				}

				var model = new CreateReferenceTermNameModel(referenceTerm)
				{
					LanguageList = LanguageUtil.GetLanguageList().ToSelectList("DisplayName", "TwoLetterCountryCode", null, true).ToList(),
					TwoLetterCountryCode = Locale.EN,
					ReferenceTermNameList = referenceTerm.DisplayNames.Select(n => new ReferenceTermNameViewModel(n)).ToList()
				};

				return View(model);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to load reference term name create page: {e}");
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

			return RedirectToAction("Edit", "ReferenceTerm", new { id });
		}

		/// <summary>
		/// Adds the new reference term.
		/// </summary>
		/// <param name="model">The <see cref="ReferenceTermViewModel"/> instance.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateReferenceTermNameModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var bundle = this.ImsiClient.Query<ReferenceTerm>(r => r.Key == model.ReferenceTermId && r.ObsoletionTime == null, 0, null, true);

					bundle.Reconstitute();

					var referenceTerm = bundle.Item.OfType<ReferenceTerm>().FirstOrDefault(r => r.Key == model.ReferenceTermId && r.ObsoletionTime == null);

					if (referenceTerm == null)
					{
						TempData["error"] = Locale.ReferenceTermNotFound;

						return RedirectToAction("Index", "ReferenceTerm");
					}

					referenceTerm.DisplayNames.Add(model.ToReferenceTermName());

					var result = this.ImsiClient.Update<ReferenceTerm>(referenceTerm);

					TempData["success"] = Locale.ReferenceTermNameCreatedSuccessfully;

					return RedirectToAction("Edit", "ReferenceTerm", new { id = result.Key });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve entity: { e }");
			}

			TempData["error"] = Locale.UnableToCreateReferenceTermName;
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
					TempData["error"] = Locale.ReferenceTermNotFound;

					return RedirectToAction("Index", "ReferenceTerm");
				}

				var index = referenceTerm.DisplayNames.FindIndex(c => c.Key == id);

				if (index == -1)
				{
					TempData["error"] = Locale.ReferenceTermNameNotFound;

					return RedirectToAction("Edit", "ReferenceTerm", new { referenceTerm.Key });
				}

				referenceTerm.DisplayNames.RemoveAt(index);

				var result = this.ImsiClient.Update<ReferenceTerm>(referenceTerm);

				TempData["success"] = Locale.ReferenceTermNameDeletedSuccessfully;

				return RedirectToAction("Edit", "ReferenceTerm", new { id = result.Key });
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve reference term: { e }");
			}

			TempData["error"] = Locale.ReferenceTermNameNotFound;

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
				var bundle = this.ImsiClient.Query<ReferenceTerm>(r => r.Key == refTermId && r.ObsoletionTime == null, 0, null, true);

				bundle.Reconstitute();

				var referenceTerm = bundle.Item.OfType<ReferenceTerm>().FirstOrDefault(r => r.Key == refTermId && r.ObsoletionTime == null);

				if (referenceTerm == null)
				{
					TempData["error"] = Locale.ReferenceTermNotFound;

					return RedirectToAction("Index", "ReferenceTerm");
				}

				var name = referenceTerm.DisplayNames.FirstOrDefault(r => r.Key == id);

				if (name == null)
				{
					TempData["error"] = Locale.ReferenceTermNameNotFound;

					return RedirectToAction("Edit", "ReferenceTerm", new { referenceTerm.Key });
				}

				var model = new EditReferenceTermNameModel(referenceTerm, name)
				{
					LanguageList = LanguageUtil.GetLanguageList().ToSelectList("DisplayName", "TwoLetterCountryCode", r => r.TwoLetterCountryCode == name.Language, true).ToList(),
					Name = name.Name,
					TwoLetterCountryCode = name.Language
				};

				return View(model);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve reference term: { e }");
			}

			this.TempData["error"] = Locale.UnableToRetrieveReferenceTerm;

			return RedirectToAction("ViewReferenceTerm", "ReferenceTerm", new { id });
		}

		/// <summary>
		/// Updates the reference term name associated with the reference term.
		/// </summary>
		/// <param name="model">The <see cref="EditReferenceTermNameModel"/> instance.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditReferenceTermNameModel model)
		{
			try
			{
				var referenceTerm = ImsiClient.Get<ReferenceTerm>(model.ReferenceTermId.Value, null) as ReferenceTerm;

				if (referenceTerm == null)
				{
					TempData["error"] = Locale.ReferenceTermNotFound;

					return RedirectToAction("Index", "ReferenceTerm");
				}

				var index = referenceTerm.DisplayNames.FindIndex(c => c.Key == model.Id);

				if (index == -1)
				{
					TempData["error"] = Locale.ReferenceTermNameNotFound;

					return RedirectToAction("Edit", "ReferenceTerm", new { referenceTerm.Key });
				}

				referenceTerm.DisplayNames[index].Language = model.TwoLetterCountryCode;
				referenceTerm.DisplayNames[index].Name = model.Name;

				var result = this.ImsiClient.Update<ReferenceTerm>(referenceTerm);

				TempData["success"] = Locale.ReferenceTermNameUpdatedSuccessfully;

				return RedirectToAction("Edit", "ReferenceTerm", new { id = result.Key });
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve entity: { e }");
			}

			TempData["error"] = Locale.UnableToUpdateReferenceTerm;

			return RedirectToAction("ViewReferenceTerm", "ReferenceTerm", new { id = model.ReferenceTermId });
		}
	}
}