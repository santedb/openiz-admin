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

using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OpenIZAdmin.Models.EntityRelationshipModels
{
	/// <summary>
	/// Represents an entity relationship view model.
	/// </summary>
	public class EntityRelationshipViewModel : IdentifiedViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipViewModel"/> class.
		/// </summary>
		public EntityRelationshipViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipViewModel"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		public EntityRelationshipViewModel(Guid id) : base(id)
		{
			this.Id = id;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipViewModel" /> class.
		/// </summary>
		/// <param name="entityRelationship">The entity relationship.</param>
		public EntityRelationshipViewModel(EntityRelationship entityRelationship)
		{
			this.Id = entityRelationship.Key.Value;
			this.Quantity = entityRelationship.Quantity;
			this.RelationshipTypeName = entityRelationship.RelationshipType != null ? string.Join(" ", entityRelationship.RelationshipType.ConceptNames.Select(c => c.Name)) : Constants.NotApplicable;
			this.TargetName = entityRelationship.TargetEntity != null ? string.Join(" ", entityRelationship.TargetEntity.Names.SelectMany(n => n.Component).Select(c => c.Value)) : Constants.NotApplicable;
			this.TargetTypeConcept = entityRelationship.TargetEntity?.TypeConcept != null ? string.Join(" ", entityRelationship.TargetEntity.TypeConcept.ConceptNames.Select(c => c.Name)) : Constants.NotApplicable;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipViewModel"/> class.
		/// </summary>
		/// <param name="baseEntityData">The base entity data.</param>
		protected EntityRelationshipViewModel(BaseEntityData baseEntityData) : base(baseEntityData)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipViewModel"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="creationTime">The creation time.</param>
		protected EntityRelationshipViewModel(Guid id, DateTimeOffset creationTime) : base(id, creationTime)
		{
		}

		/// <summary>
		/// Gets or sets the quantity.
		/// </summary>
		/// <value>The quantity.</value>
		[Display(Name = "Quantity", ResourceType = typeof(Locale))]
		public int Quantity { get; set; }

		/// <summary>
		/// Gets or sets the name of the relationship type.
		/// </summary>
		/// <value>The name of the relationship type.</value>
		[Display(Name = "RelationshipType", ResourceType = typeof(Locale))]
		public string RelationshipTypeName { get; set; }

		/// <summary>
		/// Gets or sets the name of the target.
		/// </summary>
		/// <value>The name of the target.</value>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		public string TargetName { get; set; }

		/// <summary>
		/// Gets or sets the target type concept.
		/// </summary>
		/// <value>The target type concept.</value>
		[Display(Name = "Type", ResourceType = typeof(Locale))]
		public string TargetTypeConcept { get; set; }
	}
}