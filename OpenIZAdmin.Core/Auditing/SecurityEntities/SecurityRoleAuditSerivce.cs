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
using OpenIZ.Core.Model.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Core.Auditing.SecurityEntities
{
	/// <summary>
	/// Represents a security role audit helper.
	/// </summary>
	public class SecurityRoleAuditSerivce : SecurityEntityAuditServiceBase<SecurityRole>, ISecurityEntityAuditService<SecurityRole>
	{
		/// <summary>
		/// Gets the create security entity audit code.
		/// </summary>
		/// <value>The create security entity audit code.</value>
		public AuditCode CreateSecurityEntityAuditCode => new AuditCode("SecurityRoleCreated", "OpenIZAdminOperations") { DisplayName = "Create" };

		/// <summary>
		/// Gets the delete security entity audit code.
		/// </summary>
		/// <value>The delete security entity audit code.</value>
		public AuditCode DeleteSecurityEntityAuditCode => new AuditCode("SecurityRoleDeleted", "OpenIZAdminOperations") { DisplayName = "Delete" };

		/// <summary>
		/// Gets the query security entity audit code.
		/// </summary>
		/// <value>The query security entity audit code.</value>
		public AuditCode QuerySecurityEntityAuditCode => new AuditCode("SecurityRoleQueried", "OpenIZAdminOperations") { DisplayName = "Query" };

		/// <summary>
		/// Gets the update security entity audit code.
		/// </summary>
		/// <value>The update security entity audit code.</value>
		public AuditCode UpdateSecurityEntityAuditCode => new AuditCode("SecurityRoleUpdated", "OpenIZAdminOperations") { DisplayName = "Update" };

		/// <summary>
		/// Audits the create security entity.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="securityEntity">The security entity.</param>
		public void AuditCreateSecurityEntity(OutcomeIndicator outcomeIndicator, SecurityRole securityEntity)
		{
			var audit = base.CreateSecurityResourceCreateAudit(securityEntity, this.CreateSecurityEntityAuditCode, outcomeIndicator);

			if (securityEntity != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityEntity.Key.Value,
					securityEntity.CreationTime,
					securityEntity.Name,
					securityEntity.Description
				});
			}

			AuditService.SendAudit(audit);
		}

		public void AuditDeleteSecurityEntity(OutcomeIndicator outcomeIndicator, SecurityRole securityEntity)
		{
			var audit = base.CreateSecurityResourceDeleteAudit(securityEntity, this.DeleteSecurityEntityAuditCode, outcomeIndicator);

			if (securityEntity != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.LogicalDeletion, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityEntity.Key.ToString(),
					securityEntity.CreationTime,
					securityEntity.ObsoletionTime,
					ObsoletedByKey = securityEntity.ObsoletedByKey.ToString(),
					securityEntity.Name,
					securityEntity.Description
				});
			}

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the query security entity.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="securityEntities">The security entities.</param>
		public void AuditQuerySecurityEntity(OutcomeIndicator outcomeIndicator, IEnumerable<SecurityRole> securityEntities)
		{
			var audit = base.CreateSecurityResourceQueryAudit(this.QuerySecurityEntityAuditCode, outcomeIndicator);

			if (securityEntities?.Any() == true)
			{
				base.AddObjectInfoEx(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Disclosure, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, securityEntities.Select(s => new
				{
					Key = s.Key.ToString(),
					s.CreationTime,
					s.Name,
					s.Description
				}).AsEnumerable());
			}

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the update security entity.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="securityEntity">The security entity.</param>
		public void AuditUpdateSecurityEntity(OutcomeIndicator outcomeIndicator, SecurityRole securityEntity)
		{
			var audit = this.CreateSecurityResourceUpdateAudit(securityEntity, this.UpdateSecurityEntityAuditCode, outcomeIndicator);

			if (securityEntity != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityEntity.Key.ToString(),
					securityEntity.CreationTime,
					securityEntity.UpdatedTime,
					UpdatedByKey = securityEntity.UpdatedByKey.ToString(),
					securityEntity.Name,
					securityEntity.Description
				});
			}

			AuditService.SendAudit(audit);
		}
	}
}