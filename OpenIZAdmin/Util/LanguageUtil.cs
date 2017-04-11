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
 * Date: 2016-9-5
 */

using OpenIZAdmin.Localization;
using OpenIZAdmin.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

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
			var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

			var languages = new List<Language>();

			foreach (var item in cultures.Where(c => c.TwoLetterISOLanguageName.Length == 2)
				.Select(c => c)
				.Select(c => new Language(c.TwoLetterISOLanguageName, c.DisplayName))
				.Distinct(new LanguageEqualityComparer())
				.Where(item => !languages.Contains(item)))
			{
				languages.Add(item);
			}

			return languages.AsEnumerable();
		}

        /// <summary>
        /// Gets a language list as a SelectListItem list
        /// </summary>
        /// <returns>Returns a list of SelectListItems</returns>
        public static IEnumerable<SelectListItem> GetSelectListItemLanguageList()
	    {
            var languages = GetLanguageList();
            //return languages.Select(l => new SelectListItem { Text = l.DisplayName, Value = l.TwoLetterCountryCode, Selected = l.TwoLetterCountryCode == Locale.EN }).OrderBy(l => l.Text).ToList();	        
            return languages.Select(l => new SelectListItem { Text = l.DisplayName, Value = l.TwoLetterCountryCode }).OrderBy(l => l.Text).ToList();
        }
	}

	/// <summary>
	/// Represents a language equality comparer.
	/// </summary>
	internal class LanguageEqualityComparer : IEqualityComparer<Language>
	{
		/// <summary>
		/// Determines whether the specified objects are equal.
		/// </summary>
		/// <returns>
		/// true if the specified objects are equal; otherwise, false.
		/// </returns>
		/// <param name="x">The first object of type <paramref name="x"/> to compare.</param><param name="y">The second object of type <paramref name="y"/> to compare.</param>
		public bool Equals(Language x, Language y)
		{
			return x == y;
		}

		/// <summary>
		/// Returns a hash code for the specified object.
		/// </summary>
		/// <returns>
		/// A hash code for the specified object.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
		public int GetHashCode(Language obj)
		{
			return obj.GetHashCode();
		}
	}
}