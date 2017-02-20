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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.MaterialModels
{
	/// <summary>
	/// Represents a model to create a material.
	/// </summary>
	public class CreateMaterialModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateMaterialModel"/> class.
		/// </summary>
		public CreateMaterialModel()
		{
			this.FormConcepts = new List<SelectListItem>();
			this.QuantityConcepts = new List<SelectListItem>();
		}

		/// <summary>
		/// Gets or sets the form concept for the material.
		/// </summary>
		[Display(Name = "FormConcept", ResourceType = typeof(Localization.Locale))]
		public string FormConcept { get; set; }

		/// <summary>
		/// Gets or sets a list of form concepts.
		/// </summary>
		public List<SelectListItem> FormConcepts { get; set; }

		/// <summary>
		/// Gets or sets the name of the material.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(64, ErrorMessageResourceName = "NameLength64", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the quantity concept of the material.
		/// </summary>
		[Display(Name = "QuantityConcept", ResourceType = typeof(Localization.Locale))]
		public string QuantityConcept { get; set; }

		/// <summary>
		/// Gets or sets a list of quantity concepts of the material.
		/// </summary>
		public List<SelectListItem> QuantityConcepts { get; set; }

		/// <summary>
		/// Converts a <see cref="CreateMaterialModel"/> instance to a <see cref="Material"/> instance.
		/// </summary>
		/// <returns>Returns a <see cref="Material"/> instance.</returns>
		public Material ToMaterial()
		{
			var material = new Material
			{
				Key = Guid.NewGuid(),
				Names = new List<EntityName>
				{
					new EntityName(NameUseKeys.Assigned, this.Name)
				}
			};

			Guid formConceptKey, quantityConceptKey;

			if (Guid.TryParse(this.FormConcept, out formConceptKey))
			{
				material.FormConceptKey = formConceptKey;
			}

			if (Guid.TryParse(this.QuantityConcept, out quantityConceptKey))
			{
				material.QuantityConceptKey = quantityConceptKey;
			}

			return material;
		}
	}
}