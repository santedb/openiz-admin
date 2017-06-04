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
 * Date: 2017-4-17
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.ReferenceTermNameModels
{
    /// <summary>
    /// Represents a reference term view model.
    /// </summary>
    public class EditReferenceTermNameViewModel : ReferenceTermNameModel
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="EditReferenceTermNameViewModel"/> class.
		/// </summary>
        public EditReferenceTermNameViewModel()
        {
            LanguageList = new List<SelectListItem>();
            ReferenceTermNameList = new List<ReferenceTermNameViewModel>();
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="EditReferenceTermNameViewModel"/> class.
		/// </summary>
        public EditReferenceTermNameViewModel(ReferenceTermName termName) : this()
        {
            Id = termName.Key;
            Name = termName.Name;
            Language = termName.Language;
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="EditReferenceTermNameViewModel"/> class.
		/// </summary>
        public EditReferenceTermNameViewModel(ReferenceTerm referenceTerm) : this()
        {
            ReferenceTermId = referenceTerm.Key;
            Mnemonic = referenceTerm.Mnemonic;
            ReferenceTermNameList = referenceTerm.DisplayNames.Select(n => new ReferenceTermNameViewModel(n)).ToList();
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="EditReferenceTermNameViewModel"/> class.
		/// </summary>
        public EditReferenceTermNameViewModel(ReferenceTerm referenceTerm, ReferenceTermName termName) : this(termName)
        {
            ReferenceTermId = referenceTerm.Key;
            Mnemonic = referenceTerm.Mnemonic;
            ReferenceTermNameList = referenceTerm.DisplayNames.Select(n => new ReferenceTermNameViewModel(n)).ToList();
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="EditReferenceTermNameViewModel"/> class.
		/// </summary>
        public EditReferenceTermNameViewModel(Guid? conceptId, Guid? conceptVersionId) : this()
        {
            ConceptId = conceptId;
            ConceptVersionId = conceptVersionId;
        }

        /// <summary>
        /// Gets or sets the Concept identifier
        /// </summary>
        public Guid? ConceptId { get; set; }

        /// <summary>
        /// Gets or sets the Concept version identifier
        /// </summary>
        public Guid? ConceptVersionId { get; set; }

        /// <summary>
        /// Gets or sets the language list.
        /// </summary>
        /// <value>The language list.</value>
        public List<SelectListItem> LanguageList { get; set; }

        /// <summary>
        /// Gets or sets the reference term name list
        /// </summary>
        public List<ReferenceTermNameViewModel> ReferenceTermNameList { get; set; }

        /// <summary>
        /// Gets or sets the two letter language code of the language.
        /// </summary>
        [Display(Name = "Language", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Locale))]
        public string TwoLetterCountryCode { get; set; }
    }
}