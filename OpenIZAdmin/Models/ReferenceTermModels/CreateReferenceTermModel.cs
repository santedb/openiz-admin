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

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.ReferenceTermModels
{
	/// <summary>
	/// Represents a reference term view model.
	/// </summary>
	public class CreateReferenceTermModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateReferenceTermModel"/> class.
		/// </summary>
		public CreateReferenceTermModel()
		{
			this.CodeSystemList = new List<SelectListItem>();
			this.LanguageList = new List<SelectListItem>();
		}

		/// <summary>
		/// Gets or sets the Code System
		/// </summary>
		public string CodeSystem { get; set; }

		/// <summary>
		/// Gets or sets the code system list.
		/// </summary>
		/// <value>The code system list.</value>
		[Display(Name = "CodeSystem", ResourceType = typeof(Locale))]
		public List<SelectListItem> CodeSystemList { get; set; }

		/// <summary>
		/// Gets or sets the Concept identifier
		/// </summary>
		public Guid? ConceptId { get; set; }

		/// <summary>
		/// Gets or sets the language list.
		/// </summary>
		/// <value>The language list.</value>
		public List<SelectListItem> LanguageList { get; set; }

		/// <summary>
		/// Gets or sets the mnemonic.
		/// </summary>
		/// <value>The mnemonic.</value>
		[Display(Name = "Mnemonic", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "MnemonicRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(50, ErrorMessageResourceName = "MnemonicLength50", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
		public string Mnemonic { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(256, ErrorMessageResourceName = "NameLength256", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the two letter language code of the language.
		/// </summary>
		[Display(Name = "Language", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Locale))]
		public string TwoLetterCountryCode { get; set; }

		/// <summary>
		/// Converts an <see cref="CreateReferenceTermModel"/> instance to a <see cref="ReferenceTerm"/> instance.
		/// </summary>
		/// <returns>Returns a ReferenceTerm instance.</returns>
		public ReferenceTerm ToReferenceTerm()
		{
			return new ReferenceTerm
			{
				Key = Guid.NewGuid(),
				Mnemonic = this.Mnemonic,
				CodeSystemKey = Guid.Parse(this.CodeSystem),
				DisplayNames = new List<ReferenceTermName>()
				{
					new ReferenceTermName()
					{
						Key = Guid.NewGuid(),
						Language = this.TwoLetterCountryCode,
						Name = this.Name,
						PhoneticAlgorithmKey = PhoneticAlgorithmKeys.Soundex
					}
				}
			};
		}
	}
}