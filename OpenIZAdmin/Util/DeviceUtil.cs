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
	public static class DeviceUtil
	{
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

		public static DeviceViewModel ToDeviceViewModel(SecurityDevice device)
		{
			DeviceViewModel viewModel = new DeviceViewModel();

			viewModel.CreationTime = device.CreationTime.DateTime;
			viewModel.Name = device.Name;
			viewModel.UpdatedTime = device.UpdatedTime?.DateTime;

			return viewModel;
		}

		public static SecurityDevice ToSecurityDevice(CreateDeviceModel model)
		{
			SecurityDevice device = new SecurityDevice();

			device.Name = model.Name;
			device.DeviceSecret = Guid.NewGuid().ToString();

			return device;
		}
	}
}