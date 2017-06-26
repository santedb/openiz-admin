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
using OpenIZAdmin.Models.Audit;

namespace OpenIZAdmin.Audit
{
	/// <summary>
	/// Represents a security user audit helper.
	/// </summary>
	public class SecurityUserAuditHelper : SecurityEntityAuditHelperBase<SecurityUser>
	{
		/// <summary>
		/// The create security user audit code.
		/// </summary>
		public static readonly AuditCode CreateSecurityUserAuditCode = new AuditCode("SecurityUserCreated", "OpenIZAdminOperations") { DisplayName = "Create" };

		/// <summary>
		/// The delete security user audit code.
		/// </summary>
		public static readonly AuditCode DeleteSecurityUserAuditCode = new AuditCode("SecurityUserDeleted", "OpenIZAdminOperations") { DisplayName = "Delete" };

		/// <summary>
		/// The query security user audit code.
		/// </summary>
		public static readonly AuditCode QuerySecurityUserAuditCode = new AuditCode("SecurityUserQueried", "OpenIZAdminOperations") { DisplayName = "Query" };

		/// <summary>
		/// The update security user audit code.
		/// </summary>
		public static readonly AuditCode UpdateSecurityUserAuditCode = new AuditCode("SecurityUserUpdated", "OpenIZAdminOperations") { DisplayName = "Update" };

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityUserAuditHelper" /> class.
		/// </summary>
		/// <param name="credentials">The credentials.</param>
		/// <param name="context">The context.</param>
		public SecurityUserAuditHelper(Credentials credentials, HttpContext context) : base(credentials, context)
		{
			this.IsRequestSensitive = true;
		}

		/// <summary>
		/// Audits the create security User.
		/// </summary>
		/// <param name="securityUser">The security User.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditCreateSecurityUser(OutcomeIndicator outcomeIndicator, SecurityUser securityUser)
		{
			var audit = base.CreateSecurityResourceCreateAudit(securityUser, CreateSecurityUserAuditCode, outcomeIndicator);

			if (securityUser != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityUser.Key.Value,
					Name = securityUser.UserName,
					securityUser.CreationTime,
					securityUser.Email,
					securityUser.PhoneNumber
				});
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the delete security User.
		/// </summary>
		/// <param name="securityUser">The security User.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditDeleteSecurityUser(OutcomeIndicator outcomeIndicator, SecurityUser securityUser)
		{
			var audit = base.CreateSecurityResourceDeleteAudit(securityUser, DeleteSecurityUserAuditCode, outcomeIndicator);

			if (securityUser != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.LogicalDeletion, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityUser.Key.ToString(),
					Name = securityUser.UserName,
					securityUser.CreationTime,
					securityUser.ObsoletionTime,
					ObsoletedByKey = securityUser.ObsoletedByKey.ToString(),
					securityUser.Email,
					securityUser.PhoneNumber
				});
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the query security User.
		/// </summary>
		/// <param name="securityUsers">The security Users.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditQuerySecurityUser(OutcomeIndicator outcomeIndicator, IEnumerable<SecurityUser> securityUsers)
		{
			var audit = base.CreateSecurityResourceQueryAudit(QuerySecurityUserAuditCode, outcomeIndicator);

			if (securityUsers?.Any() == true)
			{
				base.AddObjectInfoEx(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Disclosure, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, securityUsers.Select(s => new
				{
					Key = s.Key.ToString(),
					Name = s.UserName,
					s.CreationTime,
					s.Email,
					s.PhoneNumber
				}).AsEnumerable());
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the update security User.
		/// </summary>
		/// <param name="securityUser">The security User.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditUpdateSecurityUser(OutcomeIndicator outcomeIndicator, SecurityUser securityUser)
		{
			var audit = this.CreateSecurityResourceUpdateAudit(securityUser, UpdateSecurityUserAuditCode, outcomeIndicator);

			if (securityUser != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityUser.Key.ToString(),
					Name = securityUser.UserName,
					securityUser.CreationTime,
					securityUser.UpdatedTime,
					UpdatedByKey = securityUser.UpdatedByKey.ToString(),
					securityUser.Email,
					securityUser.PhoneNumber
				});
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the user security attributes changed.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="securityUser">The security user.</param>
		public void AuditUserSecurityAttributesChanged(OutcomeIndicator outcomeIndicator, SecurityUser securityUser)
		{
			var audit = this.CreateSecurityResourceUpdateAudit(securityUser, UpdateSecurityUserAuditCode, outcomeIndicator);

			audit.EventIdentifier = EventIdentifierType.SecurityAlert;
			audit.EventTypeCode = CreateAuditCode(EventTypeCode.SecurityAttributesChanged);

			if (securityUser != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityUser.Key.ToString(),
					Name = securityUser.UserName,
					securityUser.CreationTime,
					securityUser.UpdatedTime,
					UpdatedByKey = securityUser.UpdatedByKey.ToString(),
					securityUser.Email,
					securityUser.PhoneNumber
				});
			}
		}
	}
}