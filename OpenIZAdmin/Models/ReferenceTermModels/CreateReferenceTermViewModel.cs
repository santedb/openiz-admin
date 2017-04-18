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
using OpenIZAdmin.Models.Core;
using System.Web.Mvc;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZAdmin.Models.ReferenceTermModels
{
    /// <summary>
    /// Represents a reference term view model.
    /// </summary>
    public class CreateReferenceTermViewModel : ReferenceTermModel
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="CreateReferenceTermViewModel"/> class.
		/// </summary>
        public CreateReferenceTermViewModel()
        {
            LanguageList = new List<SelectListItem>();            
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="CreateReferenceTermViewModel"/> class.
		/// </summary>
        public CreateReferenceTermViewModel(Guid? conceptId, Guid? conceptVersionId) : this()
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
        /// Converts an <see cref="CreateReferenceTermViewModel"/> instance to a <see cref="ReferenceTerm"/> instance.
        /// </summary>
        /// <returns>Returns a ReferenceTerm instance.</returns>
        public ReferenceTerm ToReferenceTerm()
        {            
            return new ReferenceTerm()
            {                
                Key = Guid.NewGuid(),
                Mnemonic = this.Mnemonic,
                DisplayNames = new List<ReferenceTermName>()
                {
                    new ReferenceTermName()
                    {
                        Key = Guid.NewGuid(),
                        Language = this.TwoLetterCountryCode,
                        Name = this.Name
                    }
                }                
            };
        }
    }
}