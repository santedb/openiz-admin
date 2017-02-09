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

using System;
using System.ComponentModel.DataAnnotations;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Util;

namespace OpenIZAdmin.Models.PolicyModels.ViewModels
{
	public class PolicyViewModel
	{
		public PolicyViewModel()
		{
		}

		public PolicyViewModel(SecurityPolicy securityPolicy)
		{
			this.CreationTime = securityPolicy.CreationTime.DateTime;
			this.CanOverride = securityPolicy.CanOverride;
			this.IsPublic = securityPolicy.IsPublic;
			this.Key = securityPolicy.Key.Value;
			this.Name = securityPolicy.Name;
			this.Oid = securityPolicy.Oid;
			this.IsObsolete = securityPolicy.ObsoletionTime != null;
		}

		public PolicyViewModel(SecurityPolicyInfo securityPolicyInfo)
		{
			this.CreationTime = securityPolicyInfo.Policy.CreationTime.DateTime;
			this.CanOverride = securityPolicyInfo.Policy.CanOverride;
			this.IsPublic = securityPolicyInfo.Policy.IsPublic;
			this.Key = securityPolicyInfo.Policy.Key.Value;
			this.Name = securityPolicyInfo.Policy.Name;
			this.Oid = securityPolicyInfo.Policy.Oid;
			this.IsObsolete = securityPolicyInfo.Policy.ObsoletionTime != null;
		}

		public PolicyViewModel(SecurityPolicyInstance securityPolicyInstance) : this(securityPolicyInstance.Policy)
		{
			this.Grant = Enum.GetName(typeof(PolicyGrantType), securityPolicyInstance.GrantType);
		}

		[Display(Name = "CanOverride", ResourceType = typeof(Localization.Locale))]
		public bool CanOverride { get; set; }

		[Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
		public DateTime CreationTime { get; set; }

		[Display(Name = "Grant", ResourceType = typeof(Localization.Locale))]
		public string Grant { get; set; }

		/// <summary>
		/// Gets or sets the obsolete status of the policy.
		/// </summary>
		public bool IsObsolete { get; set; }

		[Display(Name = "IsPublic", ResourceType = typeof(Localization.Locale))]
		public bool IsPublic { get; set; }

		public Guid Key { get; set; }

		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		[Display(Name = "OIDAllCaps", ResourceType = typeof(Localization.Locale))]
		public string Oid { get; set; }
	}
}