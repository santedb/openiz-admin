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
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
    }
}