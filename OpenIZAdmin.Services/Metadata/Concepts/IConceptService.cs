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
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;

namespace OpenIZAdmin.Services.Metadata.Concepts
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
		/// <param name="loadFast">if set to <c>true</c> the concept will be retrieved from the cache instead of contacting the server.</param>
		/// <returns>Returns the concept for the given key.</returns>
		Concept GetConcept(Guid? key, bool loadFast = false);

		/// <summary>
		/// Gets the concept.
		/// </summary>
		/// <param name="mnemonic">The mnemonic.</param>
		/// <param name="loadFast">if set to <c>true</c> the concept will be retrieved from the cache instead of contacting the server.</param>
		/// <returns>Returns the concept for the given mnemonic.</returns>
		Concept GetConcept(string mnemonic, bool loadFast = false);

		/// <summary>
		/// Gets the concept reference terms.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>Returns a list of reference terms for a given concept.</returns>
		IEnumerable<ReferenceTerm> GetConceptReferenceTerms(Guid id, Guid? versionId);

		/// <summary>
		/// Gets the concepts by concept set key.
		/// </summary>
		/// <param name="conceptSetKey">The concept set key.</param>
		/// <returns>Returns a list of concepts for the concept set key.</returns>
		IEnumerable<Concept> GetConceptsByConceptSetKey(Guid conceptSetKey);

		/// <summary>
		/// Gets the concepts by concept set mnemonic.
		/// </summary>
		/// <param name="conceptSetMnemonic">The concept set mnemonic.</param>
		/// <returns>Returns a list of concepts for the concept set mnemonic.</returns>
		IEnumerable<Concept> GetConceptsByConceptSetMnemonic(string conceptSetMnemonic);

		/// <summary>
		/// Gets the concept set.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the concept set for the given key.</returns>
		ConceptSet GetConceptSet(Guid? key);

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
		IEnumerable<Guid> GetConceptSets(Guid? key);

		/// <summary>
		/// Gets the type concept for a given entity.
		/// </summary>
		/// <typeparam name="T">The type of entity.</typeparam>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the type concept for the given entity.</returns>
		Concept GetTypeConcept<T>(T entity) where T : Entity;
	}
}