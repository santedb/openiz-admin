/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
	public class UserViewModel
	{
		public UserViewModel()
		{
		}

		[Display(Name = "Email", ResourceType = typeof(Localization.Locale))]
		public string Email { get; set; }

		public int InvalidLoginAttempts { get; set; }

		public bool IsLockedOut { get; set; }

		public DateTime? LastLoginTime { get; set; }

		public string PhoneNumber { get; set; }

		[Display(Name = "Roles", ResourceType = typeof(Localization.Locale))]
		public IEnumerable<RoleViewModel> Roles { get; set; }

		[Display(Name = "UserId", ResourceType = typeof(Localization.Locale))]
		public Guid UserId { get; set; }

		[Display(Name = "Username", ResourceType = typeof(Localization.Locale))]
		public string Username { get; set; }
	}
}