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
 * Date: 2016-11-15
 */

using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using OpenIZAdmin.Models.LanguageModels;

namespace OpenIZAdmin.Models.ConceptModels
{
	/// <summary>
	/// Represents a model to edit a concept.
	/// </summary>
	public class EditConceptModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditConceptModel"/> class.
		/// </summary>
		public EditConceptModel()
		{
			this.ConceptClassList = new List<SelectListItem>
			{
				new SelectListItem { Text = string.Empty, Value = string.Empty }
			};

			this.LanguageList = new List<SelectListItem>
			{
				new SelectListItem { Text = string.Empty, Value = string.Empty }
			};

			this.Name = new List<string>();
			this.ReferenceTerms = new List<ReferenceTermModel>();
            this.Languages = new List<LanguageModel>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditConceptModel"/> class.
		/// </summary>
		/// <param name="concept">The concept.</param>
		public EditConceptModel(Concept concept) : this()
		{
			this.ConceptClass = concept.Class.Name;
			this.CreationTime = concept.CreationTime.DateTime;
			this.Id = concept.Key.Value;			
            this.Languages = concept.ConceptNames.Select(k => new LanguageModel(k.Language, k.Name, concept.Key.Value)).ToList();   
		}

		/// <summary>
		/// Gets or sets the concept class.
		/// </summary>
		/// <value>The concept class.</value>
		[Display(Name = "ConceptClass", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "ConceptClassRequired", ErrorMessageResourceType = typeof(Locale))]
		public string ConceptClass { get; set; }

		/// <summary>
		/// Gets or sets the concept class list.
		/// </summary>
		/// <value>The concept class list.</value>
		public List<SelectListItem> ConceptClassList { get; set; }

		/// <summary>
		/// Gets or sets the creation time.
		/// </summary>
		/// <value>The creation time.</value>
		[Display(Name = "CreationTime", ResourceType = typeof(Locale))]
		public DateTimeOffset CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the language list.
		/// </summary>
		/// <value>The language list.</value>
		public List<SelectListItem> LanguageList { get; set; }		

        /// <summary>
		/// Gets or sets the Language list for the Language ISO 2 digit code and the associated display name of the Concept.
		/// </summary>		
		[Display(Name = "Languages", ResourceType = typeof(Localization.Locale))]
        public List<LanguageModel> Languages { get; set; }

        /// <summary>
        /// Gets or sets the mnemonic.
        /// </summary>
        /// <value>The mnemonic.</value>
        public string Mnemonic { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public List<string> Name { get; set; }

		/// <summary>
		/// Gets or sets the reference terms.
		/// </summary>
		/// <value>The reference terms.</value>
		public List<ReferenceTermModel> ReferenceTerms { get; set; }

		/// <summary>
		/// Gets or sets the selected language.
		/// </summary>
		/// <value>The selected language.</value>
		[Display(Name = "Language", ResourceType = typeof(Locale))]
		public string SelectedLanguage { get; set; }
	}
}