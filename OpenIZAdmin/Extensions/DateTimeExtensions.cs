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

using System;
using System.Globalization;

namespace OpenIZAdmin.Extensions
{
	/// <summary>
	/// Represents a collection of extensions for the <see cref="DateTime"/> class.
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		/// The default date time format.
		/// </summary>
		public const string DefaultDateTimeFormat = "dd/MM/yyyy";

		/// <summary>
		/// Formats a <see cref="DateTime"/> to use the default format.
		/// </summary>
		/// <param name="dateTime">The date time.</param>
		/// <returns>Returns the formatted date time as a string.</returns>
		public static string DefaultFormat(this DateTime dateTime)
		{
			return DefaultFormat(new DateTimeOffset(dateTime));
		}

		/// <summary>
		/// Formats a <see cref="DateTimeOffset" /> to use the default format.
		/// </summary>
		/// <param name="dateTimeOffset">The date time offset.</param>
		/// <returns>Returns the formatted date time as a string.</returns>
		public static string DefaultFormat(this DateTimeOffset dateTimeOffset)
		{
			return dateTimeOffset.ToString(DefaultDateTimeFormat, CultureInfo.InvariantCulture);
		}
	}
}