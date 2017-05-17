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

using OpenIZ.Core.Model.AMI.DataTypes;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using System.ComponentModel.DataAnnotations;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.AssigningAuthorityModels
{
	/// <summary>
	/// Represents a create assigning authority model.
	/// </summary>
	public class CreateAssigningAuthorityModel : AssigningAuthorityModel
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateAssigningAuthorityModel"/> class.
		/// </summary>
		public CreateAssigningAuthorityModel()
		{
		}	

		/// <summary>
		/// Gets or sets a value indicating whether this instance is unique.
		/// </summary>
		/// <value><c>true</c> if this instance is unique; otherwise, <c>false</c>.</value>
		[Display(Name = "IsUnique", ResourceType = typeof(Locale))]
		public bool IsUnique { get; set; }		

		/// <summary>
		/// Converts a <see cref="CreateAssigningAuthorityModel"/> instance to an <see cref="AssigningAuthorityInfo"/> instance.
		/// </summary>
		/// <returns>Returns an <see cref="AssigningAuthorityInfo"/> instance.</returns>
		public AssigningAuthorityInfo ToAssigningAuthorityInfo()
		{
			return new AssigningAuthorityInfo
			{
				AssigningAuthority = new AssigningAuthority
				{
					Description = this.Description,
					DomainName = this.DomainName,
					IsUnique = this.IsUnique,
					Name = this.Name,
					Oid = this.Oid,
					Url = this.Url,
					ValidationRegex = this.ValidationRegex
				}
			};
		}
	}
}