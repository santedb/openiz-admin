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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.AMI.Auth;

namespace OpenIZAdmin.Services.Security.Applications
{
	/// <summary>
	/// Represents a security application service.
	/// </summary>
	public interface ISecurityApplicationService
	{
		/// <summary>
		/// Activates a security application.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="securityApplicationInfo">The security application information.</param>
		/// <returns>Returns the activated security application.</returns>
		SecurityApplicationInfo Activate(Guid key, SecurityApplicationInfo securityApplicationInfo);

		/// <summary>
		/// Creates the specified security application information.
		/// </summary>
		/// <param name="securityApplicationInfo">The security application information.</param>
		/// <returns>Returns the created security application.</returns>
		SecurityApplicationInfo Create(SecurityApplicationInfo securityApplicationInfo);

		/// <summary>
		/// Deletes a security application.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the deleted security application info.</returns>
		SecurityApplicationInfo Delete(Guid key);

		/// <summary>
		/// Gets a security application.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the security application or null if no security application is found.</returns>
		SecurityApplicationInfo Get(Guid key);

		/// <summary>
		/// Searches for a security application.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of security applications which match the given search term.</returns>
		IEnumerable<SecurityApplicationInfo> Search(string searchTerm);

		/// <summary>
		/// Updates a security application.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="securityApplicationInfo">The security application information.</param>
		/// <returns>Returns the updated security application info.</returns>
		SecurityApplicationInfo Update(Guid key, SecurityApplicationInfo securityApplicationInfo);
	}
}
