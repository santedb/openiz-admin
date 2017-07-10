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
using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Core.Extensions
{
	/// <summary>
	/// Represents extension methods for the <see cref="Entity"/> and <see cref="Concept"/> classes
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
		/// Gets the latest version of the concept.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns>Returns the list of concept which are the latest version.</returns>
		public static IEnumerable<Concept> LatestVersionOnly(this IEnumerable<Concept> source)
		{
			var latestVersions = new List<Concept>();

			var keys = source.Select(e => e.Key.Value).Distinct();

			foreach (var key in keys)
			{
				var maxVersionSequence = source.Select(e => source.Where(a => a.Key == key).Max<Concept>(a => a.VersionSequence)).FirstOrDefault();

				var latestVersion = source.FirstOrDefault(a => a.Key == key && a.VersionSequence == maxVersionSequence);

				if (latestVersion != null)
				{
					latestVersions.Add(latestVersion);
				}
			}

			return latestVersions;
		}

		/// <summary>
		/// Gets the latest version of the versioned entity data instance from a given list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <returns>Returns the latest version only of the versioned entity data.</returns>
		public static IEnumerable<T> LatestVersionOnly<T>(this IEnumerable<T> source) where T : VersionedEntityData<Entity>
		{
			var latestVersions = new List<T>();

			var keys = source.Select(e => e.Key.Value).Distinct();

			foreach (var key in keys)
			{
				var maxVersionSequence = source.Select(e => source.Where(a => a.Key == key).Max<T>(a => a.VersionSequence)).FirstOrDefault();

				var latestVersion = source.FirstOrDefault(a => a.Key == key && a.VersionSequence == maxVersionSequence);

				if (latestVersion != null)
				{
					latestVersions.Add(latestVersion);
				}
			}

			return latestVersions;
		}
	}
}
