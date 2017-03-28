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
	public class EntityIdentifierModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityIdentifierModel"/> class.
		/// </summary>
		public EntityIdentifierModel()
		{
			this.ExistingIdentifiers = new List<EntityIdentifierViewModel>();
			this.Types = new List<SelectListItem>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityIdentifierModel" /> class.
		/// </summary>
		/// <param name="entityId">The identifier for which the entity identifier is to be created.</param>
		public EntityIdentifierModel(Guid entityId) : this()
		{
			this.EntityId = entityId;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityIdentifierModel"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="entityId">The entity identifier.</param>
		public EntityIdentifierModel(Guid id, Guid entityId) : this(entityId)
		{
			this.Id = id;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityIdentifierModel" /> class.
		/// </summary>
		/// <param name="entityIdentifier">The entity identifier.</param>
		/// <param name="entityId">The entity identifier.</param>
		public EntityIdentifierModel(EntityIdentifier entityIdentifier, Guid entityId) : this(entityId)
		{
			this.Id = entityIdentifier.Key.Value;
			this.Name = entityIdentifier.Authority.Name;
			this.Type = entityIdentifier.Authority.DomainName;
			this.Value = entityIdentifier.Value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityIdentifierModel"/> class.
		/// </summary>
		/// <param name="entityIdentifier">The entity identifier.</param>
		/// <param name="entityId">The entity identifier.</param>
		/// <param name="modelType">Type of the model.</param>
		public EntityIdentifierModel(EntityIdentifier entityIdentifier, Guid entityId, string modelType) : this(entityIdentifier, entityId)
		{
			this.ModelType = modelType;
		}

		/// <summary>
		/// Gets or sets the entity identifier.
		/// </summary>
		/// <value>The entity identifier.</value>
		[Required]
		public Guid EntityId { get; set; }

		/// <summary>
		/// Gets or sets the existing identifiers.
		/// </summary>
		/// <value>The existing identifiers.</value>
		public List<EntityIdentifierViewModel> ExistingIdentifiers { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the type of the model.
		/// </summary>
		/// <value>The type of the model.</value>
		[Required]
		public string ModelType { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type of the entity identifier.
		/// </summary>
		[Display(Name = "Type", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "TypeRequired", ErrorMessageResourceType = typeof(Locale))]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the type list.
		/// </summary>
		/// <value>The type list.</value>
		public List<SelectListItem> Types { get; set; }

		/// <summary>
		/// Gets or sets the value of the entity identifier.
		/// </summary>
		[Display(Name = "Value", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "ValueRequired", ErrorMessageResourceType = typeof(Locale))]
		public string Value { get; set; }
	}
}