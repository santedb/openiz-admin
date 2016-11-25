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
	/// Provides a utility for managing concepts.
	/// </summary>
	public static class MaterialUtil
	{
		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.DataTypes.Material"/> to a <see cref="OpenIZAdmin.Models.ConceptModels.CreateMaterialModel"/>.
		/// </summary>
		/// <param name="model">The materials to convert.</param>
		/// <returns>Returns a material list.</returns>
		public static List<MaterialSearchResultViewModel> ToMaterialList(Bundle bundle)
		{
			List<MaterialSearchResultViewModel> materialList = new List<MaterialSearchResultViewModel>();
            
            for( var i = 0; i<bundle.Count; i++)
            {
                materialList.Add(
                    new MaterialSearchResultViewModel()
                    {
                        Key = (bundle.Item[i] as Material).Key.Value,
                        Name = (bundle.Item[i] as Material).Names.FirstOrDefault().Component.FirstOrDefault().Value,
                    });
            }
			return materialList;
		}

    }
}