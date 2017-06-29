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
 * Date: 2017-6-29
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Extensions
{
	/// <summary>
	/// Contains extensions for the <see cref="HttpContextBase"/> class.
	/// </summary>
	public static class HttpContextBaseExtensions
	{
		/// <summary>
		/// Gets the current language.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>System.String.</returns>
		public static string GetCurrentLanguage(this HttpContextBase context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context), Locale.ValueCannotBeNull);
			}

			var cookie = context.Request?.Cookies[LocalizationConfig.LanguageCookieName];

			return cookie?.Value ?? LocalizationConfig.DefaultLanguage;
		}
	}
}