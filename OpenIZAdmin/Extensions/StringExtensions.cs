/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Extensions
{
	/// <summary>
	/// Represents a collection of extension methods for the <see cref="System.String"/> class.
	/// </summary>
	public static class StringExtensions
	{
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