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

using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using OpenIZAdmin.Models.EntityIdentifierModels;

namespace OpenIZAdmin.Models.Core
{
	/// <summary>
	/// Represents an edit entity model.
	/// </summary>
	public abstract class EditEntityModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditEntityModel"/> class.
		/// </summary>
		protected EditEntityModel()
		{
			this.Identifiers = new List<EntityIdentifierEditModel>();
			this.TypeList = new List<SelectListItem>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditEntityModel"/> class
		/// with a specific <see cref="Entity"/> instance.
		/// </summary>
		/// <param name="entity">The <see cref="Entity"/> instance.</param>
		protected EditEntityModel(Entity entity) : this()
		{
			this.Id = entity.Key.Value;
			this.Identifiers = entity.Identifiers.Select(i => new EntityIdentifierEditModel(i)).OrderBy(i => i.Name).ToList();
			this.TypeList = entity.Identifiers.Select(i => new SelectListItem { Text = i.Authority.Name, Value = i.AuthorityKey?.ToString() }).ToList();
		}

		/// <summary>
		/// Gets or sets the id of the entity.
		/// </summary>
		[Required]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the identifiers.
		/// </summary>
		/// <value>The identifiers.</value>
		public List<EntityIdentifierEditModel> Identifiers { get; set; }

		/// <summary>
		/// Gets or sets the type list.
		/// </summary>
		/// <value>The type list.</value>
		public List<SelectListItem> TypeList { get; set; }
	}
}