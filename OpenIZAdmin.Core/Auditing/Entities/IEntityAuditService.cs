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
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;

namespace OpenIZAdmin.Core.Auditing.Entities
{
	/// <summary>
	/// Represents an entity audit service.
	/// </summary>
	public interface IEntityAuditService
	{
		/// <summary>
		/// The create entity audit code.
		/// </summary>
		AuditCode CreateEntityAuditCode { get; }

		/// <summary>
		/// The delete entity audit code.
		/// </summary>
		AuditCode DeleteEntityAuditCode { get; }

		/// <summary>
		/// The query entity audit code.
		/// </summary>
		AuditCode QueryEntityAuditCode { get; }

		/// <summary>
		/// The update entity audit code.
		/// </summary>
		AuditCode UpdateEntityAuditCode { get; }

		/// <summary>
		/// Audits the creation of an entity.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="entity">The entity.</param>
		void AuditCreateEntity(OutcomeIndicator outcomeIndicator, Entity entity);

		/// <summary>
		/// Audits the deletion of an entity.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="entity">The entity.</param>
		void AuditDeleteEntity(OutcomeIndicator outcomeIndicator, Entity entity);

		/// <summary>
		/// Audits the query of entities.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="entities">The entities.</param>
		void AuditQueryEntity(OutcomeIndicator outcomeIndicator, IEnumerable<Entity> entities);

		/// <summary>
		/// Audits the update of an entity.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="entity">The entity.</param>
		void AuditUpdateEntity(OutcomeIndicator outcomeIndicator, Entity entity);
	}
}
