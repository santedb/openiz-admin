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

using MARC.HI.EHRS.SVC.Auditing.Data;
using Microsoft.AspNet.Identity;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Core.Auditing.Core;
using OpenIZAdmin.Core.Auditing.Entities;
using OpenIZAdmin.Core.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Services.Core;
using OpenIZAdmin.Services.Metadata.Concepts;
using OpenIZAdmin.Services.Security.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace OpenIZAdmin.Services.Acts
{
	/// <summary>
	/// Represents an entity service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.ImsiServiceBase" />
	public class ActService : ImsiServiceBase, IActService
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
        /// User Service
        /// </summary>
        private readonly ISecurityUserService userService;

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityService" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="conceptService">The concept service.</param>
		/// <param name="coreAuditService">The core audit service.</param>
		/// <param name="entityAuditService">The entity audit service.</param>
		public ActService(ImsiServiceClient client, IConceptService conceptService, ICoreAuditService coreAuditService, ISecurityUserService userService) : base(client)
		{
			this.conceptService = conceptService;
			this.coreAuditService = coreAuditService;
            this.userService = userService;
		}

		/// <summary>
		/// Gets a specific entity with a given key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="modelType">Type of the model.</param>
		/// <returns>Returns the entity for the given key.</returns>
		public Act Get(Guid key, Type modelType)
		{
			var getMethod = this.Client.GetType().GetRuntimeMethod("Get", new Type[] { typeof(Guid), typeof(Guid?) }).MakeGenericMethod(modelType);

			return getMethod.Invoke(this.Client, new object[] { key, null }) as Act;
		}

		/// <summary>
		/// Gets a specific entity with a given key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the entity for the given key.</returns>
		public T Get<T>(Guid key) where T : Act
		{
			return this.Get<T>(key, null);
		}

		/// <summary>
		/// Gets a specific entity with a given key and version key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="versionKey">The version key.</param>
		/// <returns>Returns the entity for the given key and version key.</returns>
		public T Get<T>(Guid key, Guid? versionKey) where T : Act
        {
			T entity;

			try
			{
				var getMethod = this.Client.GetType().GetRuntimeMethods().First(m => m.Name == "Get" && m.GetParameters().Count() == 2).MakeGenericMethod(typeof(T));
				if (versionKey.HasValue && versionKey.Value != Guid.Empty)
				{
					entity = getMethod.Invoke(this.Client, new object[] { key, versionKey }) as T;
				}
				else
				{
					entity = getMethod.Invoke(this.Client, new object[] { key, null }) as T;
				}

                entity.CreatedBy = this.userService.GetSecurityUser(entity.CreatedByKey.Value).User;
                
            }
			catch (Exception e)
			{
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
		public T Get<T>(Guid key, Guid? versionKey, Expression<Func<T, bool>> expression) where T : Act
		{
			T entity;

			try
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

				entity = bundle.Item.OfType<T>().Where(query.Compile()).LatestVersionOnly().FirstOrDefault(query.Compile().Invoke);
				
                if (entity.TypeConceptKey.HasValue && entity.TypeConceptKey != Guid.Empty)
				{
					entity.TypeConcept = this.Client.Get<Concept>(entity.TypeConceptKey.Value, null) as Concept;
				}

			}
			catch (Exception e)
			{
				throw;
			}

			return entity;
		}

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		public IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression) where T : Act
		{
			var entities = new List<T>();

			try
			{
				var bundle = this.Client.Query(expression);

				bundle.Reconstitute();

				entities.AddRange(bundle.Item.OfType<T>().LatestVersionOnly().Where(expression.Compile()).LatestVersionOnly());

			}
			catch (Exception e)
			{
				throw;
			}

			return entities;
		}

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="all">if set to <c>true</c> load all the nested properties of the entity.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		public IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression, int offset, int? count, bool all = false) where T : Act
		{
			var entities = new List<T>();

			try
			{
				var bundle = this.Client.Query(expression, offset, count, all, null);

				bundle.Reconstitute();

				entities.AddRange(bundle.Item.OfType<T>().LatestVersionOnly().Where(expression.Compile()));

			}
			catch (Exception e)
			{
				throw;
			}

			return entities;
		}

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="expandProperties">The expand properties.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		public IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression, int offset, int? count, string[] expandProperties) where T : Act
		{
			var entities = new List<T>();

			try
			{
				var bundle = this.Client.Query(expression, offset, count, expandProperties, null);

				bundle.Reconstitute();

				entities.AddRange(bundle.Item.OfType<T>().LatestVersionOnly().Where(expression.Compile()));

			}
			catch (Exception e)
			{
				throw;
			}

			return entities;
		}

        /// <summary>
        /// Query for the specified entities using the server's stored query (faster for paging)
        /// </summary>
        public IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression, int offset, int? count, Guid queryId, string[] expandProperties) where T : Act
        {
            var entities = new List<T>();

            try
            {
                var bundle = this.Client.Query(expression, offset, count, expandProperties, queryId);

                bundle.Reconstitute();

                entities.AddRange(bundle.Item.OfType<T>().LatestVersionOnly().Where(expression.Compile()));
            }
            catch (Exception e)
            {
                throw;
            }

            return entities;
        }
    
	}
}