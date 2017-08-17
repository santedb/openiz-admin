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

using Microsoft.AspNet.Identity;
using OpenIZAdmin.Services.Security;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OpenIZAdmin.Attributes
{
	/// <summary>
	/// Validates against whether a user accessing a resource has the correct permissions.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class TokenAuthorize : AuthorizeAttribute
	{
		/// <summary>
		/// The policies to enforce as a part of the authorization.
		/// </summary>
		private readonly List<string> policies;

		/// <summary>
		/// Initializes a new instance of the <see cref="TokenAuthorize"/> class.
		/// </summary>
		public TokenAuthorize()
		{
			this.policies = new List<string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TokenAuthorize"/> class.
		/// </summary>
		/// <param name="policies">The policies.</param>
		public TokenAuthorize(params string[] policies)
		{
			this.policies = policies?.ToList() ?? new List<string>();
		}

		/// <summary>
		/// Determines whether the current <see cref="System.Security.Principal.IPrincipal"/> is authorized to access the requested resources.
		/// </summary>
		/// <param name="httpContext">The HTTP context.</param>
		/// <returns>Returns true if the current user is authorized to access the request resources.</returns>
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			var isAuthorized = false;

			var accessToken = httpContext.Request.Cookies["access_token"]?.Value;

			if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrWhiteSpace(accessToken))
			{
				try
				{
					isAuthorized = !JwtUtil.IsExpired(accessToken);

					// if there is no login or login as a service permission, we must manually validate against that
					if (!policies.Contains(Constants.Login))
					{
						// validate the login permission
						new PolicyPermission(PermissionState.Unrestricted, Constants.Login, httpContext.User).Demand();
					}
					else if (!policies.Contains(Constants.Login) && !policies.Contains(Constants.LoginAsService))
					{
						// validate the login as a service permission only if login and login as a service are missing from the validation list
						new PolicyPermission(PermissionState.Unrestricted, Constants.LoginAsService, httpContext.User).Demand();
					}

					foreach (var policy in policies)
					{
						// demand the permission
						new PolicyPermission(PermissionState.Unrestricted, policy, httpContext.User).Demand();
					}
				}
				catch (Exception e)
				{
					isAuthorized = false;
					Trace.TraceError($"Unable to decode token: {e}");
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