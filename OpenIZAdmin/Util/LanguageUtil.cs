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
 * Date: 2016-9-5
 */

using OpenIZAdmin.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Represents a language utility.
	/// </summary>
	public class LanguageUtil
	{
		/// <summary>
		/// Gets a language list.
		/// </summary>
		/// <returns>Returns a list of languages.</returns>
		public static IEnumerable<Language> GetLanguageList()
		{
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

			HashSet<Language> languages = new HashSet<Language>();

			foreach (var item in cultures.Where(c => c.TwoLetterISOLanguageName.Length == 2).Select(c => c).OrderBy(c => c.DisplayName).Select(c => new Language(c.TwoLetterISOLanguageName, c.DisplayName)).Distinct())
			{
				languages.Add(item);
			}

			return languages.AsEnumerable();
		}
	}
}