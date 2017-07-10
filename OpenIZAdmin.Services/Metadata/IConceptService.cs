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

using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;

namespace OpenIZAdmin.Services.Metadata
{
	/// <summary>
	/// Represents a concept service.
	/// </summary>
	public interface IConceptService
	{
		/// <summary>
		/// Gets the concept.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the concept for the given key.</returns>
		Concept GetConcept(Guid key);

		/// <summary>
		/// Gets the concept.
		/// </summary>
		/// <param name="mnemonic">The mnemonic.</param>
		/// <returns>Returns the concept for the given mnemonic.</returns>
		Concept GetConcept(string mnemonic);

		/// <summary>
		/// Gets the concept set.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the concept set for the given key.</returns>
		ConceptSet GetConceptSet(Guid key);

		/// <summary>
		/// Gets the concept reference terms.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>Returns a list of reference terms for a given concept.</returns>
		IEnumerable<ReferenceTerm> GetConceptReferenceTerms(Guid id, Guid? versionId);

		/// <summary>
		/// Gets the concept set.
		/// </summary>
		/// <param name="mnemonic">The mnemonic.</param>
		/// <returns>Returns the concept set for the given mnemonic.</returns>
		ConceptSet GetConceptSet(string mnemonic);

		/// <summary>
		/// Gets a list of concept for a given concept set.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns a list of <see cref="Guid"/> values which represents concept keys.</returns>
		IEnumerable<Guid> GetConceptSets(Guid key);
	}
}