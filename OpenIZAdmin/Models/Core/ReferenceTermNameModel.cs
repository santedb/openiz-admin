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
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ReferenceTermNameModels;

namespace OpenIZAdmin.Models.Core
{
    /// <summary>
	/// Represents a reference term name model.
	/// </summary>
    public abstract class ReferenceTermNameModel
    {        
        /// <summary>
        /// Gets or sets the identifier of the reference term name
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets the current language.
        /// </summary>
        /// <value>The language.</value>        
        [Display(Name = "Languages", ResourceType = typeof(Locale))]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the reference term name.
        /// </summary>
        /// <value>The name.</value>                
        [Display(Name = "Name", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(256, ErrorMessageResourceName = "NameLength256", ErrorMessageResourceType = typeof(Locale))]
        [RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the current reference term name.
        /// </summary>
        /// <value>The name.</value>                
        [Display(Name = "Mnemonic", ResourceType = typeof(Locale))]
        [StringLength(50, ErrorMessageResourceName = "MnemonicLength50", ErrorMessageResourceType = typeof(Locale))]
        [RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
        public string Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the reference term.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid? ReferenceTermId { get; set; }        
    }
}