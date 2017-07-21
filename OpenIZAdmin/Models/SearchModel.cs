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
 * Date: 2016-7-10
 */

using OpenIZAdmin.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OpenIZAdmin.Models
{
	/// <summary>
	/// Represents a search model.
	/// </summary>
	public class SearchModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchModel"/> class.
		/// </summary>
		public SearchModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchModel"/> class
		/// with a specific search term.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		public SearchModel(string searchTerm)
		{
			this.SearchTerm = searchTerm;
		}

		/// <summary>
		/// Gets or sets the search term of the search model.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(256, ErrorMessageResourceName = "NameLength256", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(Constants.RegExBasicHtmlString, ErrorMessageResourceName = "InvalidSearchEntry", ErrorMessageResourceType = typeof(Locale))]
		public string SearchTerm { get; set; }

        /// <summary>
        /// Show type of search
        /// </summary>
        public bool SearchTypeEnabled { get; set; }

        /// <summary>
        /// Type of search
        /// </summary>
		[Display(Name = "Type", ResourceType = typeof(Locale))]
        public string SearchType { get; set; }

    }
}