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
 * User: nityan
 * Date: 2017-8-3
 */

using OpenIZ.Core.Model.AMI.Auth;
using System;
using System.Collections.Generic;

namespace OpenIZAdmin.Services.Security.Devices
{
	/// <summary>
	/// Represents a security device service.
	/// </summary>
	public interface ISecurityDeviceService
	{
		/// <summary>
		/// Gets all devices.
		/// </summary>
		/// <returns>Returns a list of all devices in the system.</returns>
		IEnumerable<SecurityDeviceInfo> GetAllDevices();

		/// <summary>
		/// Gets the device.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the device which matches the given key, or null if no device is found.</returns>
		SecurityDeviceInfo GetDevice(Guid key);
	}
}