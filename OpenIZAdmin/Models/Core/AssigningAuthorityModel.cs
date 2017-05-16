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
 * Date: 2017-5-16
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.Core
{
    /// <summary>
	/// Represents a model of an assigning authority.
	/// </summary>
    public abstract class AssigningAuthorityModel
    {
        /// <summary>
		/// Gets or sets the description of the assigning authority.
		/// </summary>
		[Display(Name = "Description", ResourceType = typeof(Locale))]
        [StringLength(4000, ErrorMessageResourceName = "DescriptionLength4000", ErrorMessageResourceType = typeof(Locale))]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the domain name of the assigning authority.
        /// </summary>
        [Display(Name = "DomainName", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(32, ErrorMessageResourceName = "DomainNameLength32", ErrorMessageResourceType = typeof(Locale))]
        public string DomainName { get; set; }        

        /// <summary>
        /// Gets or sets the name of the assigning authority.
        /// </summary>
        [Display(Name = "Name", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(50, ErrorMessageResourceName = "NameLength50", ErrorMessageResourceType = typeof(Locale))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the OID of the assigning authority.
        /// </summary>
        [Display(Name = "Oid", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "OidRequired", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(256, ErrorMessageResourceName = "OidLength256", ErrorMessageResourceType = typeof(Locale))]
        public string Oid { get; set; }

        /// <summary>
        /// Gets or sets the URL of the assigning authority.
        /// </summary>
        [Display(Name = "Url", ResourceType = typeof(Locale))]
        [Url(ErrorMessageResourceName = "UrlInvalid", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(250, ErrorMessageResourceName = "UrlLength250", ErrorMessageResourceType = typeof(Locale))]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the validation regex.
        /// </summary>
        /// <value>The validation regex.</value>
        [Display(Name = "ValidationRegex", ResourceType = typeof(Locale))]
        [StringLength(64, ErrorMessageResourceName = "RegexLength64", ErrorMessageResourceType = typeof(Locale))]
        public string ValidationRegex { get; set; }
    }
}