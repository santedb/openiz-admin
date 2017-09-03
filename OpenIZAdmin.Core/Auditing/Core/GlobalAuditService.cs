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

using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZAdmin.Core.Auditing.Model;
using System.Web;

namespace OpenIZAdmin.Core.Auditing.Core
{
	/// <summary>
	/// Represents a global audit helper.
	/// </summary>
	/// <seealso cref="HttpContextAuditService" />
	public class GlobalAuditService : HttpContextAuditService, IGlobalAuditService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GlobalAuditService"/> class.
		/// </summary>
		public GlobalAuditService()
		{
		}

		/// <summary>
		/// Audits the application start.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditApplicationStart(OutcomeIndicator outcomeIndicator)
		{
			var audit = this.CreateBaseAudit(ActionType.Execute, EventTypeCode.ApplicationStart, EventIdentifierType.ApplicationActivity, outcomeIndicator);

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the application stop.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		public void AuditApplicationStop(OutcomeIndicator outcomeIndicator)
		{
			var audit = this.CreateBaseAudit(ActionType.Execute, EventTypeCode.ApplicationStart, EventIdentifierType.ApplicationActivity, outcomeIndicator);

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the forbidden access.
		/// </summary>
		public void AuditForbiddenAccess()
		{
			var audit = this.CreateBaseAudit(ActionType.Execute, EventTypeCode.ApplicationActivity, EventIdentifierType.UseOfRestrictedFunction, OutcomeIndicator.EpicFail);

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the resource not found access.
		/// </summary>
		public void AuditResourceNotFoundAccess()
		{
			var audit = this.CreateBaseAudit(ActionType.Execute, CreateAuditCode(EventTypeCode.ApplicationActivity), EventIdentifierType.SecurityAlert, OutcomeIndicator.EpicFail);

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the unauthorized access.
		/// </summary>
		public void AuditUnauthorizedAccess()
		{
			var audit = this.CreateBaseAudit(ActionType.Execute, CreateAuditCode(EventTypeCode.ApplicationActivity), EventIdentifierType.UserAuthentication, OutcomeIndicator.EpicFail);

			AuditService.SendAudit(audit);
		}
	}
}