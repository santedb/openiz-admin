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

using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.ManufacturedMaterialModels
{
	/// <summary>
	/// Represents a manufactured material view model.
	/// </summary>
	public class ManufacturedMaterialViewModel : EntityViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ManufacturedMaterialViewModel"/> class.
		/// </summary>
		public ManufacturedMaterialViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ManufacturedMaterialViewModel"/> class
		/// with a specific <see cref="ManufacturedMaterial"/> instance.
		/// </summary>
		/// <param name="manufacturedMaterial">The <see cref="ManufacturedMaterial"/> instance.</param>
		public ManufacturedMaterialViewModel(ManufacturedMaterial manufacturedMaterial) : base(manufacturedMaterial)
		{
			this.LotNumber = manufacturedMaterial.LotNumber;
		}

		/// <summary>
		/// Gets or sets the lot number of the manufactured material.
		/// </summary>
		[Display(Name = "LotNumber", ResourceType = typeof(Locale))]
		public string LotNumber { get; set; }
	}
}