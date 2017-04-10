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
 * Date: 2017-4-10
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Models.ConceptModels;

namespace OpenIZAdmin.Models.ConceptSetModels
{
    /// <summary>
	/// Represents a view model for a concept set search result.
	/// </summary>
    public class ConceptSetSearchResultViewModel
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="ConceptSetSearchResultViewModel"/> class.
		/// </summary>
		public ConceptSetSearchResultViewModel()
        {            
            ConceptList = new List<ConceptViewModel>();
        }        

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptSetSearchResultViewModel"/> class
        /// with a specific <see cref="ConceptSet"/> instance.
        /// </summary>
        /// <param name="conceptSet">The <see cref="ConceptSet"/> instance.</param>
        public ConceptSetSearchResultViewModel(ConceptSet conceptSet) : this()
        {
            this.CreationTime = conceptSet.CreationTime.DateTime;
            this.IsReadOnly = false;
            this.Id = conceptSet.Key ?? Guid.Empty;
            this.Mnemonic = conceptSet.Mnemonic;
            this.Name = conceptSet.Name;            
            this.Type = ConceptType.ConceptSet;            
            ConceptList = conceptSet.Concepts.Select(c => new ConceptViewModel(c)).ToList();
        }      

        /// <summary>
        /// Gets or sets the List of Concepts associated with the Concept Set
        /// </summary>
        public List<ConceptViewModel> ConceptList { get; set; }

        /// <summary>
        /// Gets or sets the creation time of the concept or concept set.
        /// </summary>
        [Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the key of the concept or concept set.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets whether the concept/concept set is readonly.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the mnemonic of the concept or concept set.
        /// </summary>
        [Display(Name = "Mnemonic", ResourceType = typeof(Localization.Locale))]
        public string Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets a list of names of the concept.
        /// </summary>
        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the concept search result.
        /// </summary>
        [Display(Name = "Type", ResourceType = typeof(Localization.Locale))]
        public ConceptType Type { get; set; }
    }
}