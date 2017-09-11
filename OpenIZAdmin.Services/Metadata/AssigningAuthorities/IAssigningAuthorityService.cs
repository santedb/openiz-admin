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
 * Date: 2017-9-9
 */

using OpenIZ.Core.Model.AMI.DataTypes;
using System;
using System.Collections.Generic;

namespace OpenIZAdmin.Services.Metadata.AssigningAuthorities
{
	/// <summary>
	/// Represents an assigning authority service.
	/// </summary>
	public interface IAssigningAuthorityService
	{
		/// <summary>
		/// Creates the assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityInfo">The assigning authority information.</param>
		/// <returns>Returns the created assigning authority.</returns>
		AssigningAuthorityInfo CreateAssigningAuthority(AssigningAuthorityInfo assigningAuthorityInfo);

		/// <summary>
		/// Gets the assigning authorities by domain.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <returns>Returns a list of assigning authorities whose domain matches the given domain.</returns>
		IEnumerable<AssigningAuthorityInfo> GetAssigningAuthoritiesByDomain(string domain);

		/// <summary>
		/// Gets the name of the assigning authorities by name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>Returns a list of assigning authorities whose name matches the given name.</returns>
		IEnumerable<AssigningAuthorityInfo> GetAssigningAuthoritiesByName(string name);

		/// <summary>
		/// Gets the assigning authorities by OID.
		/// </summary>
		/// <param name="oid">The oid.</param>
		/// <returns>Returns a list of assigning authorities whose OID matches the given OID.</returns>
		IEnumerable<AssigningAuthorityInfo> GetAssigningAuthoritiesByOid(string oid);

		/// <summary>
		/// Gets the assigning authority.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the assigning authority for the given key or null if no assigning authority is found.</returns>
		AssigningAuthorityInfo GetAssigningAuthority(Guid key);

		/// <summary>
		/// Determines whether the domain is a duplicate domain.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <returns><c>true</c> if the domain already exists; otherwise, <c>false</c>.</returns>
		bool IsDuplicateDomain(string domain);

		/// <summary>
		/// Determines whether the name is a duplicate name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns><c>true</c> if the name already exists; otherwise, <c>false</c>.</returns>
		bool IsDuplicateName(string name);

		/// <summary>
		/// Determines whether the OID already exists.
		/// </summary>
		/// <param name="oid">The oid.</param>
		/// <returns><c>true</c> if the OID already exists; otherwise, <c>false</c>.</returns>
		bool IsDuplicateOid(string oid);

		/// <summary>
		/// Searches the specified search term.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of assigning authorities which match the given search term.</returns>
		IEnumerable<AssigningAuthorityInfo> Search(string searchTerm);

		/// <summary>
		/// Updates the assigning authority.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="assigningAuthorityInfo">The assigning authority information.</param>
		/// <returns>Returns the updated assigning authority.</returns>
		AssigningAuthorityInfo UpdateAssigningAuthority(Guid key, AssigningAuthorityInfo assigningAuthorityInfo);
	}
}