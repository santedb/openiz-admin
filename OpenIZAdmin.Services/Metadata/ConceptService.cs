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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Core.Extensions;
using OpenIZAdmin.Services.Core;

namespace OpenIZAdmin.Services.Metadata
{
	/// <summary>
	/// Represents a concept service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.ImsiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.Metadata.IConceptService" />
	public class ConceptService : ImsiServiceBase, IConceptService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptService"/> class.
		/// </summary>
		/// <param name="client">The client.</param>
		public ConceptService(ImsiServiceClient client) : base(client)
		{
		}

		/// <summary>
		/// Gets the concept.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the concept for the given key.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public Concept GetConcept(Guid key)
		{
			return this.Client.Get<Concept>(key, null) as Concept;
		}

		/// <summary>
		/// Gets the concept.
		/// </summary>
		/// <param name="mnemonic">The mnemonic.</param>
		/// <returns>Returns the concept for the given mnemonic.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public Concept GetConcept(string mnemonic)
		{
			var bundle = this.Client.Query<Concept>(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null, 0, 1, true);

			bundle.Reconstitute();

			return bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);
		}

		/// <summary>
		/// Gets the concept set.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the concept set for the given key.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public ConceptSet GetConceptSet(Guid key)
		{
			return this.Client.Get<ConceptSet>(key, null) as ConceptSet;
		}

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
		/// <param name="mnemonic">The mnemonic.</param>
		/// <returns>Returns the concept set for the given mnemonic.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
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
		public IEnumerable<Guid> GetConceptSets(Guid key)
		{
			// ensure existing concept sets are sent up otherwise
			// the IMS will remove this concept from any associated concept set
			var bundle = this.Client.Query<ConceptSet>(cs => cs.ConceptsXml.Any(c => c == key) && cs.ObsoletionTime == null, 0, null, true);

			bundle.Reconstitute();

			return bundle.Item.OfType<ConceptSet>().Where(cs => cs.ConceptsXml.Any(c => c == key) && cs.Key.HasValue && cs.ObsoletionTime == null).Select(c => c.Key.Value).ToList();
		}
	}
}
