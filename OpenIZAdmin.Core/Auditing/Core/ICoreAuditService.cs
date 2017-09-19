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
using System;

namespace OpenIZAdmin.Core.Auditing.Core
{
	/// <summary>
	/// Represents a core audit service.
	/// </summary>
	public interface ICoreAuditService
	{
		/// <summary>
		/// Audits the generic error.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <param name="exception">The exception.</param>
		void AuditGenericError(OutcomeIndicator outcomeIndicator, AuditCode eventTypeCode, EventIdentifierType eventIdentifierType, Exception exception);

		/// <summary>
		/// Audits the generic error.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <param name="exception">The exception.</param>
		void AuditGenericError(OutcomeIndicator outcomeIndicator, EventTypeCode eventTypeCode, EventIdentifierType eventIdentifierType, Exception exception);
	}
}