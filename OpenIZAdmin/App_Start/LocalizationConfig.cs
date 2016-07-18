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
 * Date: 2016-7-17
 */
using OpenIZAdmin.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin
{
	/// <summary>
	/// Represents localization configuration for the application.
	/// </summary>
	public static class LocalizationConfig
	{
		/// <summary>
		/// The default language for the application.
		/// </summary>
		internal static readonly string DefaultLanguage = "en";

		/// <summary>
		/// Gets the preferred language of a user.
		/// </summary>
		/// <param name="userId">The id of the user for which to retrieve the language.</param>
		/// <returns>Returns the preferred language of the user.</returns>
		internal static string GetPreferredLanguage(string userId)
		{
			string preferredLanguage = LocalizationConfig.DefaultLanguage;

			using (IUnitOfWork unitOfWork = new EntityUnitOfWork(new ApplicationDbContext()))
			{
				var user = unitOfWork.UserRepository.FindById(userId);

				if (user != null && user.Language != null)
				{
					preferredLanguage = user.Language;
				}
			}

			return preferredLanguage;
		}
	}
}