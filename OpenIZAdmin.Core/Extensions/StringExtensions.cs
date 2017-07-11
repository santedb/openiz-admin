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
using System;

namespace OpenIZAdmin.Core.Extensions
{
	/// <summary>
	/// Represents a collection of extension methods for the <see cref="System.String"/> class.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Determines whether a string is contained within another string using a case insensitive comparison.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="value">The value.</param>
		/// <param name="comparison">The comparison.</param>
		/// <returns><c>true</c> Returns true if the specified value is contained within the string. Otherwise, <c>false</c>.</returns>
		public static bool Contains(this string source, string value, StringComparison comparison)
		{
			ThrowIfNullSource(source);

			return source.IndexOf(value, comparison) >= 0;
		}

		/// <summary>
		/// Determines whether the string has a trailing back slash.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <returns>Return true if the string has a trailing back slash.</returns>
		public static bool HasTrailingBackSlash(this string source)
		{
			ThrowIfNullSource(source);

			return source.EndsWith("\\");
		}

		/// <summary>
		/// Determines whether the string has a trailing forward slash.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <returns>Return true if the string has a trailing forward slash.</returns>
		public static bool HasTrailingForwardSlash(this string source)
		{
			ThrowIfNullSource(source);

			return source.EndsWith("/");
		}

		/// <summary>
		/// Removes a trailing back slash from a string.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <returns>Returns the string without the trailing back slash.</returns>
		public static string RemoveTrailingBackSlash(this string source)
		{
			return source.Substring(0, source.LastIndexOf('\\'));
		}

		/// <summary>
		/// Removes a trailing forward slash from a string.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <returns>Returns the string without the trailing forward slash.</returns>
		public static string RemoveTrailingForwardSlash(this string source)
		{
			return source.Substring(0, source.LastIndexOf('/'));
		}

		/// <summary>
		/// Converts a <see cref="string" /> to a <see cref="Guid" /> instance. This operation is null safe, unless the strict flag is used.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="strict">When set to true, parsing is directly attempted vs using TryParse.</param>
		/// <returns>Returns the parsed <see cref="Guid" /> instance.</returns>
		public static Guid? ToGuid(this string source, bool strict = false)
		{
			Guid result;

			if (strict)
			{
				return Guid.Parse(source);
			}

			if (!Guid.TryParse(source, out result))
			{
				return null;
			}

			// we don't want to parse an empty GUID value
			if (result == Guid.Empty)
			{
				return null;
			}

			return result;
		}

		/// <summary>
		/// Throws an exception if the source string is null.
		/// </summary>
		/// <param name="source">The source string.</param>
		private static void ThrowIfNullSource(string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(string.Format("{0} cannot be null", nameof(source)));
			}
		}
	}
}
