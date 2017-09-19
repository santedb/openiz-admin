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
 * User: nitya
 * Date: 2017-9-4
 */

using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Core.Auditing.Core;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Core.Auditing.SecurityEntities
{
	/// <summary>
	/// Represents a security policy audit service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Core.Auditing.Core.HttpContextAuditService" />
	public class SecurityPolicyAuditService : HttpContextAuditService, ISecurityPolicyAuditService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityPolicyAuditService" /> class.
		/// </summary>
		public SecurityPolicyAuditService()
		{
		}

		/// <summary>
		/// Gets the create security policy audit code.
		/// </summary>
		/// <value>The create security policy audit code.</value>
		public AuditCode CreateSecurityPolicyAuditCode => new AuditCode("SecurityPolicyCreated", "OpenIZAdminOperations") { DisplayName = "Create" };

		/// <summary>
		/// Gets the query security policy audit code.
		/// </summary>
		/// <value>The query security policy audit code.</value>
		public AuditCode QuerySecurityPolicyAuditCode => new AuditCode("SecurityPolicyQueried", "OpenIZAdminOperations") { DisplayName = "Query" };

		/// <summary>
		/// Audits the create policy.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="securityPolicy">The security policy.</param>
		public void AuditCreatePolicy(OutcomeIndicator outcomeIndicator, SecurityPolicy securityPolicy)
		{
			var audit = this.CreateBaseAudit(ActionType.Create, this.CreateSecurityPolicyAuditCode, EventIdentifierType.ApplicationActivity, outcomeIndicator);

			if (securityPolicy != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.Custom, AuditableObjectLifecycle.Access, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityPolicy.Key.Value,
					securityPolicy.CreationTime,
					securityPolicy.Name,
					securityPolicy.Oid
				});
			}

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the query security policy action.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="policies">The policies.</param>
		public void AuditQueryPolicies(OutcomeIndicator outcomeIndicator, IEnumerable<SecurityPolicy> policies)
		{
			var audit = base.CreateBaseAudit(ActionType.Read, this.QuerySecurityPolicyAuditCode, EventIdentifierType.ApplicationActivity, outcomeIndicator);

			if (policies?.Any() == true)
			{
				base.AddObjectInfoEx(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Disclosure, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, policies.Select(s => new
				{
					Key = s.Key.ToString(),
					s.CreationTime,
					s.Name,
					s.Oid
				}).AsEnumerable());
			}

			AuditService.SendAudit(audit);
		}
	}
}