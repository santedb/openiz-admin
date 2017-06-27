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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.EntityRelationshipModels
{
	/// <summary>
	/// Represents an entity relationship model.
	/// </summary>
	public class EntityRelationshipModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipModel"/> class.
		/// </summary>
		public EntityRelationshipModel()
		{
			this.ExistingRelationships = new List<EntityRelationshipViewModel>();
			this.TargetList = new List<SelectListItem>();
			this.RelationshipTypes = new List<SelectListItem>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipModel"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="sourceId">The source identifier.</param>
		public EntityRelationshipModel(Guid id, Guid sourceId) : this()
		{
			this.Id = id;
			this.SourceId = sourceId;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipModel"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="sourceId">The source identifier.</param>
		/// <param name="targetId">The target identifier.</param>
		public EntityRelationshipModel(Guid id, Guid sourceId, Guid targetId) : this()
		{
			this.Id = id;
			this.SourceId = sourceId;
			this.TargetId = targetId.ToString();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipModel"/> class.
		/// </summary>
		/// <param name="entityRelationship">The entity relationship.</param>
		public EntityRelationshipModel(EntityRelationship entityRelationship) : this(entityRelationship.Key.Value, entityRelationship.SourceEntityKey.Value, entityRelationship.TargetEntityKey.Value)
		{
			this.RelationshipType = entityRelationship.RelationshipTypeKey?.ToString() ?? Constants.NotApplicable;
			this.RelationshipTypeName = entityRelationship.RelationshipType != null ? string.Join(" ", entityRelationship.RelationshipType.ConceptNames.Select(c => c.Name)) : Constants.NotApplicable;
			this.TargetName = entityRelationship.TargetEntity != null ? string.Join(" ", entityRelationship.TargetEntity.Names.SelectMany(n => n.Component).Select(c => c.Value)) : Constants.NotApplicable;
			this.TargetTypeConcept = entityRelationship.TargetEntity?.TypeConcept != null ? string.Join(" ", entityRelationship.TargetEntity.TypeConcept.ConceptNames.Select(c => c.Name)) : Constants.NotApplicable;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipModel"/> class.
		/// </summary>
		/// <param name="entityRelationship">The entity relationship.</param>
		/// <param name="sourceType">Type of the source.</param>
		public EntityRelationshipModel(EntityRelationship entityRelationship, string sourceType) : this(entityRelationship)
		{
			this.SourceType = sourceType;
		}

		/// <summary>
		/// Gets or sets the existing relationships.
		/// </summary>
		/// <value>The existing relationships.</value>
		public List<EntityRelationshipViewModel> ExistingRelationships { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[Required]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the quantity.
		/// </summary>
		/// <value>The quantity.</value>
		[Display(Name = "Quantity", ResourceType = typeof(Locale))]
		public int? Quantity { get; set; }

		/// <summary>
		/// Gets or sets the type of the relationship.
		/// </summary>
		/// <value>The type of the relationship.</value>
		[Display(Name = "RelationshipType", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "RelationshipType", ErrorMessageResourceType = typeof(Locale))]
		public string RelationshipType { get; set; }

        /// <summary>
        /// Gets or sets the name of the relationship type.
        /// </summary>
        /// <value>The name of the relationship type.</value>        
        public string RelationshipTypeName { get; set; }

		/// <summary>
		/// Gets or sets the relationship types.
		/// </summary>
		/// <value>The relationship types.</value>
		public List<SelectListItem> RelationshipTypes { get; set; }

		/// <summary>
		/// Gets or sets the source identifier.
		/// </summary>
		/// <value>The source identifier.</value>
		[Required]
		public Guid SourceId { get; set; }

		/// <summary>
		/// Gets or sets the type of the source.
		/// </summary>
		/// <value>The type of the source.</value>
		public string SourceType { get; set; }

		/// <summary>
		/// Gets or sets the target identifier.
		/// </summary>
		/// <value>The target identifier.</value>
		[Display(Name = "Related", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "RelationRequired", ErrorMessageResourceType = typeof(Locale))]
		public string TargetId { get; set; }

		/// <summary>
		/// Gets or sets the target list.
		/// </summary>
		/// <value>The target list.</value>
		public List<SelectListItem> TargetList { get; set; }

		/// <summary>
		/// Gets or sets the name of the target.
		/// </summary>
		/// <value>The name of the target.</value>
		public string TargetName { get; set; }

		/// <summary>
		/// Gets or sets the target type concept.
		/// </summary>
		/// <value>The target type concept.</value>
		public string TargetTypeConcept { get; set; }
	}
}