﻿/*
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
 * Date: 2017-7-9
 */

using OpenIZ.Core.Model.DataTypes;
using System.Collections.Generic;

namespace OpenIZAdmin.Services.Entities.Materials
{
	/// <summary>
	/// Represents a material concept service.
	/// </summary>
	public interface IMaterialConceptService
	{
		/// <summary>
		/// Gets the form concepts.
		/// </summary>
		/// <returns>Returns a list of form concepts.</returns>
		IEnumerable<Concept> GetFormConcepts();

		/// <summary>
		/// Gets the material type concepts.
		/// </summary>
		/// <returns>Returns a list of concepts which are a part of the material type concept set.</returns>
		IEnumerable<Concept> GetMaterialTypeConcepts();

		/// <summary>
		/// Gets the quantity concepts.
		/// </summary>
		/// <returns>Returns a list of quantity concepts.</returns>
		IEnumerable<Concept> GetQuantityConcepts();
	}
}