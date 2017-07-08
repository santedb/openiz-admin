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
using OpenIZAdmin.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.ReferenceTermNameModels
{
	/// <summary>
	/// Represents a reference term name model class.
	/// </summary>
	public class CreateReferenceTermNameModel : ReferenceTermNameModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateReferenceTermNameModel"/> class.
		/// </summary>
		public CreateReferenceTermNameModel()
		{
			LanguageList = new List<SelectListItem>();
			ReferenceTermNameList = new List<ReferenceTermNameViewModel>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateReferenceTermNameModel"/> class.
		/// </summary>
		public CreateReferenceTermNameModel(ReferenceTerm referenceTerm) : this()
		{
			ReferenceTermId = referenceTerm.Key ?? Guid.Empty;
			Mnemonic = referenceTerm.Mnemonic;
		}

		/// <summary>
		/// Gets or sets the language list.
		/// </summary>
		/// <value>The language list.</value>
		public List<SelectListItem> LanguageList { get; set; }

		/// <summary>
		/// Gets or sets the reference term name list
		/// </summary>
		public List<ReferenceTermNameViewModel> ReferenceTermNameList { get; set; }

		/// <summary>
		/// Gets or sets the two letter language code of the language.
		/// </summary>
		[Display(Name = "Language", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Locale))]
		public string TwoLetterCountryCode { get; set; }

		/// <summary>
		/// Converts an <see cref="CreateReferenceTermNameModel"/> instance to a <see cref="ReferenceTermName"/> instance.
		/// </summary>
		/// <returns>Returns a ReferenceTermName instance.</returns>
		public ReferenceTermName ToReferenceTermName()
		{
			return new ReferenceTermName()
			{
				Key = Guid.NewGuid(),
				Language = this.TwoLetterCountryCode,
				Name = this.Name,
				PhoneticAlgorithmKey = PhoneticAlgorithmKeys.Soundex
			};
		}
	}
}