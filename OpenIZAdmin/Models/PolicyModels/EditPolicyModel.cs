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

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.PolicyModels
{
    /// <summary>
	/// Represents a policy view model class.
	/// </summary>
	public class EditPolicyModel : PolicyModel
	{
        /// <summary>
		/// Initializes a new instance of the <see cref="EditPolicyModel"/> class.
		/// </summary>
		public EditPolicyModel()
		{
			this.GrantsList = new List<SelectListItem>();
		}

        /// <summary>
		/// Initializes a new instance of the <see cref="EditPolicyModel"/> class.
		/// </summary>
		public EditPolicyModel(SecurityPolicyInfo securityPolicyInfo) : this()
		{
			this.CanOverride = securityPolicyInfo.CanOverride;
			this.GrantId = (int)securityPolicyInfo.Grant;
			this.IsPublic = securityPolicyInfo.Policy.IsPublic;
		    this.Id = securityPolicyInfo.Policy.Key ?? Guid.Empty;
			this.Name = securityPolicyInfo.Name;
			this.Oid = securityPolicyInfo.Oid;
			this.GrantsList.Add(new SelectListItem { Text = Locale.Select, Value = "" });
			this.GrantsList.Add(new SelectListItem { Text = Locale.Deny, Value = "0" });
			this.GrantsList.Add(new SelectListItem { Text = Locale.Elevate, Value = "1" });
			this.GrantsList.Add(new SelectListItem { Text = Locale.Grant, Value = "2" });
		}        

        /// <summary>
        /// Gets or sets the list of Grants
        /// </summary>
        public List<SelectListItem> GrantsList { get; set; }
        
        /// <summary>
        /// Creates a SecurityPolicyInfo instance
        /// </summary>
        /// <returns>A SecurityPolicyInfo instance with the metadata assocaited with the Policy</returns>
        public SecurityPolicyInfo ToSecurityPolicyInfo(SecurityPolicyInfo securityPolicyInfo)
		{
			return new SecurityPolicyInfo(new SecurityPolicyInstance(securityPolicyInfo.Policy, (PolicyGrantType)this.GrantId))
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