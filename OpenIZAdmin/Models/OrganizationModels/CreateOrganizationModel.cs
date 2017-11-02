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

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.OrganizationModels
{
	/// <summary>
	/// Represents a create organization model.
	/// </summary>
	public class CreateOrganizationModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateOrganizationModel"/> class.
		/// </summary>
		public CreateOrganizationModel()
		{
			this.IndustryConcepts = new List<SelectListItem>();
			this.TypeConcepts = new List<SelectListItem>();
		}

		/// <summary>
		/// Gets or sets the industry concept of the organization.
		/// </summary>
		[Display(Name = "IndustryConcept", ResourceType = typeof(Locale))]
		public string IndustryConcept { get; set; }

		/// <summary>
		/// Gets or sets the list of industry concepts.
		/// </summary>
		public List<SelectListItem> IndustryConcepts { get; set; }

		/// <summary>
		/// Gets or sets the name of the organization.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(64, ErrorMessageResourceName = "NameLength64", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type concept.
		/// </summary>
		/// <value>The type concept.</value>
		[Required(ErrorMessageResourceName = "TypeConceptRequired", ErrorMessageResourceType = typeof(Locale))]
		[Display(Name = "TypeConcept", ResourceType = typeof(Locale))]
		public string TypeConcept { get; set; }

		/// <summary>
		/// Gets or sets the type concepts.
		/// </summary>
		/// <value>The type concepts.</value>
		public List<SelectListItem> TypeConcepts { get; set; }

		/// <summary>
		/// Converts a <see cref="CreateOrganizationModel"/> instance to an <see cref="Organization"/> instance.
		/// </summary>
		/// <returns>Returns a <see cref="Organization"/> instance.</returns>
		public Organization ToOrganization()
		{
			var organization = new Organization
			{
				Key = Guid.NewGuid(),
				Names = new List<EntityName>
				{
					new EntityName(NameUseKeys.OfficialRecord, this.Name)
				},
				StatusConceptKey = StatusKeys.Active
			};

			Guid industryConceptKey;

			if (Guid.TryParse(this.IndustryConcept, out industryConceptKey))
			{
				organization.IndustryConceptKey = industryConceptKey;
			}

			Guid typeConceptKey;

			if (Guid.TryParse(this.TypeConcept, out typeConceptKey))
			{
				organization.TypeConceptKey = typeConceptKey;
			}

			return organization;
		}
	}
}