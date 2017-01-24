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
 * Date: 2016-5-31
 */

using OpenIZAdmin.Filters;
using System.Web.Mvc;

namespace OpenIZAdmin
{
	/// <summary>
	/// Represents filter configuration for the application.
	/// </summary>
	public class FilterConfig
	{
		/// <summary>
		/// Registers global filters for the application.
		/// </summary>
		/// <param name="filters">The filter collection for which to add filters.</param>
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new LanguageActionFilter());
			filters.Add(new HandleErrorAttribute());
		}
	}
}