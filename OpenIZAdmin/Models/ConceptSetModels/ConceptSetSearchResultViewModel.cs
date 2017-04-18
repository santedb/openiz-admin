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
 * User: Andrew
 * Date: 2017-4-10
 */

using OpenIZ.Core.Model.DataTypes;
using System;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.ConceptSetModels
{
	/// <summary>
	/// Represents a view model for a concept set search result.
	/// </summary>
	public sealed class ConceptSetSearchResultViewModel : ConceptSetModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptSetSearchResultViewModel"/> class.
		/// </summary>
		public ConceptSetSearchResultViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptSetSearchResultViewModel"/> class
		/// with a specific <see cref="ConceptSet"/> instance.
		/// </summary>
		/// <param name="conceptSet">The <see cref="ConceptSet"/> instance.</param>
		public ConceptSetSearchResultViewModel(ConceptSet conceptSet) : this()
		{
			CreationTime = conceptSet.CreationTime.DateTime;
			Id = conceptSet.Key ?? Guid.Empty;
			Mnemonic = conceptSet.Mnemonic;
			Name = conceptSet.Name;
		}
	}
}