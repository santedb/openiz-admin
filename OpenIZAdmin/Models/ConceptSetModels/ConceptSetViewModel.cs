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
using OpenIZAdmin.Models.ConceptModels;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.ConceptSetModels
{
	/// <summary>
	/// Represents a view model for a concept set.
	/// </summary>
	public sealed class ConceptSetViewModel : ConceptSetModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptSetViewModel"/> class.
		/// </summary>
		public ConceptSetViewModel()
		{
			this.Concepts = new List<ConceptViewModel>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptSetViewModel"/> class.
		/// </summary>
		/// <param name="conceptSet"></param>
		public ConceptSetViewModel(ConceptSet conceptSet, bool loadConcepts = false) : this()
		{
			if(loadConcepts) Concepts = conceptSet.Concepts.Select(c => new ConceptViewModel(c)).ToList();
			CreationTime = conceptSet.CreationTime.DateTime;
			Id = conceptSet.Key ?? Guid.Empty;
			Mnemonic = conceptSet.Mnemonic;
			Name = conceptSet.Name;
			Oid = conceptSet.Oid;
			Url = conceptSet.Url;
		}
	}
}