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
 * Date: 2017-7-10
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZAdmin.Core.Auditing.Model;

namespace OpenIZAdmin.Core.Auditing.Core
{
	/// <summary>
	/// Represents a global audit service.
	/// </summary>
	public interface IGlobalAuditService
	{
		/// <summary>
		/// Audits the application start.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		void AuditApplicationStart(OutcomeIndicator outcomeIndicator);

		/// <summary>
		/// Audits the application stop.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		void AuditApplicationStop(OutcomeIndicator outcomeIndicator);

		/// <summary>
		/// Audits the forbidden access.
		/// </summary>
		void AuditForbiddenAccess();

		/// <summary>
		/// Audits the resource not found access.
		/// </summary>
		void AuditResourceNotFoundAccess();

		/// <summary>
		/// Audits the unauthorized access.
		/// </summary>
		void AuditUnauthorizedAccess();
	}
}
