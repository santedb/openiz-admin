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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Models.PolicyModels;

namespace OpenIZAdmin.Models.DeviceModels
{
	public class DeviceViewModel
	{
		public DeviceViewModel()
		{
			this.Policies = new List<PolicyViewModel>();
		}

		public DeviceViewModel(SecurityDevice securityDevice)
		{
			this.CreationTime = securityDevice.CreationTime.DateTime;
			this.Id = securityDevice.Key.Value;
			this.Name = securityDevice.Name;
			this.UpdatedTime = securityDevice.UpdatedTime?.DateTime;
			this.IsObsolete = securityDevice.ObsoletionTime != null;
		}

		public DeviceViewModel(SecurityDeviceInfo securityDeviceInfo) : this()
		{
			this.CreationTime = securityDeviceInfo.Device.CreationTime.DateTime;
			this.Id = securityDeviceInfo.Device.Key.Value;
			this.Name = securityDeviceInfo.Name;
			this.DeviceSecret = securityDeviceInfo.DeviceSecret;
			this.UpdatedTime = securityDeviceInfo.Device.UpdatedTime?.DateTime;
			this.IsObsolete = securityDeviceInfo.Device.ObsoletionTime != null;
			this.HasPolicies = securityDeviceInfo.Policies.Any();

			if (this.HasPolicies)
			{
				this.Policies = securityDeviceInfo.Policies.Select(p => new PolicyViewModel(new SecurityPolicyInstance(p.Policy, p.GrantType))).OrderBy(q => q.Name).ToList();
			}
		}

		[Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
		[DisplayFormat(DataFormatString = "{0:dd/mm/yyyy hh:mm:ss tt}")]
		public DateTime CreationTime { get; set; }

		[Display(Name = "DeviceSecret", ResourceType = typeof(Localization.Locale))]
		public string DeviceSecret { get; set; }

		[Display(Name = "HasPolicies", ResourceType = typeof(Localization.Locale))]
		public bool HasPolicies { get; set; }

		public Guid Id { get; set; }

		public bool IsObsolete { get; set; }

		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		public List<PolicyViewModel> Policies { get; set; }

		[DisplayFormat(DataFormatString = "{0:dd/mm/yyyy hh:mm:ss tt}")]
		public DateTime? UpdatedTime { get; set; }
	}
}