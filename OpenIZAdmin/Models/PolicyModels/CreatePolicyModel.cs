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
 * Date: 2016-7-30
 */

using OpenIZ.Core.Model.AMI.Auth;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.PolicyModels
{
    /// <summary>
	/// Represents a policy view model class.
	/// </summary>
	public class CreatePolicyModel : PolicyModel
	{
        /// <summary>
		/// Initializes a new instance of the <see cref="CreatePolicyModel"/> class.
		/// </summary>
		public CreatePolicyModel()
		{
			this.GrantsList = new List<SelectListItem>();
		}		

        /// <summary>
        /// Gets or sets the list of Grants
        /// </summary>
		public List<SelectListItem> GrantsList { get; set; }        

        /// <summary>
        /// Creates a SecurityPolicyInfo instance
        /// </summary>
        /// <returns>A SecurityPolicyInfo instance with the metadata assocaited with the Policy</returns>
        public SecurityPolicyInfo ToSecurityPolicyInfo()
		{
			return new SecurityPolicyInfo
			{
				CanOverride = this.CanOverride,
				Name = this.Name,
				Oid = this.Oid,                
			};
		}
        
    }
}