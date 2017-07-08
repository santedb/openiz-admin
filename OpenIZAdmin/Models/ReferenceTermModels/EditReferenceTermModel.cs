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

using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ReferenceTermNameModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.ReferenceTermModels
{
	/// <summary>
	/// Represents a reference term name model class.
	/// </summary>
	public class EditReferenceTermModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateReferenceTermNameModel"/> class.
		/// </summary>
		public EditReferenceTermModel()
		{
			LanguageList = new List<SelectListItem>();
			TermNamesList = new List<ReferenceTermNameViewModel>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateReferenceTermNameModel"/> class.
		/// </summary>
		public EditReferenceTermModel(ReferenceTerm referenceTerm) : this()
		{
			this.DisplayNames = referenceTerm.DisplayNames;
			this.Id = referenceTerm.Key ?? Guid.Empty;
			this.Mnemonic = referenceTerm.Mnemonic;
			this.TermNamesList = referenceTerm.DisplayNames.Select(k => new ReferenceTermNameViewModel(k.Key, k.Language, k.Name, referenceTerm)).ToList();
		}

		/// <summary>
		/// Gets or sets the display names.
		/// </summary>
		/// <value>The display names.</value>
		public List<ReferenceTermName> DisplayNames { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[Required]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the language list.
		/// </summary>
		/// <value>The language list.</value>
		public List<SelectListItem> LanguageList { get; set; }

		/// <summary>
		/// Gets or sets the mnemonic.
		/// </summary>
		/// <value>The mnemonic.</value>
		public string Mnemonic { get; set; }

		/// <summary>
		/// Gets or sets the Language list for the Language ISO 2 digit code and the associated display name of the Concept.
		/// </summary>
		[Display(Name = "Languages", ResourceType = typeof(Locale))]
		public List<ReferenceTermNameViewModel> TermNamesList { get; set; }
	}
}