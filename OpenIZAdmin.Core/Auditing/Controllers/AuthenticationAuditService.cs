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
using Microsoft.AspNet.Identity;
using OpenIZAdmin.Core.Auditing.Core;
using OpenIZAdmin.Core.Auditing.Model;
using OpenIZAdmin.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace OpenIZAdmin.Core.Auditing.Controllers
{
	/// <summary>
	/// Represents an account controller audit helper.
	/// </summary>
	/// <seealso cref="HttpContextAuditService" />
	public class AuthenticationAuditService : HttpContextAuditService, IAuthenticationAuditService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AuthenticationAuditService"/> class.
		/// </summary>
		public AuthenticationAuditService()
		{
		}

		/// <summary>
		/// Audits the change password.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="identityName">Name of the identity.</param>
		/// <param name="deviceId">The device identifier.</param>
		public void AuditChangePassword(OutcomeIndicator outcomeIndicator, string identityName, string deviceId)
		{
			var audit = CreateBaseAudit(ActionType.Execute, CreateAuditCode(EventTypeCode.UserSecurityChanged), EventIdentifierType.UserAuthentication, outcomeIndicator);

			audit.Actors.Add(new AuditActorData
			{
				NetworkAccessPointId = deviceId,
				NetworkAccessPointType = NetworkAccessPointType.MachineName,
				UserIsRequestor = true,
				UserName = identityName
			});

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the login.
		/// </summary>
		/// <param name="identityName">Name of the identity.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="roles">The roles.</param>
		/// <param name="successfulLogin">if set to <c>true</c> [successful login].</param>
		public void AuditLogin(string identityName, string deviceId, string[] roles = null, bool successfulLogin = true)
		{
			var audit = CreateBaseAudit(ActionType.Execute, CreateAuditCode(EventTypeCode.Login), EventIdentifierType.UserAuthentication, successfulLogin ? OutcomeIndicator.Success : OutcomeIndicator.EpicFail);

			this.IsRequestSensitive = true;

			audit.Actors.Add(new AuditActorData
			{
				ActorRoleCode = roles?.Any() == true ? roles.Select(o => new AuditCode(o, null)).ToList() : new List<AuditCode>(),
				NetworkAccessPointId = deviceId,
				NetworkAccessPointType = NetworkAccessPointType.MachineName,
				UserIsRequestor = true,
				UserName = identityName
			});

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the log off.
		/// </summary>
		/// <param name="principal">The principal.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <exception cref="System.ArgumentNullException">principal</exception>
		public void AuditLogOff(IPrincipal principal, string deviceId)
		{
			if (principal == null)
			{
				throw new ArgumentNullException(nameof(principal), Locale.ValueCannotBeNull);
			}

			var audit = this.CreateBaseAudit(ActionType.Execute, EventTypeCode.Logout, EventIdentifierType.UserAuthentication, OutcomeIndicator.Success);

			audit.Actors.Add(new AuditActorData
			{
				NetworkAccessPointId = deviceId,
				NetworkAccessPointType = NetworkAccessPointType.MachineName,
				UserIsRequestor = true,
				UserName = principal.Identity.GetUserName()
			});

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the reset password.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="identityName">Name of the identity.</param>
		/// <param name="deviceId">The device identifier.</param>
		public void AuditResetPassword(OutcomeIndicator outcomeIndicator, string identityName, string deviceId)
		{
			var audit = CreateBaseAudit(ActionType.Execute, CreateAuditCode(EventTypeCode.UserSecurityChanged), EventIdentifierType.UserAuthentication, outcomeIndicator);

			this.IsRequestSensitive = true;

			audit.Actors.Add(new AuditActorData
			{
				NetworkAccessPointId = deviceId,
				NetworkAccessPointType = NetworkAccessPointType.MachineName,
				UserIsRequestor = true,
				UserName = identityName
			});

			AuditService.SendAudit(audit);
		}
	}
}