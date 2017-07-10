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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Core.Auditing.Core;
using OpenIZAdmin.Core.Auditing.Entities;
using OpenIZAdmin.Services.Entities;
using OpenIZAdmin.Services.Metadata;

namespace OpenIZAdmin.Services.Core
{
	/// <summary>
	/// Represents a base entity service.
	/// </summary>
	/// <typeparam name="T">The type of entity.</typeparam>
	/// <seealso cref="OpenIZAdmin.Services.Core.ImsiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.Entities.IEntityService{T}" />
	public abstract class EntityServiceBase<T> : ImsiServiceBase, IEntityService<T> where T : Entity
	{
		/// <summary>
		/// The concept service.
		/// </summary>
		private readonly IConceptService conceptService;

		/// <summary>
		/// The core audit service.
		/// </summary>
		private readonly ICoreAuditService coreAuditService;

		/// <summary>
		/// The entity audit service
		/// </summary>
		private readonly IEntityAuditService entityAuditService;

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityServiceBase{T}" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="conceptService">The concept service.</param>
		/// <param name="coreAuditService">The core audit service.</param>
		/// <param name="entityAuditService">The entity audit service.</param>
		protected EntityServiceBase(ImsiServiceClient client, IConceptService conceptService, ICoreAuditService coreAuditService, IEntityAuditService entityAuditService) : base(client)
		{
			this.conceptService = conceptService;
			this.coreAuditService = coreAuditService;
			this.entityAuditService = entityAuditService;
		}

		/// <summary>
		/// Activates an entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the activated entity.</returns>
		public virtual T Activate(T entity)
		{
			entity.CreationTime = DateTimeOffset.Now;
			entity.StatusConceptKey = StatusKeys.Active;
			entity.VersionKey = null;

			T updated;

			try
			{
				updated = this.Update(entity);

				this.entityAuditService.AuditUpdateEntity(OutcomeIndicator.Success, updated);
			}
			catch (Exception e)
			{
				this.coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, entityAuditService.UpdateEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return updated;
		}

		/// <summary>
		/// Creates the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the created entity.</returns>
		public virtual T Create(T entity)
		{
			// set the creation time
			entity.CreationTime = DateTimeOffset.Now;

			// remove all the relationships where I am the target entity
			entity.Relationships.RemoveAll(r => r.TargetEntityKey == entity.Key);

			// null out the version key
			entity.VersionKey = null;

			T created;

			try
			{
				var createMethod = this.Client.GetType().GetRuntimeMethod("Create", new Type[] { typeof(T) }).MakeGenericMethod(typeof(T));

				created = createMethod.Invoke(this.Client, new object[] { entity }) as T;

				this.entityAuditService.AuditCreateEntity(OutcomeIndicator.Success, created);
			}
			catch (Exception e)
			{
				this.coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, entityAuditService.CreateEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return created;
		}

		/// <summary>
		/// Deactivates an entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the deactivated entity.</returns>
		public T Deactivate(T entity)
		{
			return this.Obsolete(entity);
		}

		/// <summary>
		/// Gets a specific entity with a given key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the entity for the given key.</returns>
		public virtual T Get(Guid key)
		{
			T entity;

			try
			{
				var getMethod = this.Client.GetType().GetRuntimeMethod("Get", new Type[] { typeof(Guid), typeof(Guid?) }).MakeGenericMethod(typeof(T));

				entity = getMethod.Invoke(this.Client, new object[] { key, null }) as T;

				if (entity == null)
				{
					this.entityAuditService.AuditQueryEntity(OutcomeIndicator.SeriousFail, null);
					return null;
				}

				this.entityAuditService.AuditQueryEntity(OutcomeIndicator.Success, new List<T> { entity });
			}
			catch (Exception e)
			{
				this.coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, entityAuditService.QueryEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return entity;
		}

		/// <summary>
		/// Gets a specific entity by id, version id, and filter expression.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="versionKey">The version key.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns the entity for the given key and the given filter expression.</returns>
		public virtual T Get(Guid key, Guid? versionKey, Expression<Func<T, bool>> expression = null)
		{
			Expression<Func<T, bool>> query = o => o.Key == key;

			if (versionKey.HasValue && versionKey.Value != Guid.Empty && expression != null)
			{
				query = o => o.Key == key && o.VersionKey == versionKey && expression.Compile().Invoke(o);
			}
			else if (versionKey.HasValue && versionKey.Value != Guid.Empty)
			{
				query = o => o.Key == key && o.VersionKey == versionKey;
			}

			var bundle = this.Client.Query<T>(query, 0, null, true);

			bundle.Reconstitute();

			var entity = bundle.Item.OfType<T>().Where(query.Compile()).LatestVersionOnly().FirstOrDefault(query.Compile().Invoke);

			if (entity == null)
			{
				this.entityAuditService.AuditQueryEntity(OutcomeIndicator.SeriousFail, null);
				return null;
			}

			if (entity.TypeConceptKey.HasValue && entity.TypeConceptKey != Guid.Empty)
			{
				entity.TypeConcept = this.conceptService.GetConcept(entity.TypeConceptKey.Value);
			}

			this.entityAuditService.AuditQueryEntity(OutcomeIndicator.Success, new List<T> { entity });

			return entity;
		}

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		public abstract IEnumerable<T> Query(Expression<Func<T, bool>> expression);

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="all">if set to <c>true</c> load all the nested properties of the entity.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		public abstract IEnumerable<T> Query(Expression<Func<T, bool>> expression, int offset, int? count, bool all = false);

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="expandProperties">The expand properties.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		public abstract IEnumerable<T> Query(Expression<Func<T, bool>> expression, int offset, int? count, string[] expandProperties);

		/// <summary>
		/// Obsoletes the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the obsoleted entity.</returns>
		public virtual T Obsolete(Guid key)
		{
			return this.Obsolete(this.Get(key));
		}

		/// <summary>
		/// Obsoletes the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the obsoleted entity.</returns>
		public virtual T Obsolete(T entity)
		{
			T obsoleted;

			try
			{
				var obsoleteMethod = this.Client.GetType().GetRuntimeMethod("Obsolete", new Type[] { typeof(T) }).MakeGenericMethod(typeof(T));

				obsoleted = obsoleteMethod.Invoke(this.Client, new object[] { entity }) as T;

				this.entityAuditService.AuditDeleteEntity(OutcomeIndicator.Success, obsoleted);
			}
			catch (Exception e)
			{
				this.coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, entityAuditService.DeleteEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return obsoleted;
		}

		/// <summary>
		/// Updates the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the updated entity.</returns>
		public virtual T Update(T entity)
		{
			// set the creation time
			entity.CreationTime = DateTimeOffset.Now;

			// remove all the relationships where I am the target entity
			entity.Relationships.RemoveAll(r => r.TargetEntityKey == entity.Key);

			// null out the version key
			entity.VersionKey = null;



			T updated;

			try
			{
				var updateMethod = this.Client.GetType().GetRuntimeMethods().First(m => m.Name == "Update" && m.IsGenericMethod).MakeGenericMethod(typeof(T));

				updated = updateMethod.Invoke(this.Client, new object[] { entity }) as T;

				this.entityAuditService.AuditUpdateEntity(OutcomeIndicator.Success, updated);
			}
			catch (Exception e)
			{
				this.coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, entityAuditService.DeleteEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return updated;
		}
	}
}
