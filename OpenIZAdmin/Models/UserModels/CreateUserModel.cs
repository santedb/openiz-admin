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
 * Date: 2016-7-17
 */

using System;
using OpenIZ.Core.Model.AMI.Auth;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.UserModels
{
	/// <summary>
	/// Represents a create user model.
	/// </summary>
	public class CreateUserModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateUserModel"/> class.
		/// </summary>
		public CreateUserModel()
		{
			this.Surnames = new List<string>();
			this.GivenNames = new List<string>();
			this.RolesList = new List<SelectListItem>();
		}

		/// <summary>
		/// Gets or sets the email address of the user.
		/// </summary>
		[Display(Name = "Email", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Locale))]
		[EmailAddress(ErrorMessage = null, ErrorMessageResourceName = "InvalidEmailAddress", ErrorMessageResourceType = typeof(Locale))]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the id of the facility of the user.
		/// </summary>
		[Display(Name = "Facility", ResourceType = typeof(Locale))]
		public string Facility { get; set; }

		/// <summary>
		/// Gets or sets the givens names of the user.
		/// </summary>
		[Display(Name = "GivenName", ResourceType = typeof(Locale))]
		public List<string> GivenNames { get; set; }

		/// <summary>
		/// Gets or sets the password of the user.
		/// </summary>
		[DataType(DataType.Password)]
		[Display(Name = "Password", ResourceType = typeof(Locale))]
		[StringLength(255, ErrorMessageResourceName = "PasswordLength", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\d]{8,}$", ErrorMessageResourceName = "PasswordStrength", ErrorMessageResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Locale))]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the list of roles of the user.
		/// </summary>
		[Display(Name = "Roles", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "RolesRequired", ErrorMessageResourceType = typeof(Locale))]
		public IEnumerable<string> Roles { get; set; }

		/// <summary>
		/// Gets or sets the list of available roles
		/// </summary>
		public List<SelectListItem> RolesList { get; set; }

		/// <summary>
		/// Gets or sets the family names of the user.
		/// </summary>
		[Display(Name = "Surname", ResourceType = typeof(Locale))]
		public List<string> Surnames { get; set; }

		/// <summary>
		/// Gets or sets the username of the user.
		/// </summary>
		[Display(Name = "Username", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "UsernameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(255, ErrorMessageResourceName = "UsernameTooLong", ErrorMessageResourceType = typeof(Locale))]
		public string Username { get; set; }

		/// <summary>
		/// Converts a <see cref="CreateUserModel"/> instance to a <see cref="SecurityUserInfo"/> instance.
		/// </summary>
		/// <returns>Returns a <see cref="SecurityUserInfo"/> instance.</returns>
		public SecurityUserInfo ToSecurityUserInfo()
		{
			return new SecurityUserInfo
			{
				Email = this.Email,
				Password = this.Password,
				UserName = this.Username,
				Roles = this.Roles.Select(r => new SecurityRoleInfo { Name = r }).ToList()
			};
		}

		/// <summary>
		/// Converts a <see cref="CreateUserModel"/> instance to a <see cref="UserEntity"/>.
		/// </summary>
		/// <param name="userEntity">The <see cref="UserEntity"/> instance.</param>
		/// <returns>Returns a <see cref="UserEntity"/> instance.</returns>
		public UserEntity ToUserEntity(UserEntity userEntity)
		{
			if (this.Surnames.Any() || this.GivenNames.Any())
			{
				var name = new EntityName
				{
					NameUseKey = NameUseKeys.OfficialRecord,
					Component = new List<EntityNameComponent>()
				};

				name.Component.AddRange(this.Surnames.Select(n => new EntityNameComponent(NameComponentKeys.Family, n)));
				name.Component.AddRange(this.GivenNames.Select(n => new EntityNameComponent(NameComponentKeys.Given, n)));

				userEntity.Names = new List<EntityName> { name };
			}

			var facility = Guid.Empty;

			if (!string.IsNullOrEmpty(this.Facility) && Guid.TryParse(this.Facility, out facility))
			{
				userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, facility));
			}

			return userEntity;
		}
	}
}