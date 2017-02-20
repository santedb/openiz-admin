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

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Models.PolicyModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OpenIZAdmin.Models.RoleModels.ViewModels
{
	public class RoleViewModel
	{
		public RoleViewModel()
		{
			this.Policies = new List<PolicyViewModel>();
		}

		public RoleViewModel(SecurityRole securityRole) : this(new SecurityRoleInfo(securityRole))
		{
		}

		public RoleViewModel(SecurityRoleInfo securityRoleInfo) : this()
		{
			this.Description = securityRoleInfo.Role.Description;
			this.HasPolicies = securityRoleInfo.Policies.Any();
			this.Id = securityRoleInfo.Id.Value;
			this.IsObsolete = securityRoleInfo.Role.ObsoletionTime != null;
			this.Name = securityRoleInfo.Name;

			if (this.HasPolicies)
			{
				this.Policies = securityRoleInfo.Policies.Select(p => new PolicyViewModel(new SecurityPolicyInstance(p.Policy, p.Grant))).OrderBy(q => q.Name).ToList();
			}
		}

		[Display(Name = "Description", ResourceType = typeof(Localization.Locale))]
		public string Description { get; set; }

		[Display(Name = "HasPolicies", ResourceType = typeof(Localization.Locale))]
		public bool HasPolicies { get; set; }

		[Display(Name = "Id", ResourceType = typeof(Localization.Locale))]
		public Guid Id { get; set; }

		public bool IsObsolete { get; set; }

		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		public List<PolicyViewModel> Policies { get; set; }
	}
}