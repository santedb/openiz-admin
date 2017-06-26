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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Http;
using OpenIZ.Core.Model.Security;

namespace OpenIZAdmin.Audit
{
	/// <summary>
	/// Represents a security application audit helper.
	/// </summary>
	public class SecurityApplicationAuditHelper: SecurityEntityAuditHelper<SecurityApplication>
	{
		/// <summary>
		/// The create security application audit code.
		/// </summary>
		public static readonly AuditCode CreateSecurityApplicationAuditCode = new AuditCode("SecurityApplicationCreated", "OpenIZAdminOperations") { DisplayName = "Create" };

		/// <summary>
		/// The delete security application audit code.
		/// </summary>
		public static readonly AuditCode DeleteSecurityApplicationAuditCode = new AuditCode("SecurityApplicationDeleted", "OpenIZAdminOperations") { DisplayName = "Delete" };

		/// <summary>
		/// The query security application audit code.
		/// </summary>
		public static readonly AuditCode QuerySecurityApplicationAuditCode = new AuditCode("SecurityApplicationQueried", "OpenIZAdminOperations") { DisplayName = "Query" };

		/// <summary>
		/// The update security application audit code.
		/// </summary>
		public static readonly AuditCode UpdateSecurityApplicationAuditCode = new AuditCode("SecurityApplicationUpdated", "OpenIZAdminOperations") { DisplayName = "Update" };

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityApplicationAuditHelper" /> class.
		/// </summary>
		/// <param name="credentials">The credentials.</param>
		/// <param name="context">The context.</param>
		public SecurityApplicationAuditHelper(Credentials credentials, HttpContext context) : base(credentials, context)
		{
		}

		/// <summary>
		/// Audits the create security application.
		/// </summary>
		/// <param name="securityApplication">The security application.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditCreateSecurityApplication(OutcomeIndicator outcomeIndicator, SecurityApplication securityApplication)
		{
			var audit = base.CreateSecurityResourceCreateAudit(securityApplication, CreateSecurityApplicationAuditCode, outcomeIndicator);

			if (securityApplication != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityApplication.Key.Value,
					securityApplication.CreationTime,
					securityApplication.Name
				});
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the delete security application.
		/// </summary>
		/// <param name="securityApplication">The security application.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditDeleteSecurityApplication(OutcomeIndicator outcomeIndicator, SecurityApplication securityApplication)
		{
			var audit = base.CreateSecurityResourceDeleteAudit(securityApplication, DeleteSecurityApplicationAuditCode, outcomeIndicator);

			if (securityApplication != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.LogicalDeletion, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityApplication.Key.ToString(),
					securityApplication.CreationTime,
					securityApplication.ObsoletionTime,
					ObsoletedByKey = securityApplication.ObsoletedByKey.ToString(),
					securityApplication.Name
				});
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the query security application.
		/// </summary>
		/// <param name="securityApplications">The security applications.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditQuerySecurityApplication(OutcomeIndicator outcomeIndicator, IEnumerable<SecurityApplication> securityApplications)
		{
			var audit = base.CreateSecurityResourceQueryAudit(QuerySecurityApplicationAuditCode, outcomeIndicator);

			if (securityApplications?.Any() == true)
			{
				base.AddObjectInfoEx(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Disclosure, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, securityApplications.Select(s => new
				{
					Key = s.Key.ToString(),
					s.CreationTime,
					s.Name
				}).AsEnumerable());
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the update security application.
		/// </summary>
		/// <param name="securityApplication">The security application.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditUpdateSecurityApplication(OutcomeIndicator outcomeIndicator, SecurityApplication securityApplication)
		{
			var audit = this.CreateSecurityResourceUpdateAudit(securityApplication, UpdateSecurityApplicationAuditCode, outcomeIndicator);

			if (securityApplication != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityApplication.Key.ToString(),
					securityApplication.CreationTime,
					securityApplication.UpdatedTime,
					UpdatedByKey = securityApplication.UpdatedByKey.ToString(),
					securityApplication.Name
				});
			}

			this.SendAudit(audit);
		}
	}
}