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
using OpenIZAdmin.Models.Core;
using OpenIZAdmin.Models.RoleModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;

namespace OpenIZAdmin.Models.UserModels
{
	/// <summary>
	/// Represents a user view model.
	/// </summary>
	public class UserViewModel : SecurityViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UserViewModel"/> class.
		/// </summary>
		public UserViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserViewModel"/> class
		/// with a specific <see cref="SecurityUserInfo"/> instance.
		/// </summary>
		/// <param name="securityUserInfo">The <see cref="SecurityUserInfo"/> instance.</param>
		public UserViewModel(SecurityUserInfo securityUserInfo) : base(securityUserInfo)
		{
			this.Email = securityUserInfo.Email;
			this.HasRoles = securityUserInfo.Roles?.Any() == true;
			this.IsLockedOut = securityUserInfo.Lockout.GetValueOrDefault(false);
			this.LastLoginTime = securityUserInfo.User.LastLoginTime?.DateTime;
			this.PhoneNumber = securityUserInfo.User.PhoneNumber;
            this.Roles = new List<RoleViewModel>();            
            this.Username = securityUserInfo.UserName;
		}

        /// <summary>
		/// Initializes a new instance of the <see cref="UserViewModel"/> class
		/// with a specific <see cref="SecurityUserInfo"/> instance.
		/// </summary>
		/// <param name="userEntity">The <see cref="UserEntity"/> instance.</param>
		/// <param name="securityUserInfo">The <see cref="SecurityUserInfo"/> instance.</param>
		public UserViewModel(UserEntity userEntity, SecurityUserInfo securityUserInfo) : base(securityUserInfo)
        {
            this.Email = securityUserInfo.Email;
            this.HasRoles = securityUserInfo.Roles?.Any() == true;
            this.IsLockedOut = securityUserInfo.Lockout.GetValueOrDefault(false);
            this.LastLoginTime = securityUserInfo.User.LastLoginTime?.DateTime;
            this.Roles = new List<RoleViewModel>();
            this.Username = securityUserInfo.UserName;

            var given = userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value).ToList();
            var family = userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value).ToList();
            Name = string.Join(" ", given) + " " + string.Join(" ", family);

            if (userEntity.Telecoms.Any(t => t.AddressUseKey == TelecomAddressUseKeys.MobileContact))
            {
                this.PhoneNumber = userEntity.Telecoms.First(t => t.AddressUseKey == TelecomAddressUseKeys.MobileContact).Value;             
            }
            else
            {
                this.PhoneNumber = userEntity.Telecoms.FirstOrDefault()?.Value;                
            }            

            if (this.HasRoles) this.Roles = securityUserInfo.Roles.Select(r => new RoleViewModel(r));                        
        }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [Display(Name = "Email", ResourceType = typeof(Localization.Locale))]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets whether the user has roles assigned
		/// </summary>
		public bool HasRoles { get; set; }

		/// <summary>
		/// Gets or sets the health facility of the user.
		/// </summary>
		[Display(Name = "HealthFacility", ResourceType = typeof(Localization.Locale))]
		public string HealthFacility { get; set; }

		/// <summary>
		/// Gets or sets the locked out status of the user.
		/// </summary>
		[Display(Name = "LockedOut", ResourceType = typeof(Localization.Locale))]
		public bool IsLockedOut { get; set; }

		/// <summary>
		/// Gets or sets the last login time of the user.
		/// </summary>
		[Display(Name = "LastLoginTime", ResourceType = typeof(Localization.Locale))]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
		public DateTime? LastLoginTime { get; set; }

		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the phone number of the user.
		/// </summary>
		[Display(Name = "Phone", ResourceType = typeof(Localization.Locale))]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// Gets or sets the roles of the user.
		/// </summary>
		[Display(Name = "Roles", ResourceType = typeof(Localization.Locale))]
		public IEnumerable<RoleViewModel> Roles { get; set; }

		/// <summary>
		/// Gets or sets the username of the user.
		/// </summary>
		[Display(Name = "Username", ResourceType = typeof(Localization.Locale))]
		public string Username { get; set; }
	}
}