/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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

using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.ConceptModels
{
	public class SearchConceptModel
	{
		public SearchConceptModel()
		{
		}

		[Display(Name = "SearchType", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "SearchTypeRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		public ConceptSearchType SearchType { get; set; }

		[Display(Name = "Mnemonic", ResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "MnemonicTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Mnemonic { get; set; }

		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "NameTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }
	}
}