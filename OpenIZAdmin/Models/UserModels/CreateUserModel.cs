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

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.UserModels
{
	/// <summary>
	/// Represents a create user model.
	/// </summary>
	public class CreateUserModel : UserModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateUserModel"/> class.
		/// </summary>
		public CreateUserModel()
		{
			this.Surnames = new List<string>();
			this.GivenNames = new List<string>();
            this.PhoneTypeList = new List<SelectListItem>();
            this.RolesList = new List<SelectListItem>();
		}		

		/// <summary>
		/// Gets or sets the password of the user.
		/// </summary>
		[DataType(DataType.Password)]
		[Display(Name = "Password", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(50, ErrorMessageResourceName = "PasswordLength50", ErrorMessageResourceType = typeof(Locale))]		
        [RegularExpression(Constants.RegExPassword, ErrorMessageResourceName = "PasswordValidationErrorMessage", ErrorMessageResourceType = typeof(Locale))]
        public string Password { get; set; }
       
	    /// <summary>
	    /// Gets or sets the username of the user.
	    /// </summary>
	    [Display(Name = "Username", ResourceType = typeof(Locale))]
	    [Required(ErrorMessageResourceName = "UsernameRequired", ErrorMessageResourceType = typeof(Locale))]
	    [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "UsernameLength50", ErrorMessageResourceType = typeof(Locale))]        
        [RegularExpression(Constants.RegExUsername, ErrorMessageResourceName = "UsernameValidationErrorMessage", ErrorMessageResourceType = typeof(Locale))]        
        public string Username { get; set; }	            	    

	    /// <summary>
        /// Converts a <see cref="CreateUserModel"/> instance to a <see cref="SecurityUserInfo"/> instance.
        /// </summary>
        /// <returns>Returns a <see cref="SecurityUserInfo"/> instance.</returns>
        public SecurityUserInfo ToSecurityUserInfo()
		{
			return new SecurityUserInfo
			{
				Lockout = null,
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

		    var facility = ConvertFacilityToGuid();
            if (facility != null)
			{
				userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, facility));
			}

            if (HasPhoneNumberAndType())
            {
                var phoneType = ConvertPhoneTypeToGuid();
                if (phoneType != null)
                {
                    userEntity.Telecoms.Clear();
                    userEntity.Telecoms.Add(new EntityTelecomAddress((Guid)phoneType, PhoneNumber));
                }
                else
                {
                    userEntity.Telecoms.RemoveAll(t => t.AddressUseKey == TelecomAddressUseKeys.MobileContact);
                    userEntity.Telecoms.Add(new EntityTelecomAddress(TelecomAddressUseKeys.MobileContact, PhoneNumber));
                }
            }

            userEntity.CreationTime = DateTimeOffset.Now;
			userEntity.VersionKey = null;

			return userEntity;
		}
	}
}