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
 * Date: 2016-7-23
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
	/// Represents a material search result view model.
	/// </summary>
	public class MaterialSearchResultViewModel : EntityViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialSearchResultViewModel"/> class.
		/// </summary>
		public MaterialSearchResultViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialSearchResultViewModel"/> class
		/// with a specific <see cref="Material"/> instance.
		/// </summary>
		/// <param name="material">The <see cref="Material"/> instance.</param>
		public MaterialSearchResultViewModel(Material material) : base(material)
		{
			this.Name = string.Join(", ", material.Names.Where(n => n.NameUseKey == NameUseKeys.Assigned).SelectMany(m => m.Component).Select(c => c.Value));
		}
	}
}