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
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Core.Auditing.Core;
using OpenIZAdmin.Core.Auditing.Entities;
using OpenIZAdmin.Core.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Services.Metadata.Concepts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OpenIZAdmin.Services.Core
{
	/// <summary>
	/// Represents an entity service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.IEntityService" />
	/// <seealso cref="OpenIZAdmin.Services.Core.ImsiServiceBase" />
	public class EntityService : ImsiServiceBase, IEntityService
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
		/// Initializes a new instance of the <see cref="EntityService" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="conceptService">The concept service.</param>
		/// <param name="coreAuditService">The core audit service.</param>
		/// <param name="entityAuditService">The entity audit service.</param>
		public EntityService(ImsiServiceClient client, IConceptService conceptService, ICoreAuditService coreAuditService, IEntityAuditService entityAuditService) : base(client)
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
		public Entity Activate(Entity entity)
		{
			entity.CreationTime = DateTimeOffset.Now;
			entity.StatusConceptKey = StatusKeys.Active;
			entity.VersionKey = null;

			Entity updated;

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
		public Entity Create(Entity entity)
		{
			// set the creation time
			entity.CreationTime = DateTimeOffset.Now;

			// remove all the relationships where I am the target entity
			entity.Relationships.RemoveAll(r => r.TargetEntityKey == entity.Key);

			// null out the version key
			entity.VersionKey = null;

			Entity created;

			try
			{
				var createMethod = this.Client.GetType().GetRuntimeMethods().First(m => m.Name == "Create" && m.ContainsGenericParameters && m.GetParameters().Count() == 1).MakeGenericMethod(entity.GetType());

				created = createMethod.Invoke(this.Client, new object[] { entity }) as Entity;

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
		public Entity Deactivate(Entity entity)
		{
			return this.Obsolete(entity);
		}

		/// <summary>
		/// Gets a specific entity with a given key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="modelType">Type of the model.</param>
		/// <returns>Returns the entity for the given key.</returns>
		public Entity Get(Guid key, Type modelType)
		{
			var getMethod = this.Client.GetType().GetRuntimeMethod("Get", new Type[] { typeof(Guid), typeof(Guid?) }).MakeGenericMethod(modelType);

			return getMethod.Invoke(this.Client, new object[] { key, null }) as Entity;
		}

		/// <summary>
		/// Gets a specific entity with a given key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the entity for the given key.</returns>
		public T Get<T>(Guid key) where T : Entity
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
		public T Get<T>(Guid key, Guid? versionKey) where T : Entity
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

				if (entity == null)
				{
					this.entityAuditService.AuditQueryEntity(OutcomeIndicator.SeriousFail, null);
					return null;
				}

				this.entityAuditService.AuditQueryEntity(OutcomeIndicator.Success, new List<Entity> { entity });
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
		public T Get<T>(Guid key, Guid? versionKey, Expression<Func<T, bool>> expression) where T : Entity
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

				if (entity == null)
				{
					this.entityAuditService.AuditQueryEntity(OutcomeIndicator.SeriousFail, null);
					return null;
				}

				if (entity.TypeConceptKey.HasValue && entity.TypeConceptKey != Guid.Empty)
				{
					entity.TypeConcept = this.Client.Get<Concept>(entity.TypeConceptKey.Value, null) as Concept;
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
		/// Gets the entity relationships.
		/// </summary>
		/// <typeparam name="TTargetType">The type of the t target type.</typeparam>
		/// <param name="id">The identifier.</param>
		/// <param name="entityRelationshipExpression">The entity relationship expression.</param>
		/// <returns>Returns a list of entity relationships for the given entity.</returns>
		public IEnumerable<EntityRelationship> GetEntityRelationships<TTargetType>(Guid id, Expression<Func<EntityRelationship, bool>> entityRelationshipExpression = null) where TTargetType : Entity
		{
			Bundle bundle;

			if (entityRelationshipExpression != null)
			{
				bundle = this.Client.Query<EntityRelationship>(c => c.SourceEntityKey == id && entityRelationshipExpression.Compile().Invoke(c));
			}
			else
			{
				bundle = this.Client.Query<EntityRelationship>(c => c.SourceEntityKey == id);
			}

			bundle.Reconstitute();

			var entityRelationships = bundle.Item.OfType<EntityRelationship>().ToArray();

			if (entityRelationshipExpression != null)
			{
				entityRelationships = entityRelationships.Where(c => entityRelationshipExpression.Compile().Invoke(c)).ToArray();
			}

			foreach (var entityRelationship in entityRelationships)
			{
				// only load the relationship type concept if the IMS didn't return the relationship type concept of the relationship
				if (entityRelationship.RelationshipType == null && entityRelationship.RelationshipTypeKey.HasValue && entityRelationship.RelationshipTypeKey.Value != Guid.Empty)
				{
					entityRelationship.RelationshipType = this.conceptService.GetConcept(entityRelationship.RelationshipTypeKey.Value, true);
				}

				// only load the target entity if the IMS didn't return the target entity by default
				if (entityRelationship.TargetEntity == null && entityRelationship.TargetEntityKey.HasValue && entityRelationship.TargetEntityKey.Value != Guid.Empty)
				{
					entityRelationship.TargetEntity = this.Client.Get<TTargetType>(entityRelationship.TargetEntityKey.Value, null) as TTargetType;
				}

				// only load the type concept of the target entity if the IMS didn't return the type concept of the target entity
				if (entityRelationship.TargetEntity?.TypeConcept == null && entityRelationship.TargetEntity?.TypeConceptKey.HasValue == true && entityRelationship.TargetEntity?.TypeConceptKey != Guid.Empty)
				{
					entityRelationship.TargetEntity.TypeConcept = this.conceptService.GetConcept(entityRelationship.TargetEntity.TypeConceptKey.Value, true);
				}
			}

			return entityRelationships;
		}

		/// <summary>
		/// Gets the type of the model.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>Returns the type for a given model type.</returns>
		/// <exception cref="System.ArgumentNullException">If the type is null or empty.</exception>
		/// <exception cref="System.ArgumentException">If the model type is not supported.</exception>
		public Type GetModelType(string type)
		{
			if (string.IsNullOrEmpty(type) || string.IsNullOrWhiteSpace(type))
				throw new ArgumentNullException(nameof(type), Locale.ValueCannotBeNull);

			Type modelType;

			switch (type.ToLower())
			{
				case "manufacturedmaterial":
					modelType = typeof(ManufacturedMaterial);
					break;

				case "material":
					modelType = typeof(Material);
					break;

				case "place":
					modelType = typeof(Place);
					break;

				case "organization":
					modelType = typeof(Organization);
					break;

				default:
					throw new ArgumentException($"Unsupported type: { type }");
			}

			return modelType;
		}

		/// <summary>
		/// Obsoletes the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the obsoleted entity.</returns>
		public Entity Obsolete<T>(Guid key) where T : Entity
		{
			return this.Obsolete(this.Get<T>(key));
		}

		/// <summary>
		/// Obsoletes the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the obsoleted entity.</returns>
		public Entity Obsolete(Entity entity)
		{
			Entity obsoleted;

			try
			{
				var obsoleteMethod = this.Client.GetType().GetGenericMethod("Obsolete", new Type[] { entity.GetType() }, new Type[] { entity.GetType() });

				obsoleted = obsoleteMethod.Invoke(this.Client, new object[] { entity }) as Entity;

				this.entityAuditService.AuditDeleteEntity(OutcomeIndicator.Success, obsoleted);
			}
			catch (Exception e)
			{
				Trace.TraceError(e.ToString());
				this.coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, entityAuditService.DeleteEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return obsoleted;
		}

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		public IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression) where T : Entity
		{
			var entities = new List<T>();

			try
			{
				var bundle = this.Client.Query(expression);

				bundle.Reconstitute();

				entities.AddRange(bundle.Item.OfType<T>().LatestVersionOnly().Where(expression.Compile()));

				this.entityAuditService.AuditQueryEntity(OutcomeIndicator.Success, entities);
			}
			catch (Exception e)
			{
				coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, this.entityAuditService.QueryEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
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
		public IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression, int offset, int? count, bool all = false) where T : Entity
		{
			var entities = new List<T>();

			try
			{
				var bundle = this.Client.Query(expression, offset, count, all, null);

				bundle.Reconstitute();

				entities.AddRange(bundle.Item.OfType<T>().Where(expression.Compile()));

				this.entityAuditService.AuditQueryEntity(OutcomeIndicator.Success, entities);
			}
			catch (Exception e)
			{
				coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, this.entityAuditService.QueryEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
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
		public IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression, int offset, int? count, string[] expandProperties) where T : Entity
		{
			var entities = new List<T>();

			try
			{
				var bundle = this.Client.Query(expression, offset, count, expandProperties, null);

				bundle.Reconstitute();

				entities.AddRange(bundle.Item.OfType<T>().Where(expression.Compile()));

				this.entityAuditService.AuditQueryEntity(OutcomeIndicator.Success, entities);
			}
			catch (Exception e)
			{
				coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, this.entityAuditService.QueryEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return entities;
		}

		/// <summary>
		/// Searches for a specific entity by search term.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of entities which match the given search term.</returns>
		public IEnumerable<T> Search<T>(string searchTerm) where T : Entity
		{
			var results = new List<T>();

			try
			{
				Guid classConceptKey;

				if (typeof(T) == typeof(ManufacturedMaterial))
				{
					classConceptKey = EntityClassKeys.ManufacturedMaterial;
				}
				else if (typeof(T) == typeof(Material))
				{
					classConceptKey = EntityClassKeys.Material;
				}
				else if (typeof(T) == typeof(Organization))
				{
					classConceptKey = EntityClassKeys.Organization;
				}
				else if (typeof(T) == typeof(Place))
				{
					classConceptKey = EntityClassKeys.Place;
				}
				else
				{
					throw new ArgumentException($"Unsupported search type: {typeof(T).Name}");
				}

				Bundle bundle;

				Expression<Func<T, bool>> nameExpression = p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));

				if (searchTerm == "*")
				{
					bundle = this.Client.Query<T>(p => p.ClassConceptKey == classConceptKey, 0, null, false);

					foreach (var item in bundle.Item.OfType<T>().LatestVersionOnly().Where(p => p.ClassConceptKey == classConceptKey))
					{
						item.TypeConcept = this.conceptService.GetTypeConcept(item);
					}

					results = bundle.Item.OfType<T>().LatestVersionOnly().Where(p => p.ClassConceptKey == classConceptKey).ToList();
				}
				else
				{
					Guid id;

					if (!Guid.TryParse(searchTerm, out id))
					{
						bundle = this.Client.Query<T>(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.ClassConceptKey == classConceptKey, 0, null, false);

						foreach (var item in bundle.Item.OfType<T>().LatestVersionOnly().Where(p => p.ClassConceptKey == classConceptKey))
						{
							item.TypeConcept = this.conceptService.GetTypeConcept(item);
						}

						results = bundle.Item.OfType<T>().Where(nameExpression.Compile()).LatestVersionOnly().Where(p => p.ClassConceptKey == classConceptKey).ToList();
					}
					else
					{
						var entity = this.Get<T>(id);

						if (entity != null)
						{
							results.Add(entity);
						}
					}
				}

				this.entityAuditService.AuditQueryEntity(OutcomeIndicator.Success, results);
			}
			catch (Exception e)
			{
				coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, this.entityAuditService.QueryEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return results;
		}

		/// <summary>
		/// Updates the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the updated entity.</returns>
		public Entity Update(Entity entity)
		{
			// set the creation time
			entity.CreationTime = DateTimeOffset.Now;

			// remove all the relationships where I am the target entity
			entity.Relationships.RemoveAll(r => r.TargetEntityKey == entity.Key);

			// null out the version key
			entity.VersionKey = null;

			Entity updated;

			try
			{
				var updateMethod = this.Client.GetType().GetRuntimeMethods().First(m => m.Name == "Update" && m.IsGenericMethod).MakeGenericMethod(entity.GetType());

				updated = updateMethod.Invoke(this.Client, new object[] { entity }) as Entity;

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