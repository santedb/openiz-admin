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
 * User: nitya
 * Date: 2017-9-19
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZAdmin.Services.Entities.Organizations
{
	/// <summary>
	/// Represents an organization concept service.
	/// </summary>
	public interface IOrganizationConceptService
	{
		/// <summary>
		/// Gets the industry concepts.
		/// </summary>
		/// <returns>Returns a list of industry concepts.</returns>
		IEnumerable<Concept> GetIndustryConcepts();
	}
}
