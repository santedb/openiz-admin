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
 * Date: 2016-8-15
 */

using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace OpenIZAdmin.Models.ProviderModels
{
	public class CreateProviderModel
	{
		public CreateProviderModel()
		{
		}

        /// <summary>
        /// Gets or sets the date of birth of the provider.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
		/// Gets or sets the family names of the provider.
		/// </summary>
		[Display(Name = "FamilyNames", ResourceType = typeof(Localization.Locale))]
        public List<string> FamilyNames { get; set; }

        public string Gender { get; set; }

		public List<string> GivenNames { get; set; }

		public List<string> Prefixes { get; set; }

		public List<string> Suffixes { get; set; }




        //public CreateUserModel()
        //{
        //    this.FacilityList = new List<SelectListItem>();
        //    this.RolesList = new List<SelectListItem>();
        //}

        //[Display(Name = "Email", ResourceType = typeof(Localization.Locale))]
        //[Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        //[EmailAddress(ErrorMessage = null, ErrorMessageResourceName = "InvalidEmailAddress", ErrorMessageResourceType = typeof(Localization.Locale))]
        //public string Email { get; set; }

        ///// <summary>
        ///// Gets or sets the id of the facility of the user.
        ///// </summary>
        //[Display(Name = "Facility", ResourceType = typeof(Localization.Locale))]
        //[Required(ErrorMessageResourceName = "FacilityRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        //public string FacilityId { get; set; }

        ///// <summary>
        ///// Gets or sets the list of facilities.
        ///// </summary>
        //public List<SelectListItem> FacilityList { get; set; }

        ///// <summary>
        ///// Gets or sets the family names of the user.
        ///// </summary>
        //[Display(Name = "FamilyNames", ResourceType = typeof(Localization.Locale))]
        //public List<string> FamilyNames { get; set; }

        ///// <summary>
        ///// Gets or sets the givens names of the user.
        ///// </summary>
        //[Display(Name = "Names", ResourceType = typeof(Localization.Locale))]
        //public List<string> GivenNames { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Password", ResourceType = typeof(Localization.Locale))]
        //[Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        //public string Password { get; set; }

        //[Display(Name = "Roles", ResourceType = typeof(Localization.Locale))]
        //[Required(ErrorMessageResourceName = "RolesRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        //public IEnumerable<string> Roles { get; set; }

        //public List<SelectListItem> RolesList { get; set; }

        //[Display(Name = "Username", ResourceType = typeof(Localization.Locale))]
        //[Required(ErrorMessageResourceName = "UsernameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        //[StringLength(255, ErrorMessageResourceName = "UsernameTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
        //public string Username { get; set; }

    }
}