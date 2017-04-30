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
 * User: khannan
 * Date: 2017-3-20
 */

using Elmah;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.EntityIdentifierModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing entity identifiers.
	/// </summary>
	[TokenAuthorize]
	public class EntityIdentifierController : AssociationController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityIdentifierController"/> class.
		/// </summary>
		public EntityIdentifierController()
		{
		}

		/// <summary>
		/// Adds the new identifier.
		/// </summary>
		/// <param name="id">The entity identifier for which to add a new identifier.</param>
		/// <param name="type">The type.</param>
		/// <returns>ActionResult.</returns>
		[HttpGet]
		public ActionResult Create(Guid id, string type)
		{
			var model = new EntityIdentifierModel(id);

			try
			{
				var modelType = this.GetModelType(type);
				var entity = this.GetEntity(model.EntityId, modelType);

				model.ExistingIdentifiers = entity.Identifiers.Select(i => new EntityIdentifierViewModel(i)).ToList();
				model.ModelType = type;
				model.Types = RemoveExistingIdentifiers(this.GetAssigningAuthorities().ToSelectList("Name", "Key").ToList(), entity.Identifiers);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve entity: { e }");
			}

			return View(model);
		}

		/// <summary>
		/// Creates the specified model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(EntityIdentifierModel model)
		{
			var identifiers = new List<EntityIdentifier>();

			try
			{
				if (this.ModelState.IsValid)
				{
					var returnUrl = this.Request.UrlReferrer ?? new Uri(this.Request.Url.GetLeftPart(UriPartial.Authority));

					var modelType = this.GetModelType(model.ModelType);
					var entity = this.GetEntity(model.EntityId, modelType);

					if (entity == null)
					{
						this.TempData["error"] = Locale.Entity + " " + Locale.NotFound;

						return Redirect(returnUrl.ToString());
					}

					identifiers = entity.Identifiers;

					var authority = this.ImsiClient.Get<AssigningAuthority>(Guid.Parse(model.Type), null) as AssigningAuthority;

					if (authority == null)
					{
						this.TempData["error"] = Locale.AssigningAuthority + " " + Locale.NotFound;
						return Redirect(returnUrl.ToString());
					}

					if (!this.IsValidIdentifier(authority, model.Value))
					{
						this.ModelState.AddModelError("", Locale.IdentifierFormatInvalid);

						model = this.RepopulateModel(model, identifiers);

						this.TempData["error"] = Locale.IdentifierFormatInvalid;
						return View(model);
					}

					entity.Identifiers.Add(new EntityIdentifier(authority, model.Value));
					entity.CreationTime = DateTimeOffset.Now;
					entity.VersionKey = null;

					var updatedEntity = this.UpdateEntity(entity, modelType);

					this.TempData["success"] = Locale.Identifier + " " + Locale.Created + " " + Locale.Successfully;

					return RedirectToAction("Edit", model.ModelType, new { id = updatedEntity.Key.Value, versionId = updatedEntity.VersionKey });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to create entity identifier: { e }");
			}

			model.Types = RemoveExistingIdentifiers(this.GetAssigningAuthorities().ToSelectList("Name", "Key").ToList(), identifiers);

			this.TempData["error"] = Locale.UnableToCreate + " " + Locale.Identifier;

			return View(model);
		}

		/// <summary>
		/// Deletes the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="entityId">The entity identifier.</param>
		/// <param name="type">The type.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id, Guid entityId, string type)
		{
			try
			{
				var modelType = this.GetModelType(type);
				var entity = this.GetEntity(entityId, modelType);

				entity.Identifiers.RemoveAll(i => i.Key == id);

				var updatedEntity = this.UpdateEntity(entity, modelType);

				this.TempData["success"] = Locale.IdentifierDeletedSuccessfully;

				return RedirectToAction("Edit", type, new { id = updatedEntity.Key.Value });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to delete entity identifier: { e }");
			}

			this.TempData["error"] = Locale.UnableToDeleteIdentifier;

			return RedirectToAction("Edit", type, new { id = entityId });
		}

		/// <summary>
		/// Edits the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="entityId">The entity identifier.</param>
		/// <param name="type">The type.</param>
		/// <returns>ActionResult.</returns>
		[HttpGet]
		public ActionResult Edit(Guid id, Guid entityId, string type)
		{
			var model = new EntityIdentifierModel(id, entityId);

			try
			{
				var modelType = this.GetModelType(type);
				var entity = this.GetEntity(model.EntityId, modelType);

				model.ExistingIdentifiers = entity.Identifiers.Select(i => new EntityIdentifierViewModel(i)).ToList();

				if (entity.Identifiers.Any(i => i.Key == id))
				{
					var entityIdentifier = entity.Identifiers.First(i => i.Key == id);
					model.Type = entityIdentifier.AuthorityKey?.ToString();
					model.Value = entityIdentifier.Value;
				}

				model.ModelType = type;

				if (!string.IsNullOrEmpty(model.Type) && !string.IsNullOrWhiteSpace(model.Type))
				{
					model.Types = RemoveExistingIdentifiers(this.GetAssigningAuthorities().ToSelectList("Name", "Key", a => a.Key == Guid.Parse(model.Type)), entity.Identifiers);
				}
				else
				{
					model.Types = RemoveExistingIdentifiers(this.GetAssigningAuthorities().ToSelectList("Name", "Key"), entity.Identifiers);
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve entity identifier: { e }");
			}

			return View(model);
		}

		/// <summary>
		/// Edits the specified model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EntityIdentifierModel model)
		{
			var identifiers = new List<EntityIdentifier>();

			try
			{
				if (this.ModelState.IsValid)
				{
					var returnUrl = this.Request.UrlReferrer ?? new Uri(this.Request.Url.GetLeftPart(UriPartial.Authority));

					var modelType = this.GetModelType(model.ModelType);
					var entity = this.GetEntity(model.EntityId, modelType);

					if (entity == null)
					{
						this.TempData["error"] = Locale.Entity + " " + Locale.NotFound;

						return Redirect(returnUrl.ToString());
					}

					var authority = this.ImsiClient.Get<AssigningAuthority>(Guid.Parse(model.Type), null) as AssigningAuthority;

					if (authority == null)
					{
						this.TempData["error"] = Locale.AssigningAuthority + " " + Locale.NotFound;
						return Redirect(returnUrl.ToString());
					}

					if (!this.IsValidIdentifier(authority, model.Value))
					{
						this.ModelState.AddModelError("", Locale.IdentifierFormatInvalid);

						model = this.RepopulateModel(model, identifiers);

						this.TempData["error"] = Locale.IdentifierFormatInvalid;
						return View(model);
					}

					entity.Identifiers.RemoveAll(i => i.Key == model.Id);
					entity.Identifiers.Add(new EntityIdentifier(authority, model.Value));

					var updatedEntity = this.UpdateEntity(entity, modelType);

					this.TempData["success"] = Locale.Identifier + " " + Locale.Created + " " + Locale.Successfully;

					return RedirectToAction("Edit", model.ModelType, new { id = updatedEntity.Key.Value, versionId = updatedEntity.VersionKey });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to create entity identifier: { e }");
			}

			if (!string.IsNullOrEmpty(model.Type) && !string.IsNullOrWhiteSpace(model.Type))
			{
				model.Types = RemoveExistingIdentifiers(this.GetAssigningAuthorities().ToSelectList("Name", "Key", a => a.Key == Guid.Parse(model.Type)), identifiers);
			}
			else
			{
				model.Types = RemoveExistingIdentifiers(this.GetAssigningAuthorities().ToSelectList("Name", "Key"), identifiers);
			}

			this.TempData["error"] = Locale.UnableToCreate + " " + Locale.Identifier;

			return View(model);
		}

		/// <summary>
		/// Gets the assigning authorities.
		/// </summary>
		/// <returns>List&lt;AssigningAuthority&gt;.</returns>
		private IEnumerable<AssigningAuthority> GetAssigningAuthorities()
		{
			var bundle = this.ImsiClient.Query<AssigningAuthority>(a => a.ObsoletionTime == null);

			bundle.Reconstitute();

			return bundle.Item.OfType<AssigningAuthority>().Where(a => a.ObsoletionTime == null);
		}

		/// <summary>
		/// Determines whether the identifier is valid for the given assigning authority.
		/// </summary>
		/// <param name="assigningAuthority">The assigning authority.</param>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> Determines whether the identifier is valid for the given assigning authority; Otherwise, <c>false</c>.</returns>
		private bool IsValidIdentifier(AssigningAuthority assigningAuthority, string value)
		{
			var status = false;

			if (!string.IsNullOrEmpty(assigningAuthority.ValidationRegex) && !string.IsNullOrWhiteSpace(assigningAuthority.ValidationRegex))
			{
				if (new Regex(assigningAuthority.ValidationRegex).IsMatch(value))
				{
					status = true;
				}
			}
			else
			{
				status = true;
			}

			return status;
		}

		/// <summary>
		/// Removes the existing identifiers.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <param name="identifiers">The identifiers.</param>
		/// <returns>List&lt;SelectListItem&gt;.</returns>
		private List<SelectListItem> RemoveExistingIdentifiers(List<SelectListItem> items, IEnumerable<EntityIdentifier> identifiers)
		{
			foreach (var entityIdentifier in identifiers)
			{
				items.RemoveAll(x => x.Value == entityIdentifier.AuthorityKey?.ToString() && !x.Selected);
			}

			return items.OrderBy(i => i.Text).ToList();
		}

		/// <summary>
		/// Repopulates the model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="identifiers">The identifiers.</param>
		/// <returns>EntityIdentifierModel.</returns>
		private EntityIdentifierModel RepopulateModel(EntityIdentifierModel model, List<EntityIdentifier> identifiers)
		{
			model.ExistingIdentifiers = identifiers.Select(i => new EntityIdentifierViewModel(i)).OrderBy(i => i.Name).ToList();

			if (!string.IsNullOrEmpty(model.Type) && !string.IsNullOrWhiteSpace(model.Type))
			{
				model.Types = RemoveExistingIdentifiers(this.GetAssigningAuthorities().ToSelectList("Name", "Key", a => a.Key == Guid.Parse(model.Type)), identifiers);
			}
			else
			{
				model.Types = RemoveExistingIdentifiers(this.GetAssigningAuthorities().ToSelectList("Name", "Key"), identifiers);
			}

			return model;
		}
	}
}