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
 * Date: 2017-2-20
 */

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.EntityIdentifierModels;
using OpenIZAdmin.Models.EntityRelationshipModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.Core
{
	/// <summary>
	/// Represents an edit entity model.
	/// </summary>
	public abstract class EditEntityModel : IdentifiedEditModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditEntityModel"/> class.
		/// </summary>
		protected EditEntityModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditEntityModel"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		protected EditEntityModel(Guid id) : base(id)
		{
			this.Identifiers = new List<EntityIdentifierModel>();
			this.Relationships = new List<EntityRelationshipModel>();
			this.Types = new List<SelectListItem>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditEntityModel"/> class
		/// with a specific <see cref="Entity"/> instance.
		/// </summary>
		/// <param name="entity">The <see cref="Entity"/> instance.</param>
		protected EditEntityModel(Entity entity) : this(entity.Key.Value)
		{
			this.Identifiers = entity.Identifiers.Select(i => new EntityIdentifierModel(i, entity.Key.Value, entity.Type)).OrderBy(i => i.Name).ToList();
			this.IsObsolete = entity.StatusConceptKey == StatusKeys.Obsolete;
			this.Relationships = entity.Relationships.Select(r => new EntityRelationshipModel(r, entity.Type, entity.ClassConceptKey?.ToString()) { Quantity = r.Quantity }).ToList();
			this.Types = entity.Identifiers.Select(i => new SelectListItem { Text = i.Authority.Name, Value = i.AuthorityKey?.ToString() }).ToList();
			this.UpdatedTime = entity.CreationTime.DateTime.ToString(CultureInfo.InvariantCulture);
			this.VersionKey = entity.VersionKey;
		}

		/// <summary>
		/// Gets or sets the identifiers.
		/// </summary>
		/// <value>The identifiers.</value>
		public List<EntityIdentifierModel> Identifiers { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is obsolete.
		/// </summary>
		/// <value><c>true</c> if this instance is obsolete; otherwise, <c>false</c>.</value>
		public bool IsObsolete { get; set; }

		/// <summary>
		/// Gets or sets the relationships.
		/// </summary>
		/// <value>The relationships.</value>
		public List<EntityRelationshipModel> Relationships { get; set; }

		/// <summary>
		/// Gets or sets the type list.
		/// </summary>
		/// <value>The type list.</value>
		public List<SelectListItem> Types { get; set; }

		/// <summary>
		/// Gets or sets the updated by.
		/// </summary>
		/// <value>The updated by.</value>
		[Display(Name = "UpdatedBy", ResourceType = typeof(Locale))]
		public string UpdatedBy { get; set; }

		/// <summary>
		/// Gets or sets the updated time.
		/// </summary>
		/// <value>The updated time.</value>
		[Display(Name = "UpdatedTime", ResourceType = typeof(Locale))]
		public string UpdatedTime { get; set; }

		/// <summary>
		/// Gets or sets the version key.
		/// </summary>
		/// <value>The version key.</value>
		public Guid? VersionKey { get; set; }
	}
}