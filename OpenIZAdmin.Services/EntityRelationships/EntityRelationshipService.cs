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
 * Date: 2017-8-3
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Core;
using OpenIZAdmin.Services.Core;
using OpenIZAdmin.Services.Metadata;

namespace OpenIZAdmin.Services.EntityRelationships
{
	/// <summary>
	/// Represents an entity relationship service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.ImsiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.EntityRelationships.IEntityRelationshipService" />
	public class EntityRelationshipService : ImsiServiceBase, IEntityRelationshipService
	{
		/// <summary>
		/// The concept service.
		/// </summary>
		private readonly IConceptService conceptService;

		/// <summary>
		/// The entity service.
		/// </summary>
		private readonly IEntityService entityService;

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipService" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="conceptService">The concept service.</param>
		public EntityRelationshipService(ImsiServiceClient client, IConceptService conceptService, IEntityService entityService) : base(client)
		{
			this.conceptService = conceptService;
			this.entityService = entityService;
		}

		/// <summary>
		/// Creates an entity relationship.
		/// </summary>
		/// <param name="sourceKey">The source key.</param>
		/// <param name="targetKey">The target key.</param>
		/// <param name="relationshipType">Type of the relationship.</param>
		/// <param name="quantity">The quantity.</param>
		/// <returns>Returns the created entity relationship.</returns>
		public EntityRelationship Create(Guid sourceKey, Guid targetKey, Guid relationshipType, uint quantity)
		{
			var entityRelationship = new EntityRelationship(relationshipType, targetKey)
			{
				SourceEntityKey = sourceKey,
				Quantity = Convert.ToInt32(quantity)
			};

			return this.Client.Create(entityRelationship);
		}

		/// <summary>
		/// Gets the entity relationship.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns an entity relationships which matches the given key.</returns>
		public EntityRelationship Get(Guid key)
		{
			return this.Client.Get<EntityRelationship>(key, null) as EntityRelationship;
		}

		/// <summary>
		/// Gets the entity relationships by source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns>Returns a list of entity relationships by source key.</returns>
		public IEnumerable<EntityRelationship> GetEntityRelationshipsBySource(Guid source)
		{
			var bundle = this.Client.Query<EntityRelationship>(r => r.SourceEntityKey == source && r.ObsoleteVersionSequenceId == null);

			bundle.Reconstitute();

			return bundle.Item.OfType<EntityRelationship>().Where(r => r.SourceEntityKey == source && r.ObsoleteVersionSequenceId == null);
		}

		/// <summary>
		/// Gets the entity relationships.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="relationshipTypes">The relationship types.</param>
		/// <returns>Returns a list of entity relationships for a given source key and filtered by relationship types.</returns>
		public IEnumerable<EntityRelationship> GetEntityRelationshipsBySource(Guid source, params Guid[] relationshipTypes)
		{
			var relationshipTypeFilters = relationshipTypes.ToList();

			var bundle = this.Client.Query<EntityRelationship>(r => r.SourceEntityKey == source && r.ObsoleteVersionSequenceId == null, 0, null, new string[] { Constants.RelationshipTarget });

			bundle.Reconstitute();

			return this.LoadNested(bundle, r => r.SourceEntityKey == source && relationshipTypeFilters.Contains(r.RelationshipTypeKey.Value) && r.ObsoleteVersionSequenceId == null);
		}

		/// <summary>
		/// Gets the entity relationships by target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <returns>Returns a list of entity relationships which match the given target key.</returns>
		public IEnumerable<EntityRelationship> GetEntityRelationshipsByTarget(Guid target)
		{
			var bundle = this.Client.Query<EntityRelationship>(r => r.TargetEntityKey == target && r.ObsoleteVersionSequenceId == null);

			bundle.Reconstitute();

			return this.LoadNested(bundle, r => r.TargetEntityKey == target && r.ObsoleteVersionSequenceId == null);
		}

		/// <summary>
		/// Gets the entity relationships.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="relationshipTypes">The relationship types.</param>
		/// <returns>Returns a list of entity relationships for a given target key and filtered by relationship types.</returns>
		public IEnumerable<EntityRelationship> GetEntityRelationshipsByTarget(Guid target, params Guid[] relationshipTypes)
		{
			var relationshipTypeFilters = relationshipTypes.ToList();

			var bundle = this.Client.Query<EntityRelationship>(r => r.TargetEntityKey == target && r.ObsoleteVersionSequenceId == null);

			bundle.Reconstitute();

			return bundle.Item.OfType<EntityRelationship>().Where(r => r.TargetEntityKey == target && relationshipTypeFilters.Contains(r.RelationshipTypeKey.Value) && r.ObsoleteVersionSequenceId == null);
		}

		/// <summary>
		/// Loads the nested data for the entity relationships.
		/// </summary>
		/// <param name="bundle">The bundle.</param>
		/// <returns>Returns a list of entity relationships with the nested data.</returns>
		private IEnumerable<EntityRelationship> LoadNested(Bundle bundle, Expression<Func<EntityRelationship, bool>> expression)
		{
			var relationships = new List<EntityRelationship>();

			foreach (var relationship in bundle.Item.OfType<EntityRelationship>().Where(r => expression.Compile().Invoke(r)))
			{
				relationship.RelationshipType = relationship.LoadProperty<Concept>(nameof(EntityRelationship.RelationshipType));
				relationship.TargetEntity = relationship.LoadProperty(nameof(EntityRelationship.TargetEntity)) as Entity;

				if (relationship.TargetEntity?.ShouldSerializeTypeConceptKey() == true && relationship.TargetEntity.TypeConcept == null)
				{
					relationship.TargetEntity.TypeConcept = relationship.TargetEntity.LoadProperty<Concept>("TypeConcept");
				}

				relationships.Add(relationship);
			}

			return relationships;

		}
	}
}
