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

using OpenIZAdmin.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using OpenIZ.Core.Model.DataTypes;

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
		}

		public EditConceptModel(Concept concept) : this()
		{
			this.ConceptClass = concept.Class.Name;
			this.CreationTime = concept.CreationTime.DateTime;
			this.Id = concept.Key.Value;
			this.Languages = concept.ConceptNames.Select(c => c.Language).ToList();
			this.Name = concept.ConceptNames.Select(c => c.Name).ToList();

			if (!this.Languages.Contains(Locale.EN))
			{
				this.Languages.Add(Locale.EN);
				this.Name.Add(Locale.English);
			}

			if (!this.Languages.Contains(Locale.SW))
			{
				this.Languages.Add(Locale.SW);
				this.Name.Add(Locale.Kiswahili);
			}
		}

		[Display(Name = "ConceptClass", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "ConceptClassRequired", ErrorMessageResourceType = typeof(Locale))]
		public string ConceptClass { get; set; }

		public List<SelectListItem> ConceptClassList { get; set; }

		[Display(Name = "CreationTime", ResourceType = typeof(Locale))]
		public DateTimeOffset CreationTime { get; set; }

		public Guid Id { get; set; }

		public List<SelectListItem> LanguageList { get; set; }

		public List<string> Languages { get; set; }

		public string Mnemonic { get; set; }

		public List<string> Name { get; set; }

		public List<ReferenceTermModel> ReferenceTerms { get; set; }

		[Display(Name = "Language", ResourceType = typeof(Locale))]
		public string SelectedLanguage { get; set; }
	}
}