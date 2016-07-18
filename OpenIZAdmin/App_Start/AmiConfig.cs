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
 * Date: 2016-7-10
 */

using System;
using System.Configuration;
using System.Globalization;

namespace OpenIZAdmin
{
	/// <summary>
	/// Represents AMI configuration settings.
	/// </summary>
	public static class AmiConfig
	{
		/// <summary>
		/// The scope for which to request against the AMI.
		/// </summary>
		public static Uri Scope { get; set; }

		public static void Initialize()
		{
			Scope = new Uri(Setting<string>("scope"));
		}

		/// <summary>
		/// Gets a setting from the AppSettings configuration section by name.
		/// </summary>
		/// <typeparam name="T">The data type of the value being retrieved.</typeparam>
		/// <param name="name">The name of the key.</param>
		/// <returns>Returns the value for a given key in the AppSettings configuration section.</returns>
		private static T Setting<T>(string name)
		{
			string value = ConfigurationManager.AppSettings[name];

			if (value == null)
			{
				throw new ConfigurationErrorsException(string.Format("Could not find setting '{0}',", name));
			}

			return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
		}
	}
}