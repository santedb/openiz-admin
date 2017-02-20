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
 * Date: 2016-7-8
 */

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.DeviceModels
{
	/// <summary>
	/// Represents a device view model.
	/// </summary>
	public class DeviceViewModel : SecurityViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DeviceViewModel"/> class.
		/// </summary>
		public DeviceViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DeviceViewModel"/> class
		/// with a specific <see cref="SecurityDeviceInfo"/> instance.
		/// </summary>
		/// <param name="securityDeviceInfo">The <see cref="SecurityDeviceInfo"/> instance.</param>
		public DeviceViewModel(SecurityDeviceInfo securityDeviceInfo) : base(securityDeviceInfo)
		{
			this.Name = securityDeviceInfo.Name;
		}

		/// <summary>
		/// Gets or sets the name of the device.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		public string Name { get; set; }
	}
}