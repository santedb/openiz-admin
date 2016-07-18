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

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using OpenIZAdmin.Models.Domain;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenIZAdmin.DAL
{
	/// <summary>
	/// Represents the user manager for the application.
	/// </summary>
	public class ApplicationUserManager : UserManager<ApplicationUser>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.DAL.ApplicationUserManager"/> class
		/// with a specified <see cref="Microsoft.AspNet.Identity.IUserStore{TUser}"/> instance.
		/// </summary>
		/// <param name="store"></param>
		public ApplicationUserManager(IUserStore<ApplicationUser> store)
			: base(store)
		{
		}

		/// <summary>
		/// Creates a application user manager instance.
		/// </summary>
		/// <param name="options">The identity options to use for creating the user manager.</param>
		/// <param name="context">The authentication context.</param>
		/// <returns>Returns the newly created instance of the application user manager.</returns>
		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
		{
			var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
			// Configure validation logic for usernames
			manager.UserValidator = new UserValidator<ApplicationUser>(manager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = false
			};

			// Configure validation logic for passwords
			manager.PasswordValidator = new PasswordValidator
			{
				RequiredLength = 6,
				RequireNonLetterOrDigit = true,
				RequireDigit = true,
				RequireLowercase = true,
				RequireUppercase = true,
			};

			// Configure user lockout defaults
			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			manager.MaxFailedAccessAttemptsBeforeLockout = 5;

			// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
			// You can write your own provider and plug it in here.
			manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
			{
				MessageFormat = "Your security code is {0}"
			});
			manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
			{
				Subject = "Security Code",
				BodyFormat = "Your security code is {0}"
			});

			var dataProtectionProvider = options.DataProtectionProvider;

			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
			}

			return manager;
		}

		/// <summary>
		/// Creates an identity result.
		/// </summary>
		/// <param name="user">The user from which to create the identity.</param>
		/// <returns>Returns the newly created identity result.</returns>
		public override Task<IdentityResult> CreateAsync(ApplicationUser user)
		{
			using (IUnitOfWork unitOfWork = new EntityUnitOfWork(new ApplicationDbContext()))
			{
				var activeRealm = unitOfWork.RealmRepository.Get(r => r.ObsoletionTime == null).Single();
				user.RealmId = activeRealm.Id;
			}

			return base.CreateAsync(user);
		}

		/// <summary>
		/// Creates a claims identity.
		/// </summary>
		/// <param name="user">The user from which to create the claims identity.</param>
		/// <param name="authenticationType">The authentication type to use when creating the claims identity.</param>
		/// <returns>Returns the newly created claims identity.</returns>
		public override Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user, string authenticationType)
		{
			return this.ClaimsIdentityFactory.CreateAsync(this, user, authenticationType);
		}
	}
}