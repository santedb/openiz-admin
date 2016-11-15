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
	public class EditProviderModel
	{
        public EditProviderModel()
        {            
            this.FamilyNameList = new List<SelectListItem>();
            this.FamilyNames = new List<string>();
            this.GivenNamesList = new List<SelectListItem>();
            this.GivenNames = new List<string>();            
        }

        /// <summary>
        /// Gets or sets the date of birth of the provider.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        public List<SelectListItem> FamilyNameList { get; set; }

        /// <summary>
        /// Gets or sets the family names of the user.
        /// </summary>
        [Display(Name = "FamilyNames", ResourceType = typeof(Localization.Locale))]
        public List<string> FamilyNames { get; set; }        

        public List<SelectListItem> GivenNamesList { get; set; }

        /// <summary>
        /// Gets or sets the givens names of the user.
        /// </summary>
        [Display(Name = "Names", ResourceType = typeof(Localization.Locale))]
        public List<string> GivenNames { get; set; }

        public string Gender { get; set; }       

        public List<string> Prefixes { get; set; }

        public List<string> Suffixes { get; set; }

        /// <summary>
		/// Gets or sets the user id of the user.
		/// </summary>
		[Required]
        public Guid UserId { get; set; }
    }
}