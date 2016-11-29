/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Models.MaterialModels;
using OpenIZAdmin.Models.MaterialModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing materials.
	/// </summary>
	public static class MaterialUtil
	{
		/// <summary>
		/// Converts a <see cref="Material"/> instance to a <see cref="MaterialSearchResultViewModel"/> instance.
		/// </summary>
		/// <param name="material">The material to convert.</param>
		/// <returns>Returns a material search result view model.</returns>
		public static MaterialSearchResultViewModel ToMaterialSearchResultViewModel(Material material)
		{
			var viewModel = new MaterialSearchResultViewModel
			{
				CreationTime = material.CreationTime.DateTime,
				Key = material.Key.GetValueOrDefault(Guid.Empty),
				Name = string.Join(" ", material.Names.SelectMany(m => m.Component).Select(c => c.Value)),
				VersionKey = material.VersionKey
			};

			return viewModel;
		}
    }
}