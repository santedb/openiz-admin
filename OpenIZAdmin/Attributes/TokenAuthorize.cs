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
 * Date: 2016-7-10
 */

using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Elmah;
using Microsoft.AspNet.Identity;

namespace OpenIZAdmin.Attributes
{
	/// <summary>
	/// Validates against whether a user accessing a resource has the correct permissions.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class TokenAuthorize : AuthorizeAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Attributes.TokenAuthorize"/> class.
		/// </summary>
		public TokenAuthorize()
		{
		}

		/// <summary>
		/// Determines whether the current <see cref="System.Security.Principal.IPrincipal"/> is authorized to access the requested resources.
		/// </summary>
		/// <param name="httpContext">The HTTP context.</param>
		/// <returns>Returns true if the current user is authorized to access the request resources.</returns>
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			bool isAuthorized = false;

			var accessToken = httpContext.Request.Cookies["access_token"]?.Value;

			if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrWhiteSpace(accessToken))
			{
				try
				{
					var securityToken = new JwtSecurityToken(accessToken);

					// is the token expired?
					if (securityToken.ValidTo <= DateTime.UtcNow)
					{
						isAuthorized = false;
					}
					else
					{
						isAuthorized = true;
					}
				}
				catch (Exception e)
				{
					isAuthorized = false;
					ErrorLog.GetDefault(httpContext.ApplicationInstance.Context).Log(new Error(e, httpContext.ApplicationInstance.Context));
				}
			}

			return base.AuthorizeCore(httpContext) && isAuthorized;
		}

		/// <summary>
		/// Handles an unauthorized request.
		/// </summary>
		/// <param name="filterContext">The filter context of the request.</param>
		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			filterContext.HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

			filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
			{
				{ "action", "Login" },
				{ "controller", "Account" }
			});

			if (filterContext.HttpContext.Request.IsAjaxRequest())
			{
				filterContext.Result = new HttpUnauthorizedResult();

				filterContext.HttpContext.Response.Clear();
				filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
			}

			filterContext.HttpContext.Response.Cookies.Remove("access_token");
		}
	}
}