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

namespace OpenIZAdmin.Models.UserModels
{
	public class CreateUserModel
	{
		public CreateUserModel()
		{
		}

		public CreateUserModel(SecurityUserInfo userInfo)
		{
			this.Email = userInfo.Email;
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

		[Required]
		[StringLength(255)]
		public string Username { get; set; }

		public SecurityUserInfo ToSecurityUserInfo()
		{
			//List<EntityNameComponent> patientNames = new List<EntityNameComponent>();
			//if (this.GivenNames != null)
			//{
			//	patientNames.AddRange(this.GivenNames.Select(x => new OpenIZ.Core.Model.Entities.EntityNameComponent(NameComponentKeys.Given, x)).ToList());
			//}
			//if (this.FamilyNames != null)
			//{
			//	patientNames.AddRange(this.FamilyNames.Select(x => new OpenIZ.Core.Model.Entities.EntityNameComponent(NameComponentKeys.Family, x)).ToList());
			//}

			SecurityUserInfo userInfo = new SecurityUserInfo
			{
				Email = this.Email,
				Password = this.Password,
				User = new SecurityUser
				{
					Entities = new List<Person>()
				},
				UserName = this.Username
			};

			//userInfo.User.AddPersonNames(NameUseKeys.Legal, NameComponentKeys.Given, new List<string> { this.FirstName });
			//userInfo.User.AddPersonNames(NameUseKeys.Legal, NameComponentKeys.Family, new List<string> { this.LastName });

			List<Guid> roleIds = new List<Guid>();

			foreach (var roleId in Roles)
			{
				Guid role = Guid.Empty;

				if (Guid.TryParse(roleId, out role))
				{
					roleIds.Add(role);
				}
			}

			userInfo.User.CreateRoles(roleIds);

			return userInfo;
		}
	}
}