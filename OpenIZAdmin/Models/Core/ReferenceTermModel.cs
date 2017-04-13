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
 * Date: 2016-11-29
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZAdmin.Models.Core
{
	/// <summary>
	/// Represents a reference term model.
	/// </summary>
	public abstract class ReferenceTermModel
	{        
        /// <summary>
        /// Gets or sets the list of reference term names 
        /// </summary>
        public List<ReferenceTermName> DisplayNames { get; set; }

	    /// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the current language.
        /// </summary>
        /// <value>The language.</value>        
        [Display(Name = "Languages", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        public virtual string Language { get; set; }

        ///// <summary>
        ///// Gets or sets the language list.
        ///// </summary>
        ///// <value>The language list.</value>
        //public List<SelectListItem> LanguageList { get; set; }

        /// <summary>
        /// Gets or sets the mnemonic.
        /// </summary>
        /// <value>The mnemonic.</value>
        [Display(Name = "Mnemonic", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "MnemonicRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(50, ErrorMessageResourceName = "MnemonicLength50", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(256, ErrorMessageResourceName = "NameLength255", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the two letter language code of the language.
        /// </summary>
        [Display(Name = "Language", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string TwoLetterCountryCode { get; set; }

        ///// <summary>
        ///// Gets or sets the version identifier of the Reference Term
        ///// </summary>
        //public Guid? VersionKey { get; set; }
    }
}