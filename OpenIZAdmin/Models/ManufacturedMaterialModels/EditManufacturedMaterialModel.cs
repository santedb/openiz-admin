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
 * User: khannan
 * Date: 2017-8-24
 */

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.MaterialModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.ManufacturedMaterialModels
{
	/// <summary>
	/// Represents a model to edit a manufactured material.
	/// </summary>
	public class EditManufacturedMaterialModel : EditMaterialModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditManufacturedMaterialModel"/> class.
		/// </summary>
		public EditManufacturedMaterialModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditManufacturedMaterialModel"/> class.
		/// </summary>
		/// <param name="material">The material.</param>
		public EditManufacturedMaterialModel(ManufacturedMaterial material) : base(material)
		{
			this.LotNumber = material.LotNumber;
		}

		/// <summary>
		/// Gets or sets the lot number.
		/// </summary>
		/// <value>The lot number.</value>
		[Display(Name = "LotNumber", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "LotNumberRequired", ErrorMessageResourceType = typeof(Locale))]
		public string LotNumber { get; set; }

		/// <summary>
		/// Converts a <see cref="EditManufacturedMaterialModel"/> instance to an <see cref="ManufacturedMaterial"/> instance.
		/// </summary>
		/// <param name="manufacturedMaterial">The manufactured material.</param>
		/// <returns>Returns the converted <see cref="ManufacturedMaterial"/> instance.</returns>
		public ManufacturedMaterial ToManufacturedMaterial(ManufacturedMaterial manufacturedMaterial)
		{
			manufacturedMaterial.CreationTime = DateTimeOffset.Now;
			manufacturedMaterial.ExpiryDate = this.ExpiryDate;
			manufacturedMaterial.Names.RemoveAll(n => n.NameUseKey == NameUseKeys.Assigned);
			manufacturedMaterial.Names.Add(new EntityName(NameUseKeys.Assigned, this.Name));
			manufacturedMaterial.Names.Add(new EntityName(NameUseKeys.Search, this.CommonName));

			Guid formConceptKey, quantityConceptKey, typeConceptKey;

			if (Guid.TryParse(this.FormConcept, out formConceptKey))
			{
				manufacturedMaterial.FormConceptKey = formConceptKey;
			}

			if (Guid.TryParse(this.QuantityConcept, out quantityConceptKey))
			{
				manufacturedMaterial.QuantityConceptKey = quantityConceptKey;
			}

			if (Guid.TryParse(this.TypeConcept, out typeConceptKey))
			{
				manufacturedMaterial.TypeConceptKey = typeConceptKey;
			}

			manufacturedMaterial.LotNumber = this.LotNumber;
			manufacturedMaterial.VersionKey = null;

			return manufacturedMaterial;
		}
	}
}