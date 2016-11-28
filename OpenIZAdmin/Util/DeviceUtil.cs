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
            viewModel.IsObsolete = IsActiveStatus(device.ObsoletionTime);

            //if(viewModel.Policies != null && viewModel.Policies.Count() > 0)
            //{
            //    viewModel.Policies = device.Policies.Select(p => PolicyUtil.ToPolicyViewModel(p.Policy)).ToList();
            //}
            

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
        /// Gets all the Security Policies that can be applied to a device
        /// </summary>
        /// <param name="client">The Ami Service Client.</param>        
        /// <returns>Returns a list of policies</returns>
        internal static IEnumerable<SecurityPolicyInfo> GetAllPolicies(AmiServiceClient client)
        {
            IEnumerable<SecurityPolicyInfo> policyList = new List<SecurityPolicyInfo>();

            try
            {
                var policies = client.GetPolicies(p => p.ObsoletionTime != null);
                if(policies != null)
                {
                    //policyList = policies.CollectionItem.Select(p => DeviceUtil.ToEditDeviceModel(p));
                    policyList = policies.CollectionItem.ToList();
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.TraceError("Unable to retrieve policies: {0}", e.StackTrace);
#endif
                Trace.TraceError("Unable to retrieve policies: {0}", e.Message);
            }

            return policyList;
        }

        /// <summary>
        /// Gets the policy objects that have been selected to be added to a device
        /// </summary>
        /// <param name="client">The Ami Service Client.</param> 
        /// <param name="pList">The string list with selected policy names.</param>         
        /// <returns>Returns a list of device SecurityPolicyInfo objects</returns>
        internal static List<SecurityPolicy> GetNewPolicies(AmiServiceClient client, IEnumerable<string> pList)
        {
            var policies = new List<SecurityPolicy>();

			policies.AddRange(from name
								in pList
								where IsValidString(name)
								select client.GetPolicies(r => r.Name == name)
								into result
								where result.CollectionItem.Count != 0
								select result.CollectionItem.FirstOrDefault()
								into infoResult
								where infoResult.Policy != null
								select infoResult.Policy);

	        return policies;
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
		        Policies = deviceInfo.Policies.Select(p => PolicyUtil.ToPolicyViewModel(p.Policy)).ToList(),
		        UpdatedTime = deviceInfo.Device.UpdatedTime?.DateTime,
		        IsObsolete = IsActiveStatus(deviceInfo.Device.ObsoletionTime),
		        HasPolicies = IsPolicy(deviceInfo.Policies)
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
            viewModel.IsObsolete = IsActiveStatus(device.ObsoletionTime);            

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
	        var viewModel = new EditDeviceModel
	        {
		        Device = deviceInfo.Device,
		        CreationTime = deviceInfo.Device.CreationTime.DateTime,
		        Id = deviceInfo.Device.Key.Value,
		        DeviceSecret = deviceInfo.DeviceSecret,
		        Name = deviceInfo.Name,
		        UpdatedTime = deviceInfo.Device.UpdatedTime?.DateTime,
		        DevicePolicies = (deviceInfo.Policies != null) ? GetDevicePolicies(deviceInfo.Policies) : null
	        };    

            viewModel.PoliciesList.Add(new SelectListItem { Text = "", Value = "" });
            viewModel.PoliciesList.AddRange(DeviceUtil.GetAllPolicies(client).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }));

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
        /// <param name="addPolicies">The property that contains the selected policies to add to the device.</param>
        /// <returns>Returns a security device info object.</returns>
        public static SecurityDeviceInfo ToSecurityDeviceInfo(EditDeviceModel model, SecurityDeviceInfo deviceInfo, List<SecurityPolicy> addPolicies)
        {
	        deviceInfo.Device.Key = model.Id;
	        deviceInfo.Device.Name = model.Name;
            deviceInfo.Device.DeviceSecret = model.DeviceSecret;

	        deviceInfo.Policies.AddRange(addPolicies.Select(p => new SecurityPolicyInstance(p, PolicyGrantType.Grant)));

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

        /// <summary>
        /// Checks if a device has policies
        /// </summary>
        /// <param name="pList">A list with the policies applied to the device</param>        
        /// <returns>Returns true if policies exist, false if no policies exist</returns>
        private static bool IsPolicy(List<SecurityPolicyInstance> pList)
        {
            if (pList != null && pList.Count() > 0)
                return true;
            else
                return false;
        }

        
    }
}
