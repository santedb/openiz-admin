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
 * Date: 2016-8-15
 */

using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Models.RoleModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.ProviderModels.ViewModels
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderViewModel"/> class.
    /// </summary>
    public class ProviderViewModel
	{
        /// <summary>
        /// Default constructor
        /// </summary>
        public ProviderViewModel()
        {            
        }
       
        /// <summary>
        /// Gets or sets the last login time of the user.
        /// </summary>
        //[Display(Name = "LastLoginTime", ResourceType = typeof(Localization.Locale))]
        [Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
        public DateTimeOffset? CreationTime { get; set; }       

        /// <summary>
        /// Gets or sets the names of the user.
        /// </summary>
        [Display(Name = "Names", ResourceType = typeof(Localization.Locale))]
        public List<EntityName> Names { get; set; }

        /// <summary>
        /// Gets or sets the names of the user.
        /// </summary>
        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        //[Display(Name = "ProviderSpecialty", ResourceType = typeof(Localization.Locale))]
        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        public Concept ProviderSpecialty { get; set; }

        /// <summary>
        /// Gets or sets the id of the user.
        /// </summary>
        [Display(Name = "UserId", ResourceType = typeof(Localization.Locale))]
        //public IdentifierBase<Entity> UserId { get; set; }
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        [Display(Name = "Username", ResourceType = typeof(Localization.Locale))]
        public string Username { get; set; }
    }
}