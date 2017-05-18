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
 * Date: 2017-2-19
 */

using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.EntityIdentifierModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Models.EntityRelationshipModels;
using OpenIZAdmin.Models.EntityTagModels;

namespace OpenIZAdmin.Models.Core
{
	/// <summary>
	/// Represents an entity view model.
	/// </summary>
	public abstract class EntityViewModel : IdentifiedViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityViewModel"/> class.
		/// </summary>
		protected EntityViewModel()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityViewModel"/> class.
		/// </summary>
		protected EntityViewModel(Guid id) : base(id)
		{
			this.Identifiers = new List<EntityIdentifierViewModel>();
			this.Relationships = new List<EntityRelationshipViewModel>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityViewModel"/> class
		/// with a specific <see cref="Entity"/> instance.
		/// </summary>
		/// <param name="entity">The <see cref="Entity"/> instance.</param>
		protected EntityViewModel(Entity entity) : base(entity)
		{
			this.Identifiers = entity.Identifiers.Select(i => new EntityIdentifierViewModel(i)).OrderBy(i => i.Name).ToList();
			this.IsObsolete = entity.StatusConceptKey == StatusKeys.Obsolete;
			this.Name = string.Join(" ", entity.Names.SelectMany(n => n.Component).Select(c => c.Value));
			this.ObsoletionTime = entity.ObsoletionTime?.DateTime;
			this.Relationships = entity.Relationships.Select(r => new EntityRelationshipViewModel(r)).OrderBy(r => r.TargetName).ToList();
			this.Tags = entity.Tags.Select(t => new EntityTagViewModel(t)).ToList();

			if (entity.TypeConcept != null)
			{
				this.Type = entity.TypeConcept.ConceptNames?.Any() == true ? string.Join(" ", entity.TypeConcept.ConceptNames.Select(c => c.Name)) : entity.TypeConcept.Mnemonic;
			}
			else
			{
				this.Type = Constants.NotApplicable;
			}

			this.VersionKey = entity.VersionKey;
			this.VersionSequence = entity.VersionSequence;
		}

		/// <summary>
		/// Gets or sets a list of identifiers associated with the entity.
		/// </summary>
		public List<EntityIdentifierViewModel> Identifiers { get; set; }

		/// <summary>
		/// Gets or sets whether the entity is obsolete.
		/// </summary>
		public bool IsObsolete { get; set; }

		/// <summary>
		/// Gets or sets the name of the entity.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
        [StringLength(256, ErrorMessageResourceName = "NameLength256", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the obsoletion time of the entity.
		/// </summary>
		[Display(Name = "ObsoletionTime", ResourceType = typeof(Locale))]
		public DateTime? ObsoletionTime { get; set; }

		/// <summary>
		/// Gets or sets the relationships.
		/// </summary>
		/// <value>The relationships.</value>
		public List<EntityRelationshipViewModel> Relationships { get; set; }

		/// <summary>
		/// Gets or sets the tags.
		/// </summary>
		/// <value>The tags.</value>
		public List<EntityTagViewModel> Tags { get; set; }

		/// <summary>
		/// Gets or sets the type of the entity.
		/// </summary>
		[Display(Name = "Type", ResourceType = typeof(Locale))]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the version sequence.
		/// </summary>
		/// <value>The version sequence.</value>
		public decimal? VersionSequence { get; set; }

		/// <summary>
		/// Gets or sets the version key.
		/// </summary>
		/// <value>The version key.</value>
		public Guid? VersionKey { get; set; }
	}
}