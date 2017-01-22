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

using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;
using OpenIZAdmin.Models.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZAdmin.DAL
{
	/// <summary>
	/// Represents the sign in manager for the application.
	/// </summary>
	public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.DAL.ApplicationSignInManager"/> class
		/// with a specified <see cref="OpenIZAdmin.DAL.ApplicationUserManager"/> instance and a
		/// specified <see cref="Microsoft.Owin.Security.IAuthenticationManager"/> instance.
		/// </summary>
		/// <param name="userManager">The user manager instance.</param>
		/// <param name="authenticationManager">The authentication manager instance.</param>
		public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
			: base(userManager, authenticationManager)
		{
		}

		/// <summary>
		/// Gets the access token.
		/// </summary>
		public string AccessToken { get; private set; }

		/// <summary>
		/// Creates a user identity.
		/// </summary>
		/// <param name="user">The user for which to create the identity from.</param>
		/// <returns>Returns the newly created user identity.</returns>
		public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
		{
			return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
		}

		/// <summary>
		/// Creates an instance of the <see cref="OpenIZAdmin.DAL.ApplicationSignInManager"/> class.
		/// </summary>
		/// <param name="options">The options of the sign in manager.</param>
		/// <param name="context">The context of the sign in manager.</param>
		/// <returns></returns>
		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
		{
			return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
		}

		/// <summary>
		/// Signs in a user using a username and password.
		/// </summary>
		/// <param name="userName">The username of the user.</param>
		/// <param name="password">The password of the user.</param>
		/// <param name="isPersistent">Whether the user session is persistent.</param>
		/// <param name="shouldLockout">Whether the user should be locked out.</param>
		/// <returns>Returns a sign in status.</returns>
		public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
		{
			Realm currentRealm = RealmConfig.GetCurrentRealm();

			if (currentRealm == null)
			{
				throw new InvalidOperationException("Must be joined to realm before attempting to sign in");
			}

			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.Add("Authorization", "BASIC " + Convert.ToBase64String(Encoding.UTF8.GetBytes(currentRealm.ApplicationId + ":" + currentRealm.ApplicationSecret)));

				StringContent content = new StringContent(string.Format("grant_type=password&username={0}&password={1}&scope={2}/imsi", userName, password, currentRealm.Address));

				// HACK: have to remove the headers before adding them...
				content.Headers.Remove("Content-Type");
				content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

				var result = await client.PostAsync(string.Format("{0}/auth/oauth2_token", currentRealm.Address), content);

				if (result.IsSuccessStatusCode)
				{
					return await this.SignInAsync(result, userName);
				}
				else
				{
					return SignInStatus.Failure;
				}
			}
		}

		/// <summary>
		/// Signs in a user.
		/// </summary>
		/// <param name="result">The HTTP response message from the IMS.</param>
		/// <param name="username">The username of the user.</param>
		/// <returns>Returns a sign in status successful if the user is signed in successfully.</returns>
		private async Task<SignInStatus> SignInAsync(HttpResponseMessage result, string username)
		{
			var responseAsString = await result.Content.ReadAsStringAsync();

			var response = JObject.Parse(responseAsString);

			string accessToken = response.GetValue("access_token").ToString();
			string expiresIn = response.GetValue("expires_in").ToString();
			string tokenType = response.GetValue("token_type").ToString();
#if DEBUG
			Trace.TraceInformation("Access token: {0}", accessToken);
			Trace.TraceInformation("Expires in: {0}", expiresIn);
			Trace.TraceInformation("Token type {0}", tokenType);
#endif
			Dictionary<string, string> authenticationDictionary = new Dictionary<string, string>();

			authenticationDictionary.Add("username", username);
			authenticationDictionary.Add("access_token", accessToken);
			authenticationDictionary.Add("token_type", tokenType);

			AuthenticationProperties properties = new AuthenticationProperties(authenticationDictionary);

			properties.IsPersistent = false;

			JwtSecurityToken securityToken = new JwtSecurityToken(accessToken);

			var user = await this.UserManager.FindByIdAsync(securityToken.Claims.First(c => c.Type == "sub").Value);

			if (user == null)
			{
				user = new ApplicationUser
				{
					Id = securityToken.Claims.First(c => c.Type == "sub").Value,
					UserName = securityToken.Claims.First(c => c.Type == "unique_name").Value
				};

				string email = securityToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

				if (email != null)
				{
					if (email.StartsWith("mailto:"))
					{
						int mailto = email.LastIndexOf("mailto:");
						email = email.Substring(7, email.Length - 7);
					}

					user.Email = email;
				}

				foreach (var claim in securityToken.Claims)
				{
					IdentityUserClaim identityUserClaim = new IdentityUserClaim
					{
						ClaimType = claim.Type,
						ClaimValue = claim.Value,
						UserId = securityToken.Claims.First(c => c.Type == "sub").Value
					};

					user.Claims.Add(identityUserClaim);
				}

				var identityResult = await this.UserManager.CreateAsync(user);

				if (!identityResult.Succeeded)
				{
					return SignInStatus.Failure;
				}
				else
				{
					var userIdentity = await this.CreateUserIdentityAsync(user);

					this.AuthenticationManager.SignIn(properties, userIdentity);
					this.AccessToken = accessToken;
				}
			}
			else
			{
				var userIdentity = await this.CreateUserIdentityAsync(user);

				this.AuthenticationManager.SignIn(properties, userIdentity);
				this.AccessToken = accessToken;
			}

			return SignInStatus.Success;
		}
	}
}