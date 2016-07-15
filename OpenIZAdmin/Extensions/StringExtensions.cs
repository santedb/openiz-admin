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
	public static class StringExtensions
	{
		public static bool HasTrailingBackSlash(this string source)
		{
			ThrowIfNullSource(source);

			return source.EndsWith("\\");
			
		}

		public static bool HasTrailingForwardSlash(this string source)
		{
			ThrowIfNullSource(source);

			return source.EndsWith("/");
		}

		public static string RemoveTrailingBackSlash(this string source)
		{
			return source.Substring(0, source.LastIndexOf('\\'));
		}

		public static string RemoveTrailingForwardSlash(this string source)
		{
			return source.Substring(0, source.LastIndexOf('/'));
		}

		private static void ThrowIfNullSource(object source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(string.Format("{0} cannot be null", nameof(source)));
			}
		}
	}
}