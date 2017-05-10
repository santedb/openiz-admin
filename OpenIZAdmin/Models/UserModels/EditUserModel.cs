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
 * User: khannan
 * Date: 2016-8-14
 */

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Models.RoleModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.UserModels
{
	/// <summary>
	/// Represents an edit user model.
	/// </summary>
	public class EditUserModel : UserModel
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="EditUserModel"/> class.
		/// </summary>
		public EditUserModel()
		{
			this.FacilityList = new List<SelectListItem>();
			this.SurnameList = new List<SelectListItem>();
			this.Surnames = new List<string>();
			this.GivenNames = new List<string>();
			this.GivenNamesList = new List<SelectListItem>();
			this.PhoneTypeList = new List<SelectListItem>();
			this.RolesList = new List<SelectListItem>();
			this.Roles = new List<string>();
			this.UserRoles = new List<RoleViewModel>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditUserModel"/> class
		/// with a specific <see cref="UserEntity"/> instance and
		/// a specific <see cref="SecurityUserInfo"/> instance.
		/// </summary>
		/// <param name="userEntity">The <see cref="UserEntity"/> instance.</param>
		/// <param name="securityUserInfo">The <see cref="SecurityUserInfo"/> instance.</param>
		public EditUserModel(UserEntity userEntity, SecurityUserInfo securityUserInfo) : this()
		{
			this.CreationTime = securityUserInfo.User.CreationTime.DateTime;
			this.Email = securityUserInfo.User.Email;
			this.GivenNames = userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value).ToList();
			this.GivenNamesList.AddRange(this.GivenNames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));
			this.Surnames = userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value).ToList();
			this.SurnameList.AddRange(this.Surnames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));
			this.Roles = securityUserInfo.Roles.Select(r => r.Id.ToString()).ToList();
			this.Id = securityUserInfo.UserId.Value;
			this.UserRoles = securityUserInfo.Roles.Select(r => new RoleViewModel(r)).OrderBy(q => q.Name).ToList();

			if (userEntity.Telecoms.Any(t => t.AddressUseKey == TelecomAddressUseKeys.MobileContact))
			{
				this.PhoneNumber = userEntity.Telecoms.First(t => t.AddressUseKey == TelecomAddressUseKeys.MobileContact).Value;
				this.PhoneType = TelecomAddressUseKeys.MobileContact.ToString();
			}
			else
			{
				this.PhoneNumber = userEntity.Telecoms.FirstOrDefault()?.Value;
				this.PhoneType = userEntity.Telecoms.FirstOrDefault()?.AddressUseKey?.ToString();
			}

			this.IsObsolete = securityUserInfo.User.ObsoletionTime != null;
		}

		/// <summary>
		/// Gets or sets the creation time of the user account.
		/// </summary>
		[Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
		public DateTimeOffset CreationTime { get; set; }  

		/// <summary>
		/// Gets or sets the list of facilities.
		/// </summary>
		public List<SelectListItem> FacilityList { get; set; }		

		/// <summary>
		/// Gets or sets the list of given names.
		/// </summary>
		public List<SelectListItem> GivenNamesList { get; set; }

		/// <summary>
		/// Gets or sets the user id of the user.
		/// </summary>
		[Required]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets whether the security entity is obsolete.
		/// </summary>
		public bool IsObsolete { get; set; }        

		/// <summary>
		/// Gets or sets the list of family names.
		/// </summary>
		public List<SelectListItem> SurnameList { get; set; }		

		/// <summary>
		/// Gets or sets the current roles of the user.
		/// </summary>
		public IEnumerable<RoleViewModel> UserRoles { get; set; }     

        /// <summary>
        /// Converts an <see cref="EditUserModel"/> instance to a <see cref="SecurityUserInfo"/> instance.
        /// </summary>
        /// <param name="userEntity">The <see cref="UserEntity"/> instance.</param>
        /// <returns>Returns a <see cref="SecurityUserInfo"/> instance.</returns>
        public SecurityUserInfo ToSecurityUserInfo(UserEntity userEntity)
		{
			var securityUserInfo = new SecurityUserInfo
			{
				Email = this.Email,
				UserId = this.Id,
				User = userEntity.SecurityUser
			};

			securityUserInfo.User.Email = this.Email;
			securityUserInfo.User.PhoneNumber = this.PhoneNumber;

			return securityUserInfo;
		}

		/// <summary>
		/// Converts an <see cref="EditUserModel"/> instance to an <see cref="UserEntity"/> instance.
		/// </summary>
		/// <param name="userEntity">The <see cref="UserEntity"/> instance.</param>
		/// <returns>Returns a <see cref="UserEntity"/> instance.</returns>
		public UserEntity ToUserEntity(UserEntity userEntity)
		{
			if (this.Surnames.Any() || this.GivenNames.Any())
			{
				userEntity.Names.RemoveAll(n => n.NameUseKey == NameUseKeys.OfficialRecord);

				var name = new EntityName
				{
					NameUseKey = NameUseKeys.OfficialRecord,
					Component = new List<EntityNameComponent>()
				};

				name.Component.AddRange(this.Surnames.Select(n => new EntityNameComponent(NameComponentKeys.Family, n)));
				name.Component.AddRange(this.GivenNames.Select(n => new EntityNameComponent(NameComponentKeys.Given, n)));

				userEntity.Names = new List<EntityName> { name };
			}

            var facilityId = ConvertFacilityToGuid();

            if (facilityId == null)
            {
                userEntity.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);
            }
            else if (HasSelectedNewFacility(userEntity, facilityId))
            {
                userEntity.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);
                userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, facilityId));
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