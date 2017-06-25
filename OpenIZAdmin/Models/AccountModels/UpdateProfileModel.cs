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
 * Date: 2016-9-5
 */

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.AccountModels
{
	/// <summary>
	/// Represents a model to allow a user to update their profile.
	/// </summary>
	public class UpdateProfileModel : UserModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateProfileModel"/> class.
		/// </summary>
		public UpdateProfileModel()
		{
			this.FacilityList = new List<SelectListItem>();
			this.PhoneTypeList = new List<SelectListItem>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateProfileModel"/> class
		/// with a specific <see cref="UserEntity"/> instance.
		/// </summary>
		/// <param name="userEntity">The <see cref="UserEntity"/> instance.</param>
		public UpdateProfileModel(UserEntity userEntity) : this()
		{
			this.Surname = string.Join(", ", userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value).ToList());
			this.GivenName = string.Join(", ", userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value).ToList());
			this.Email = userEntity.SecurityUser.Email;
			this.Language = userEntity.LanguageCommunication.FirstOrDefault(l => l.IsPreferred)?.LanguageCode;
		}

		/// <summary>
		/// Gets or sets the list of facilities.
		/// </summary>
		public List<SelectListItem> FacilityList { get; set; }

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
		/// Converts an <see cref="UpdateProfileModel"/> instance to a <see cref="UserEntity"/> instance.
		/// </summary>
		/// <param name="userEntity">The current <see cref="UserEntity"/> instance.</param>
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

			// only update the facility if it actually changes
			var facilityId = this.Facility.ToGuid();

			if (facilityId == null)
			{
				userEntity.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);
			}
			else if (HasSelectedNewFacility(userEntity, facilityId))
			{
				userEntity.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);
				userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, facilityId));
			}

			if (!string.IsNullOrWhiteSpace(Language))
			{
				var currentLanguage = userEntity.LanguageCommunication.Find(l => l.IsPreferred);

				if (currentLanguage != null)
				{
					userEntity.LanguageCommunication.RemoveAll(l => l.IsPreferred);
				}

				userEntity.LanguageCommunication.Add(new PersonLanguageCommunication(this.Language, true));
			}

			if (HasPhoneNumberAndType())
			{
				var phoneType = this.PhoneType?.ToGuid();
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