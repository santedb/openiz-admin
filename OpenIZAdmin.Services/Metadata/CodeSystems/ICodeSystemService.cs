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
 * Date: 2017-9-7
 */

using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;

namespace OpenIZAdmin.Services.Metadata.CodeSystems
{
	/// <summary>
	/// Represents a code system service.
	/// </summary>
	public interface ICodeSystemService
	{
		/// <summary>
		/// Creates the code system.
		/// </summary>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns the created code system.</returns>
		CodeSystem CreateCodeSystem(CodeSystem codeSystem);

		/// <summary>
		/// Gets the code system.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the code system for the given key or null if no code system is found.</returns>
		CodeSystem GetCodeSystem(Guid key);

		/// <summary>
		/// Gets the code systems by domain.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <returns>Returns a list of code systems which match the given domain.</returns>
		IEnumerable<CodeSystem> GetCodeSystemsByDomain(string domain);

		/// <summary>
		/// Gets the code systems by oid.
		/// </summary>
		/// <param name="oid">The oid.</param>
		/// <returns>Returns a list of code systems which match the given OID.</returns>
		IEnumerable<CodeSystem> GetCodeSystemsByOid(string oid);

		/// <summary>
		/// Gets the code systems by URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns>Returns a list of code systems which match the given URL.</returns>
		IEnumerable<CodeSystem> GetCodeSystemsByUrl(string url);

		/// <summary>
		/// Determines whether the domain is a duplicate domain.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <returns><c>true</c> if the domain already exists; otherwise, <c>false</c>.</returns>
		bool IsDuplicateDomain(string domain);

		/// <summary>
		/// Determines whether the OID is a duplicate OID.
		/// </summary>
		/// <param name="oid">The oid.</param>
		/// <returns><c>true</c> if the OID already exists; otherwise, <c>false</c>.</returns>
		bool IsDuplicateOid(string oid);

		/// <summary>
		/// Determines whether the URL is a duplicate URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns><c>true</c> if the URL already exists; otherwise, <c>false</c>.</returns>
		bool IsDuplicateUrl(string url);

		/// <summary>
		/// Searches for a code system using a given search term.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of code systems which match the given search term.</returns>
		IEnumerable<CodeSystem> Search(string searchTerm);

		/// <summary>
		/// Updates the code system.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns the updated code system.</returns>
		CodeSystem UpdateCodeSystem(Guid key, CodeSystem codeSystem);
	}
}