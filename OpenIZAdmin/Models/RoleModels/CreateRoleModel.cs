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
 * Date: 2016-7-17
 */

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Localization;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.RoleModels
{
	/// <summary>
	/// Represents a create role model.
	/// </summary>
	public class CreateRoleModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateRoleModel"/> class.
		/// </summary>
		public CreateRoleModel()
		{
		}

		/// <summary>
		/// Gets or sets the description of the role.
		/// </summary>
		[Display(Name = "Description", ResourceType = typeof(Locale))]
		[StringLength(256, ErrorMessageResourceName = "DescriptionLength256", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the name of the role.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(50, ErrorMessageResourceName = "NameLength50", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Converts a <see cref="CreateRoleModel"/> instance to a <see cref="SecurityRoleInfo"/> instance.
		/// </summary>
		/// <returns>Returns a <see cref="SecurityRoleInfo"/> instance.</returns>
		public SecurityRoleInfo ToSecurityRoleInfo()
		{
			return new SecurityRoleInfo
			{
				Role = new SecurityRole
				{
					Description = this.Description
				},
				Name = this.Name
			};
		}
	}
}