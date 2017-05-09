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
using OpenIZAdmin.Localization;
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
	public class UpdateProfileModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateProfileModel"/> class.
		/// </summary>
		public UpdateProfileModel()
		{
			this.FacilityList = new List<SelectListItem>();
			this.GivenNames = new List<string>();
			this.GivenNamesList = new List<SelectListItem>();
			this.LanguageList = new List<SelectListItem>();
			this.PhoneTypeList = new List<SelectListItem>();
			this.Surnames = new List<string>();
			this.SurnamesList = new List<SelectListItem>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateProfileModel"/> class
		/// with a specific <see cref="UserEntity"/> instance.
		/// </summary>
		/// <param name="userEntity">The <see cref="UserEntity"/> instance.</param>
		public UpdateProfileModel(UserEntity userEntity) : this()
		{
			this.Surnames = userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value).ToList();
			this.GivenNames = userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value).ToList();
			this.Email = userEntity.SecurityUser.Email;
			this.SurnamesList.AddRange(this.Surnames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));
			this.GivenNamesList.AddRange(this.GivenNames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));

			this.Language = userEntity.LanguageCommunication.FirstOrDefault(l => l.IsPreferred)?.LanguageCode;

			this.LanguageList = new List<SelectListItem>
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

            if (userEntity.Telecoms.Any())
            {
                //can have more than one contact - default to show mobile
                if (userEntity.Telecoms.Any(t => t.AddressUseKey == TelecomAddressUseKeys.MobileContact))
                {
                    PhoneNumber = userEntity.Telecoms.First(t => t.AddressUseKey == TelecomAddressUseKeys.MobileContact).Value;
                    PhoneType = TelecomAddressUseKeys.MobileContact.ToString();
                }
                else
                {
                    PhoneNumber = userEntity.Telecoms.FirstOrDefault()?.Value;
                    PhoneType = userEntity.Telecoms.FirstOrDefault()?.AddressUseKey?.ToString();
                }                
            }
            else
            {
                //Default to Mobile - requirement
                PhoneType = TelecomAddressUseKeys.MobileContact.ToString();
            }   
		}

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>		
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessageResourceName = "InvalidEmailAddress", ErrorMessageResourceType = typeof(Locale))]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the id of the facility of the user.
		/// </summary>
		[Display(Name = "Facility", ResourceType = typeof(Locale))]
		public string Facility { get; set; }

		/// <summary>
		/// Gets or sets the list of facilities.
		/// </summary>
		public List<SelectListItem> FacilityList { get; set; }

		/// <summary>
		/// Gets or sets the givens names of the user.
		/// </summary>
		[Display(Name = "GivenName", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "GivenNameRequired", ErrorMessageResourceType = typeof(Locale))]
		public List<string> GivenNames { get; set; }

		/// <summary>
		/// Gets or sets the list of given names of the user.
		/// </summary>
		public List<SelectListItem> GivenNamesList { get; set; }

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
        /// Gets or sets the phone number of the user.
        /// </summary>
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "PhoneNumberRequired", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(25, ErrorMessageResourceName = "PhoneNumberTooLong", ErrorMessageResourceType = typeof(Locale))]        
        [RegularExpression(Constants.RegExPhoneNumberTanzania, ErrorMessageResourceName = "InvalidPhoneNumber", ErrorMessageResourceType = typeof(Locale))]
        //[Phone(ErrorMessageResourceName = "InvalidPhoneNumber", ErrorMessageResourceType = typeof(Locale))]
        public string PhoneNumber { get; set; }

		/// <summary>
		/// Gets or sets the phone type of the user.
		/// </summary>
		[Display(Name = "PhoneType", ResourceType = typeof(Locale))]
		public string PhoneType { get; set; }

		/// <summary>
		/// Gets or sets the types of phones.
		/// </summary>
		public List<SelectListItem> PhoneTypeList { get; set; }

		/// <summary>
		/// Gets or sets the family names of the user.
		/// </summary>
		[Display(Name = "Surname", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "SurnameRequired", ErrorMessageResourceType = typeof(Locale))]
		public List<string> Surnames { get; set; }

		/// <summary>
		/// Gets or sets the list of family names of the user.
		/// </summary>
		public List<SelectListItem> SurnamesList { get; set; }

		/// <summary>
		/// Converts an <see cref="UpdateProfileModel"/> instance to a <see cref="UserEntity"/> instance.
		/// </summary>
		/// <param name="userEntity">The current <see cref="UserEntity"/> instance.</param>
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

			Guid facility;

			// only update the facility if it actually changes
			if (!string.IsNullOrEmpty(this.Facility) &&
				Guid.TryParse(this.Facility, out facility) &&
				userEntity.Relationships.Find(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation && r.TargetEntityKey == facility) == null)
			{
				userEntity.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);
				userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, facility));
			}

			if (!string.IsNullOrEmpty(this.Language))
			{
				var currentLanguage = userEntity.LanguageCommunication.Find(l => l.IsPreferred);

				if (currentLanguage != null)
				{
					userEntity.LanguageCommunication.RemoveAll(l => l.IsPreferred);
				}

				userEntity.LanguageCommunication.Add(new PersonLanguageCommunication(this.Language, true));
			}

			if (!string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrWhiteSpace(PhoneNumber))
			{
				var phoneType = Guid.Empty;

				if (Guid.TryParse(this.PhoneType, out phoneType))
				{
					userEntity.Telecoms.Clear();
					userEntity.Telecoms.Add(new EntityTelecomAddress(phoneType, this.PhoneNumber));
				}
				else
				{
					userEntity.Telecoms.RemoveAll(t => t.AddressUseKey == TelecomAddressUseKeys.MobileContact);
					userEntity.Telecoms.Add(new EntityTelecomAddress(TelecomAddressUseKeys.MobileContact, this.PhoneNumber));
				}
			}

			userEntity.CreationTime = DateTimeOffset.Now;
			userEntity.VersionKey = null;

			return userEntity;
		}
	}
}