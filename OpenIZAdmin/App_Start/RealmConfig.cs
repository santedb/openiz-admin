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
 * Date: 2016-7-15
 */

using System;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Models.Domain;
using System.Linq;
using System.Runtime.Caching;

namespace OpenIZAdmin
{
	/// <summary>
	/// Represents realm configuration for the application.
	/// </summary>
	public static class RealmConfig
	{
		/// <summary>
		/// The realm cache key.
		/// </summary>
		public const string RealmCacheKey = "JoinedToRealm";

		/// <summary>
		/// The realm data cache key.
		/// </summary>
		public const string RealmDataCacheKey = "Realm";

		/// <summary>
		/// Gets the current realm of the application.
		/// </summary>
		/// <returns>Returns the current realm of the application.</returns>
		public static Realm GetCurrentRealm()
		{
			var currentRealm = MvcApplication.MemoryCache.Get(RealmDataCacheKey) as Realm;

			if (currentRealm == null)
			{
				using (IUnitOfWork unitOfWork = new EntityUnitOfWork(new ApplicationDbContext()))
				{
					currentRealm = unitOfWork.RealmRepository.Get(r => r.ObsoletionTime == null).FirstOrDefault();
				}

				if (currentRealm != null)
				{
					MvcApplication.MemoryCache.Set(RealmDataCacheKey, currentRealm, MvcApplication.CacheItemPolicy);
				}
			}

			return currentRealm;
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public static void Initialize()
		{
			if (IsJoinedToRealm())
			{
				MvcApplication.MemoryCache.Set(RealmCacheKey, true, ObjectCache.InfiniteAbsoluteExpiration);
			}
		}

		/// <summary>
		/// Determines whether the application is joined to a realm.
		/// </summary>
		/// <returns>Returns true if the application is joined to a realm.</returns>
		public static bool IsJoinedToRealm()
		{
			var isJoinedToRealm = Convert.ToBoolean(MvcApplication.MemoryCache.Get(RealmCacheKey));

			if (isJoinedToRealm)
			{
				return isJoinedToRealm;
			}

			return GetCurrentRealm() != null;
		}
	}
}