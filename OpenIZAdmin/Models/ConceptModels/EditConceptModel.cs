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
using OpenIZAdmin.Models.LanguageModels;
using OpenIZAdmin.Models.ReferenceTermModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using OpenIZ.Core.Model.Constants;

namespace OpenIZAdmin.Models.ConceptModels
{
	/// <summary>
	/// Represents a model to edit a concept.
	/// </summary>
	public sealed class EditConceptModel : ConceptModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditConceptModel"/> class.
		/// </summary>
		public EditConceptModel()
		{
			ConceptClassList = new List<SelectListItem>
			{
				new SelectListItem { Text = string.Empty, Value = string.Empty }
			};

			LanguageList = new List<SelectListItem>
			{
				new SelectListItem { Text = string.Empty, Value = string.Empty }
			};

			ReferenceTerms = new List<ReferenceTermViewModel>();
			Languages = new List<LanguageViewModel>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditConceptModel"/> class.
		/// </summary>
		/// <param name="concept">The concept.</param>
		public EditConceptModel(Concept concept) : this()
		{
			CreationTime = concept.CreationTime.DateTime;
			Id = concept.Key ?? Guid.Empty;
			IsObsolete = concept.StatusConceptKey == StatusKeys.Obsolete;
			IsSystemConcept = concept.IsSystemConcept;
			Mnemonic = concept.Mnemonic;
			Name = string.Join(" ", concept.ConceptNames.Select(c => c.Name));
			Languages = concept.ConceptNames.Select(k => new LanguageViewModel(k.Language, k.Name, concept)).ToList();
			VersionKey = concept.VersionKey;
		}

		/// <summary>
		/// Gets or sets the concept class.
		/// </summary>
		/// <value>The concept class.</value>
		[Display(Name = "ConceptClass", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "ConceptClassRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string ConceptClass { get; set; }

		/// <summary>
		/// Gets or sets the concept class list.
		/// </summary>
		/// <value>The concept class list.</value>
		public List<SelectListItem> ConceptClassList { get; set; }

		//      /// <summary>
		///// Gets or sets the language.
		///// </summary>
		///// <value>The language.</value>
		//[Display(Name = "Language", ResourceType = typeof(Localization.Locale))]
		//      //[Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		//      [StringLength(2, ErrorMessageResourceName = "LanguagCodeTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		//      public string Language { get; set; }

		/// <summary>
		/// Gets or sets the language list.
		/// </summary>
		/// <value>The language list.</value>
		public List<SelectListItem> LanguageList { get; set; }

		/// <summary>
		/// Gets or sets the Language list for the Language ISO 2 digit code and the associated display name of the Concept.
		/// </summary>
		[Display(Name = "Languages", ResourceType = typeof(Localization.Locale))]
		public List<LanguageViewModel> Languages { get; set; }

		//      /// <summary>
		///// Gets or sets the mnemonic.
		///// </summary>
		///// <value>The mnemonic.</value>
		//[Display(Name = "Mnemonic", ResourceType = typeof(Localization.Locale))]
		//      [Required(ErrorMessageResourceName = "MnemonicRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		//      [StringLength(255, ErrorMessageResourceName = "MnemonicTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		//      public sealed override string Mnemonic { get; set; }

		///// <summary>
		///// Gets or sets the name.
		///// </summary>
		///// <value>The name.</value>
		//[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		//[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		//[StringLength(255, ErrorMessageResourceName = "NameLength255", ErrorMessageResourceType = typeof(Localization.Locale))]
		//public string Name { get; set; }

		/// <summary>
		/// Converts an <see cref="EditConceptModel"/> instance to a <see cref="Concept"/> instance.
		/// </summary>
		/// <param name="concept">The concept.</param>
		/// <returns>Returns the converted concept instance.</returns>
		public Concept ToEditConceptModel(Concept concept)
		{
			if (!string.Equals(this.ConceptClass, concept.ClassKey.ToString()))
			{
				concept.Class = new ConceptClass
				{
					Key = Guid.Parse(this.ConceptClass)
				};
			}

			concept.Mnemonic = this.Mnemonic;

			return concept;
		}
	}
}