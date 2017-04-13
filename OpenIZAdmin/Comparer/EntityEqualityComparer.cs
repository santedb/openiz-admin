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
 * Date: 2017-3-29
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.Entities;

namespace OpenIZAdmin.Comparer
{
	/// <summary>
	/// Represents an entity equality comparer
	/// </summary>
	public class EntityEqualityComparer : IEqualityComparer<Entity>
	{
#pragma warning disable CS1734 // XML comment has a paramref tag, but there is no parameter by that name
                              /// <summary>
                              /// Determines whether the specified objects are equal.
                              /// </summary>
                              /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
                              /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
                              /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(Entity x, Entity y)
#pragma warning restore CS1734 // XML comment has a paramref tag, but there is no parameter by that name
        {
			return x.Key == y.Key;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
		/// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
		public int GetHashCode(Entity obj)
		{
			return obj.GetHashCode();
		}
	}
}