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
 * Date: 2017-6-25
 */
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Http;
using OpenIZ.Core.Model.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Audit
{
	/// <summary>
	/// Represents a security role audit helper.
	/// </summary>
	public class SecurityRoleAuditHelper : SecurityEntityAuditHelperBase<SecurityRole>
	{
		/// <summary>
		/// The create security role audit code.
		/// </summary>
		public static readonly AuditCode CreateSecurityRoleAuditCode = new AuditCode("SecurityRoleCreated", "OpenIZAdminOperations") { DisplayName = "Create" };

		/// <summary>
		/// The delete security role audit code.
		/// </summary>
		public static readonly AuditCode DeleteSecurityRoleAuditCode = new AuditCode("SecurityRoleDeleted", "OpenIZAdminOperations") { DisplayName = "Delete" };

		/// <summary>
		/// The query security role audit code.
		/// </summary>
		public static readonly AuditCode QuerySecurityRoleAuditCode = new AuditCode("SecurityRoleQueried", "OpenIZAdminOperations") { DisplayName = "Query" };

		/// <summary>
		/// The update security role audit code.
		/// </summary>
		public static readonly AuditCode UpdateSecurityRoleAuditCode = new AuditCode("SecurityRoleUpdated", "OpenIZAdminOperations") { DisplayName = "Update" };

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityRoleAuditHelper" /> class.
		/// </summary>
		/// <param name="credentials">The credentials.</param>
		/// <param name="context">The context.</param>
		public SecurityRoleAuditHelper(Credentials credentials, HttpContext context) : base(credentials, context)
		{
		}

		/// <summary>
		/// Audits the create security Role.
		/// </summary>
		/// <param name="securityRole">The security Role.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditCreateSecurityRole(OutcomeIndicator outcomeIndicator, SecurityRole securityRole)
		{
			var audit = base.CreateSecurityResourceCreateAudit(securityRole, CreateSecurityRoleAuditCode, outcomeIndicator);

			if (securityRole != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityRole.Key.Value,
					securityRole.CreationTime,
					securityRole.Name,
					securityRole.Description
				});
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the delete security Role.
		/// </summary>
		/// <param name="securityRole">The security Role.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditDeleteSecurityRole(OutcomeIndicator outcomeIndicator, SecurityRole securityRole)
		{
			var audit = base.CreateSecurityResourceDeleteAudit(securityRole, DeleteSecurityRoleAuditCode, outcomeIndicator);

			if (securityRole != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.LogicalDeletion, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityRole.Key.ToString(),
					securityRole.CreationTime,
					securityRole.ObsoletionTime,
					ObsoletedByKey = securityRole.ObsoletedByKey.ToString(),
					securityRole.Name,
					securityRole.Description
				});
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the query security Role.
		/// </summary>
		/// <param name="securityRoles">The security Roles.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditQuerySecurityRole(OutcomeIndicator outcomeIndicator, IEnumerable<SecurityRole> securityRoles)
		{
			var audit = base.CreateSecurityResourceQueryAudit(QuerySecurityRoleAuditCode, outcomeIndicator);

			if (securityRoles?.Any() == true)
			{
				base.AddObjectInfoEx(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Disclosure, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, securityRoles.Select(s => new
				{
					Key = s.Key.ToString(),
					s.CreationTime,
					s.Name,
					s.Description
				}).AsEnumerable());
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the update security Role.
		/// </summary>
		/// <param name="securityRole">The security Role.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditUpdateSecurityRole(OutcomeIndicator outcomeIndicator, SecurityRole securityRole)
		{
			var audit = this.CreateSecurityResourceUpdateAudit(securityRole, UpdateSecurityRoleAuditCode, outcomeIndicator);

			if (securityRole != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityRole.Key.ToString(),
					securityRole.CreationTime,
					securityRole.UpdatedTime,
					UpdatedByKey = securityRole.UpdatedByKey.ToString(),
					securityRole.Name,
					securityRole.Description
				});
			}

			this.SendAudit(audit);
		}
	}
}