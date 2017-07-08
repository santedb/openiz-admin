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

namespace OpenIZAdmin.Models
{
	/// <summary>
	/// Represents a language.
	/// </summary>
	public class Language
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Language"/> class.
		/// </summary>
		public Language()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Language"/> class
		/// with a specified code and display name.
		/// </summary>
		/// <param name="code">The language code.</param>
		/// <param name="displayName">The language display name.</param>
		public Language(string code, string displayName)
		{
			this.DisplayName = displayName;
			this.TwoLetterCountryCode = code;
		}

		/// <summary>
		/// Gets or sets the display name of the language.
		/// </summary>
		public virtual string DisplayName { get; }

		/// <summary>
		/// Gets or sets the two letter language code of the language.
		/// </summary>
		public virtual string TwoLetterCountryCode { get; }

		/// <summary>
		/// Compares if two languages are not equal.
		/// </summary>
		/// <param name="left">The first language to compare.</param>
		/// <param name="right">The second language to compare.</param>
		/// <returns>Returns true if the languages are not equal.</returns>
		public static bool operator !=(Language left, Language right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Compares if two languages are equal.
		/// </summary>
		/// <param name="left">The first language to compare.</param>
		/// <param name="right">The second language to compare.</param>
		/// <returns>Returns true if the languages are equal.</returns>
		public static bool operator ==(Language left, Language right)
		{
			if (object.ReferenceEquals(left, right))
			{
				return true;
			}

			return left?.TwoLetterCountryCode == right?.TwoLetterCountryCode;
		}

		/// <summary>
		/// Compares a language is equal to another.
		/// </summary>
		/// <param name="obj">The language to compare against.</param>
		/// <returns>Returns true if the language is equal.</returns>
		public override bool Equals(object obj)
		{
			var language = obj as Language;

			if (language == null)
			{
				return false;
			}

			return language.TwoLetterCountryCode == this.TwoLetterCountryCode;
		}

		/// <summary>
		/// Gets the hash code of the language.
		/// </summary>
		/// <returns>Returns the hash code of the language.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode() ^ TwoLetterCountryCode.GetHashCode();
		}
	}
}