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
 * Date: 2017-5-16
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.Core;
using OpenIZAdmin.Models.ReferenceTermModels;

namespace OpenIZAdmin.Models.AuthorityScope
{
    /// <summary>
	/// Represents a model of an authority scope.
	/// </summary>
    public sealed class AuthorityScopeViewModel : ConceptModel
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="ConceptViewModel"/> class.
		/// </summary>
		public AuthorityScopeViewModel()
        {
            this.Names = new List<string>();
            this.ReferenceTerms = new List<ReferenceTermViewModel>();
            //this.Languages = new List<Language>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptViewModel"/> class.
        /// </summary>
        /// <param name="concept">The concept.</param>
        public AuthorityScopeViewModel(Concept concept) : this()
		{
            Class = concept.Class?.Name;
            AssigingAuthorityId = Guid.Empty;            
            Id = concept.Key ?? Guid.Empty;            
            Mnemonic = concept.Mnemonic;
            Names = concept.ConceptNames.Select(c => c.Name).ToList();
            ConceptNames = (Names.Any()) ? string.Join(", ", Names) : string.Empty;            
            VersionKey = concept.VersionKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptViewModel"/> class.
        /// </summary>
        /// <param name="concept">The concept.</param>
        /// <param name="assigningAuthorityId">The guid of the Assigning Authority that the Concept is associated with</param>
        public AuthorityScopeViewModel(Concept concept, Guid assigningAuthorityId) : this(concept)
		{
            AssigingAuthorityId = assigningAuthorityId;
        }

        /// <summary>
        /// Gets or sets the class of the concept.
        /// </summary>
        [Display(Name = "ConceptClass", ResourceType = typeof(Localization.Locale))]
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets the Concept Set identifier associated with the Concept instance
        /// </summary>
        public Guid? AssigingAuthorityId { get; set; }       
    }
}