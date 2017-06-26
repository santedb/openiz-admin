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
using Microsoft.AspNet.Identity;
using OpenIZ.Core.Http;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace OpenIZAdmin.Audit
{
	/// <summary>
	/// Represents an account controller audit helper.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Audit.HttpContextAuditHelperBase" />
	public class AccountControllerAuditHelper : HttpContextAuditHelperBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccountControllerAuditHelper"/> class.
		/// </summary>
		/// <param name="credentials">The credentials.</param>
		/// <param name="context">The context.</param>
		public AccountControllerAuditHelper(Credentials credentials, HttpContext context) : base(credentials, context)
		{
		}

		/// <summary>
		/// Audits the login.
		/// </summary>
		/// <param name="identityName">Name of the identity.</param>
		/// <param name="roles">The roles.</param>
		/// <param name="successfulLogin">if set to <c>true</c> [successful login].</param>
		public void AuditLogin(string identityName, string[] roles = null, bool successfulLogin = true)
		{
			var audit = CreateBaseAudit(ActionType.Execute, CreateAuditCode(EventTypeCode.Login), EventIdentifierType.UserAuthentication, successfulLogin ? OutcomeIndicator.Success : OutcomeIndicator.EpicFail);

			audit.Actors.Add(new AuditActorData
			{
				ActorRoleCode = roles?.Any() == true ? roles.Select(o => new AuditCode(o, null)).ToList() : new List<AuditCode>(),
				NetworkAccessPointId = GetDeviceIdentifier(),
				NetworkAccessPointType = NetworkAccessPointType.MachineName,
				UserIsRequestor = true,
				UserName = identityName
			});

			this.SendAudit(audit);
		}

		/// <summary>
		/// Audits the log off.
		/// </summary>
		/// <param name="principal">The principal.</param>
		/// <exception cref="System.ArgumentNullException">principal</exception>
		public void AuditLogOff(IPrincipal principal)
		{
			if (principal == null)
			{
				throw new ArgumentNullException(nameof(principal), Locale.ValueCannotBeNull);
			}

			var audit = this.CreateBaseAudit(ActionType.Execute, EventTypeCode.Logout, EventIdentifierType.UserAuthentication, OutcomeIndicator.Success);

			audit.Actors.Add(new AuditActorData
			{
				NetworkAccessPointId = GetDeviceIdentifier(),
				NetworkAccessPointType = NetworkAccessPointType.MachineName,
				UserIsRequestor = true,
				UserName = principal.Identity.GetUserName()
			});

			this.SendAudit(audit);
		}
	}
}