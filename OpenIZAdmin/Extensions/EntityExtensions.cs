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
 * Date: 2017-6-29
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Extensions
{
	/// <summary>
	/// Contains extensions for the <see cref="Entity"/> class.
	/// </summary>
	public static class EntityExtensions
	{
		/// <summary>
		/// Gets the full name of the user entity.
		/// </summary>
		/// <param name="entity">The user entity.</param>
		/// <param name="nameUseKey">The name use key.</param>
		/// <returns>Returns the full name of the user entity.</returns>
		/// <exception cref="System.ArgumentNullException">If the entity is null.</exception>
		public static string GetFullName(this Entity entity, Guid nameUseKey)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity), Locale.ValueCannotBeNull);
			}

			var given = entity.Names.Where(n => n.NameUseKey == nameUseKey).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value).ToList();
			var family = entity.Names.Where(n => n.NameUseKey == nameUseKey).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value).ToList();

			return string.Join(" ", given) + " " + string.Join(" ", family);
		}

		/// <summary>
		/// Gets the name of the entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="nameUseKey">The name use key.</param>
		/// <returns>Returns the name of the entity.</returns>
		/// <exception cref="System.ArgumentNullException">If the entity is null.</exception>
		public static string GetName(this Entity entity, Guid nameUseKey)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity), Locale.ValueCannotBeNull);
			}

			return string.Join(" ", entity.Names.Where(n => n.NameUseKey == nameUseKey).SelectMany(n => n.Component).Select(c => c.Value).ToList());
		}
	}
}