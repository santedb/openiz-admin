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
 * Date: 2016-7-30
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.PolicyModels.ViewModels
{
	public class PolicyViewModel
	{
		public PolicyViewModel()
		{
		}

		[Display(Name = "CanOverride", ResourceType = typeof(Localization.Locale))]
		public bool CanOverride { get; set; }

		[Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
		public DateTime CreationTime { get; set; }

		[Display(Name = "Grant", ResourceType = typeof(Localization.Locale))]
		public string Grant { get; set; }

        /// <summary>
		/// Gets or sets the obsolete status of the policy.
		/// </summary>
		public bool IsObsolete { get; set; }

        [Display(Name = "IsPublic", ResourceType = typeof(Localization.Locale))]
		public bool IsPublic { get; set; }

		public Guid Key { get; set; }

		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		[Display(Name = "OIDAllCaps", ResourceType = typeof(Localization.Locale))]
		public string Oid { get; set; }
	}
}