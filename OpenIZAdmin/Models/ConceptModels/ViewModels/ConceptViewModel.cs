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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZAdmin.Models.ConceptModels.ViewModels
{
	/// <summary>
	/// Represents a model to view a concept.
	/// </summary>
	public class ConceptViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptViewModel"/> class.
		/// </summary>
		public ConceptViewModel()
		{
			this.Names = new List<string>();
			this.Languages = new List<string>();
			this.ReferenceTerms = new List<ReferenceTermModel>();
		}

		public ConceptViewModel(Concept concept) : this()
		{
			this.Class = concept.Class?.Name;
			this.CreationTime = concept.CreationTime.DateTime;
			this.Id = concept.Key.Value;
			this.Mnemonic = concept.Mnemonic;

			concept.ConceptNames.ForEach(c =>
			{
				this.Names.Add(c.Name);
				this.Languages.Add(c.Language);
			});
		}

        /// <summary>
        /// Gets or sets the class of the concept.
        /// </summary>
        [Display(Name = "ConceptClass", ResourceType = typeof(Localization.Locale))]
        public string Class { get; set; }

		/// <summary>
		/// Gets or sets the creation time of the concept.
		/// </summary>
		[Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the key of the concept.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the list of languages associated with the concept.
		/// </summary>
		public List<string> Languages { get; set; }

		/// <summary>
		/// Gets or sets the mnemonic of the concept.
		/// </summary>
		[Display(Name = "Mnemonic", ResourceType = typeof(Localization.Locale))]
		public string Mnemonic { get; set; }

		/// <summary>
		/// Gets or sets a list of names associated with the concept.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		public List<string> Names { get; set; }

		/// <summary>
		/// Gets or sets the list of reference terms associated with the concept.
		/// </summary>
		[Display(Name = "ReferenceTerms", ResourceType = typeof(Localization.Locale))]
		public List<ReferenceTermModel> ReferenceTerms { get; set; }
	}
}