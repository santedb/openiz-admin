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
 * Date: 2017-7-10
 */

using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZAdmin.Core.Auditing.Core;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Core.Auditing.Entities
{
	/// <summary>
	/// Represents an entity audit helper.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Core.Auditing.Core.HttpContextAuditService" />
	/// <seealso cref="OpenIZAdmin.Core.Auditing.Entities.IEntityAuditService" />
	public class EntityAuditService : HttpContextAuditService, IEntityAuditService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityAuditService"/> class.
		/// </summary>
		public EntityAuditService()
		{
		}

		/// <summary>
		/// The create entity audit code.
		/// </summary>
		/// <value>The create entity audit code.</value>
		public AuditCode CreateEntityAuditCode => new AuditCode("EntityCreated", "OpenIZAdminOperations") { DisplayName = "Create" };

		/// <summary>
		/// The delete entity audit code.
		/// </summary>
		/// <value>The delete entity audit code.</value>
		public AuditCode DeleteEntityAuditCode => new AuditCode("EntityDeleted", "OpenIZAdminOperations") { DisplayName = "Delete" };

		/// <summary>
		/// The query entity audit code.
		/// </summary>
		/// <value>The query entity audit code.</value>
		public AuditCode QueryEntityAuditCode => new AuditCode("EntityQueried", "OpenIZAdminOperations") { DisplayName = "Query" };

		/// <summary>
		/// The update entity audit code.
		/// </summary>
		/// <value>The update entity audit code.</value>
		public AuditCode UpdateEntityAuditCode => new AuditCode("EntityUpdated", "OpenIZAdminOperations") { DisplayName = "Update" };

		/// <summary>
		/// Audits the creation of an entity.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="entity">The entity.</param>
		public void AuditCreateEntity(OutcomeIndicator outcomeIndicator, Entity entity)
		{
			var audit = base.CreateBaseAudit(ActionType.Create, CreateEntityAuditCode, EventIdentifierType.ApplicationActivity, outcomeIndicator);

			if (entity != null)
			{
				var auditableObjectType = AuditableObjectType.Other;

				if (entity is Organization)
				{
					auditableObjectType = AuditableObjectType.Organization;
				}
				else if (entity is Provider || entity is Person)
				{
					auditableObjectType = AuditableObjectType.Person;
				}

				base.AddObjectInfo(audit, AuditableObjectIdType.Custom, AuditableObjectLifecycle.Creation, AuditableObjectRole.Resource, auditableObjectType, "Key", "Key", true, new
				{
					Key = entity.Key.ToString(),
					entity.CreationTime,
					entity.CreatedByKey,
					entity.ClassConceptKey
				});
			}

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the deletion of an entity.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="entity">The entity.</param>
		public void AuditDeleteEntity(OutcomeIndicator outcomeIndicator, Entity entity)
		{
			var audit = base.CreateBaseAudit(ActionType.Delete, DeleteEntityAuditCode, EventIdentifierType.ApplicationActivity, outcomeIndicator);

			if (entity != null)
			{
				var auditableObjectType = AuditableObjectType.Other;

				if (entity is Organization)
				{
					auditableObjectType = AuditableObjectType.Organization;
				}
				else if (entity is Provider || entity is Person)
				{
					auditableObjectType = AuditableObjectType.Person;
				}

				base.AddObjectInfo(audit, AuditableObjectIdType.Custom, AuditableObjectLifecycle.LogicalDeletion, AuditableObjectRole.Resource, auditableObjectType, "Key", "Key", true, new
				{
					Key = entity.Key.ToString(),
					entity.CreationTime,
					entity.CreatedByKey,
					entity.ClassConceptKey,
					entity.ObsoletedByKey,
					entity.ObsoletionTime
				});
			}

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the query of entities.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="entities">The entities.</param>
		public void AuditQueryEntity(OutcomeIndicator outcomeIndicator, IEnumerable<Entity> entities)
		{
			var audit = base.CreateBaseAudit(ActionType.Read, QueryEntityAuditCode, EventIdentifierType.ApplicationActivity, outcomeIndicator);

			if (entities?.Any() == true)
			{
				base.AddObjectInfoEx(audit, AuditableObjectIdType.Custom, AuditableObjectLifecycle.Disclosure, AuditableObjectRole.Resource, AuditableObjectType.Other, "Key", "Key", true, entities.Select(e => new
				{
					Key = e.Key.ToString(),
					e.CreationTime,
					e.CreatedByKey,
					e.ClassConceptKey
				}));
			}

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the update of an entity.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="entity">The entity.</param>
		public void AuditUpdateEntity(OutcomeIndicator outcomeIndicator, Entity entity)
		{
			var audit = base.CreateBaseAudit(ActionType.Update, UpdateEntityAuditCode, EventIdentifierType.ApplicationActivity, outcomeIndicator);

			if (entity != null)
			{
				var auditableObjectType = AuditableObjectType.Other;

				if (entity is Organization)
				{
					auditableObjectType = AuditableObjectType.Organization;
				}
				else if (entity is Provider || entity is Person)
				{
					auditableObjectType = AuditableObjectType.Person;
				}

				base.AddObjectInfo(audit, AuditableObjectIdType.Custom, AuditableObjectLifecycle.Creation, AuditableObjectRole.Resource, auditableObjectType, "Key", "Key", true, new
				{
					Key = entity.Key.ToString(),
					entity.CreationTime,
					entity.CreatedByKey,
					entity.ClassConceptKey,
					entity.ObsoletedByKey,
					entity.ObsoletionTime
				});
			}

			AuditService.SendAudit(audit);
		}
	}
}