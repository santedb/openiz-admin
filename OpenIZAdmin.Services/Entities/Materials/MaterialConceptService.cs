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
 * Date: 2017-7-10
 */

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Core.Caching;
using OpenIZAdmin.Services.Core;
using OpenIZAdmin.Services.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Services.Entities.Materials
{
	/// <summary>
	/// Represents a material concept service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Entities.Materials.IMaterialConceptService" />
	public class MaterialConceptService : ImsiServiceBase, IMaterialConceptService
	{
		/// <summary>
		/// The material types mnemonic.
		/// </summary>
		private const string MaterialTypesMnemonic = "MaterialTypes";

		/// <summary>
		/// The cache service.
		/// </summary>
		private readonly ICacheService cacheService;

		/// <summary>
		/// The concept service.
		/// </summary>
		private readonly IConceptService conceptService;

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialConceptService"/> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="cacheService">The cache service.</param>
		/// <param name="conceptService">The concept service.</param>
		public MaterialConceptService(ImsiServiceClient client, ICacheService cacheService, IConceptService conceptService) : base(client)
		{
			this.cacheService = cacheService;
			this.conceptService = conceptService;
		}

		/// <summary>
		/// Gets the form concepts.
		/// </summary>
		/// <returns>Returns a list of form concepts.</returns>
		public IEnumerable<Concept> GetFormConcepts()
		{
			return cacheService.Get(ConceptClassKeys.Form.ToString(), () =>
			{
				var bundle = this.Client.Query<Concept>(c => c.ClassKey == ConceptClassKeys.Form && c.ObsoletionTime == null);

				bundle.Reconstitute();

				return bundle.Item.OfType<Concept>().Where(c => c.ClassKey == ConceptClassKeys.Form && c.ObsoletionTime == null);
			});
		}

		/// <summary>
		/// Gets the material type concepts.
		/// </summary>
		/// <returns>Returns a list of concepts which are a part of the material type concept set.</returns>
		public IEnumerable<Concept> GetMaterialTypeConcepts()
		{
			return this.cacheService.Get<IEnumerable<Concept>>(MaterialTypesMnemonic, () =>
			{
				var bundle = this.Client.Query<ConceptSet>(c => c.Mnemonic == MaterialTypesMnemonic && c.ObsoletionTime == null, 0, null, new[] { "concept" });

				bundle.Reconstitute();

				var conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Mnemonic == MaterialTypesMnemonic && c.ObsoletionTime == null);

				var concepts = new List<Concept>();

				if (conceptSet != null)
				{
					concepts.AddRange(conceptSet.ConceptsXml.Select(c => this.conceptService.GetConcept(c)).ToList());
				}

				return concepts;
			});
		}

		/// <summary>
		/// Gets the quantity concepts.
		/// </summary>
		/// <returns>Returns a list of quantity concepts.</returns>
		public IEnumerable<Concept> GetQuantityConcepts()
		{
			return cacheService.Get(ConceptClassKeys.UnitOfMeasure.ToString(), () =>
			{
				var bundle = this.Client.Query<Concept>(c => c.ClassKey == ConceptClassKeys.UnitOfMeasure && c.ObsoletionTime == null);

				bundle.Reconstitute();

				return bundle.Item.OfType<Concept>().Where(c => c.ClassKey == ConceptClassKeys.UnitOfMeasure && c.ObsoletionTime == null);
			});
		}
	}
}