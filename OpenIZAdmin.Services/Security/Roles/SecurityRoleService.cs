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
 * Date: 2017-8-3
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Core.Auditing.Core;
using OpenIZAdmin.Core.Auditing.Model;
using OpenIZAdmin.Core.Auditing.SecurityEntities;
using OpenIZAdmin.Services.Core;

namespace OpenIZAdmin.Services.Security.Roles
{
	/// <summary>
	/// Represents a security role service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.AmiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.Security.Roles.ISecurityRoleService" />
	public class SecurityRoleService : AmiServiceBase, ISecurityRoleService
	{
		/// <summary>
		/// The core audit service.
		/// </summary>
		private readonly ICoreAuditService coreAuditService;

		/// <summary>
		/// The security entity audit service.
		/// </summary>
		private readonly ISecurityEntityAuditService<SecurityRole> securityEntityAuditService;

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityRoleService" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="coreAuditService">The core audit service.</param>
		/// <param name="securityEntityAuditService">The security entity audit service.</param>
		public SecurityRoleService(AmiServiceClient client, ICoreAuditService coreAuditService, ISecurityEntityAuditService<SecurityRole> securityEntityAuditService) : base(client)
		{
			this.coreAuditService = coreAuditService;
			this.securityEntityAuditService = securityEntityAuditService;
		}

		/// <summary>
		/// Gets all roles.
		/// </summary>
		/// <returns>Returns a list of all roles in the system.</returns>
		public IEnumerable<SecurityRoleInfo> GetAllRoles()
		{
			IEnumerable<SecurityRoleInfo> roles;

			try
			{
				roles = this.Client.GetRoles(r => r.ObsoletionTime == null).CollectionItem;
				this.securityEntityAuditService.AuditQuerySecurityEntity(OutcomeIndicator.Success, roles.Select(r => r.Role));
			}
			catch (Exception e)
			{
				this.coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, securityEntityAuditService.QuerySecurityEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return roles;
		}
	}
}
