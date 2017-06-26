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
using System.Web;

namespace OpenIZAdmin.Audit
{
	/// <summary>
	/// Represents a security entity audit helper.
	/// </summary>
	/// <typeparam name="T">The type of security entity.</typeparam>
	/// <seealso cref="OpenIZAdmin.Audit.HttpContextAuditHelperBase" />
	public abstract class SecurityEntityAuditHelperBase<T> : HttpContextAuditHelperBase where T : SecurityEntity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityEntityAuditHelperBase{T}" /> class.
		/// </summary>
		/// <param name="credentials">The credentials.</param>
		/// <param name="context">The context.</param>
		protected SecurityEntityAuditHelperBase(Credentials credentials, HttpContext context) : base(credentials, context)
		{
		}

		/// <summary>
		/// Creates the security resource create audit.
		/// </summary>
		/// <param name="securityEntity">The security entity.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <returns>Returns the created audit.</returns>
		protected virtual AuditData CreateSecurityResourceCreateAudit(T securityEntity, AuditCode eventTypeCode, OutcomeIndicator outcomeIndicator)
		{
			var audit = this.CreateBaseAudit(ActionType.Create, eventTypeCode, EventIdentifierType.ApplicationActivity, outcomeIndicator);

			if (typeof(T) == typeof(SecurityUser))
			{
				audit.AuditableObjects.Add(this.CreateBaseAuditableObject(AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, securityEntity.Key.ToString(), AuditableObjectRole.SecurityUser, AuditableObjectType.Person));
			}
			else if (typeof(T) == typeof(SecurityRole))
			{
				audit.AuditableObjects.Add(this.CreateBaseAuditableObject(AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, securityEntity.Key.ToString(), AuditableObjectRole.SecurityGroup, AuditableObjectType.Other));
			}
			else
			{
				audit.AuditableObjects.Add(this.CreateBaseAuditableObject(AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, securityEntity.Key.ToString(), AuditableObjectRole.SecurityResource, AuditableObjectType.Other));
			}

			return audit;
		}

		/// <summary>
		/// Creates the security resource delete audit.
		/// </summary>
		/// <param name="securityEntity">The security entity.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <returns>Returns the created audit.</returns>
		protected virtual AuditData CreateSecurityResourceDeleteAudit(T securityEntity, AuditCode eventTypeCode, OutcomeIndicator outcomeIndicator)
		{
			var audit = this.CreateBaseAudit(ActionType.Delete, eventTypeCode, EventIdentifierType.ApplicationActivity, outcomeIndicator);

			if (typeof(T) == typeof(SecurityUser))
			{
				audit.AuditableObjects.Add(this.CreateBaseAuditableObject(AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.LogicalDeletion, securityEntity.Key.ToString(), AuditableObjectRole.SecurityUser, AuditableObjectType.Person));
			}
			else if (typeof(T) == typeof(SecurityRole))
			{
				audit.AuditableObjects.Add(this.CreateBaseAuditableObject(AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.LogicalDeletion, securityEntity.Key.ToString(), AuditableObjectRole.SecurityGroup, AuditableObjectType.Other));
			}
			else
			{
				audit.AuditableObjects.Add(this.CreateBaseAuditableObject(AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.LogicalDeletion, securityEntity.Key.ToString(), AuditableObjectRole.SecurityResource, AuditableObjectType.Other));
			}

			return audit;
		}

		/// <summary>
		/// Creates the security resource query audit.
		/// </summary>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <returns>Returns the created audit.</returns>
		protected virtual AuditData CreateSecurityResourceQueryAudit(AuditCode eventTypeCode, OutcomeIndicator outcomeIndicator)
		{
			return this.CreateBaseAudit(ActionType.Read, eventTypeCode, EventIdentifierType.ApplicationActivity, outcomeIndicator);
		}

		/// <summary>
		/// Creates the security resource update audit.
		/// </summary>
		/// <param name="securityEntity">The security entity.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <returns>Returns the created audit.</returns>
		protected virtual AuditData CreateSecurityResourceUpdateAudit(T securityEntity, AuditCode eventTypeCode, OutcomeIndicator outcomeIndicator)
		{
			var audit = this.CreateBaseAudit(ActionType.Delete, eventTypeCode, EventIdentifierType.ApplicationActivity, outcomeIndicator);

			if (typeof(T) == typeof(SecurityUser))
			{
				audit.AuditableObjects.Add(this.CreateBaseAuditableObject(AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, securityEntity.Key.ToString(), AuditableObjectRole.SecurityUser, AuditableObjectType.Person));
			}
			else if (typeof(T) == typeof(SecurityRole))
			{
				audit.AuditableObjects.Add(this.CreateBaseAuditableObject(AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, securityEntity.Key.ToString(), AuditableObjectRole.SecurityGroup, AuditableObjectType.Other));
			}
			else
			{
				audit.AuditableObjects.Add(this.CreateBaseAuditableObject(AuditableObjectIdType.UserIdentifier, AuditableObjectLifecycle.Creation, securityEntity.Key.ToString(), AuditableObjectRole.SecurityResource, AuditableObjectType.Other));
			}

			return audit;
		}
	}
}