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
using OpenIZAdmin.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.PolicyModels
{
	/// <summary>
	/// Represents a policy view model class.
	/// </summary>
	public class PolicyViewModel : PolicyModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PolicyViewModel"/> class.
		/// </summary>
		public PolicyViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PolicyViewModel"/> class
		/// with a specific <see cref="SecurityPolicy"/> instance.
		/// </summary>
		/// <param name="securityPolicy">The security policy.</param>
		public PolicyViewModel(SecurityPolicy securityPolicy)
		{
			this.CreationTime = securityPolicy.CreationTime.DateTime;
			this.CanOverride = securityPolicy.CanOverride;
			this.IsPublic = securityPolicy.IsPublic;
			this.Id = securityPolicy.Key ?? Guid.Empty;
			this.Name = securityPolicy.Name;
			this.Oid = securityPolicy.Oid;
			this.IsObsolete = securityPolicy.ObsoletionTime != null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PolicyViewModel"/> class
		/// with a specific <see cref="SecurityPolicyInfo"/> instance.
		/// </summary>
		/// <param name="securityPolicyInfo">The security policy information.</param>
		public PolicyViewModel(SecurityPolicyInfo securityPolicyInfo)
		{
			this.CreationTime = securityPolicyInfo.Policy.CreationTime.DateTime;
			this.CanOverride = securityPolicyInfo.Policy.CanOverride;
            this.Grant = Enum.GetName(typeof(PolicyGrantType), securityPolicyInfo.Grant);
            this.IsPublic = securityPolicyInfo.Policy.IsPublic;
		    this.Id = securityPolicyInfo.Policy.Key ?? Guid.Empty;
			this.Name = securityPolicyInfo.Policy.Name;
			this.Oid = securityPolicyInfo.Policy.Oid;
			this.IsObsolete = securityPolicyInfo.Policy.ObsoletionTime != null;            
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="PolicyViewModel"/> class
		/// with a specific <see cref="SecurityPolicyInstance"/> instance.
		/// </summary>
		/// <param name="securityPolicyInstance">The security policy instance.</param>
		public PolicyViewModel(SecurityPolicyInstance securityPolicyInstance) : this(securityPolicyInstance.Policy)
		{
			this.Grant = Enum.GetName(typeof(PolicyGrantType), securityPolicyInstance.GrantType);
		}
		
	}
}