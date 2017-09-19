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
using System.Runtime.Caching;

namespace OpenIZAdmin.Core.Caching
{
	/// <summary>
	/// Represents a memory cache service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Core.Caching.IMemoryCacheService" />
	public class MemoryCacheService : IMemoryCacheService
	{
		/// <summary>
		/// Gets the cache.
		/// </summary>
		/// <value>The cache.</value>
		private MemoryCache Cache => MemoryCache.Default;

		/// <summary>
		/// Check if the cache contains a value under a key.
		/// </summary>
		/// <param name="key">The key of the cache entry to check if it exists.</param>
		/// <returns>True if a cache entry for the key exists, false if nothing was found.</returns>
		public bool Contains(string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			return Cache.Contains(key);
		}

		/// <summary>
		/// Gets a cache item by its key.
		/// </summary>
		/// <typeparam name="T">The type of item to retrieve.</typeparam>
		/// <param name="key">The key the item is under.</param>
		/// <returns>Stored cache value, or the default value of the type.</returns>
		public T Get<T>(string key)
		{
			var value = Cache[key];

			if (value == null)
				return default(T);

			return (T)Cache[key];
		}

		/// <summary>
		/// Gets a cache item by its key, if it doesn't exist, it runs <paramref name="retrieve"/> and stores the result under the key.
		/// </summary>
		/// <typeparam name="T">The type of item to retrieve.</typeparam>
		/// <param name="key">The key the item is under.</param>
		/// <param name="retrieve">Function to retrieve the value associated with the key if it is not cached.</param>
		/// <returns>Stored cache value.</returns>
		public T Get<T>(string key, Func<T> retrieve)
		{
			var item = Get<T>(key);

			if (item != null)
				return item;

			item = retrieve();

			Set(key, item, TimeSpan.FromHours(24));

			return item;
		}

		/// <summary>
		/// Remove a cache entry from the cache.
		/// </summary>
		/// <param name="key">The key of the cache entry to remove.</param>
		public void Remove(string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			Cache.Remove(key);
		}

		/// <summary>
		/// Stores an item into the cache under a key.
		/// </summary>
		/// <param name="key">The key to store the value under.</param>
		/// <param name="value">The value to store.</param>
		/// <param name="duration">The duration to store.</param>
		public void Set(string key, object value, TimeSpan duration)
		{
			if (value == null)
				return;

			if (key == null)
				throw new ArgumentNullException(nameof(key));

			var policy = new CacheItemPolicy
			{
				AbsoluteExpiration = DateTime.UtcNow + duration
			};

			Cache.Add(new CacheItem(key, value), policy);
		}

		/// <summary>
		/// Get's an item from the cache based on the provided key.
		/// The return value indicates if it was successful.
		/// </summary>
		/// <typeparam name="T">The type of item being retrieved.</typeparam>
		/// <param name="key">The key of the item to retrieve.</param>
		/// <param name="value">The item retrieved if successful, otherwise the default value of the type.</param>
		/// <returns>True if the item was retrieved, false if the item was not found.</returns>
		public bool TryGetValue<T>(string key, out T value)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			value = default(T);

			var item = Cache.Get(key);

			if (item == null)
				return false;

			value = (T)item;

			return true;
		}
	}
}