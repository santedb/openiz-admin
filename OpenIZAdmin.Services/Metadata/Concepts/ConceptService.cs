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
 * Date: 2017-7-10
 */

using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Core.Caching;
using OpenIZAdmin.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Services.Metadata.Concepts
{
	/// <summary>
	/// Represents a concept service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.ImsiServiceBase" />
	/// <seealso cref="IConceptService" />
	public class ConceptService : ImsiServiceBase, IConceptService
	{
		/// <summary>
		/// The cache service.
		/// </summary>
		private readonly ICacheService cacheService;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptService" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="cacheService">The cache service.</param>
		public ConceptService(ImsiServiceClient client, ICacheService cacheService) : base(client)
		{
			this.cacheService = cacheService;
		}

		/// <summary>
		/// Gets the concept.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="loadFast">if set to <c>true</c> the concept will be retrieved from the cache instead of contacting the server.</param>
		/// <returns>Returns the concept for the given key.</returns>
		public Concept GetConcept(Guid? key, bool loadFast = false)
		{
			// if the key doesn't have a value, we want to exit
			if (!key.HasValue || key == Guid.Empty)
			{
				return null;
			}

			return loadFast ? cacheService.Get<Concept>(key.ToString(), () => this.GetConceptInternal(key.Value)) : this.GetConceptInternal(key.Value);
		}

		/// <summary>
		/// Gets the concept.
		/// </summary>
		/// <param name="mnemonic">The mnemonic.</param>
		/// <param name="loadFast">if set to <c>true</c> the concept will be retrieved from the cache instead of contacting the server.</param>
		/// <returns>Returns the concept for the given mnemonic.</returns>
		public Concept GetConcept(string mnemonic, bool loadFast = false)
		{
			// if the mnemonic is null or empty, we want to exit
			if (string.IsNullOrEmpty(mnemonic) || string.IsNullOrWhiteSpace(mnemonic))
			{
				return null;
			}

			return loadFast ? cacheService.Get<Concept>(mnemonic, () => this.GetConceptInternal(mnemonic)) : this.GetConceptInternal(mnemonic);
		}

		/// <summary>
		/// Gets the concept reference terms.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>Returns a list of reference terms for a given concept.</returns>
		public IEnumerable<ReferenceTerm> GetConceptReferenceTerms(Guid id, Guid? versionId)
		{
			var referenceTerms = new List<ReferenceTerm>();

			var bundle = this.Client.Query<Concept>(c => c.Key == id && c.VersionKey == versionId && c.ObsoletionTime == null);

			bundle.Reconstitute();

			foreach (var conceptReferenceTerm in bundle.Item.OfType<Concept>().LatestVersionOnly().Where(c => c.Key == id && c.VersionKey == versionId && c.ObsoletionTime == null).SelectMany(c => c.ReferenceTerms))
			{
				var referenceTerm = conceptReferenceTerm.ReferenceTerm;

				if (referenceTerm == null && conceptReferenceTerm.ReferenceTermKey.HasValue && conceptReferenceTerm.ReferenceTermKey.Value != Guid.Empty)
				{
					var referenceTermBundle = this.Client.Query<ReferenceTerm>(c => c.Key == conceptReferenceTerm.ReferenceTermKey && c.ObsoletionTime == null, 0, null, true);

					referenceTermBundle.Reconstitute();

					referenceTerm = referenceTermBundle.Item.OfType<ReferenceTerm>().FirstOrDefault(c => c.Key == conceptReferenceTerm.ReferenceTermKey && c.ObsoletionTime == null);
				}

				if (referenceTerm != null)
				{
					referenceTerms.Add(referenceTerm);
				}
			}

			return referenceTerms;
		}

		/// <summary>
		/// Gets the concept set.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the concept set for the given key.</returns>
		public ConceptSet GetConceptSet(Guid? key)
		{
			return key.HasValue && key.Value != Guid.Empty ? this.Client.Get<ConceptSet>(key.Value, null) as ConceptSet : null;
		}

		/// <summary>
		/// Gets the concept set.
		/// </summary>
		/// <param name="mnemonic">The mnemonic.</param>
		/// <returns>Returns the concept set for the given mnemonic.</returns>
		public ConceptSet GetConceptSet(string mnemonic)
		{
			var bundle = this.Client.Query<ConceptSet>(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null, 0, 1, true);

			bundle.Reconstitute();

			return bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);
		}

		/// <summary>
		/// Gets a list of concept for a given concept set.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns a list of <see cref="Guid" /> values which represents concept keys.</returns>
		public IEnumerable<Guid> GetConceptSets(Guid? key)
		{
			// ensure existing concept sets are sent up otherwise
			// the IMS will remove this concept from any associated concept set
			var bundle = this.Client.Query<ConceptSet>(cs => cs.ConceptsXml.Any(c => c == key) && cs.ObsoletionTime == null, 0, null, true);

			bundle.Reconstitute();

			return bundle.Item.OfType<ConceptSet>().Where(cs => cs.ConceptsXml.Any(c => c == key) && cs.Key.HasValue && cs.ObsoletionTime == null).Select(c => c.Key.Value).ToList();
		}

		/// <summary>
		/// Gets the type concept for a given entity.
		/// </summary>
		/// <typeparam name="T">The type of entity.</typeparam>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the type concept for the given entity.</returns>
		public Concept GetTypeConcept<T>(T entity) where T : Entity
		{
			Concept typeConcept = null;

			if (entity.TypeConcept == null && entity.TypeConceptKey.HasValue && entity.TypeConceptKey.Value != Guid.Empty)
			{
				typeConcept = this.cacheService.Get<Concept>(entity.TypeConceptKey.ToString(), () => this.GetConcept(entity.TypeConceptKey.Value));
			}

			return typeConcept;
		}

		/// <summary>
		/// Gets the concept.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the concept for the given key.</returns>
		private Concept GetConceptInternal(Guid key)
		{
			return this.Client.Get<Concept>(key, null) as Concept;
		}

		/// <summary>
		/// Gets the concept by mnemonic.
		/// </summary>
		/// <param name="mnemonic">The mnemonic.</param>
		/// <returns>Returns the concept for the given mnemonic.</returns>
		private Concept GetConceptInternal(string mnemonic)
		{
			var bundle = this.Client.Query<Concept>(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null, 0, 1, true);

			bundle.Reconstitute();

			return bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);
		}
	}
}