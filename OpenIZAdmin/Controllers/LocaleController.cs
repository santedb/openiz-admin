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
 * Date: 2016-7-24
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations to change locale.
	/// </summary>
    public class LocaleController : Controller
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Controllers.LocaleController"/> class.
		/// </summary>
		public LocaleController()
		{

		}

		/// <summary>
		/// Changes a users language.
		/// </summary>
		/// <param name="language">The language to change to.</param>
		/// <returns>Returns an action result.</returns>
		[HttpPost]
		public ActionResult ChangeLanguage(string language = "en")
		{
			// only supporting 2 letter language codes to start
			if (language.Length != 2)
			{
				return Json(LocalizationConfig.DefaultLanguage, JsonRequestBehavior.AllowGet);
			}

			Response.Cookies.Add(new HttpCookie(LocalizationConfig.LanguageCookieName, language));

			return Json(Thread.CurrentThread.CurrentUICulture.ToString(), JsonRequestBehavior.AllowGet);
		}
    }
}