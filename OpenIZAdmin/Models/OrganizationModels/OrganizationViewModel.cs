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
using OpenIZAdmin.Models.Core;
using OpenIZAdmin.Models.ManufacturedMaterialModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OpenIZAdmin.Models.OrganizationModels
{
	/// <summary>
	/// Represents an organization view model.
	/// </summary>
	public class OrganizationViewModel : EntityViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OrganizationViewModel"/> class.
		/// </summary>
		public OrganizationViewModel()
		{
			this.ManufacturedMaterials = new List<ManufacturedMaterialViewModel>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OrganizationViewModel"/> class
		/// with a specific <see cref="Organization"/> instance.
		/// </summary>
		/// <param name="organization"></param>
		public OrganizationViewModel(Organization organization) : base(organization)
		{
			if (organization.IndustryConcept != null)
			{
				this.IndustryConcept = string.Join(" ", organization.IndustryConcept.ConceptNames.Select(c => c.Name));
			}

			this.ManufacturedMaterials = organization.Relationships.Where(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.WarrantedProduct)
													.Select(r => r.TargetEntity)
													.OfType<ManufacturedMaterial>()
													.Select(m => new ManufacturedMaterialViewModel(m))
													.OrderBy(m => m.Name)
													.ToList();
		}

		/// <summary>
		/// Gets or sets the industry of the organization.
		/// </summary>
		[Display(Name = "Industry", ResourceType = typeof(Locale))]
		public string IndustryConcept { get; set; }

		/// <summary>
		/// Gets or sets the list of manufactured materials associated with the organization.
		/// </summary>
		public List<ManufacturedMaterialViewModel> ManufacturedMaterials { get; set; }
	}
}