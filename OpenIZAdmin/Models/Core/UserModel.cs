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
 * User: Andrew
 * Date: 2017-5-10
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.Core
{
    /// <summary>
    /// Represents a model of a user.
    /// </summary>
    public abstract class UserModel
    {
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>		
        [Display(Name = "Email", ResourceType = typeof(Locale))]
        [EmailAddress(ErrorMessage = null, ErrorMessageResourceName = "InvalidEmailAddress",
            ErrorMessageResourceType = typeof(Locale))]
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
        [Required(ErrorMessageResourceName = "GivenNameRequired", ErrorMessageResourceType = typeof(Locale))]
        public List<string> GivenNames { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "PhoneNumberRequired", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(25, ErrorMessageResourceName = "PhoneNumberTooLong", ErrorMessageResourceType = typeof(Locale))]
        [RegularExpression(Constants.RegExPhoneNumberTanzania, ErrorMessageResourceName = "InvalidPhoneNumber",
            ErrorMessageResourceType = typeof(Locale))]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the phone type of the user.
        /// </summary>
        [Display(Name = "PhoneType", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "PhoneTypeRequired", ErrorMessageResourceType = typeof(Locale))]
        public string PhoneType { get; set; }

        /// <summary>
        /// Gets or sets the types of phones.
        /// </summary>
        public List<SelectListItem> PhoneTypeList { get; set; }

        /// <summary>
        /// Gets or sets the list of roles of the user.
        /// </summary>
        [Display(Name = "Roles", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "RolesRequired", ErrorMessageResourceType = typeof(Locale))]
        public List<string> Roles { get; set; }

        /// <summary>
        /// Gets or sets the list of available roles
        /// </summary>
        public List<SelectListItem> RolesList { get; set; }

        /// <summary>
        /// Gets or sets the family names of the user.
        /// </summary>
        [Display(Name = "Surname", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "SurnameRequired", ErrorMessageResourceType = typeof(Locale))]
        public List<string> Surnames { get; set; }

        /// <summary>
        /// Checks if any of the Role(s) assigned are an empty selection
        /// </summary>
        /// <returns>Returns true if an empty string is contained in the List</returns>
        public void CheckForEmptyRoleAssigned()
        {
            if (Roles != null && Roles.Any())
            {
                Roles.RemoveAll(string.IsNullOrWhiteSpace);
            }
            //return Roles.Any() && Roles.All(r => string.IsNullOrWhiteSpace(r) || string.IsNullOrEmpty(r));
        }

        /// <summary>
        /// Converts the phone type entry from string to a guid
        /// </summary>
        /// <returns>Returns the phone type guid, null if the operation was unsuccessful</returns>
        public Guid? ConvertFacilityToGuid()
        {
            if (string.IsNullOrWhiteSpace(Facility)) return null;

            Guid facilityId;

            if (Guid.TryParse(Facility, out facilityId)) return facilityId;
            
            return null;
        }

        /// <summary>
        /// Converts the phone type entry from string to a guid
        /// </summary>
        /// <returns>Returns the phone type guid, null if the operation was unsuccessful</returns>
        public Guid? ConvertPhoneTypeToGuid()
        {
            if (string.IsNullOrWhiteSpace(PhoneType)) return null;

            Guid phoneTypeId;

            if (Guid.TryParse(PhoneType, out phoneTypeId)) return phoneTypeId;

            return null;
        }

        /// <summary>
        /// Checks if the Phone Number and Type input contains an entry
        /// </summary>
        /// <returns>Returns true if a number and type exists, false if no phone number or type is assigned</returns>
        public bool HasPhoneNumberAndType() => !string.IsNullOrWhiteSpace(PhoneNumber) && !string.IsNullOrWhiteSpace(PhoneType);                     
    }
}