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

using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenIZ.Core.Model.Constants;

namespace OpenIZAdmin.Models.ConceptModels
{
	/// <summary>
	/// Represents a view model for a concept search result.
	/// </summary>
	public sealed class ConceptSearchResultViewModel : ConceptModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptSearchResultViewModel"/> class.
		/// </summary>
		public ConceptSearchResultViewModel()
		{
			this.Names = new List<string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptSearchResultViewModel"/> class
		/// with a specific <see cref="Concept"/> instance.
		/// </summary>
		/// <param name="concept">The <see cref="Concept"/> instance.</param>
		public ConceptSearchResultViewModel(Concept concept) : this()
		{
			CreationTime = concept.CreationTime.DateTime;
			Id = concept.Key ?? Guid.Empty;
			IsObsolete = concept.StatusConceptKey == StatusKeys.Obsolete;
			IsSystemConcept = concept.IsSystemConcept;
			Mnemonic = concept.Mnemonic;
			Names = concept.ConceptNames.Select(c => c.Name).ToList();
			ConceptNames = (Names.Any()) ? string.Join(", ", Names) : string.Empty;
			VersionKey = concept.VersionKey;
		}
	}
}