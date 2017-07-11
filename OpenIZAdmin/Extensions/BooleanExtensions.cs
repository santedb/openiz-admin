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
 * Date: 2017-6-18
 */

using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Extensions
{
	/// <summary>
	/// Contains extension methods for the <see cref="bool"/> class.
	/// </summary>
	public static class BooleanExtensions
	{
		/// <summary>
		/// Converts a boolean to a string representation to display account status
		/// </summary>
		/// <param name="value">if set to <c>true</c> [value].</param>
		/// <returns>Returns a string based on the boolean value</returns>
		public static string ToLockoutStatus(this bool value)
		{
			return value ? Locale.Locked : Locale.Unlocked;
		}

		/// <summary>
		/// Converts a boolean to a string representation to display user profile activation/deactivation
		/// </summary>
		/// <param name="value">if set to <c>true</c> [value].</param>
		/// <returns>Returns a string based on the boolean value</returns>
		public static string ToObsoleteStatus(this bool value)
		{
			return value ? Locale.Deactivated : Locale.Active;
		}

		/// <summary>
		/// Converts a boolean to a string representation to display Yes or No
		/// </summary>
		/// <param name="value">if set to <c>true</c> [value].</param>
		/// <returns>Returns a string based on the boolean value</returns>
		public static string ToYesOrNo(this bool value)
		{
			return value ? Locale.Yes : Locale.No;
		}
	}
}