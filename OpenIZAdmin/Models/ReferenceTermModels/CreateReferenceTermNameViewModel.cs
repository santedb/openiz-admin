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
 * Date: 2017-4-13
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Models.Core;
using OpenIZAdmin.Models.PolicyModels;

namespace OpenIZAdmin.Models.ReferenceTermModels
{
    /// <summary>
	/// Represents a reference term name model class.
	/// </summary>
    public class CreateReferenceTermNameViewModel : ReferenceTermNameModel
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="CreateReferenceTermNameViewModel"/> class.
		/// </summary>
		public CreateReferenceTermNameViewModel()
        {
            LanguageList = new List<SelectListItem>();
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="CreateReferenceTermNameViewModel"/> class.
		/// </summary>
		public CreateReferenceTermNameViewModel(ReferenceTerm referenceTerm) : this()
        {
            ReferenceTermId = referenceTerm.Key ?? Guid.Empty;
        }

        /// <summary>
        /// Gets or sets the language list.
        /// </summary>
        /// <value>The language list.</value>
        public List<SelectListItem> LanguageList { get; set; }

        /// <summary>
        /// Gets or sets the two letter language code of the language.
        /// </summary>
        [Display(Name = "Language", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string TwoLetterCountryCode { get; set; }
    }
}