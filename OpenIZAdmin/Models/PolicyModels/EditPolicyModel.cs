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
 * Date: 2016-11-16
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.PolicyModels
{
	public class EditPolicyModel
	{
		public EditPolicyModel()
		{
			this.GrantsList = new List<SelectListItem>();
		}

		public EditPolicyModel(SecurityPolicyInfo securityPolicyInfo) : this()
		{
			this.CanOverride = securityPolicyInfo.CanOverride;
			this.Grant = (int)securityPolicyInfo.Grant;
			this.IsPublic = securityPolicyInfo.Policy.IsPublic;
			this.Id = securityPolicyInfo.Policy.Key.Value;
			this.Name = securityPolicyInfo.Name;
			this.Oid = securityPolicyInfo.Oid;
			this.GrantsList.Add(new SelectListItem { Text = Locale.Select, Value = "" });
			this.GrantsList.Add(new SelectListItem { Text = Locale.Deny, Value = "0" });
			this.GrantsList.Add(new SelectListItem { Text = Locale.Elevate, Value = "1" });
			this.GrantsList.Add(new SelectListItem { Text = Locale.Grant, Value = "2" });
		}

		[Display(Name = "CanOverride", ResourceType = typeof(Localization.Locale))]
		public bool CanOverride { get; set; }

		[Display(Name = "Grants", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "GrantsRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		public int Grant { get; set; }

		public List<SelectListItem> GrantsList { get; set; }

		[Display(Name = "IsPublic", ResourceType = typeof(Localization.Locale))]
		public bool IsPublic { get; set; }

		[Required]
		public Guid Id { get; set; }

		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "NameTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		[Display(Name = "Oid", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "OidRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Oid { get; set; }

		public SecurityPolicyInfo ToSecurityPolicyInfo(SecurityPolicyInfo securityPolicyInfo)
		{
			return new SecurityPolicyInfo(new SecurityPolicyInstance(securityPolicyInfo.Policy, (PolicyGrantType)this.Grant))
			{
				Policy = new SecurityPolicy
				{
					CanOverride = this.CanOverride,
					Name = this.Name,
					Oid = this.Oid
				}

			};
		}
	}
}