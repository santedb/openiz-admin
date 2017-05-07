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
 * Date: 2017-5-6
 */

using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations to handle errors.
	/// </summary>
	/// <seealso cref="System.Web.Mvc.Controller" />
	[RoutePrefix("Error")]
	public class ErrorController : Controller
	{
		/// <summary>
		/// Displays the forbidden page.
		/// </summary>
		/// <returns>Returns an action result instance.</returns>
		[HttpGet]
		[Route("Forbidden")]
		public ActionResult Forbidden()
		{
			return View();
		}

		/// <summary>
		/// Displays the internal server error page.
		/// </summary>
		/// <returns>Returns an action result instance.</returns>
		[HttpGet]
		[Route("InternalServerError")]
		public ActionResult InternalServerError()
		{
			return View();
		}

		/// <summary>
		/// Displays the not found view.
		/// </summary>
		/// <returns>Returns an action result instance.</returns>
		[HttpGet]
		[Route("NotFound")]
		public ActionResult NotFound()
		{
			return View();
		}
	}
}