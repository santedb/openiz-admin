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
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.DeviceModels
{
	/// <summary>
	/// Represents a create device model.
	/// </summary>
	public class CreateDeviceModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateDeviceModel"/> class.
		/// </summary>
		public CreateDeviceModel()
		{
		}

		/// <summary>
		/// Gets or sets the device secret.
		/// </summary>
		[Display(Name = "DeviceSecret", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "DeviceSecretRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "DeviceSecretTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string DeviceSecret { get; set; }

		/// <summary>
		/// Gets or sets the name of the device.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "NameTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		public SecurityDeviceInfo ToSecurityDeviceInfo()
		{
			return new SecurityDeviceInfo
			{
				Device = new SecurityDevice
				{
					DeviceSecret = this.DeviceSecret,
					Name = this.Name
				}
			};
		}
	}
}