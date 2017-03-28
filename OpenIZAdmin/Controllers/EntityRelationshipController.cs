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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.EntityRelationshipModels;

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
			try
			{
				var modelType = this.GetModelType(type);
				var entity = this.GetEntity(sourceId, modelType);

				entity.Relationships.RemoveAll(r => r.Key == id);

				var updatedEntity = this.UpdateEntity(entity, modelType);

				this.TempData["success"] = Locale.Relationship + " " + Locale.Deleted + " " + Locale.Successfully;

				return RedirectToAction("View" + type, type, new { id = updatedEntity.Key.Value });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to delete entity relationship: { e }");
			}

			this.TempData["error"] = Locale.UnableToDelete + " " + Locale.Relationship;

			return RedirectToAction("View" + type, type, new { id = sourceId });
		}
	}
}