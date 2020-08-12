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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.MaterialModels;

namespace OpenIZAdmin.Models.ManufacturedMaterialModels
{
	/// <summary>
	/// Represents a model to create a manufactured material.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Models.MaterialModels.CreateMaterialModel" />
	public class CreateManufacturedMaterialModel : CreateMaterialModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateManufacturedMaterialModel"/> class.
		/// </summary>
		public CreateManufacturedMaterialModel()
		{
			
		}

		/// <summary>
		/// Gets or sets the lot number.
		/// </summary>
		/// <value>The lot number.</value>
		[Display(Name = "LotNumber", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "LotNumberRequired", ErrorMessageResourceType = typeof(Locale))]
		public string LotNumber { get; set; }

		/// <summary>
		/// Converts a <see cref="CreateMaterialModel"/> instance to a <see cref="ManufacturedMaterial"/> instance.
		/// </summary>
		/// <returns>Returns a <see cref="ManufacturedMaterial"/> instance.</returns>
		public ManufacturedMaterial ToManufacturedMaterial()
		{
			var manufacturedMaterial = new ManufacturedMaterial
			{
				CreationTime = DateTimeOffset.Now,
				ExpiryDate = this.ExpiryDate,
				Key = Guid.NewGuid(),
				LotNumber = this.LotNumber,
				Names = new List<EntityName>
				{
					new EntityName(NameUseKeys.Assigned, this.Name),
                    new EntityName(NameUseKeys.Search, this.CommonName)
				},
				StatusConceptKey = StatusKeys.Active
			};

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

			return manufacturedMaterial;
		}
	}
}