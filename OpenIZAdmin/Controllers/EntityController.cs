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
 * User: Nityan
 * Date: 2017-4-23
 */

using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;
using System;
using System.Diagnostics;
using System.Web.Mvc;
using OpenIZAdmin.Services.Entities;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing entities.
	/// </summary>
	[TokenAuthorize]
	public class EntityController : BaseController
	{
		/// <summary>
		/// The entity service.
		/// </summary>
		private readonly IEntityService entityService;

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityController"/> class.
		/// </summary>
		public EntityController(IEntityService entityService)
		{
			this.entityService = entityService;
		}

		/// <summary>
		/// Displays the create alert view.
		/// </summary>
		/// <returns>Returns the create alert view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Verify(string returnUrl, VerifyEntityModel model)
		{
			Entity entity = null;

			try
			{
				if (this.ModelState.IsValid)
				{
					entity = this.entityService.Verify(model.Id, model.Type);

					this.TempData["success"] = Locale.DataVerifiedSuccessfully;
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to verify data: {e}");
				this.TempData["error"] = Locale.UnableToVerifyData;
			}

			if (entity == null)
			{
				this.TempData["error"] = Locale.UnableToVerifyData;
				return Redirect(returnUrl);
			}

			return RedirectToAction("View" + model.Type, model.Type, new { id = entity.Key.Value, versionId = entity.VersionKey });
		}
	}
}