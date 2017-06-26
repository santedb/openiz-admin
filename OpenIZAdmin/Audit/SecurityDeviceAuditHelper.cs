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
	/// Represents a security device audit helper.
	/// </summary>
	public class SecurityDeviceAuditHelper : SecurityEntityAuditHelperBase<SecurityDevice>
	{
		/// <summary>
		/// The create security application audit code.
		/// </summary>
		private static readonly AuditCode CreateSecurityDeviceAuditCode = new AuditCode("SecurityDeviceCreated", "OpenIZAdminOperations") { DisplayName = "Create" };

		/// <summary>
		/// The delete security application audit code.
		/// </summary>
		private static readonly AuditCode DeleteSecurityDeviceAuditCode = new AuditCode("SecurityDeviceDeleted", "OpenIZAdminOperations") { DisplayName = "Delete" };

		/// <summary>
		/// The query security application audit code.
		/// </summary>
		private static readonly AuditCode QuerySecurityDeviceAuditCode = new AuditCode("SecurityDeviceQueried", "OpenIZAdminOperations") { DisplayName = "Query" };

		/// <summary>
		/// The update security application audit code.
		/// </summary>
		private static readonly AuditCode UpdateSecurityDeviceAuditCode = new AuditCode("SecurityDeviceUpdated", "OpenIZAdminOperations") { DisplayName = "Update" };

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityDeviceAuditHelper" /> class.
		/// </summary>
		/// <param name="credentials">The credentials.</param>
		/// <param name="context">The context.</param>
		public SecurityDeviceAuditHelper(Credentials credentials, HttpContext context) : base(credentials, context)
		{
		}

		/// <summary>
		/// Audits the create security device.
		/// </summary>
		/// <param name="securityDevice">The security device.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditCreateSecurityDevice(OutcomeIndicator outcomeIndicator, SecurityDevice securityDevice)
		{
			var audit = base.CreateSecurityResourceCreateAudit(securityDevice, CreateSecurityDeviceAuditCode, outcomeIndicator);

			if (securityDevice != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityDevice.Key.Value,
					securityDevice.CreationTime,
					securityDevice.Name
				});
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the delete security device.
		/// </summary>
		/// <param name="securityDevice">The security device.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditDeleteSecurityDevice(OutcomeIndicator outcomeIndicator, SecurityDevice securityDevice)
		{
			var audit = base.CreateSecurityResourceDeleteAudit(securityDevice, DeleteSecurityDeviceAuditCode, outcomeIndicator);

			if (securityDevice != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.LogicalDeletion, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityDevice.Key.ToString(),
					securityDevice.CreationTime,
					securityDevice.ObsoletionTime,
					ObsoletedByKey = securityDevice.ObsoletedByKey.ToString(),
					securityDevice.Name
				});
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the query security device.
		/// </summary>
		/// <param name="securityDevices">The security devices.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditQuerySecurityDevice(OutcomeIndicator outcomeIndicator, IEnumerable<SecurityDevice> securityDevices)
		{
			var audit = base.CreateSecurityResourceQueryAudit(QuerySecurityDeviceAuditCode, outcomeIndicator);

			if (securityDevices?.Any() == true)
			{
				base.AddObjectInfoEx(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Disclosure, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, securityDevices.Select(s => new
				{
					Key = s.Key.ToString(),
					s.CreationTime,
					s.Name
				}).AsEnumerable());
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the update security device.
		/// </summary>
		/// <param name="securityDevice">The security device.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditUpdateSecurityDevice(OutcomeIndicator outcomeIndicator, SecurityDevice securityDevice)
		{
			var audit = this.CreateSecurityResourceUpdateAudit(securityDevice, UpdateSecurityDeviceAuditCode, outcomeIndicator);

			if (securityDevice != null)
			{
				base.AddObjectInfo(audit, AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, AuditableObjectRole.SecurityResource, AuditableObjectType.Other, "Key", "Name", true, new
				{
					Key = securityDevice.Key.ToString(),
					securityDevice.CreationTime,
					securityDevice.UpdatedTime,
					UpdatedByKey = securityDevice.UpdatedByKey.ToString(),
					securityDevice.Name
				});
			}

			this.SendAudit(audit);
		}
	}
}