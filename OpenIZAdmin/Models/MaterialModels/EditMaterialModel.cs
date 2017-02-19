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
 * Date: 2016-8-1
 */

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.MaterialModels
{
	/// <summary>
	/// Represents a model to edit a material.
	/// </summary>
	public class EditMaterialModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditMaterialModel"/> class.
		/// </summary>
		public EditMaterialModel()
		{
			this.FormConcepts = new List<SelectListItem>();
			this.QuantityConcepts = new List<SelectListItem>();
		}

		public EditMaterialModel(Material material)
		{
			this.FormConcept = material.FormConceptKey?.ToString();
			this.Id = material.Key.Value;
			this.Name = string.Join(" ", material.Names.SelectMany(n => n.Component).Select(c => c.Value));
			this.QuantityConcept = material.QuantityConceptKey?.ToString();
		}

		/// <summary>
		/// Gets or sets the form concept of the material.
		/// </summary>
		[Display(Name = "FormConcept", ResourceType = typeof(Localization.Locale))]
		public string FormConcept { get; set; }

		/// <summary>
		/// Gets or sets a list of form concepts.
		/// </summary>
		public List<SelectListItem> FormConcepts { get; set; }

		/// <summary>
		/// Gets or sets the key of the material.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the material.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "NameTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the quantity concept of the material.
		/// </summary>
		[Display(Name = "QuantityConcept", ResourceType = typeof(Localization.Locale))]
		public string QuantityConcept { get; set; }

		/// <summary>
		/// Gets or sets a list of quantity concepts.
		/// </summary>
		public List<SelectListItem> QuantityConcepts { get; set; }

		public Material ToMaterial()
		{
			return new Material
			{
				FormConceptKey = Guid.Parse(this.FormConcept),
				Names = new List<EntityName>
				{
					new EntityName(NameUseKeys.OfficialRecord, this.Name)
				},
				QuantityConceptKey = Guid.Parse(this.QuantityConcept)
			};
		}
	}
}