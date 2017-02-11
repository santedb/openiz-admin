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
using OpenIZAdmin.Models.DeviceModels.ViewModels;
using OpenIZAdmin.Models.PolicyModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing devices.
	/// </summary>
	public static class DeviceUtil
	{
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

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.Security.SecurityDevice"/> to a <see cref="OpenIZAdmin.Models.DeviceModels.EditDeviceModel"/>
		/// </summary>
		/// <param name="client">The Ami Service Client.</param>
		/// /// <param name="deviceInfo">The device object to convert to a EditDeviceModel.</param>
		/// <returns>Returns a EditDeviceModel object.</returns>
		public static EditDeviceModel ToEditDeviceModel(AmiServiceClient client, SecurityDeviceInfo deviceInfo)
		{
			var viewModel = new EditDeviceModel
			{
				Device = deviceInfo.Device,
				CreationTime = deviceInfo.Device.CreationTime.DateTime,
				Id = deviceInfo.Device.Key.Value,
				DeviceSecret = deviceInfo.DeviceSecret,
				Name = deviceInfo.Name,
				UpdatedTime = deviceInfo.Device.UpdatedTime?.DateTime,
				DevicePolicies = deviceInfo.Policies.Select(p => new PolicyViewModel(p)).OrderBy(q => q.Name).ToList()
			};

			if (viewModel.DevicePolicies.Any())
			{
				viewModel.Policies = viewModel.DevicePolicies.Select(p => p.Key.ToString()).ToList();
			}

			viewModel.PoliciesList.Add(new SelectListItem { Text = "", Value = "" });
			viewModel.PoliciesList.AddRange(CommonUtil.GetAllPolicies(client).Select(r => new SelectListItem { Text = r.Name, Value = r.Key.ToString() }).OrderBy(q => q.Text));

			return viewModel;
		}

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
				deviceInfo.Policies.AddRange(policyList.Select(p => new SecurityPolicyInstance(p, PolicyGrantType.Grant)));
			}

			return deviceInfo;
		}
	}
}