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
 * Date: 2017-4-12
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
	/// Represents a policy view model.
	/// </summary>
    public abstract class PolicyModel
    {
        /// <summary>
		/// Gets or sets a value indicating whether this instance can override.
		/// </summary>
		/// <value><c>true</c> if this instance can override; otherwise, <c>false</c>.</value>
		[Display(Name = "CanOverride", ResourceType = typeof(Locale))]
        public bool CanOverride { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>The creation time.</value>
        [Display(Name = "CreationTime", ResourceType = typeof(Locale))]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the grant.
        /// </summary>
        /// <value>The grant.</value>
        [Display(Name = "Grant", ResourceType = typeof(Locale))]
        public string Grant { get; set; }

        /// <summary>
        /// Gets or sets the grant enum/Id
        /// </summary>
        [Display(Name = "Grants", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "GrantsRequired", ErrorMessageResourceType = typeof(Locale))]
        public int GrantId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Display(Name = "Grants", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "GrantsRequired", ErrorMessageResourceType = typeof(Locale))]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the obsolete status of the policy.
        /// </summary>
        public bool IsObsolete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is public.
        /// </summary>
        /// <value><c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
        [Display(Name = "IsPublic", ResourceType = typeof(Locale))]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(64, ErrorMessageResourceName = "NameLength64", ErrorMessageResourceType = typeof(Locale))]
        [RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the oid.
        /// </summary>
        /// <value>The oid.</value>
        [Display(Name = "Oid", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "OidRequired", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(64, ErrorMessageResourceName = "OidLength64", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Oid { get; set; }
    }
}