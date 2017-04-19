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
 * Date: 2017-4-12
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Models.Core;
using OpenIZAdmin.Models.ReferenceTermNameModels;

namespace OpenIZAdmin.Models.ReferenceTermModels
{
    /// <summary>
	/// Represents a model to view a reference term.
	/// </summary>
    public class ReferenceTermViewModel : ReferenceTermModel
    {

        /// <summary>
		/// Initializes a new instance of the <see cref="ReferenceTermViewModel"/> class.
		/// </summary>
		public ReferenceTermViewModel()
        {            
            DisplayNames = new List<ReferenceTermName>();
            ReferenceTermNamesList = new List<ReferenceTermNameViewModel>();
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="ReferenceTermViewModel"/> class.
		/// </summary>
		public ReferenceTermViewModel(ReferenceTerm referenceTerm) : this()
        {
            DisplayNames = referenceTerm.DisplayNames;
            Id = referenceTerm.Key ?? Guid.Empty;            
            Mnemonic = referenceTerm.Mnemonic;            
            Name = string.Join(" ", referenceTerm.DisplayNames.Select(d => d.Name));
            ReferenceTermNamesList = referenceTerm.DisplayNames.Select(n => new ReferenceTermNameViewModel(n)).ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTermViewModel"/> class.
        /// </summary>
        public ReferenceTermViewModel(ReferenceTerm term, Concept concept) : this(term)
        {
            ConceptId = concept?.Key;
            ConceptVersionKey = concept?.VersionKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTermViewModel"/> class.
        /// </summary>
        public ReferenceTermViewModel(Concept concept)
        {
            ConceptId = concept?.Key;
            ConceptVersionKey = concept?.VersionKey;            
        }



        /// <summary>
        /// Gets or sets the concept identifier associated with the reference term
        /// </summary>
        public Guid? ConceptId { get; set; }
        /// <summary>
        ///  Gets or sets the concept version identifier associated with the reference term
        /// </summary>
        public Guid? ConceptVersionKey { get; set; }

        /// <summary>
        /// Gets or sets the creation time of the concept.
        /// </summary>
        [Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the concatenated display names of the Reference Term
        /// </summary>
        [Display(Name = "Names", ResourceType = typeof(Localization.Locale))]
        public string Names { get; set; }

        /// <summary>
        /// Gets or sets the list of reference names associated with the reference term
        /// </summary>
        public List<ReferenceTermNameViewModel> ReferenceTermNamesList{ get; set; }
    }
}