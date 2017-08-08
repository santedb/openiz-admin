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

using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;

namespace OpenIZAdmin.Services.EntityRelationships
{
	/// <summary>
	/// Represents an entity relationship service.
	/// </summary>
	public interface IEntityRelationshipService
	{
		/// <summary>
		/// Creates an entity relationship.
		/// </summary>
		/// <param name="sourceKey">The source key.</param>
		/// <param name="targetKey">The target key.</param>
		/// <param name="relationshipType">Type of the relationship.</param>
		/// <param name="quantity">The quantity.</param>
		/// <returns>Returns the created entity relationship.</returns>
		EntityRelationship Create(Guid sourceKey, Guid targetKey, Guid relationshipType, uint quantity);

		/// <summary>
		/// Gets the entity relationship.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns an entity relationships which matches the given key.</returns>
		EntityRelationship Get(Guid key);

		/// <summary>
		/// Gets the entity relationships by source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns>Returns a list of entity relationships which match the given source key.</returns>
		IEnumerable<EntityRelationship> GetEntityRelationshipsBySource(Guid source);

		/// <summary>
		/// Gets the entity relationships.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="relationshipType">Type of the relationship.</param>
		/// <returns>Returns a list of entity relationships for a given source key and filtered by relationship types.</returns>
		IEnumerable<EntityRelationship> GetEntityRelationshipsBySource(Guid source, Guid? relationshipType);

		/// <summary>
		/// Gets the entity relationships by target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <returns>Returns a list of entity relationships which match the given target key.</returns>
		IEnumerable<EntityRelationship> GetEntityRelationshipsByTarget(Guid target);

		/// <summary>
		/// Gets the entity relationships.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="relationshipType">Type of the relationship.</param>
		/// <returns>Returns a list of entity relationships for a given target key and filtered by relationship types.</returns>
		IEnumerable<EntityRelationship> GetEntityRelationshipsByTarget(Guid target, Guid? relationshipType);
	}
}