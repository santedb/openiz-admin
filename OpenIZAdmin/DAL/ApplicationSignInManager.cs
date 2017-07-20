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
using Microsoft.AspNet.Identity;
using OpenIZAdmin.Security;

namespace OpenIZAdmin.DAL
{
	/// <summary>
	/// Represents the sign in manager for the application.
	/// </summary>
	public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationSignInManager"/> class
		/// with a specified <see cref="ApplicationUserManager"/> instance and a
		/// specified <see cref="IAuthenticationManager"/> instance.
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
		/// Login to the IMS as the device.
		/// </summary>
		/// <returns>Returns an IPrincipal representing the logged in device or null if the login fails.</returns>
		/// <exception cref="System.InvalidOperationException">If the application is not joined to a realm.</exception>
		public static DeviceIdentity LoginAsDevice()
		{
			DeviceIdentity deviceIdentity = null;

			using (var client = new HttpClient())
			using (var unitOfWork = new EntityUnitOfWork(new ApplicationDbContext()))
			{
				var realm = unitOfWork.RealmRepository.AsQueryable().SingleOrDefault(r => r.ObsoletionTime == null);

				if (realm == null)
				{
					throw new InvalidOperationException("Not joined to realm");
				}

				client.DefaultRequestHeaders.Add("Authorization", "BASIC " + Convert.ToBase64String(Encoding.UTF8.GetBytes(realm.ApplicationId + ":" + realm.ApplicationSecret)));

				var content = new StringContent($"grant_type=password&username={realm.DeviceId}&password={realm.DeviceSecret}&scope={realm.Address}/imsi");

				// HACK: have to remove the headers before adding them...
				content.Headers.Remove("Content-Type");
				content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

				var result = client.PostAsync($"{realm.Address}/auth/oauth2_token", content).Result;

				if (result.IsSuccessStatusCode)
				{
					var response = JObject.Parse(result.Content.ReadAsStringAsync().Result);

					var accessToken = response.GetValue("access_token").ToString();
#if DEBUG
					Trace.TraceInformation($"Access token: {accessToken}");
#endif
					var securityToken = new JwtSecurityToken(accessToken);

					deviceIdentity = new DeviceIdentity(Guid.Parse(securityToken.Claims.First(c => c.Type == "sub").Value), realm.DeviceId, true)
					{
						AccessToken = accessToken
					};
				}
			}

			return deviceIdentity;
		}

		/// <summary>
		/// Tfas the sign in asynchronous.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <param name="tfaSecret">The tfa secret.</param>
		/// <returns>Task&lt;SignInStatus&gt;.</returns>
		/// <exception cref="System.InvalidOperationException">Must be joined to realm before attempting to sign in</exception>
		public async Task<SignInStatus> TfaSignInAsync(string username, string password, string tfaSecret)
		{
			var currentRealm = RealmConfig.GetCurrentRealm();

			if (currentRealm == null)
			{
				throw new InvalidOperationException("Must be joined to realm before attempting to sign in");
			}

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Add("Authorization", "BASIC " + Convert.ToBase64String(Encoding.UTF8.GetBytes(currentRealm.ApplicationId + ":" + currentRealm.ApplicationSecret)));
				client.DefaultRequestHeaders.Add("X-OpenIZ-TfaSecret", tfaSecret);

				var content = new StringContent($"grant_type=password&username={username}&password={password}&scope={currentRealm.Address}/imsi");

				// HACK: have to remove the headers before adding them...
				content.Headers.Remove("Content-Type");
				content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

				var result = await client.PostAsync($"{currentRealm.Address}/auth/oauth2_token", content);

				if (result.IsSuccessStatusCode)
				{
					var response = JObject.Parse(await result.Content.ReadAsStringAsync());

					this.AccessToken = response.GetValue("access_token").ToString();

					return SignInStatus.Success;
				}

				return SignInStatus.Failure;
			}
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
			var currentRealm = RealmConfig.GetCurrentRealm();

			if (currentRealm == null)
			{
				throw new InvalidOperationException("Must be joined to realm before attempting to sign in");
			}

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Add("Authorization", "BASIC " + Convert.ToBase64String(Encoding.UTF8.GetBytes(currentRealm.ApplicationId + ":" + currentRealm.ApplicationSecret)));

				var content = new StringContent($"grant_type=password&username={userName}&password={password}&scope={currentRealm.Address}/imsi");

				// HACK: have to remove the headers before adding them...
				content.Headers.Remove("Content-Type");
				content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

				var result = await client.PostAsync($"{currentRealm.Address}/auth/oauth2_token", content);

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

			var accessToken = response.GetValue("access_token").ToString();
			var tokenType = response.GetValue("token_type").ToString();
			var authenticationDictionary = new Dictionary<string, string>
			{
				{ "username", username },
				{ "access_token", accessToken },
				{ "token_type", tokenType }
			};

			var properties = new AuthenticationProperties(authenticationDictionary)
			{
				IsPersistent = false
			};

			var securityToken = new JwtSecurityToken(accessToken);

			// if the user is not a part of the administrators group, we don't allow them to login
			if (!securityToken.Claims.Any(o=>o.Type == Constants.OpenIzGrantedPolicyClaim && o.Value == Constants.Login))
			{
				return SignInStatus.Failure;
			}

			var user = await this.UserManager.FindByIdAsync(securityToken.Claims.First(c => c.Type == "sub").Value);

			if (user == null)
			{
				user = new ApplicationUser
				{
					Id = securityToken.Claims.First(c => c.Type == "sub").Value,
					UserName = securityToken.Claims.First(c => c.Type == "unique_name").Value
				};

				var email = securityToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

				if (email != null)
				{
					if (email.StartsWith("mailto:"))
					{
						email = email.Substring(7, email.Length - 7);
					}

					user.Email = email;
				}

				foreach (var identityUserClaim in securityToken.Claims.Select(claim => new IdentityUserClaim { ClaimType = claim.Type, ClaimValue = claim.Value, UserId = securityToken.Claims.First(c => c.Type == "sub").Value }))
				{
					user.Claims.Add(identityUserClaim);
				}

				var identityResult = await this.UserManager.CreateAsync(user);

				if (!identityResult.Succeeded)
				{
					return SignInStatus.Failure;
				}

				var userIdentity = await this.CreateUserIdentityAsync(user);

                // Add the things we're allowed to do as we will use them in the UI
                foreach (var stgp in securityToken.Claims.Where(o => o.Type == Constants.OpenIzGrantedPolicyClaim))
                    userIdentity.AddClaim(new Claim(Constants.OpenIzGrantedPolicyClaim, stgp.Value));

				this.AuthenticationManager.SignIn(properties, userIdentity);
				this.AccessToken = accessToken;
			}
			else
			{
				var userIdentity = await this.CreateUserIdentityAsync(user);

                foreach (var stgp in securityToken.Claims.Where(o => o.Type == Constants.OpenIzGrantedPolicyClaim))
                    userIdentity.AddClaim(new Claim(Constants.OpenIzGrantedPolicyClaim, stgp.Value));

                this.AuthenticationManager.SignIn(properties, userIdentity);
				this.AccessToken = accessToken;
			}

			return SignInStatus.Success;
		}
	}
}