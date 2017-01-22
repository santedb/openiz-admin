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
 * Date: 2016-7-14
 */

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OpenIZAdmin.DAL
{
	/// <summary>
	/// Contains operations for accessing entities.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IRepository<T> where T : class
	{
		/// <summary>
		/// Add an entity to the repository.
		/// </summary>
		/// <param name="entity">The entity to be added.</param>
		void Add(T entity);

		/// <summary>
		/// Get the repository as a queryable.
		/// </summary>
		/// <returns>Returns the entity as an IQueryable.</returns>
		IQueryable<T> AsQueryable();

		/// <summary>
		/// Create an entity.
		/// </summary>
		/// <returns>Returns the created entity.</returns>
		T Create();

		/// <summary>
		/// Delete an entity from the repository.
		/// </summary>
		/// <param name="entity">The entity to be deleted.</param>
		void Delete(T entity);

		/// <summary>
		/// Delete an entity from the repository by its id.
		/// </summary>
		/// <param name="id">The id of the entity to be deleted.</param>
		void Delete(object id);

		/// <summary>
		/// Get an entity from the repository by its id.
		/// </summary>
		/// <param name="id">The primary key of the entity.</param>
		/// <returns>Returns the entity.</returns>
		T FindById(object id);

		/// <summary>
		/// Get an entity from the repository by its id.
		/// </summary>
		/// <param name="id">The primary key of the entity.</param>
		/// <returns>Returns the entity.</returns>
		Task<T> FindByIdAsync(object id);

		/// <summary>
		/// Query the repository.
		/// </summary>
		/// <param name="filter">The filter for the query.</param>
		/// <param name="orderBy">The order criteria for the results.</param>
		/// <returns>Returns an IQueryable based on the filter criteria.</returns>
		IQueryable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

		/// <summary>
		/// Update an existing entity.
		/// </summary>
		/// <param name="entity">The entity to be updated.</param>
		void Update(T entity);

		/// <summary>
		/// Updates an existing entity.
		/// </summary>
		/// <param name="entity">The entity to be updated.</param>
		/// <returns>Returns a task.</returns>
		Task UpdateAsync(T entity);
	}
}