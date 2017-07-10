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
 * Date: 2017-7-9
 */

using System;

namespace OpenIZAdmin.Core.Caching
{
	/// <summary>
	/// Represents a cache service.
	/// </summary>
	public interface ICacheService
	{
		/// <summary>
		/// Determines whether [contains] [the specified key].
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns><c>true</c> if [contains] [the specified key]; otherwise, <c>false</c>.</returns>
		bool Contains(string key);

		/// <summary>
		/// Gets the specified key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <returns>Returns the value for a given key.</returns>
		T Get<T>(string key);

		/// <summary>
		/// Gets the specified key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="retrieve">The retrieve.</param>
		/// <returns>Returns the value for a given key, if the value is null the retrieve function is executed.</returns>
		T Get<T>(string key, Func<T> retrieve);

		/// <summary>
		/// Removes the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		void Remove(string key);

		/// <summary>
		/// Sets the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="duration">The duration.</param>
		void Set(string key, object value, TimeSpan duration);

		/// <summary>
		/// Tries the get value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if the value exists, <c>false</c> otherwise.</returns>
		bool TryGetValue<T>(string key, out T value);
	}
}