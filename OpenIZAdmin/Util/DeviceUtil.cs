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
            List<SecurityPolicy> policyList = new List<SecurityPolicy>();

            try
            {
                foreach (string name in pList)
                {
                    if (IsValidString(name))
                    {
                        //SecurityPolicyInfo result = DeviceUtil.GetPolicy(client, name);
                        var result = client.GetPolicies(r => r.Name == name);
                        if (result.CollectionItem.Count != 0)
                        {
                            SecurityPolicyInfo infoResult =  result.CollectionItem.FirstOrDefault();
                            if(infoResult.Policy != null)
                            {
                                policyList.Add(infoResult.Policy);
                            }
                        }                        
                    }
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
        /// <param name="device">The security device to convert.</param>
        /// <returns>Returns a DeviceViewModel model.</returns>
        public static DeviceViewModel ToDeviceViewModel(SecurityDevice device)
        {
            DeviceViewModel viewModel = new DeviceViewModel();

            viewModel.CreationTime = device.CreationTime.DateTime;
            viewModel.Id = device.Key.Value;
            viewModel.Name = device.Name;
            viewModel.Policies = device.Policies.Select(p => PolicyUtil.ToPolicyViewModel(p.Policy)).ToList();
            viewModel.UpdatedTime = device.UpdatedTime?.DateTime;
            viewModel.IsObsolete = IsActiveStatus(device.ObsoletionTime);
            viewModel.HasPolicies = IsPolicy(device.Policies);

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
        /// Converts a <see cref="OpenIZ.Core.Model.AMI.Auth"/> to a <see cref="OpenIZAdmin.Models.DeviceModels.EditDeviceModel"/>
        /// </summary>
        /// <param name="client">The Ami Service Client.</param>
        /// /// <param name="device">The device object to convert to a EditDeviceModel.</param>
        /// <returns>Returns a EditDeviceModel object.</returns>
		public static EditDeviceModel ToEditDeviceModel(AmiServiceClient client, SecurityDevice device)
        {
            EditDeviceModel viewModel = new EditDeviceModel();

            viewModel.Device = device;
            viewModel.CreationTime = device.CreationTime.DateTime;
            viewModel.Id = device.Key.Value;
            viewModel.DeviceSecret = device.DeviceSecret;
            viewModel.Name = device.Name;                        
            viewModel.UpdatedTime = device.UpdatedTime?.DateTime;

            viewModel.DevicePolicies = (device.Policies != null) ? GetDevicePolicies(device.Policies) : null;

            //viewModel.DevicePolicies = (device.Policies != null) ? device.Policies : new List<SecurityPolicyInstance>();

            //needed to be able to display additional data for the policies
            //viewModel.DevicePoliciesViewModel = new List<SecurityPolicyInfoViewModel>();

            //foreach(SecurityPolicyInstance p in device.Policies)
            //{
            //    viewModel.DevicePoliciesViewModel.Add(new SecurityPolicyInfoViewModel(p));
            //}           

            viewModel.PoliciesList.Add(new SelectListItem { Text = "", Value = "" });
            viewModel.PoliciesList.AddRange(DeviceUtil.GetAllPolicies(client).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }));

            return viewModel;
        }

        /// <summary>
        /// Gets the list of policies that a device has - used for UI display purposes
        /// </summary>
        /// <param name="pList">A list of SecurityPolicyInstance objects.</param>        
        /// <returns>Returns a IEnumerable<PolicyViewModel> model.</returns>
        internal static IEnumerable<PolicyViewModel> GetDevicePolicies(List<SecurityPolicyInstance> pList)
        {
            IEnumerable<PolicyViewModel> viewModels = new List<PolicyViewModel>();

            try
            {
                //var policies = client.GetPolicies(p => p.IsPublic == true);

                //viewModels = policies.CollectionItem.Select(p => PolicyUtil.ToPolicyViewModel(p));
                return viewModels = pList.Select(p => PolicyUtil.ToPolicyViewModel(p));               
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.TraceError("Unable to retrieve policies: {0}", e.StackTrace);
#endif
                Trace.TraceError("Unable to retrieve policies: {0}", e.Message);
            }

            return viewModels;
        }

        /// <summary>
        /// Converts a <see cref="OpenIZAdmin.Models.DeviceModels.CreateDeviceModel"/> to a <see cref="OpenIZ.Core.Model.Security.SecurityDevice"/>.
        /// </summary>
        /// <param name="model">The create device model to convert.</param>
        /// <returns>Returns a SecurityDeviceInfo object.</returns>
        public static SecurityDeviceInfo ToSecurityDevice(CreateDeviceModel model)
		{
	        var device = new SecurityDeviceInfo
	        {
		        Name = model.Name,
		        DeviceSecret = model.DeviceSecret
	        };

	        return device;
		}

        /// <summary>
        /// Converts a <see cref="OpenIZ.Core.Model.Security.SecurityDevice"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth"/>
        /// </summary>        
        /// /// <param name="device">The device object to activate.</param>
        /// <returns>Returns a security device info object.</returns>        
        public static SecurityDeviceInfo ToActivateSecurityDeviceInfo(SecurityDevice device)
        {
            SecurityDeviceInfo deviceInfo = new SecurityDeviceInfo();
            deviceInfo.Device = device;

            deviceInfo.DeviceSecret = device.DeviceSecret;
            deviceInfo.Name = device.Name;
            deviceInfo.Id = device.Key.Value;
            deviceInfo.Device.ObsoletedBy = null;
            deviceInfo.Device.ObsoletionTime = null;

            return deviceInfo;
        }

        /// <summary>
        /// Converts a <see cref="OpenIZAdmin.Models.DeviceModels.EditDeviceModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth"/>
        /// </summary>
        /// <param name="model">The edit device model to convert.</param>
        /// <param name="device">The device object to apply the changes to.</param>
        /// <param name="addPolicies">The property that contains the selected policies to add to the device.</param>
        /// <returns>Returns a SecurityDeviceInfo object.</returns>
        public static SecurityDeviceInfo ToSecurityDeviceInfo(EditDeviceModel model, SecurityDevice device, List<SecurityPolicy> addPolicies)
        {
            SecurityDeviceInfo deviceInfo = new SecurityDeviceInfo();
            deviceInfo.Device = device;

            deviceInfo.Id = model.Id;
            deviceInfo.Device.Name = model.Name;
            deviceInfo.Device.DeviceSecret = model.DeviceSecret;
            deviceInfo.Policies = (model.Policies != null) ? model.Policies : new List<SecurityPolicyInstance>();

            //add the new policies
            foreach(SecurityPolicy p in addPolicies)
            {                
                SecurityPolicyInstance policy = new SecurityPolicyInstance(p, (PolicyGrantType)2);
                deviceInfo.Policies.Add(policy);
            }            

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
