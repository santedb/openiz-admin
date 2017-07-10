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
	/// Represents a security entity audit service.
	/// </summary>
	public interface ISecurityEntityAuditService<in T> where T : SecurityEntity
	{

		/// <summary>
		/// Gets the create security entity audit code.
		/// </summary>
		/// <value>The create security entity audit code.</value>
		AuditCode CreateSecurityEntityAuditCode { get; }

		/// <summary>
		/// Gets the delete security entity audit code.
		/// </summary>
		/// <value>The delete security entity audit code.</value>
		AuditCode DeleteSecurityEntityAuditCode { get; }

		/// <summary>
		/// Gets the query security entity audit code.
		/// </summary>
		/// <value>The query security entity audit code.</value>
		AuditCode QuerySecurityEntityAuditCode { get; }

		/// <summary>
		/// Gets the update security entity audit code.
		/// </summary>
		/// <value>The update security entity audit code.</value>
		AuditCode UpdateSecurityEntityAuditCode { get; }

		/// <summary>
		/// Audits the create security entity.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="securityEntity">The security entity.</param>
		void AuditCreateSecurityEntity(OutcomeIndicator outcomeIndicator, T securityEntity);

		/// <summary>
		/// Audits the delete security entity.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="securityEntity">The security entity.</param>
		void AuditDeleteSecurityEntity(OutcomeIndicator outcomeIndicator, T securityEntity);

		/// <summary>
		/// Audits the query security entity.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="securityEntities">The security entities.</param>
		void AuditQuerySecurityEntity(OutcomeIndicator outcomeIndicator, IEnumerable<T> securityEntities);

		/// <summary>
		/// Audits the update security entity.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="securityEntity">The security entity.</param>
		void AuditUpdateSecurityEntity(OutcomeIndicator outcomeIndicator, T securityEntity);
	}
}
