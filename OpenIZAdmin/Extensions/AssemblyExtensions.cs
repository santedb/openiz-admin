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
 * Date: 2017-7-8
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Extensions
{
	/// <summary>
	/// Contains extension methods for the <see cref="Assembly"/> class.
	/// </summary>
	public static class AssemblyExtensions
	{
		// this is courtesy of https://stackoverflow.com/questions/1600962/displaying-the-build-date

		/// <summary>
		/// Gets the build date time.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="targetTimeZoneInfo">The target time zone information.</param>
		/// <returns>Returns the linker time of the assembly.</returns>
		/// <exception cref="System.ArgumentNullException">source</exception>
		public static DateTime? GetBuildDateTime(this Assembly source, TimeZoneInfo targetTimeZoneInfo = null)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source), Locale.ValueCannotBeNull);
			}

			// set the constants
			const int peHeaderOffset = 60;
			const int linkerTimestampOffset = 8;

			var buffer = new byte[2048];

			try
			{
				// read in the assembly file
				using (var stream = new FileStream(source.Location, FileMode.Open, FileAccess.Read))
				{
					stream.Read(buffer, 0, 2048);
				}
			}
			catch (NotSupportedException e)
			{
				Trace.TraceError($"Unable to retrieve build date/time for assembly: {e}");
				return null;
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve build date/time for assembly: {source.Location} {Environment.NewLine} {e}");
				return null;
			}

			// calculate the time
			var offset = BitConverter.ToInt32(buffer, peHeaderOffset);
			var secondsSince1970 = BitConverter.ToInt32(buffer, offset + linkerTimestampOffset);
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

			return TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, targetTimeZoneInfo ?? TimeZoneInfo.Local);
		}
	}
}