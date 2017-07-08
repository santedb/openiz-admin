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
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;
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
	public class EditMaterialModel : EditEntityModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditMaterialModel"/> class.
		/// </summary>
		public EditMaterialModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditMaterialModel"/> class.
		/// </summary>
		/// <param name="material">The material.</param>
		public EditMaterialModel(Material material) : base(material)
		{
			this.ExpiryDate = material.ExpiryDate ?? DateTime.Now;
			this.FormConcepts = new List<SelectListItem>();
			this.QuantityConcepts = new List<SelectListItem>();
			this.TypeConcepts = new List<SelectListItem>();

			this.FormConcept = material.FormConceptKey?.ToString();

			if (material.Names.Any(n => n.NameUseKey == NameUseKeys.Assigned))
			{
				this.Name = string.Join(" ", material.Names.Where(n => n.NameUseKey == NameUseKeys.Assigned).SelectMany(n => n.Component).Select(c => c.Value));
			}
			else if (material.Names.Any(n => n.NameUseKey == NameUseKeys.OfficialRecord))
			{
				this.Name = string.Join(" ", material.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Select(c => c.Value));
			}
			else
			{
				this.Name = string.Join(" ", material.Names.SelectMany(n => n.Component).Select(c => c.Value));
			}

			this.QuantityConcept = material.QuantityConceptKey?.ToString();
			this.TypeConcept = material.TypeConceptKey?.ToString();
		}

		/// <summary>
		/// Gets or sets the expiry date of the material.
		/// </summary>
		[Display(Name = "ExpiryDate", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "ExpiryDateRequired", ErrorMessageResourceType = typeof(Locale))]
		public DateTime ExpiryDate { get; set; }

		/// <summary>
		/// Gets or sets the form concept of the material.
		/// </summary>
		[Display(Name = "FormConcept", ResourceType = typeof(Locale))]
		public string FormConcept { get; set; }

		/// <summary>
		/// Gets or sets a list of form concepts.
		/// </summary>
		public List<SelectListItem> FormConcepts { get; set; }

		/// <summary>
		/// Gets or sets the name of the material.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(64, ErrorMessageResourceName = "NameLength64", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the quantity concept of the material.
		/// </summary>
		[Display(Name = "QuantityConcept", ResourceType = typeof(Locale))]
		public string QuantityConcept { get; set; }

		/// <summary>
		/// Gets or sets a list of quantity concepts.
		/// </summary>
		public List<SelectListItem> QuantityConcepts { get; set; }

		/// <summary>
		/// Gets or sets the type concept.
		/// </summary>
		/// <value>The type concept.</value>
		[Display(Name = "TypeConcept", ResourceType = typeof(Locale))]
		public string TypeConcept { get; set; }

		/// <summary>
		/// Gets or sets the type concepts.
		/// </summary>
		/// <value>The type concepts.</value>
		public List<SelectListItem> TypeConcepts { get; set; }

		/// <summary>
		/// Converts an <see cref="EditMaterialModel"/> instance to a <see cref="Material"/> instance.
		/// </summary>
		/// <param name="material">The <see cref="Material"/> instance.</param>
		/// <returns>Returns a <see cref="Material"/> instance.</returns>
		public Material ToMaterial(Material material)
		{
			material.CreationTime = DateTimeOffset.Now;
			material.ExpiryDate = this.ExpiryDate;
			material.Names.RemoveAll(n => n.NameUseKey == NameUseKeys.Assigned);
			material.Names.Add(new EntityName(NameUseKeys.Assigned, this.Name));

			Guid formConceptKey, quantityConceptKey, typeConceptKey;

			if (Guid.TryParse(this.FormConcept, out formConceptKey))
			{
				material.FormConceptKey = formConceptKey;
			}

			if (Guid.TryParse(this.QuantityConcept, out quantityConceptKey))
			{
				material.QuantityConceptKey = quantityConceptKey;
			}

			if (Guid.TryParse(this.TypeConcept, out typeConceptKey))
			{
				material.TypeConceptKey = typeConceptKey;
			}

			material.VersionKey = null;

			return material;
		}
	}
}