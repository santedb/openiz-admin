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
 * Date: 2017-2-19
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.OrganizationModels
{
	/// <summary>
	/// Represents an edit organization model.
	/// </summary>
	public class EditOrganizationModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditOrganizationModel"/> class.
		/// </summary>
		public EditOrganizationModel()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditOrganizationModel"/> class
		/// with a specific <see cref="Organization"/> instance.
		/// </summary>
		/// <param name="organization"></param>
		public EditOrganizationModel(OpenIZ.Core.Model.Entities.Organization organization)
		{
			this.Id = organization.Key.Value;
			this.Name = string.Join(" ", organization.Names.SelectMany(n => n.Component).Select(c => c.Value));
		}

		/// <summary>
		/// Gets or sets the id of the organization.
		/// </summary>
		[Required]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the organization.
		/// </summary>
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Converts an <see cref="EditOrganizationModel"/> instance to an <see cref="Organization"/> instance.
		/// </summary>
		/// <returns>Returns an <see cref="Organization"/> instance.</returns>
		public OpenIZ.Core.Model.Entities.Organization ToOrganization()
		{
			return new OpenIZ.Core.Model.Entities.Organization
			{
				Key = this.Id,
				Names = new List<EntityName>
				{
					new EntityName(NameUseKeys.OfficialRecord, this.Name)
				}
			};
		}
	}
}