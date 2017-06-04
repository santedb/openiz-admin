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
 * Date: 2016-7-17
 */

namespace OpenIZAdmin
{
	/// <summary>
	/// Represents localization configuration for the application.
	/// </summary>
	public static class LocalizationConfig
	{
        /// <summary>
        /// Represents localization language codes
        /// </summary>
        public static class LanguageCode
        {
            /// <summary>
            /// The language code for English
            /// </summary>
            public const string English = "en";

            /// <summary>
            /// The language code for Swahili
            /// </summary>
            public const string Swahili = "sw";
        }

        /// <summary>
        /// The default language for the application.
        /// </summary>
        public static readonly string DefaultLanguage = "en";

		/// <summary>
		/// The default cookie name which contains the users preferred language.
		/// </summary>
		public static readonly string LanguageCookieName = "__openiz_language";
	}
}