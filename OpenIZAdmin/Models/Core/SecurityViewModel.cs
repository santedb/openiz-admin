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
 * Date: 2017-2-20
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.PolicyModels;
using OpenIZAdmin.Models.RoleModels;
using OpenIZAdmin.Models.UserModels;

namespace OpenIZAdmin.Models.Core
{
	/// <summary>
	/// Represents a security view model.
	/// </summary>
	public abstract class SecurityViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityViewModel"/> class.
		/// </summary>
		protected SecurityViewModel()
		{
			this.Policies = new List<PolicyViewModel>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityViewModel"/> class
		/// with a specific policy list.
		/// </summary>
		/// <param name="policies">The <see cref="SecurityPolicyInfo"/> list.</param>
		private SecurityViewModel(IEnumerable<SecurityPolicyInfo> policies) : this()
		{
			this.HasPolicies = policies?.Any() == true;

			if (this.HasPolicies)
			{
				this.Policies = policies.Select(p => new PolicyViewModel(new SecurityPolicyInstance(p.Policy, p.Grant))).OrderBy(q => q.Name).ToList();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityViewModel"/> class
		/// with a specific <see cref="SecurityEntity"/> instance
		/// and a specific <see cref="SecurityPolicyInfo"/> list.
		/// </summary>
		/// <param name="securityEntity">The <see cref="SecurityEntity"/> instance.</param>
		/// <param name="policies">The <see cref="SecurityPolicyInfo"/> list.</param>
		private SecurityViewModel(SecurityEntity securityEntity, IEnumerable<SecurityPolicyInfo> policies) : this(policies)
		{
			this.CreationTime = securityEntity.CreationTime.DateTime;
			this.Id = securityEntity.Key.Value;
			this.IsObsolete = securityEntity.ObsoletionTime != null;
			this.ObsoletionTime = securityEntity.ObsoletionTime?.DateTime;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityViewModel"/> class
		/// with a specific <see cref="SecurityApplicationInfo"/> instance.
		/// </summary>
		/// <param name="securityApplicationInfo">The <see cref="SecurityApplicationInfo"/> instance.</param>
		protected SecurityViewModel(SecurityApplicationInfo securityApplicationInfo) : this(securityApplicationInfo.Application, securityApplicationInfo.Policies)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityViewModel"/> class
		/// with a specific <see cref="SecurityDeviceInfo"/> instance.
		/// </summary>
		/// <param name="securityDeviceInfo">The <see cref="SecurityDeviceInfo"/> instance.</param>
		protected SecurityViewModel(SecurityDeviceInfo securityDeviceInfo) : this(securityDeviceInfo.Device, securityDeviceInfo.Policies)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityViewModel"/> class
		/// with a specific <see cref="SecurityRoleInfo"/> instance.
		/// </summary>
		/// <param name="securityRoleInfo">The <see cref="SecurityRoleInfo"/> instance.</param>
		protected SecurityViewModel(SecurityRoleInfo securityRoleInfo) : this(securityRoleInfo.Role, securityRoleInfo.Policies)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityViewModel"/> class
		/// with a specific <see cref="SecurityRoleInfo"/> instance.
		/// </summary>
		/// <param name="securityEntity">The <see cref="SecurityEntity"/> instance.</param>
		/// <param name="roles">The <see cref="SecurityRoleInfo"/> list.</param>
		protected SecurityViewModel(SecurityEntity securityEntity, IEnumerable<SecurityRoleInfo> roles) : this(securityEntity, roles.SelectMany(r => r.Policies))
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserViewModel"/> class
		/// with a specific <see cref="SecurityUserInfo"/> instance.
		/// </summary>
		/// <param name="securityUserInfo">The <see cref="SecurityUserInfo"/> instance.</param>
		protected SecurityViewModel(SecurityUserInfo securityUserInfo) : this(securityUserInfo.User, securityUserInfo.Roles)
		{

		}

		/// <summary>
		/// Gets or sets the creation time of the security entity.
		/// </summary>
		[Display(Name = "CreationTime", ResourceType = typeof(Locale))]
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Gets or sets whether the security entity has policies associated.
		/// </summary>
		[Display(Name = "HasPolicies", ResourceType = typeof(Locale))]
		public bool HasPolicies { get; set; }

		/// <summary>
		/// Gets or sets the id of the security entity.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets whether the security entity is obsolete.
		/// </summary>
		public bool IsObsolete { get; set; }

		/// <summary>
		/// Gets or sets the obsoletion time of the security entity.
		/// </summary>
		public DateTime? ObsoletionTime { get; set; }

		/// <summary>
		/// Gets or sets the list of policies associated with the security entity.
		/// </summary>
		public List<PolicyViewModel> Policies { get; set; }
	}
}