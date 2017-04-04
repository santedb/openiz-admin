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
 * Date: 2016-7-30
 */

using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.IMSI.Client;
using System;
using System.Linq;
using OpenIZ.Messaging.AMI.Client;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing users.
	/// </summary>
	public static class UserUtil
	{
		/// <summary>
		/// Gets a user entity.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
		/// <param name="userName">The username entered to verify doesn't exist in the database.</param>
		/// <returns>Returns a user entity or null if no user entity is found.</returns>
		public static bool CheckForUserName(AmiServiceClient client, string userName)
		{
			return client.GetUsers(u => u.UserName == userName).CollectionItem.Any();			
        }

		/// <summary>
		/// Gets a user entity.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
		/// <param name="securityUserId">The user id of the user to retrieve.</param>
		/// <returns>Returns a user entity or null if no user entity is found.</returns>
		public static UserEntity GetUserEntityBySecurityUserKey(ImsiServiceClient client, Guid securityUserId)
		{
			var bundle = client.Query<UserEntity>(u => u.SecurityUserKey == securityUserId && u.ObsoletionTime == null, 0, null, true);

			bundle.Reconstitute();

			return bundle.Item.OfType<UserEntity>().FirstOrDefault(u => u.SecurityUserKey == securityUserId);
		}
	}
}