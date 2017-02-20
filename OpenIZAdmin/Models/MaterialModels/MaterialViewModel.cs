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

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.MaterialModels
{
	/// <summary>
	/// Represents a material view model.
	/// </summary>
	public class MaterialViewModel : EntityViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialViewModel"/> class.
		/// </summary>
		public MaterialViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialViewModel"/> class
		/// with a specific <see cref="Material"/> instance.
		/// </summary>
		/// <param name="material">The <see cref="Material"/> instance.</param>
		public MaterialViewModel(Material material) : base(material)
		{
			this.FormConcept = material.FormConcept?.ConceptNames.Any() == true ? string.Join(" ", material.FormConcept?.ConceptNames.Select(c => c.Name)) : material.FormConcept?.Mnemonic;
			this.Name = string.Join(" ", material.Names.Where(n => n.NameUseKey == NameUseKeys.Assigned).SelectMany(n => n.Component).Select(c => c.Value));
			this.QuantityConcept = material.QuantityConcept?.ConceptNames.Any() == true ? string.Join(" ", material.QuantityConcept?.ConceptNames.Select(c => c.Name)) : material.QuantityConcept?.Mnemonic;
		}

		/// <summary>
		/// Gets or sets the form concept of the material.
		/// </summary>
		[Display(Name = "FormConcept", ResourceType = typeof(Localization.Locale))]
		public string FormConcept { get; set; }

		/// <summary>
		/// Gets or sets the quantity concept of the material.
		/// </summary>
		[Display(Name = "QuantityConcept", ResourceType = typeof(Localization.Locale))]
		public string QuantityConcept { get; set; }
	}
}