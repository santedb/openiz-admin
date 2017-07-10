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


using Microsoft.AspNet.Identity;
using OpenIZ.Core.Alert.Alerting;
using OpenIZ.Core.Model.AMI.Alerting;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AlertModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web.Mvc;
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing alerts.
	/// </summary>
	[TokenAuthorize]
	public class EntityController : EntityBaseController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityController"/> class.
		/// </summary>
		public EntityController()
		{
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
					var modelType = this.GetModelType(model.Type);

					entity = this.GetEntity(model.Id, this.GetModelType(model.Type));

					entity.Tags.RemoveAll(t => t.TagKey == Constants.ImportedDataTag && t.Value == "true");
					entity.Relationships.RemoveAll(r => r.TargetEntityKey == entity.Key);

					entity = this.UpdateEntity(entity, modelType);

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