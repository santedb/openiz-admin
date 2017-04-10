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
 * Date: 2016-7-23
 */

using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.ConceptSetModels
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateConceptSetModel"/> class.
    /// </summary>
    public class CreateConceptSetModel : ConceptSetModel
	{		
        /// <summary>
        /// Gets or sets the mnemonic of the concept.
        /// </summary>
        [Display(Name = "Mnemonic", ResourceType = typeof(Localization.Locale))]
        [Required]
		public override string Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the name of the concept set.
        /// </summary>
        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        [Required]
		public override string Name { get; set; }

        /// <summary>
        /// Get or sets the OID of the concept set.
        /// </summary>
        [Required]
		public override string Oid { get; set; }

        /// <summary>
        /// Get or sets the URL of the concept set.
        /// </summary>
		[Required]
		public override string Url { get; set; }
	}
}