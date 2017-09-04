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
 * Date: 2017-8-3
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
using System.Linq;

namespace OpenIZAdmin.Services.Security.Devices
{
	/// <summary>
	/// Represents a security device service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.AmiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.Security.Devices.ISecurityDeviceService" />
	public class SecurityDeviceService : AmiServiceBase, ISecurityDeviceService

	{
		/// <summary>
		/// The core audit service.
		/// </summary>
		private readonly ICoreAuditService coreAuditService;

		/// <summary>
		/// The security entity audit service.
		/// </summary>
		private readonly ISecurityEntityAuditService<SecurityDevice> securityEntityAuditService;

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityDeviceService" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="coreAuditService">The core audit service.</param>
		/// <param name="securityEntityAuditService">The security entity audit service.</param>
		public SecurityDeviceService(AmiServiceClient client, ICoreAuditService coreAuditService, ISecurityEntityAuditService<SecurityDevice> securityEntityAuditService) : base(client)
		{
			this.coreAuditService = coreAuditService;
			this.securityEntityAuditService = securityEntityAuditService;
		}

		/// <summary>
		/// Gets all devices.
		/// </summary>
		/// <returns>Returns a list of all devices in the system.</returns>
		public IEnumerable<SecurityDeviceInfo> GetAllDevices()
		{
			List<SecurityDeviceInfo> devices;

			try
			{
				devices = this.Client.GetDevices(c => c.Name != string.Empty).CollectionItem;
				this.securityEntityAuditService.AuditQuerySecurityEntity(OutcomeIndicator.Success, devices.Select(d => d.Device));
			}
			catch (Exception e)
			{
				this.coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, securityEntityAuditService.QuerySecurityEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return devices;
		}

		/// <summary>
		/// Gets the device.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the device which matches the given key, or null if no device is found.</returns>
		public SecurityDeviceInfo GetDevice(Guid key)
		{
			SecurityDeviceInfo device;

			try
			{
				device = this.Client.GetDevice(key.ToString());
				this.securityEntityAuditService.AuditQuerySecurityEntity(OutcomeIndicator.Success, new List<SecurityDevice> { device?.Device });
			}
			catch (Exception e)
			{
				this.coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, securityEntityAuditService.QuerySecurityEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return device;
		}
	}
}