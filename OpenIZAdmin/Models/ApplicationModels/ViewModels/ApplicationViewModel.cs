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
 * Date: 2016-7-8
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.ApplicationModels.ViewModels
{
    public class ApplicationViewModel
    {
        public ApplicationViewModel()
        {           
        }

        [Display(Name = "ApplicationId", ResourceType = typeof(Localization.Locale))]
        public string ApplicationId { get; set; }

        [Display(Name = "ApplicationName", ResourceType = typeof(Localization.Locale))]
        public string ApplicationName { get; set; }

        [Display(Name = "ApplicationSecret", ResourceType = typeof(Localization.Locale))]
        public string ApplicationSecret { get; set; }

        [Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
        public DateTime CreationTime { get; set; }

        public string EntityVersionId { get; set; }

        [Display(Name = "HasPolicies", ResourceType = typeof(Localization.Locale))]
        public bool HasPolicies { get; set; }

        public Guid Id { get; set; }        
        
        [Display(Name = "VendorName", ResourceType = typeof(Localization.Locale))]
        public string VendorName { get; set; }

        [Display(Name = "VersionName", ResourceType = typeof(Localization.Locale))]
        public string VersionName { get; set; }


    }
}