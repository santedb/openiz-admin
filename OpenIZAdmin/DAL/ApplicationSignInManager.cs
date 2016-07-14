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
	public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
	{
		public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
			: base(userManager, authenticationManager)
		{
		}

		public string AccessToken { get; private set; }

		public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
		{
			return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
		}

		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
		{
			return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
		}

		public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
		{
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.Add("Authorization", "BASIC " + Convert.ToBase64String(Encoding.UTF8.GetBytes(AmiConfig.ApplicationId + ":" + AmiConfig.ApplicationSecret)));

				StringContent content = new StringContent(string.Format("grant_type=password&username={0}&password={1}&scope={2}", userName, password, AmiConfig.Scope));

				// HACK: have to remove the headers before adding them...
				content.Headers.Remove("Content-Type");
				content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

				var result = await client.PostAsync(AmiConfig.AmiTokenEndpoint, content);

				if (result.IsSuccessStatusCode)
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

					authenticationDictionary.Add("username", userName);
					authenticationDictionary.Add("access_token", accessToken);
					authenticationDictionary.Add("token_type", tokenType);

					AuthenticationProperties properties = new AuthenticationProperties(authenticationDictionary);

					properties.IsPersistent = false;

					JwtSecurityToken securityToken = new JwtSecurityToken(accessToken);

					var user = await this.UserManager.FindByIdAsync(securityToken.Claims.First(c => c.Type == "nameid").Value);

					if (user == null)
					{
						string email = securityToken.Claims.First(c => c.Type == "email").Value;
						int mailto = email.LastIndexOf("mailto:");
						email = email.Substring(7, email.Length - 7);

						user = new ApplicationUser
						{
							Id = securityToken.Claims.First(c => c.Type == "nameid").Value,
							Email = email,
							UserName = securityToken.Claims.First(c => c.Type == "unique_name").Value
						};

						foreach (var claim in securityToken.Claims)
						{
							IdentityUserClaim identityUserClaim = new IdentityUserClaim
							{
								ClaimType = claim.Type,
								ClaimValue = claim.Value,
								UserId = securityToken.Claims.First(c => c.Type == "nameid").Value
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
				else
				{
					return SignInStatus.Failure;
				}
			}
		}
	}
}