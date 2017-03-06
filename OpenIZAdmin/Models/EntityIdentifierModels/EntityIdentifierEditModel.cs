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
 * Date: 2017-3-6
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.EntityIdentifierModels
{
	/// <summary>
	/// Represents an entity identifier edit model.
	/// </summary>
	public class EntityIdentifierEditModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityIdentifierEditModel"/> class.
		/// </summary>
		public EntityIdentifierEditModel()
		{
			this.TypeList = new List<SelectListItem>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityIdentifierEditModel"/> class.
		/// </summary>
		/// <param name="entityIdentifier">The entity identifier.</param>
		public EntityIdentifierEditModel(EntityIdentifier entityIdentifier) : this()
		{
			this.Id = entityIdentifier.Key.Value;
			this.Name = entityIdentifier.Authority.Name;
			this.Type = entityIdentifier.Authority.DomainName;
			this.Value = entityIdentifier.Value;
		}

		/// <summary>
		/// Gets or sets the id of the entity identifier.
		/// </summary>
		[Required]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the entity identifier.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type of the entity identifier.
		/// </summary>
		[Display(Name = "Type", ResourceType = typeof(Locale))]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the type list.
		/// </summary>
		/// <value>The type list.</value>
		public List<SelectListItem> TypeList { get; set; }

		/// <summary>
		/// Gets or sets the value of the entity identifier.
		/// </summary>
		[Display(Name = "Value", ResourceType = typeof(Locale))]
		public string Value { get; set; }
	}
}