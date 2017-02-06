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

using OpenIZAdmin.Models.ConceptModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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
			this.ConceptClassList = new List<SelectListItem>();
			this.LanguageList = new List<SelectListItem>();
			this.Name = new List<string>();
			this.ReferenceTerms = new List<ReferenceTermModel>();
		}

		[Display(Name = "ConceptClass", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "ConceptClassRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string ConceptClass { get; set; }

		public List<SelectListItem> ConceptClassList { get; set; }

		[Display(Name = "Created By")]
		public string CreatedBy { get; set; }

		[Display(Name = "Creation Time")]
		//public string CreationTime { get; set; }
        public DateTimeOffset CreationTime { get; set; }

        [Display(Name = "Concept Details")]
		public List<DetailedConceptViewModel> Details { get; set; }

		[Display(Name = "Is Read Only?")]
		public bool IsReadOnly { get; set; }

		public Guid Key { get; set; }

		public List<SelectListItem> LanguageList { get; set; }
		public List<string> Languages { get; set; }

		public string Mnemonic { get; set; }

		public List<string> Name { get; set; }
		public List<ReferenceTermModel> ReferenceTerms { get; set; }

		[Display(Name = "Language", ResourceType = typeof(Localization.Locale))]
		public string SelectedLanguage { get; set; }
	}
}