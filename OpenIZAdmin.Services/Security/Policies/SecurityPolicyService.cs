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
 * Date: 2017-9-4
 */

using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Core.Auditing.Core;
using OpenIZAdmin.Core.Auditing.SecurityEntities;
using OpenIZAdmin.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Services.Security.Policies
{
	/// <summary>
	/// Represents a security policy service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.AmiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.Security.Policies.ISecurityPolicyService" />
	public class SecurityPolicyService : AmiServiceBase, ISecurityPolicyService
	{
		/// <summary>
		/// The core audit service.
		/// </summary>
		private readonly ICoreAuditService coreAuditService;

		/// <summary>
		/// The security policy audit service.
		/// </summary>
		private readonly ISecurityPolicyAuditService securityPolicyAuditService;

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityPolicyService" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="coreAuditService">The core audit service.</param>
		/// <param name="securityPolicyAuditService">The security policy audit service.</param>
		public SecurityPolicyService(AmiServiceClient client, ICoreAuditService coreAuditService, ISecurityPolicyAuditService securityPolicyAuditService) : base(client)
		{
			this.coreAuditService = coreAuditService;
			this.securityPolicyAuditService = securityPolicyAuditService;
		}

		/// <summary>
		/// Creates the security policy.
		/// </summary>
		/// <param name="securityPolicyInfo">The security policy information.</param>
		/// <returns>Returns the created security policy.</returns>
		public SecurityPolicyInfo CreateSecurityPolicy(SecurityPolicyInfo securityPolicyInfo)
		{
			SecurityPolicyInfo createdPolicyInfo;

			try
			{
				createdPolicyInfo = this.Client.CreatePolicy(securityPolicyInfo);

				if (createdPolicyInfo != null)
				{
					this.securityPolicyAuditService.AuditCreatePolicy(OutcomeIndicator.Success, createdPolicyInfo.Policy);
				}
				else
				{
					this.securityPolicyAuditService.AuditCreatePolicy(OutcomeIndicator.SeriousFail, null);
				}
			}
			catch (Exception e)
			{
				this.coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, this.securityPolicyAuditService.CreateSecurityPolicyAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return createdPolicyInfo;
		}

		/// <summary>
		/// Gets all policies.
		/// </summary>
		/// <returns>Returns a list of security policies.</returns>
		public IEnumerable<SecurityPolicyInfo> GetAllPolicies()
		{
			var policies = new List<SecurityPolicyInfo>();

			try
			{
				policies.AddRange(this.Client.GetPolicies(p => p.ObsoletionTime == null).CollectionItem);
				securityPolicyAuditService.AuditQueryPolicies(OutcomeIndicator.Success, policies.Select(p => p.Policy));
			}
			catch (Exception e)
			{
				this.coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, securityPolicyAuditService.QuerySecurityPolicyAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return policies;
		}
	}
}