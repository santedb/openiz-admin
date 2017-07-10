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
	}
}
