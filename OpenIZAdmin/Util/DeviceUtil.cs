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
		/// Converts a <see cref="OpenIZ.Core.Model.Security.SecurityDevice"/> to a <see cref="OpenIZAdmin.Models.DeviceModels.ViewModels.DeviceViewModel"/>.
		/// </summary>
		/// <param name="device">The security device to delete the policy from.</param>
		/// <returns>Returns a DeviceViewModel model.</returns>
		public static DeviceViewModel DeletePolicy(SecurityDevice device)
		{
			DeviceViewModel viewModel = new DeviceViewModel();

			viewModel.CreationTime = device.CreationTime.DateTime;
			viewModel.Id = device.Key.Value;
			viewModel.Name = device.Name;
			viewModel.UpdatedTime = device.UpdatedTime?.DateTime;
			viewModel.IsObsolete = (device.ObsoletionTime != null) ? true : false;

			return viewModel;
		}

		/// <summary>
		/// Queries for a specific device by device key
		/// </summary>
		/// <param name="client">The AMI service client</param>
		/// /// <param name="key">The device guid identifier key </param>
		/// <returns>Returns device object, null if not found</returns>
		public static SecurityDeviceInfo GetDevice(AmiServiceClient client, Guid key)
		{
			try
			{
				var result = client.GetDevices(r => r.Id == key);

				if (result.CollectionItem.Count != 0)
				{
					return result.CollectionItem.FirstOrDefault();
				}
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve device: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve device: {0}", e.Message);
			}

			return null;
		}

		/// <summary>
		/// Gets a list of all devices.
		/// </summary>
		/// <param name="client">The <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.</param>
		/// <returns>Returns a list of devices.</returns>
		internal static IEnumerable<DeviceViewModel> GetAllDevices(AmiServiceClient client)
		{
			// HACK
			return client.GetDevices(d => d.Name != string.Empty).CollectionItem.Select(DeviceUtil.ToDeviceViewModel);
		}

		/// <summary>
		/// Gets a policy that matches the search parameter
		/// </summary>
		/// <param name="client">The Ami Service Client.</param>
		/// <param name="name">The search string parameter applied to the Name field.</param>
		/// <returns>Returns a SecurityPolicyInfo object that matches the search parameter</returns>
		private static SecurityPolicyInfo GetPolicy(AmiServiceClient client, string name)
		{
			try
			{
				var result = client.GetPolicies(r => r.Name == name);
				if (result.CollectionItem.Count != 0)
				{
					return result.CollectionItem.FirstOrDefault();
				}
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve policy: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve policy: {0}", e.Message);
			}

			return null;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.Security.SecurityDevice"/> to a <see cref="OpenIZAdmin.Models.DeviceModels.ViewModels.DeviceViewModel"/>.
		/// </summary>
		/// <param name="deviceInfo">The security device to convert.</param>
		/// <returns>Returns a DeviceViewModel model.</returns>
		public static DeviceViewModel ToDeviceViewModel(SecurityDeviceInfo deviceInfo)
		{
			var viewModel = new DeviceViewModel
			{
				CreationTime = deviceInfo.Device.CreationTime.DateTime,
				Id = deviceInfo.Device.Key.Value,
				Name = deviceInfo.Name,
				DeviceSecret = deviceInfo.DeviceSecret,
				Policies = deviceInfo.Policies.Select(p => PolicyUtil.ToPolicyViewModel(p.Policy)).ToList(),
				UpdatedTime = deviceInfo.Device.UpdatedTime?.DateTime,
				IsObsolete = (deviceInfo.Device.ObsoletionTime != null) ? true : false,
				HasPolicies = (deviceInfo.Policies.Any()) ? true : false
			};

			return viewModel;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.Security.SecurityDevice"/> to a <see cref="OpenIZAdmin.Models.DeviceModels.ViewModels.DeviceViewModel"/>.
		/// </summary>
		/// <param name="device">The security device to convert.</param>
		/// <param name="searchTerm">The string search parameter.</param>
		/// <returns>Returns a DeviceViewModel model.</returns>
		public static DeviceViewModel ToDeviceViewModel(SecurityDevice device, string searchTerm)
		{
			DeviceViewModel viewModel = new DeviceViewModel();

			viewModel.CreationTime = device.CreationTime.DateTime;
			viewModel.Id = device.Key.Value;
			viewModel.Name = device.Name;
			viewModel.Policies = device.Policies.Select(p => PolicyUtil.ToPolicyViewModel(p.Policy)).ToList();
			viewModel.UpdatedTime = device.UpdatedTime?.DateTime;
			viewModel.IsObsolete = (device.ObsoletionTime != null) ? true : false;

			return viewModel;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.Security.SecurityDevice"/> to a <see cref="OpenIZAdmin.Models.DeviceModels.EditDeviceModel"/>
		/// </summary>
		/// <param name="client">The Ami Service Client.</param>
		/// /// <param name="deviceInfo">The device object to convert to a EditDeviceModel.</param>
		/// <returns>Returns a EditDeviceModel object.</returns>
		public static EditDeviceModel ToEditDeviceModel(AmiServiceClient client, SecurityDeviceInfo deviceInfo)
		{
			EditDeviceModel viewModel = new EditDeviceModel();

			viewModel.Device = deviceInfo.Device;
			viewModel.CreationTime = deviceInfo.Device.CreationTime.DateTime;
			viewModel.Id = deviceInfo.Device.Key.Value;
			viewModel.DeviceSecret = deviceInfo.DeviceSecret;
			viewModel.Name = deviceInfo.Name;
			viewModel.UpdatedTime = deviceInfo.Device.UpdatedTime?.DateTime;

			viewModel.DevicePolicies = (deviceInfo.Policies != null && deviceInfo.Policies.Any()) ? viewModel.DevicePolicies = deviceInfo.Policies.Select(p => PolicyUtil.ToPolicyViewModel(p)).OrderBy(q => q.Name).ToList() : new List<PolicyViewModel>();

			if (viewModel.DevicePolicies.Any())
			{
				viewModel.Policies = viewModel.DevicePolicies.Select(p => p.Key.ToString()).ToList();
			}

			viewModel.PoliciesList.Add(new SelectListItem { Text = "", Value = "" });
			viewModel.PoliciesList.AddRange(CommonUtil.GetAllPolicies(client).Select(r => new SelectListItem { Text = r.Name, Value = r.Key.ToString() }).OrderBy(q => q.Text));

			return viewModel;
		}

		/// <summary>
		/// Gets the list of policies that a device has - used for UI display purposes
		/// </summary>
		/// <param name="policyInstances">A list of SecurityPolicyInstance objects.</param>
		/// <returns>Returns a IEnumerable<PolicyViewModel> model.</returns>
		internal static IEnumerable<PolicyViewModel> GetDevicePolicies(List<SecurityPolicyInstance> policyInstances)
		{
			return policyInstances.Select(PolicyUtil.ToPolicyViewModel);
		}

		/// <summary>
		/// Converts a <see cref="OpenIZAdmin.Models.DeviceModels.CreateDeviceModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityDeviceInfo"/>.
		/// </summary>
		/// <param name="model">The create device model to convert.</param>
		/// <returns>Returns a SecurityDeviceInfo object.</returns>
		public static SecurityDeviceInfo ToSecurityDevice(CreateDeviceModel model)
		{
			return new SecurityDeviceInfo
			{
				Device = new SecurityDevice
				{
					DeviceSecret = model.DeviceSecret,
					Name = model.Name
				}
			};
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.Security.SecurityDevice"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityDeviceInfo"/>
		/// </summary>
		/// /// <param name="device">The device object to activate.</param>
		/// <returns>Returns a security device info object.</returns>
		public static SecurityDeviceInfo ToActivateSecurityDeviceInfo(SecurityDevice device)
		{
			var deviceInfo = new SecurityDeviceInfo
			{
				Device = device,
				DeviceSecret = device.DeviceSecret,
				Name = device.Name,
				Id = device.Key.Value
			};

			deviceInfo.Device.ObsoletedBy = null;
			deviceInfo.Device.ObsoletionTime = null;

			return deviceInfo;
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