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

using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZAdmin.Services.Acts
{
	/// <summary>
	/// Represents an entity service.
	/// </summary>
	public interface IActService
	{
		
		/// <summary>
		/// Gets a specific act with a given key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="modelType">Type of the model.</param>
		/// <returns>Returns the act for the given key.</returns>
		Act Get(Guid key, Type modelType);

		/// <summary>
		/// Gets a specific entity with a given key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the entity for the given key.</returns>
		T Get<T>(Guid key) where T : Act;

		/// <summary>
		/// Gets a specific entity with a given key and version key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="versionKey">The version key.</param>
		/// <returns>Returns the entity for the given key and version key.</returns>
		T Get<T>(Guid key, Guid? versionKey) where T : Act;

		/// <summary>
		/// Gets a specific entity by id, version id, and filter expression.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="versionKey">The version key.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns the entity for the given key and the given filter expression.</returns>
		T Get<T>(Guid key, Guid? versionKey, Expression<Func<T, bool>> expression) where T : Act;

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression) where T : Act;

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="all">if set to <c>true</c> load all the nested properties of the entity.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression, int offset, int? count, bool all = false) where T : Act;

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="expandProperties">The expand properties.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression, int offset, int? count, string[] expandProperties) where T : Act;

        /// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="expandProperties">The expand properties.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression, int offset, int? count, Guid queryId, string[] expandProperties) where T : Act;

	}
}