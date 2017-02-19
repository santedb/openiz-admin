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
 * User: khannan
 * Date: 2016-8-14
 */

using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Models.PolicyModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using OpenIZ.Core.Model.AMI.Auth;

namespace OpenIZAdmin.Models.DeviceModels
{
	/// <summary>
	/// Represents an edit device model.
	/// </summary>
	public class EditDeviceModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditDeviceModel"/> class.
		/// </summary>
		public EditDeviceModel()
		{
			this.DevicePolicies = new List<PolicyViewModel>();
			this.Policies = new List<string>();
			this.PoliciesList = new List<SelectListItem>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditDeviceModel"/> class
		/// with a specific <see cref="SecurityDeviceInfo"/> instance.
		/// </summary>
		/// <param name="securityDeviceInfo">The <see cref="SecurityDeviceInfo"/> instance.</param>
		public EditDeviceModel(SecurityDeviceInfo securityDeviceInfo) : this()
		{
			this.Device = securityDeviceInfo.Device;
			this.CreationTime = securityDeviceInfo.Device.CreationTime.DateTime;
			this.Id = securityDeviceInfo.Device.Key.Value;
			this.IsObsolete = securityDeviceInfo.Device.ObsoletionTime != null;
			this.DeviceSecret = securityDeviceInfo.DeviceSecret;
			this.Name = securityDeviceInfo.Name;
			this.UpdatedTime = securityDeviceInfo.Device.UpdatedTime?.DateTime;
			this.DevicePolicies = securityDeviceInfo.Policies.Select(p => new PolicyViewModel(p)).OrderBy(q => q.Name).ToList();
			this.Policies = this.DevicePolicies.Select(p => p.Key.ToString()).ToList();
		}

		/// <summary>
		/// Gets or sets the creation time of the device.
		/// </summary>
		[Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the security device.
		/// </summary>
		public SecurityDevice Device { get; set; }

		/// <summary>
		/// Gets or sets the list of policies associated with the device.
		/// </summary>
		public IEnumerable<PolicyViewModel> DevicePolicies { get; set; }

		/// <summary>
		/// Gets or sets the device secret.
		/// </summary>
		[Display(Name = "DeviceSecret", ResourceType = typeof(Localization.Locale))]
		public string DeviceSecret { get; set; }

		/// <summary>
		/// Gets or sets the id of the device.
		/// </summary>
		[Required]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets whether the device is obsolete.
		/// </summary>
		public bool IsObsolete { get; set; }

		/// <summary>
		/// Gets or sets the name of the device.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "NameTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the list of policies associated with the device.
		/// </summary>
		[Display(Name = "Policies", ResourceType = typeof(Localization.Locale))]
		public List<string> Policies { get; set; }

		/// <summary>
		/// Gets or sets the select list of policies.
		/// </summary>
		public List<SelectListItem> PoliciesList { get; set; }

		/// <summary>
		/// Gets or sets the updated time of the device.
		/// </summary>
		public DateTime? UpdatedTime { get; set; }
	}
}