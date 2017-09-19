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
 * Date: 2017-8-15
 */

using OpenIZ.Core.Model.DataTypes;
using System.Collections.Generic;

namespace OpenIZAdmin.Services.EntityRelationships
{
	/// <summary>
	/// Represents an entity relationship concept service.
	/// </summary>
	public interface IEntityRelationshipConceptService
	{
		/// <summary>
		/// Gets a list of concepts which relate materials to manufactured materials.
		/// </summary>
		/// <returns>Returns a list of concepts which relate materials to manufactured materials.</returns>
		IEnumerable<Concept> GetMaterialManufacturedMaterialRelationshipConcepts();
	}
}