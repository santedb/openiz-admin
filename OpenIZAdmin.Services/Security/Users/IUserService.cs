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
 * Date: 2017-7-10
 */

using OpenIZ.Core.Model.Entities;
using System;

namespace OpenIZAdmin.Services.Security.Users
{
	/// <summary>
	/// Represents a user service.
	/// </summary>
	public interface IUserService
	{
		/// <summary>
		/// Gets the user entity by security user key.
		/// </summary>
		/// <param name="securityUserId">The security user identifier.</param>
		/// <returns>Returns the user entity for the given security user key.</returns>
		UserEntity GetUserEntityBySecurityUserKey(Guid securityUserId);

		/// <summary>
		/// Updates the user entity.
		/// </summary>
		/// <param name="userEntity">The user entity.</param>
		/// <returns>Returns the updated user entity.</returns>
		UserEntity UpdateUserEntity(UserEntity userEntity);
	}
}