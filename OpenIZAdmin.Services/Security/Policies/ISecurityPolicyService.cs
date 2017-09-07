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
 * Date: 2017-9-4
 */

using OpenIZ.Core.Model.AMI.Auth;
using System;
using System.Collections.Generic;

namespace OpenIZAdmin.Services.Security.Policies
{
	/// <summary>
	/// Represents a security policy service.
	/// </summary>
	public interface ISecurityPolicyService
	{
		/// <summary>
		/// Creates the security policy.
		/// </summary>
		/// <param name="securityPolicyInfo">The security policy information.</param>
		/// <returns>Returns the created security policy.</returns>
		SecurityPolicyInfo CreateSecurityPolicy(SecurityPolicyInfo securityPolicyInfo);

		/// <summary>
		/// Gets all policies.
		/// </summary>
		/// <returns>Returns a list of security policies.</returns>
		IEnumerable<SecurityPolicyInfo> GetAllPolicies();

		/// <summary>
		/// Gets the policies by OID.
		/// </summary>
		/// <param name="oid">The OID.</param>
		/// <returns>Returns a list of policies which match the given OID value.</returns>
		IEnumerable<SecurityPolicyInfo> GetPoliciesByOid(string oid);

		/// <summary>
		/// Gets a security policy.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the security policy which matches the given id or null or no security policy is found.</returns>
		SecurityPolicyInfo GetSecurityPolicy(Guid key);

		/// <summary>
		/// Searches for a policy using a given term.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of policies which match the given search term.</returns>
		IEnumerable<SecurityPolicyInfo> Search(string searchTerm);
	}
}