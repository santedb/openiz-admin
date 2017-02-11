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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using OpenIZ.Core.Model.AMI.Auth;

namespace OpenIZAdmin.Models.UserModels
{
    /// <summary>
    /// Class Model for Create User
    /// </summary>
    public class CreateUserModel
	{
        /// <summary>
        /// Constructor to initialize a CreateUserModel instance
        /// </summary>
        public CreateUserModel()
		{
            this.Facilities = new List<string>();            
            this.Surnames = new List<string>();            
            this.GivenNames = new List<string>();
            this.RolesList = new List<SelectListItem>();
		}

        /// <summary>
        /// Gets or sets the email of the user
        /// </summary>
        [Display(Name = "Email", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[EmailAddress(ErrorMessage = null, ErrorMessageResourceName = "InvalidEmailAddress", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Email { get; set; }

        /// <summary>
		/// Gets or sets the id of the facility of the user.
		/// </summary>
		[Display(Name = "Facility", ResourceType = typeof(Localization.Locale))]
        public List<string> Facilities { get; set; }        

        /// <summary>
		/// Gets or sets the family names of the user.
		/// </summary>
		[Display(Name = "Surname", ResourceType = typeof(Localization.Locale))]
        public List<string> Surnames { get; set; }

        /// <summary>
        /// Gets or sets the givens names of the user.
        /// </summary>
        [Display(Name = "GivenName", ResourceType = typeof(Localization.Locale))]
        public List<string> GivenNames { get; set; }
       
        /// <summary>
        /// Gets or sets the user password
        /// </summary>
        [DataType(DataType.Password)]
		[Display(Name = "Password", ResourceType = typeof(Localization.Locale))]
        [StringLength(255, ErrorMessageResourceName = "PasswordLength", ErrorMessageResourceType = typeof(Localization.Locale))]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\d]{8,}$", ErrorMessageResourceName = "PasswordStrength", ErrorMessageResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Localization.Locale))]        
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the list of roles for the user
        /// </summary>
		[Display(Name = "Roles", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "RolesRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		public IEnumerable<string> Roles { get; set; }

        /// <summary>
        /// Gets or sets the list of available roles
        /// </summary>
		public List<SelectListItem> RolesList { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
		[Display(Name = "Username", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "UsernameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "UsernameTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Username { get; set; }

	    public SecurityUserInfo ToSecurityUserInfo()
	    {
		    return new SecurityUserInfo
		    {
			    Email = this.Email,
			    Password = this.Password,
			    UserName = this.Username,
			    Roles = this.Roles.Select(r => new SecurityRoleInfo {Name = r}).ToList()
		    };
	    }
	}
}