/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing devices.
	/// </summary>
	public static class DeviceUtil
	{

        public static SecurityDevice GetDevice(AmiServiceClient client, Guid key)
        {
            try
            {
                var result = client.GetDevices(r => r.Key == key);

                if (result.CollectionItem.Count != 0)
                {
                    return result.CollectionItem.FirstOrDefault(); ;
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
		/// Gets a list of devices.
		/// </summary>
		/// <param name="client">The <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.</param>
		/// <returns>Returns a list of devices.</returns>
		internal static IEnumerable<DeviceViewModel> GetAllDevices(AmiServiceClient client)
		{
			IEnumerable<DeviceViewModel> viewModels = new List<DeviceViewModel>();

			try
			{
				// HACK
				var devices = client.GetDevices(d => d.Name != string.Empty);

				viewModels = devices.CollectionItem.Select(d => DeviceUtil.ToDeviceViewModel(d));
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve devices: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve devices: {0}", e.Message);
			}

			return viewModels;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.Security.SecurityDevice"/> to a <see cref="OpenIZAdmin.Models.DeviceModels.ViewModels.DeviceViewModel"/>.
		/// </summary>
		/// <param name="device">The security device to convert.</param>
		/// <returns>Returns a device view model.</returns>
		public static DeviceViewModel ToDeviceViewModel(SecurityDevice device)
		{
			DeviceViewModel viewModel = new DeviceViewModel();

			viewModel.CreationTime = device.CreationTime.DateTime;
			viewModel.Id = device.Key.Value;
			viewModel.Name = device.Name;
			viewModel.Policies = device.Policies.Select(p => PolicyUtil.ToPolicyViewModel(p.Policy)).ToList();
			viewModel.UpdatedTime = device.UpdatedTime?.DateTime;
            viewModel.IsObsolete = IsActiveStatus(device.ObsoletionTime);            

            return viewModel;
		}

        /// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.Security.SecurityDevice"/> to a <see cref="OpenIZAdmin.Models.DeviceModels.EditDeviceModel"/>.
		/// </summary>
		/// <param name="device">The security device to convert.</param>
		/// <returns>Returns a edit device view model.</returns>
		public static EditDeviceModel ToEditDeviceModel(SecurityDevice device)
        {
            EditDeviceModel viewModel = new EditDeviceModel();

            viewModel.CreationTime = device.CreationTime.DateTime;
            viewModel.Key = device.Key.Value;
            viewModel.Name = device.Name;
            viewModel.Policies = device.Policies.Select(p => PolicyUtil.ToPolicyViewModel(p.Policy)).ToList();
            viewModel.UpdatedTime = device.UpdatedTime?.DateTime;
            viewModel.IsObsolete = IsActiveStatus(device.ObsoletionTime);

            return viewModel;
        }

        /// <summary>
        /// Converts a <see cref="OpenIZAdmin.Models.DeviceModels.CreateDeviceModel"/> to a <see cref="OpenIZ.Core.Model.Security.SecurityDevice"/>.
        /// </summary>
        /// <param name="model">The create device model to convert.</param>
        /// <returns>Returns a security device.</returns>
        public static SecurityDevice ToSecurityDevice(CreateDeviceModel model)
		{
			SecurityDevice device = new SecurityDevice();

			device.Name = model.Name;
			device.DeviceSecret = Guid.NewGuid().ToString();

			return device;
		}

        /// <summary>
        /// Converts a <see cref="OpenIZAdmin.Models.DeviceModels.EditDeviceModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth"/>
        /// </summary>
        /// <param name="model">The edit device model to convert.</param>
        /// /// <param name="device">The device object to apply the changes to.</param>
        /// <returns>Returns a security device info object.</returns>
        public static SecurityDeviceInfo ToSecurityDeviceInfo(EditDeviceModel model, SecurityDevice device)
        {
            SecurityDeviceInfo deviceInfo = new SecurityDeviceInfo();
            deviceInfo.Device = device;
            
            deviceInfo.Device.ObsoletedBy = null;
            deviceInfo.Device.ObsoletionTime = null;

            return deviceInfo;
        }

        /// <summary>
        /// Verifies a valid string parameter
        /// </summary>
        /// <param name="key">The string to validate</param>        
        /// <returns>Returns true if valid, false if empty or whitespace</returns>
        public static bool IsValidString(string key)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if a device is active or inactive
        /// </summary>
        /// <param name="date">A DateTimeOffset object</param>        
        /// <returns>Returns true if active, false if inactive</returns>
        private static bool IsActiveStatus(DateTimeOffset? date)
        {
            if (date != null)
                return true;
            else
                return false;
        }

	}
}