/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OpenIZAdmin.DAL
{
	/// <summary>
	/// Contains operations for accessing entities.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EntityRepository<T> : IRepository<T> where T : class
	{
		private DbContext context;
		private DbSet<T> dbSet;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.DAL.EntityRepository{T}"/> class.
		/// </summary>
		/// <param name="context"></param>
		public EntityRepository(DbContext context)
		{
			this.context = context;
			this.dbSet = context.Set<T>();
		}

		/// <summary>
		/// Create an entity.
		/// </summary>
		/// <returns>Returns the created entity.</returns>
		public virtual T Create()
		{
			return dbSet.Create();
		}

		/// <summary>
		/// Add an entity to the repository.
		/// </summary>
		/// <param name="entity">The entity to be added.</param>
		public virtual void Add(T entity)
		{
			dbSet.Add(entity);
		}

		/// <summary>
		/// Update an existing entity.
		/// </summary>
		/// <param name="entity">The entity to be updated.</param>
		public virtual void Update(T entity)
		{
			dbSet.Attach(entity);
			context.Entry(entity).State = EntityState.Modified;
		}

		/// <summary>
		/// Updates an existing entity.
		/// </summary>
		/// <param name="entity">The entity to be updated.</param>
		/// <returns>Returns a task.</returns>
		public async virtual Task UpdateAsync(T entity)
		{
			await Task.Run(() =>
				{
					Update(entity);
				});
		}

		/// <summary>
		/// Delete an entity from the repository.
		/// </summary>
		/// <param name="entity">The entity to be deleted.</param>
		public virtual void Delete(T entity)
		{
			if (context.Entry(entity).State == EntityState.Detached)
			{
				dbSet.Attach(entity);
			}
			dbSet.Remove(entity);
		}

		/// <summary>
		/// Delete an entity from the repository by its id.
		/// </summary>
		/// <param name="id">The id of the entity to be deleted.</param>
		public virtual void Delete(object id)
		{
			T entity = dbSet.Find(id);
			Delete(entity);
		}

		/// <summary>
		/// Query the repository.
		/// </summary>
		/// <param name="filter">The filter for the query.</param>
		/// <param name="orderBy">The order criteria for the results.</param>
		/// <returns>Returns an IQueryable based on the filter criteria.</returns>
		public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
		{
			IQueryable<T> query = AsQueryable();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (orderBy != null)
			{
				return orderBy(query);
			}

			return query;
		}

		/// <summary>
		/// Get an entity from the repository by its id.
		/// </summary>
		/// <param name="id">The primary key of the entity.</param>
		/// <returns>Returns the entity.</returns>
		public virtual T FindById(object id)
		{
			return dbSet.Find(id);
		}

		/// <summary>
		/// Get an entity from the repository by its id.
		/// </summary>
		/// <param name="id">The primary key of the entity.</param>
		/// <returns>Returns the entity.</returns>
		public async virtual Task<T> FindByIdAsync(object id)
		{
			return await dbSet.FindAsync(id);
		}

		/// <summary>
		/// Get the repository as a queryable.
		/// </summary>
		/// <returns>Returns the entity as an IQueryable.</returns>
		public virtual IQueryable<T> AsQueryable()
		{
			return dbSet;
		}
	}
}