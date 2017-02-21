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
 * Date: 2016-7-30
 */

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Models.DeviceModels;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing devices.
	/// </summary>
	public static class DeviceUtil
	{
		/// <summary>
		/// Converts a <see cref="OpenIZAdmin.Models.DeviceModels.EditDeviceModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityDeviceInfo"/>
		/// </summary>
		/// <param name="model">The edit device model to convert.</param>
		/// <param name="deviceInfo">The device object to apply the changes to.</param>
		/// <returns>Returns a security device info object.</returns>
		public static SecurityDeviceInfo ToSecurityDeviceInfo(AmiServiceClient amiClient, EditDeviceModel model, SecurityDeviceInfo deviceInfo)
		{
			deviceInfo.Device.Key = model.Id;
			deviceInfo.Device.Name = model.Name;
			deviceInfo.Device.DeviceSecret = model.DeviceSecret;

			var policyList = CommonUtil.GetNewPolicies(amiClient, model.Policies);

			if (policyList.Any())
			{
				deviceInfo.Policies.Clear();
				deviceInfo.Policies.AddRange(policyList.Select(p => new SecurityPolicyInfo(p)
				{
					Grant = PolicyGrantType.Grant
				}));
			}

			return deviceInfo;
		}

		/// <summary>
		/// Gets a list of all devices.
		/// </summary>
		/// <param name="client">The <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.</param>
		/// <returns>Returns a list of devices.</returns>
		internal static IEnumerable<DeviceViewModel> GetAllDevices(AmiServiceClient client)
		{
			// HACK
			return client.GetDevices(d => d.Name != string.Empty).CollectionItem.Select(d => new DeviceViewModel(d));
		}
	}
}