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
 * Date: 2017-9-4
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

namespace OpenIZAdmin.Services.Security.Applications
{
	/// <summary>
	/// Represents a security application service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.AmiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.Security.Applications.ISecurityApplicationService" />
	public class SecurityApplicationService : AmiServiceBase, ISecurityApplicationService
	{
		/// <summary>
		/// The core audit service.
		/// </summary>
		private readonly ICoreAuditService coreAuditService;

		/// <summary>
		/// The security entity audit service.
		/// </summary>
		private readonly ISecurityEntityAuditService<SecurityApplication> securityEntityAuditService;

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityApplicationService" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="coreAuditService">The core audit service.</param>
		/// <param name="securityEntityAuditService">The security entity audit service.</param>
		public SecurityApplicationService(AmiServiceClient client, ICoreAuditService coreAuditService, ISecurityEntityAuditService<SecurityApplication> securityEntityAuditService) : base(client)
		{
			this.coreAuditService = coreAuditService;
			this.securityEntityAuditService = securityEntityAuditService;
		}

		/// <summary>
		/// Activates a security application.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="securityApplicationInfo">The security application information.</param>
		/// <returns>Returns the activated security application.</returns>
		public SecurityApplicationInfo Activate(Guid key, SecurityApplicationInfo securityApplicationInfo)
		{
			SecurityApplicationInfo activatedApplication;

			try
			{
				securityApplicationInfo.Id = key;
				securityApplicationInfo.Application.ObsoletedBy = null;
				securityApplicationInfo.Application.ObsoletionTime = null;
				securityApplicationInfo.Application.ObsoletionTimeXml = null;

				activatedApplication = this.Client.UpdateApplication(key.ToString(), securityApplicationInfo);

				if (activatedApplication != null)
				{
					securityEntityAuditService.AuditUpdateSecurityEntity(OutcomeIndicator.Success, activatedApplication.Application);
				}
				else
				{
					securityEntityAuditService.AuditUpdateSecurityEntity(OutcomeIndicator.SeriousFail, null);
				}
			}
			catch (Exception e)
			{
				this.coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, securityEntityAuditService.UpdateSecurityEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return activatedApplication;
		}

		/// <summary>
		/// Creates the specified security application information.
		/// </summary>
		/// <param name="securityApplicationInfo">The security application information.</param>
		/// <returns>Returns the created security application.</returns>
		public SecurityApplicationInfo Create(SecurityApplicationInfo securityApplicationInfo)
		{
			SecurityApplicationInfo createdApplication;

			try
			{
				createdApplication = this.Client.CreateApplication(securityApplicationInfo);

				if (createdApplication != null)
				{
					securityEntityAuditService.AuditCreateSecurityEntity(OutcomeIndicator.Success, createdApplication.Application);
				}
				else
				{
					securityEntityAuditService.AuditCreateSecurityEntity(OutcomeIndicator.SeriousFail, null);
				}
			}
			catch (Exception e)
			{
				coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, securityEntityAuditService.CreateSecurityEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return createdApplication;
		}

		/// <summary>
		/// Deletes a security application.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the deleted security application info.</returns>
		public SecurityApplicationInfo Delete(Guid key)
		{
			SecurityApplicationInfo securityApplicationInfo;

			try
			{
				securityApplicationInfo = this.Client.DeleteApplication(key.ToString());

				if (securityApplicationInfo != null)
				{
					securityEntityAuditService.AuditDeleteSecurityEntity(OutcomeIndicator.Success, securityApplicationInfo.Application);
				}
				else
				{
					securityEntityAuditService.AuditDeleteSecurityEntity(OutcomeIndicator.SeriousFail, null);
				}
			}
			catch (Exception e)
			{
				coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, securityEntityAuditService.DeleteSecurityEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return securityApplicationInfo;
		}

		/// <summary>
		/// Gets a security application.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the security application or null if no security application is found.</returns>
		public SecurityApplicationInfo Get(Guid key)
		{
			SecurityApplicationInfo securityApplicationInfo;

			try
			{
				securityApplicationInfo = this.Client.GetApplication(key.ToString());

				if (securityApplicationInfo != null)
				{
					securityEntityAuditService.AuditQuerySecurityEntity(OutcomeIndicator.Success, new List<SecurityApplication> { securityApplicationInfo.Application });
				}
				else
				{
					securityEntityAuditService.AuditQuerySecurityEntity(OutcomeIndicator.SeriousFail, null);
				}
			}
			catch (Exception e)
			{
				coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, securityEntityAuditService.QuerySecurityEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return securityApplicationInfo;
		}

		/// <summary>
		/// Searches for a security application.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of security applications which match the given search term.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IEnumerable<SecurityApplicationInfo> Search(string searchTerm)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Updates a security application.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="securityApplicationInfo">The security application information.</param>
		/// <returns>Returns the updated security application info.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public SecurityApplicationInfo Update(Guid key, SecurityApplicationInfo securityApplicationInfo)
		{
			throw new NotImplementedException();
		}
	}
}