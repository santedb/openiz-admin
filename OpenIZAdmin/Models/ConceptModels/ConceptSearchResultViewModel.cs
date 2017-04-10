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

using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OpenIZAdmin.Models.ConceptModels
{
	/// <summary>
	/// Represents a view model for a concept search result.
	/// </summary>
	public class ConceptSearchResultViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptSearchResultViewModel"/> class.
		/// </summary>
		public ConceptSearchResultViewModel()
		{
			this.Names = new List<string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptSearchResultViewModel"/> class
		/// with a specific <see cref="Concept"/> instance.
		/// </summary>
		/// <param name="concept">The <see cref="Concept"/> instance.</param>
		public ConceptSearchResultViewModel(Concept concept)
		{
			CreationTime = concept.CreationTime.DateTime;
			IsReadOnly = concept.IsSystemConcept;
			Id = concept.Key ?? Guid.Empty;
            Mnemonic = concept.Mnemonic;
			Names = concept.ConceptNames.Select(c => c.Name).ToList();
		    ConceptNames = string.Join(", ", Names);
            Type = ConceptType.Concept;
		}

		///// <summary>
		///// Initializes a new instance of the <see cref="ConceptSearchResultViewModel"/> class
		///// with a specific <see cref="ConceptSet"/> instance.
		///// </summary>
		///// <param name="conceptSet">The <see cref="ConceptSet"/> instance.</param>
		//public ConceptSearchResultViewModel(ConceptSet conceptSet)
		//{
		//	this.CreationTime = conceptSet.CreationTime.DateTime;
		//	this.IsReadOnly = false;
		//	this.Id = conceptSet.Key ?? Guid.Empty;
		//	this.Mnemonic = conceptSet.Mnemonic;
  //          this.Names = new List<string> { conceptSet.Name };		    
  //          ConceptNames = string.Join(", ", Names);
  //          this.Type = ConceptType.ConceptSet;
		//}

        /// <summary>
        /// Gets or sets the string of Concept Names
        /// </summary>
        public string ConceptNames { get; set; }

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
		public List<string> Names { get; set; }

		/// <summary>
		/// Gets or sets the type of the concept search result.
		/// </summary>
		[Display(Name = "Type", ResourceType = typeof(Localization.Locale))]
		public ConceptType Type { get; set; }
	}
}