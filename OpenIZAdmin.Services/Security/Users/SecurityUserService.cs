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
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Core.Auditing.Core;
using OpenIZAdmin.Core.Auditing.SecurityEntities;
using OpenIZAdmin.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZAdmin.Services.Security.Users
{
	/// <summary>
	/// Represents a security user service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.AmiServiceBase" />
	/// <seealso cref="ISecurityUserService" />
	public class SecurityUserService : AmiServiceBase, ISecurityUserService

	{
		/// <summary>
		/// The core audit service.
		/// </summary>
		private readonly ICoreAuditService coreAuditService;

		/// <summary>
		/// The security entity audit service.
		/// </summary>
		private readonly ISecurityEntityAuditService<SecurityUser> securityEntityAuditService;

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityUserService" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="coreAuditService">The core audit service.</param>
		/// <param name="securityEntityAuditService">The security entity audit service.</param>
		public SecurityUserService(AmiServiceClient client, ICoreAuditService coreAuditService, ISecurityEntityAuditService<SecurityUser> securityEntityAuditService) : base(client)
		{
			this.coreAuditService = coreAuditService;
			this.securityEntityAuditService = securityEntityAuditService;
		}

		/// <summary>
		/// Activates the security user.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the activated security user.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public SecurityUserInfo ActivateSecurityUser(Guid key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Changes the password of a user.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="newPassword">The new password.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		public void ChangePassword(Guid key, string newPassword)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates the security user.
		/// </summary>
		/// <param name="userInfo">The user information.</param>
		/// <returns>Returns the created security user info.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public SecurityUserInfo CreateSecurityUser(SecurityUserInfo userInfo)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Deactivates the security user.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the deactivated security user.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public SecurityUserInfo DeactivateSecurityUser(Guid key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the security user.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the security user for the given key.</returns>
		public SecurityUserInfo GetSecurityUser(Guid key)
		{
			SecurityUserInfo securityUserInfo;

			try
			{
				securityUserInfo = this.Client.GetUser(key.ToString());

				if (securityUserInfo != null)
				{
					this.securityEntityAuditService.AuditQuerySecurityEntity(OutcomeIndicator.Success, new List<SecurityUser> { securityUserInfo.User });
				}
				else
				{
					this.securityEntityAuditService.AuditQuerySecurityEntity(OutcomeIndicator.SeriousFail, null);
				}
			}
			catch (Exception e)
			{
				coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, this.securityEntityAuditService.QuerySecurityEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return securityUserInfo;
		}

		/// <summary>
		/// Queries for security users using a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of security users which match the given expression.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IEnumerable<SecurityUserInfo> Query(Expression<Func<SecurityUser, bool>> expression)
		{
			throw new NotImplementedException();
		}
	}
}