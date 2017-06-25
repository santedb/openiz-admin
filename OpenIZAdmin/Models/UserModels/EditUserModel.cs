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
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;
using OpenIZAdmin.Models.RoleModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

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
			this.GivenName = string.Join(", ", userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value).ToList());
			this.Id = securityUserInfo.UserId.Value;
			this.Language = userEntity.LanguageCommunication.FirstOrDefault(l => l.IsPreferred)?.LanguageCode;
			this.LockoutStatus = securityUserInfo.Lockout.GetValueOrDefault(false).ToLockoutStatus();
			this.IsObsolete = securityUserInfo.User.ObsoletionTime.HasValue;
			this.Roles = securityUserInfo.Roles.Select(r => r.Id.ToString()).ToList();
			this.Surname = string.Join(", ", userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value).ToList());
			this.Username = securityUserInfo.UserName;
			this.UserRoles = securityUserInfo.Roles.Select(r => new RoleViewModel(r)).OrderBy(q => q.Name).ToList();
		}

		/// <summary>
		/// Gets or sets the creation time of the user account.
		/// </summary>
		[Display(Name = "CreationTime", ResourceType = typeof(Locale))]
		public DateTimeOffset CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the list of facilities.
		/// </summary>
		public List<SelectListItem> FacilityList { get; set; }

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
		/// Gets or sets the default language of the user.
		/// </summary>
		[Display(Name = "Language", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Locale))]
		public string Language { get; set; }

		/// <summary>
		/// Gets or sets the list of languages.
		/// </summary>
		public List<SelectListItem> LanguageList { get; set; }

		/// <summary>
		/// Gets or sets the locked out status of the user.
		/// </summary>
		[Display(Name = "LockoutStatus", ResourceType = typeof(Locale))]
		public string LockoutStatus { get; set; }

		/// <summary>
		/// Gets or sets the current roles of the user.
		/// </summary>
		public IEnumerable<RoleViewModel> UserRoles { get; set; }

		/// <summary>
		/// Initializes the Language list drop down
		/// </summary>
		public void CreateLanguageList()
		{
			LanguageList = new List<SelectListItem>
			{
				new SelectListItem
				{
					Text = string.Empty,
					Value = string.Empty
				},
				new SelectListItem
				{
					Selected = this.Language == LocalizationConfig.LanguageCode.English,
					Text = Locale.English,
					Value = LocalizationConfig.LanguageCode.English
				},
				new SelectListItem
				{
					Selected = this.Language == LocalizationConfig.LanguageCode.Swahili,
					Text = Locale.Kiswahili,
					Value = LocalizationConfig.LanguageCode.Swahili
				}
			};
		}

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
			var name = new EntityName
			{
				NameUseKey = NameUseKeys.OfficialRecord,
				Component = new List<EntityNameComponent>()
			};

			if (!string.IsNullOrEmpty(this.GivenName) && !string.IsNullOrWhiteSpace(this.GivenName))
			{
				name.Component.AddRange(this.GivenName.Split(',').Select(n => new EntityNameComponent(NameComponentKeys.Given, n)));
			}

			if (!string.IsNullOrEmpty(this.Surname) && !string.IsNullOrWhiteSpace(this.Surname))
			{
				name.Component.AddRange(this.Surname.Split(',').Select(n => new EntityNameComponent(NameComponentKeys.Family, n)));
			}

			// add the name if there are any components
			if (name.Component.Any())
			{
				userEntity.Names = new List<EntityName> { name };
			}

			var facilityId = this.Facility?.ToGuid();

			if (facilityId == null)
			{
				userEntity.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);
			}
			else if (HasSelectedNewFacility(userEntity, facilityId))
			{
				userEntity.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);
				userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, facilityId));
			}

			if (!string.IsNullOrWhiteSpace(this.Language))
			{
				var currentLanguage = userEntity.LanguageCommunication.FirstOrDefault(l => l.IsPreferred);

				if (currentLanguage != null)
				{
					userEntity.LanguageCommunication.RemoveAll(l => l.IsPreferred);
				}

				userEntity.LanguageCommunication.Add(new PersonLanguageCommunication(this.Language, true));
			}

			if (HasPhoneNumberAndType())
			{
				var phoneType = this.PhoneType.ToGuid();

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