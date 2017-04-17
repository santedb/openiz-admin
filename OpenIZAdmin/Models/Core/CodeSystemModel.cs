﻿/*
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
 * Date: 2017-4-17
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Models.Core
{
    /// <summary>
	/// Represents a code system model.
	/// </summary>
    public abstract class CodeSystemModel
    {
        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        [Display(Name = "Description", ResourceType = typeof(Localization.Locale))]
        [StringLength(256, ErrorMessageResourceName = "DescriptionLength255", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Domain
        /// </summary>
        [Display(Name = "Domain", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "DomainRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(50, ErrorMessageResourceName = "DomainLength50", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the code system identifier
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(256, ErrorMessageResourceName = "NameLength255", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Oid
        /// </summary>
        [Display(Name = "Oid", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "OidRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(64, ErrorMessageResourceName = "OidLength64", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Oid { get; set; }

        /// <summary>
        /// Gets or sets the Url
        /// </summary>
        [Display(Name = "Url", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "UrlRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(256, ErrorMessageResourceName = "UrlLength256", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the Version
        /// </summary>
        [Display(Name = "Version", ResourceType = typeof(Localization.Locale))]
        [StringLength(10, ErrorMessageResourceName = "VersionLength10", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Version { get; set; }        
    }
}