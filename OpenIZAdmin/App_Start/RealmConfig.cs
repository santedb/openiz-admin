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

using OpenIZAdmin.DAL;
using OpenIZAdmin.Models.Domain;
using System.Linq;

namespace OpenIZAdmin
{
	/// <summary>
	/// Represents realm configuration for the application.
	/// </summary>
	public static class RealmConfig
	{
		/// <summary>
		/// Determines whether the application is joined to a realm.
		/// </summary>
		/// <returns>Returns true if the application is joined to a realm.</returns>
		public static bool IsJoinedToRealm()
		{
			bool isJoinedToRealm = false;

			using (IUnitOfWork unitOfWork = new EntityUnitOfWork(new ApplicationDbContext()))
			{
				isJoinedToRealm = unitOfWork.RealmRepository.AsQueryable().Count(r => r.ObsoletionTime == null) > 0;
			}

			return isJoinedToRealm;
		}

		/// <summary>
		/// Gets the current realm of the application.
		/// </summary>
		/// <returns>Returns the current realm of the application.</returns>
		public static Realm GetCurrentRealm()
		{
			Realm currentRealm = null;

			using (IUnitOfWork unitOfWork = new EntityUnitOfWork(new ApplicationDbContext()))
			{
				currentRealm = unitOfWork.RealmRepository.Get(r => r.ObsoletionTime == null).Single();
			}

			return currentRealm;
		}
	}
}