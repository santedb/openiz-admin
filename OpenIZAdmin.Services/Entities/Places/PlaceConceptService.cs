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
 * Date: 2017-7-27
 */
using System.Collections.Generic;
using System.Linq;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Services.Core;
using OpenIZAdmin.Services.Metadata;

namespace OpenIZAdmin.Services.Entities.Places
{
	/// <summary>
	/// Represents a place concept service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.ImsiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.Entities.Places.IPlaceConceptService" />
	public class PlaceConceptService : ImsiServiceBase, IPlaceConceptService
	{
		/// <summary>
		/// The concept service.
		/// </summary>
		private readonly IConceptService conceptService;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlaceConceptService"/> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="conceptService">The concept service.</param>
		public PlaceConceptService(ImsiServiceClient client, IConceptService conceptService) : base(client)
		{
			this.conceptService = conceptService;
		}

		public IEnumerable<Concept> GetPlaceTypeConcepts(params string[] conceptSetMnemonicFilters)
		{
			var typeConcepts = new List<Concept>();

			foreach (var conceptSetMnemonicFilter in conceptSetMnemonicFilters.Where(f => !string.IsNullOrEmpty(f) && !string.IsNullOrWhiteSpace(f)))
			{
				typeConcepts.AddRange(this.conceptService.GetConceptSet(conceptSetMnemonicFilter).Concepts);
			}

			// only default the items in the list if the list is empty
			if (!typeConcepts.Any())
			{
				typeConcepts.AddRange(this.Client.Query<Concept>(m => m.ClassKey == ConceptClassKeys.Other && m.ObsoletionTime == null).Item.OfType<Concept>().Where(m => m.ClassKey == ConceptClassKeys.Other && m.ObsoletionTime == null));
			}

			return typeConcepts;
		}
	}
}
