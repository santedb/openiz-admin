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
 * Date: 2017-3-27
 */

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.EntityRelationshipModels;
using OpenIZAdmin.Services.Entities;
using OpenIZAdmin.Services.EntityRelationships;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing entity relationships.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Controllers.BaseController" />
	public class EntityRelationshipController : Controller
	{
		/// <summary>
		/// The entity relationship concept service.
		/// </summary>
		private readonly IEntityRelationshipConceptService entityRelationshipConceptService;

		/// <summary>
		/// The entity relationship service.
		/// </summary>
		private readonly IEntityRelationshipService entityRelationshipService;

		/// <summary>
		/// The entity service.
		/// </summary>
		private readonly IEntityService entityService;

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipController" /> class.
		/// </summary>
		/// <param name="entityRelationshipConceptService">The entity relationship concept service.</param>
		/// <param name="entityRelationshipService">The entity relationship service.</param>
		/// <param name="entityService">The entity service.</param>
		public EntityRelationshipController(IEntityRelationshipConceptService entityRelationshipConceptService, IEntityRelationshipService entityRelationshipService, IEntityService entityService)
		{
			this.entityRelationshipConceptService = entityRelationshipConceptService;
			this.entityRelationshipService = entityRelationshipService;
			this.entityService = entityService;
		}

		/// <summary>
		/// Deletes the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="sourceId">The source identifier.</param>
		/// <param name="type">The type.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id, Guid sourceId, string type)
		{
			try
			{
				var entityRelationship = entityRelationshipService.Get(id);

				if (entityRelationship == null)
				{
					this.TempData["error"] = Locale.RelationshipNotFound;
					return RedirectToAction("Edit", type, new { id = sourceId });
				}

				entityRelationshipService.Delete(id);

				this.TempData["success"] = Locale.RelationshipDeletedSuccessfully;

				return RedirectToAction("Edit", type, new { id = sourceId });
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to delete entity relationship: { e }");
			}

			this.TempData["error"] = Locale.UnableToDeleteRelationship;

			return RedirectToAction("Edit", type, new { id = sourceId });
		}

		/// <summary>
		/// Edits an entity relationship.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="sourceId">The source identifier.</param>
		/// <param name="type">The type.</param>
		/// <returns>Returns the entity relationship edit view.</returns>
		[HttpGet]
		public ActionResult Edit(Guid id, Guid sourceId, string type)
		{
			Guid? versionKey = null;

			try
			{
				var entityRelationship = entityRelationshipService.Get(id);

				var modelType = this.entityService.GetModelType(type);
				var entity = this.entityService.Get(sourceId, modelType);
				versionKey = entity.VersionKey;

				var model = new EntityRelationshipModel(id, sourceId, versionKey ?? Guid.Empty)
				{
					ModelType = type,
					Quantity = entityRelationship.Quantity,
					TargetId = entityRelationship.TargetEntityKey.ToString()
					//ExistingRelationships = place.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList()
				};

				if (modelType == typeof(Material))
				{
					model.RelationshipType = EntityRelationshipTypeKeys.Instance.ToString();
					model.RelationshipTypes.AddRange(entityRelationshipConceptService.GetMaterialManufacturedMaterialRelationshipConcepts().ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == EntityRelationshipTypeKeys.Instance).ToList());
				}
				else if (modelType == typeof(Organization))
				{
				}
				else if (modelType == typeof(Place))
				{
				}

				return View(model);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to delete entity relationship: {e}");
			}

			this.TempData["error"] = Locale.UnableToEditRelationship;

			return RedirectToAction("Edit" + type, type, new { id = sourceId, versionId = versionKey });
		}

		/// <summary>
		/// Edits an entity relationship.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Returns an action result instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EntityRelationshipModel model)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					var modelType = entityService.GetModelType(model.ModelType);

					this.entityRelationshipService.Update(model.Id, model.SourceId, Guid.Parse(model.TargetId), Guid.Parse(model.RelationshipType), modelType, model.Quantity);

					this.TempData["success"] = Locale.RelationshipUpdatedSuccessfully;

					return RedirectToAction("Edit", model.ModelType, new { id = model.SourceId });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update relationship: {e}");
			}

			this.TempData["error"] = Locale.UnableToUpdateRelationship;

			return View(model);
		}
	}
}