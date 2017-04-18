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

using Elmah;
using OpenIZAdmin.Localization;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing entity relationships.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Controllers.BaseController" />
	public class EntityRelationshipController : AssociationController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipController"/> class.
		/// </summary>
		public EntityRelationshipController()
		{
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
			Guid? versionKey = null;

			try
			{
				var modelType = this.GetModelType(type);
				var entity = this.GetEntity(sourceId, modelType);
				versionKey = entity.VersionKey;

				entity.Relationships.RemoveAll(r => r.Key == id);

				var updatedEntity = this.UpdateEntity(entity, modelType);

				this.TempData["success"] = Locale.Relationship + " " + Locale.Deleted + " " + Locale.Successfully;

				return RedirectToAction("Edit", type, new { id = updatedEntity.Key.Value, versionId = updatedEntity.VersionKey.Value });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to delete entity relationship: { e }");
			}

			this.TempData["error"] = Locale.UnableToDelete + " " + Locale.Relationship;

			return RedirectToAction("Edit" + type, type, new { id = sourceId, versionId = versionKey });
		}
	}
}