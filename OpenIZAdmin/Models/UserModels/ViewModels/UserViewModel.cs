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

using OpenIZAdmin.Models.RoleModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.UserModels.ViewModels
{
	/// <summary>
	/// Represents a user view model.
	/// </summary>
	public class UserViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UserViewModel"/> class.
		/// </summary>
		public UserViewModel()
		{
		}

		[Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
		public DateTimeOffset CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the email address of the user.
		/// </summary>
		[Display(Name = "Email", ResourceType = typeof(Localization.Locale))]
		public string Email { get; set; }

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
		/// Gets or sets the obsolete status of the user.
		/// </summary>
		public bool IsObsolete { get; set; }

		/// <summary>
		/// Gets or sets the last login time of the user.
		/// </summary>
		[Display(Name = "LastLoginTime", ResourceType = typeof(Localization.Locale))]
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
		/// Gets or sets the id of the user.
		/// </summary>
		[Display(Name = "UserId", ResourceType = typeof(Localization.Locale))]
		public Guid UserId { get; set; }

		/// <summary>
		/// Gets or sets the username of the user.
		/// </summary>
		[Display(Name = "Username", ResourceType = typeof(Localization.Locale))]
		public string Username { get; set; }
	}
}