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
 * Date: 2016-8-1
 */

using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using OpenIZAdmin.Util;

namespace OpenIZAdmin.Models.ConceptModels
{
	/// <summary>
	/// Represents a create concept model.
	/// </summary>
	public class CreateConceptModel : ConceptModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateConceptModel"/> class.
		/// </summary>
		public CreateConceptModel()
		{
		    LanguageList = new List<SelectListItem>();
			ConceptClassList = new List<SelectListItem>();            
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

		///// <summary>
		///// Gets or sets the language.
		///// </summary>
		///// <value>The language.</value>
		//[Display(Name = "Language", ResourceType = typeof(Localization.Locale))]
		//[Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
  //      [StringLength(2, ErrorMessageResourceName = "LanguagCodeTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
  //      public string Language { get; set; }

		/// <summary>
		/// Gets or sets the language list.
		/// </summary>
		/// <value>The language list.</value>
		public List<SelectListItem> LanguageList { get; set; }

		///// <summary>
		///// Gets or sets the mnemonic.
		///// </summary>
		///// <value>The mnemonic.</value>
		//[Display(Name = "Mnemonic", ResourceType = typeof(Localization.Locale))]
		//[Required(ErrorMessageResourceName = "MnemonicRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		//[StringLength(255, ErrorMessageResourceName = "MnemonicTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		//public override string Mnemonic { get; set; }

		///// <summary>
		///// Gets or sets the name.
		///// </summary>
		///// <value>The name.</value>
		//[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		//[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		//[StringLength(255, ErrorMessageResourceName = "NameLength255", ErrorMessageResourceType = typeof(Localization.Locale))]
		//public string Name { get; set; }

		/// <summary>
		/// Converts an <see cref="CreateConceptModel"/> instance to a <see cref="Concept"/> instance.
		/// </summary>
		/// <returns>Returns a concept instance.</returns>
		public Concept ToConcept()
		{
			return new Concept
			{
				Class = new ConceptClass
				{
					Key = Guid.Parse(this.ConceptClass)
				},
				ConceptNames = new List<ConceptName>
				{
					new ConceptName
					{
						Language = this.Language,
						Name = this.Name
					}
				},
				Key = Guid.NewGuid(),
				Mnemonic = this.Mnemonic,
			};
		}
	}
}