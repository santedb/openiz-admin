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

		public CreateUserModel(SecurityUserInfo userInfo)
		{
			this.Email = userInfo.Email;
			this.RolesList = new List<SelectListItem>();
			this.Username = userInfo.UserName;
		}

		[Required]
		[EmailAddress]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[Required]
		[StringLength(255)]
		public string FirstName { get; set; }

		[Required]
		[StringLength(255)]
		public string LastName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		public IEnumerable<string> Roles { get; set; }

		public List<SelectListItem> RolesList { get; set; }

		[Required]
		[StringLength(255)]
		public string Username { get; set; }
	}
}