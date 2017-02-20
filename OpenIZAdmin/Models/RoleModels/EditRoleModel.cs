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
 * Date: 2016-9-5
 */

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OpenIZAdmin.Models.PolicyModels;

namespace OpenIZAdmin.Models.RoleModels
{
	/// <summary>
	/// Represents an edit role model.
	/// </summary>
	public class EditRoleModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditRoleModel"/> class.
		/// </summary>
		public EditRoleModel()
		{
			this.Policies = new List<string>();
			this.PoliciesList = new List<SelectListItem>();
			this.RolePolicies = new List<PolicyViewModel>();
		}

		/// <summary>
		/// Gets or sets the description of the role.
		/// </summary>
		[Display(Name = "Description")]
		[StringLength(1000, ErrorMessageResourceName = "DescriptionTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the id of the role.
		/// </summary>
		[Required]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the role.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "NameLength255", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the list of policies associated with the role.
		/// </summary>
		[Display(Name = "Policies", ResourceType = typeof(Localization.Locale))]
		public List<string> Policies { get; set; }

		/// <summary>
		/// Gets or sets the policy list.
		/// </summary>
		public List<SelectListItem> PoliciesList { get; set; }

		/// <summary>
		/// Gets or sets the policies associated with the role.
		/// </summary>
		public IEnumerable<PolicyViewModel> RolePolicies { get; set; }
	}
}