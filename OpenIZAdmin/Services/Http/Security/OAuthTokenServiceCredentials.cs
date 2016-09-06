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
 * Date: 2016-7-23
 */

using OpenIZ.Core.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace OpenIZAdmin.Services.Http.Security
{
	/// <summary>
	/// Represents credentials for the IMS authorization endpoint instance.
	/// </summary>
	public class OAuthTokenServiceCredentials : Credentials
	{
		/// <summary>
		/// The internal reference to the authorization key.
		/// </summary>
		private readonly string authKey;

		/// <summary>
		/// The internal reference to the authorization value.
		/// </summary>
		private readonly string authValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="OAuthTokenServiceCredentials"/> class.
		/// </summary>
		/// <param name="principal">Principal.</param>
		public OAuthTokenServiceCredentials(IPrincipal principal) : base(principal)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OAuthTokenServiceCredentials"/> class
		/// with a specified principal, authorization key, and authorization value.
		/// </summary>
		/// <param name="principal">The principal for the request.</param>
		/// <param name="authKey">The authorization key.</param>
		/// <param name="authValue">The authorization value.</param>
		public OAuthTokenServiceCredentials(IPrincipal principal, string authKey, string authValue) : base(principal)
		{
			this.authKey = authKey;
			this.authValue = authValue;
		}

		#region Credentials

		/// <summary>
		/// Get the HTTP headers which are required for the credential
		/// </summary>
		/// <returns>The HTTP headers.</returns>
		public override Dictionary<string, string> GetHttpHeaders()
		{
			// App ID credentials
			string appAuthString = string.Format("{0}:{1}", authKey, authValue);

			// TODO: Add claims
			List<Claim> claims = new List<Claim>()
			{
			};

			// Additional claims?
			if (this.Principal is ClaimsPrincipal)
			{
				claims.AddRange((this.Principal as ClaimsPrincipal).Claims);
			}

			// Build the claim string
			StringBuilder claimString = new StringBuilder();
			foreach (var claim in claims)
			{
				claimString.AppendFormat("{0},", Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}={1}", claim.Type, claim.Value))));
			}

			if (claimString.Length > 0)
			{
				claimString.Remove(claimString.Length - 1, 1);
			}

			// Add authentication header
			var headers = new Dictionary<string, string>()
			{
				{ "Authorization", String.Format("BASIC {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(appAuthString))) }
			};

			if (claimString.Length > 0)
			{
				headers.Add("X-OpenIZClient-Claim", claimString.ToString());
			}

			return headers;
		}

		#endregion Credentials
	}
}