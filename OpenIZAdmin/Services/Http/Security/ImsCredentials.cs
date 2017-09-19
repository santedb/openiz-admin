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
 * Date: 2016-8-1
 */

using OpenIZ.Core.Http;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;

namespace OpenIZAdmin.Services.Http.Security
{
	/// <summary>
	/// Represents credentials for the IMS instance.
	/// </summary>
	public class ImsCredentials : Credentials
	{
		/// <summary>
		/// The internal reference to the HTTP headers.
		/// </summary>
		private Dictionary<string, string> httpHeaders;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImsCredentials"/> class
		/// with a specified <see cref="IPrincipal"/> instance.
		/// </summary>
		/// <param name="principal"></param>
		public ImsCredentials(IPrincipal principal) : this(principal, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImsCredentials"/> class
		/// with a specified <see cref="IPrincipal"/> instance and
		/// a <see cref="HttpRequestBase"/> instance.
		/// </summary>
		/// <param name="principal">The current principal for the request.</param>
		/// <param name="request">The HTTP request used to create the headers for the <see cref="ImsCredentials"/>.</param>
		public ImsCredentials(IPrincipal principal, HttpRequestBase request) : base(principal)
		{
			this.httpHeaders = new Dictionary<string, string>();
			this.Request = request;
		}

		/// <summary>
		/// Gets the HTTP request message.
		/// </summary>
		public HttpRequestBase Request { get; }

		/// <summary>
		/// Gets the HTTP headers for the request.
		/// </summary>
		/// <returns>Returns the HTTP headers.</returns>
		public override Dictionary<string, string> GetHttpHeaders()
		{
			if (this.httpHeaders.ContainsKey("Authorization"))
			{
				this.httpHeaders.Remove("Authorization");
			}

			this.httpHeaders.Add("Authorization", $"Bearer {this.Request.Cookies.Get("access_token")?.Value}");

			return this.httpHeaders;
		}
	}
}