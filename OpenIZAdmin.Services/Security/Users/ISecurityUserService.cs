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

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZAdmin.Services.Security.Users
{
	/// <summary>
	/// Represents an security user service.
	/// </summary>
	public interface ISecurityUserService
	{
		/// <summary>
		/// Activates the security user.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the activated security user.</returns>
		SecurityUserInfo ActivateSecurityUser(Guid key);

		/// <summary>
		/// Changes the password of a user.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="newPassword">The new password.</param>
		void ChangePassword(Guid key, string newPassword);

		/// <summary>
		/// Creates the security user.
		/// </summary>
		/// <param name="userInfo">The user information.</param>
		/// <returns>Returns the created security user info.</returns>
		SecurityUserInfo CreateSecurityUser(SecurityUserInfo userInfo);

		/// <summary>
		/// Deactivates the security user.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the deactivated security user.</returns>
		SecurityUserInfo DeactivateSecurityUser(Guid key);

		/// <summary>
		/// Gets the security user.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the security user for the given key.</returns>
		SecurityUserInfo GetSecurityUser(Guid key);

		/// <summary>
		/// Queries for security users using a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of security users which match the given expression.</returns>
		IEnumerable<SecurityUserInfo> Query(Expression<Func<SecurityUser, bool>> expression);
	}
}