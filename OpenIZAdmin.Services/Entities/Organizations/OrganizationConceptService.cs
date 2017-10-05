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
 * Date: 2017-9-19
 */

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Core.Caching;
using OpenIZAdmin.Services.Core;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Services.Entities.Organizations
{
	/// <summary>
	/// Represents an organization concept service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.ImsiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.Entities.Organizations.IOrganizationConceptService" />
	public class OrganizationConceptService : ImsiServiceBase, IOrganizationConceptService
	{
		/// <summary>
		/// The cache service.
		/// </summary>
		private readonly ICacheService cacheService;

		/// <summary>
		/// Initializes a new instance of the <see cref="OrganizationConceptService" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="cacheService">The cache service.</param>
		public OrganizationConceptService(ImsiServiceClient client, ICacheService cacheService) : base(client)
		{
			this.cacheService = cacheService;
		}

		/// <summary>
		/// Gets the industry concepts.
		/// </summary>
		/// <returns>Returns a list of industry concepts.</returns>
		public IEnumerable<Concept> GetIndustryConcepts()
		{
			return this.cacheService.Get<IEnumerable<Concept>>(ConceptSetKeys.IndustryCode.ToString(), () =>
			{
				var bundle = this.Client.Query<ConceptSet>(c => c.Key == ConceptSetKeys.IndustryCode, 0, null, new[] { "concept" });

				bundle.Reconstitute();

				return bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Key == ConceptSetKeys.IndustryCode)?.Concepts;
			});
		}
	}
}