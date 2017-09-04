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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Model.Security;

namespace OpenIZAdmin.Core.Auditing.SecurityEntities
{
	/// <summary>
	/// Represents a security policy audit service.
	/// </summary>
	public interface ISecurityPolicyAuditService
	{
		/// <summary>
		/// Gets the create security policy audit code.
		/// </summary>
		/// <value>The create security policy audit code.</value>
		AuditCode CreateSecurityPolicyAuditCode { get; }

		/// <summary>
		/// Gets the query security policy audit code.
		/// </summary>
		/// <value>The query security policy audit code.</value>
		AuditCode QuerySecurityPolicyAuditCode { get; }

		/// <summary>
		/// Audits the create policy.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="securityPolicy">The security policy.</param>
		void AuditCreatePolicy(OutcomeIndicator outcomeIndicator, SecurityPolicy securityPolicy);

		/// <summary>
		/// Audits the query security policy action.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="policies">The policies.</param>
		void AuditQueryPolicies(OutcomeIndicator outcomeIndicator, IEnumerable<SecurityPolicy> policies);
	}
}
