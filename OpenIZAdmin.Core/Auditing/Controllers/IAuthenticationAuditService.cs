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
using System.Security.Principal;

namespace OpenIZAdmin.Core.Auditing.Controllers
{
	/// <summary>
	/// Represents an authentication audit service.
	/// </summary>
	public interface IAuthenticationAuditService
	{
		/// <summary>
		/// Audits the change password.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="identityName">Name of the identity.</param>
		/// <param name="deviceId">The device identifier.</param>
		void AuditChangePassword(OutcomeIndicator outcomeIndicator, string identityName, string deviceId);

		/// <summary>
		/// Audits the login.
		/// </summary>
		/// <param name="identityName">Name of the identity.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="roles">The roles.</param>
		/// <param name="successfulLogin">if set to <c>true</c> [successful login].</param>
		void AuditLogin(string identityName, string deviceId, string[] roles = null, bool successfulLogin = true);

		/// <summary>
		/// Audits the log off.
		/// </summary>
		/// <param name="principal">The principal.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <exception cref="System.ArgumentNullException">principal</exception>
		void AuditLogOff(IPrincipal principal, string deviceId);

		/// <summary>
		/// Audits the reset password.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="identityName">Name of the identity.</param>
		/// <param name="deviceId">The device identifier.</param>
		void AuditResetPassword(OutcomeIndicator outcomeIndicator, string identityName, string deviceId);
	}
}