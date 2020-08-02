﻿/*
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
 * Date: 2016-7-10
 */

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;
using OpenIZAdmin.Models.UserModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.RoleModels
{
	/// <summary>
	/// Represents a role view model.
	/// </summary>
	public class RoleViewModel : SecurityViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RoleViewModel"/> class.
		/// </summary>
		public RoleViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RoleViewModel"/> class
		/// with a specific <see cref="SecurityRoleInfo"/> instance.
		/// </summary>
		/// <param name="securityRoleInfo">The <see cref="SecurityRoleInfo"/> instance.</param>
		public RoleViewModel(SecurityRoleInfo securityRoleInfo) : base(securityRoleInfo)
		{
			this.Description = securityRoleInfo.Role.Description;
			this.Name = securityRoleInfo.Name;
			//HasPolicies = securityRoleInfo.Policies?.Any() == true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RoleViewModel"/> class
		/// with a specific <see cref="SecurityRoleInfo"/> instance.
		/// </summary>
		/// <param name="securityRoleInfo">The <see cref="SecurityRoleInfo"/> instance.</param>
		/// <param name="users">The <see cref="List{UserViewModel}"/> instance.</param>
		public RoleViewModel(SecurityRoleInfo securityRoleInfo, List<UserViewModel> users) : base(securityRoleInfo)
		{
			this.Description = securityRoleInfo.Role.Description;
			this.Name = securityRoleInfo.Name;
			this.Users = users;
		}

		/// <summary>
		/// Gets or sets the description of the role.
		/// </summary>
		[Display(Name = "Description", ResourceType = typeof(Locale))]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the name of the role.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Users associated to the role.
		/// </summary>
		[Display(Name = "Users", ResourceType = typeof(Locale))]
		public List<UserViewModel> Users { get; set; }
	}
}