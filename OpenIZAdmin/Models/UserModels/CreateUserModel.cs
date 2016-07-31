﻿/*
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
 * Date: 2016-7-17
 */

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.UserModels
{
	public class CreateUserModel
	{
		public CreateUserModel()
		{
			this.RolesList = new List<SelectListItem>();
		}

		[Display(Name = "Email", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[EmailAddress(ErrorMessageResourceName = "InvalidEmailAddress", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Email { get; set; }

		[Display(Name = "FirstName", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "FirstNameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "FirstNameTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string FirstName { get; set; }

		[Display(Name = "LastName", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "LastNameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "LastNameTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string LastName { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Password", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Password { get; set; }

		[Display(Name = "Roles", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "RolesRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		public IEnumerable<string> Roles { get; set; }

		public List<SelectListItem> RolesList { get; set; }

		[Display(Name = "Username", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "UsernameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "UsernameTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Username { get; set; }
	}
}