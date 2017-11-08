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
 * Date: 2017-7-9
 */

using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZAdmin.Services.Entities
{
	/// <summary>
	/// Represents an entity service.
	/// </summary>
	public interface IEntityService
	{
		/// <summary>
		/// Activates an entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the activated entity.</returns>
		Entity Activate(Entity entity);

		/// <summary>
		/// Creates the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the created entity.</returns>
		Entity Create(Entity entity);

		/// <summary>
		/// Deactivates an entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the deactivated entity.</returns>
		Entity Deactivate(Entity entity);

		/// <summary>
		/// Gets a specific entity with a given key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="modelType">Type of the model.</param>
		/// <returns>Returns the entity for the given key.</returns>
		Entity Get(Guid key, Type modelType);

		/// <summary>
		/// Gets a specific entity with a given key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the entity for the given key.</returns>
		T Get<T>(Guid key) where T : Entity;

		/// <summary>
		/// Gets a specific entity with a given key and version key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="versionKey">The version key.</param>
		/// <returns>Returns the entity for the given key and version key.</returns>
		T Get<T>(Guid key, Guid? versionKey) where T : Entity;

		/// <summary>
		/// Gets a specific entity by id, version id, and filter expression.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="versionKey">The version key.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns the entity for the given key and the given filter expression.</returns>
		T Get<T>(Guid key, Guid? versionKey, Expression<Func<T, bool>> expression) where T : Entity;

		/// <summary>
		/// Gets the entity relationship.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the entity relationship for the given key or null if no entity relationship is found.</returns>
		EntityRelationship GetEntityRelationship(Guid key);

		/// <summary>
		/// Gets the entity relationships.
		/// </summary>
		/// <typeparam name="TTargetType">The type of the t target type.</typeparam>
		/// <param name="id">The identifier.</param>
		/// <param name="entityRelationshipExpression">The entity relationship expression.</param>
		/// <returns>Returns a list of entity relationships for the given entity.</returns>
		IEnumerable<EntityRelationship> GetEntityRelationships<TTargetType>(Guid id, Expression<Func<EntityRelationship, bool>> entityRelationshipExpression = null) where TTargetType : Entity;

		/// <summary>
		/// Gets the type of the model.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>Type.</returns>
		Type GetModelType(string type);

		/// <summary>
		/// Obsoletes the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the obsoleted entity.</returns>
		Entity Obsolete<T>(Guid key) where T : Entity;

		/// <summary>
		/// Obsoletes the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the obsoleted entity.</returns>
		Entity Obsolete(Entity entity);

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression) where T : Entity;

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="all">if set to <c>true</c> load all the nested properties of the entity.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression, int offset, int? count, bool all = false) where T : Entity;

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="expandProperties">The expand properties.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression, int offset, int? count, string[] expandProperties) where T : Entity;

		/// <summary>
		/// Searches for a specific entity by search term.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of entities which match the given search term.</returns>
		IEnumerable<T> Search<T>(string searchTerm) where T : Entity;

		/// <summary>
		/// Updates the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the updated entity.</returns>
		Entity Update(Entity entity);

		/// <summary>
		/// Verifies the entity.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="modelType">Type of the model.</param>
		/// <returns>Returns the verified entity.</returns>
		Entity Verify(Guid key, string modelType);
	}
}