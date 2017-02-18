﻿/*
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
 * User: khannan
 * Date: 2016-5-31
 */

using Elmah;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AccountModels;
using OpenIZAdmin.Util;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations to manage an account.
	/// </summary>
	[Authorize]
	public class AccountController : BaseController
	{
		/// <summary>
		/// The internal reference to the <see cref="ApplicationUserManager"/> instance.
		/// </summary>
		private ApplicationSignInManager signInManager;

		/// <summary>
		/// The internal reference to the <see cref="ApplicationSignInManager"/> instance.
		/// </summary>
		private ApplicationUserManager userManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountController"/> class.
		/// </summary>
		public AccountController()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountController"/> class
		/// with a specified user manager instance and sign in manager instance.
		/// </summary>
		/// <param name="userManager">The user manager.</param>
		/// <param name="signInManager">The sign in manager.</param>
		public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;
		}

		/// <summary>
		/// Gets the sign in manager.
		/// </summary>
		public ApplicationSignInManager SignInManager
		{
			get
			{
				return signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				signInManager = value;
			}
		}

		/// <summary>
		/// Gets the user manager.
		/// </summary>
		public ApplicationUserManager UserManager
		{
			get
			{
				return userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				userManager = value;
			}
		}

		/// <summary>
		/// Displays the change password view.
		/// </summary>
		/// <returns>Returns the change password view.</returns>
		[HttpGet]
		public ActionResult ChangePassword()
		{
			ChangePasswordModel model = new ChangePasswordModel();
			try
			{
				var userId = Guid.Parse(User.Identity.GetUserId());
				SecurityUserInfo user = this.AmiClient.GetUser(userId.ToString());

				if (user != null)
					model.Username = user.UserName;
				else
					TempData["error"] = Locale.User + " " + Locale.NotFound;
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to change password: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to change password: {0}", e.Message);
			}

			return View(model);
		}

		/// <summary>
		/// Changes the password of a user.
		/// </summary>
		/// <param name="model">The model containing the users new password.</param>
		/// <returns>Returns the Home Index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangePassword(ChangePasswordModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var userId = Guid.Parse(User.Identity.GetUserId());
					SecurityUserInfo user = this.AmiClient.GetUser(userId.ToString());

					if (user != null && !user.Lockout.GetValueOrDefault(false))
					{
						var result = await SignInManager.PasswordSignInAsync(model.Username, model.CurrentPassword, false, shouldLockout: false);

						if (result == SignInStatus.Success)
						{
							user.Password = model.Password;
							user = this.AmiClient.UpdateUser(userId, user);
						}
						else
						{
							TempData["error"] = Locale.InvalidCurrrentPassword;
							return View(model);
						}
					}

					TempData["success"] = Locale.PasswordChanged + " " + Locale.Successfully;
					return RedirectToAction("Index", "Home");
				}
				catch (Exception e)
				{
					ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
#if DEBUG
					Trace.TraceError("Unable to change password: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to change password: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.UnableToChangePassword;
			return View(model);
		}

		/// <summary>
		/// Displays the login view.
		/// </summary>
		/// <param name="returnUrl">The return URL for an unauthenticated user.</param>
		/// <returns></returns>
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			if (!RealmConfig.IsJoinedToRealm())
			{
				return RedirectToAction("JoinRealm", "Realm");
			}

			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		/// <summary>
		/// Logs in the user.
		/// </summary>
		/// <param name="model">The login model containing a username and password.</param>
		/// <param name="returnUrl">The return url to redirect to once the user is authenticated.</param>
		/// <returns>Returns a <see cref="System.Threading.Tasks.Task"/> with the result of the login action.</returns>
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, false, shouldLockout: false);

			switch (result)
			{
				case SignInStatus.Success:
					Response.Cookies.Add(new HttpCookie("access_token", SignInManager.AccessToken));
					return RedirectToLocal(returnUrl);

				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", Locale.IncorrectUsernameOrPassword);
					return View(model);
			}
		}

		//
		// POST: /Account/LogOff
		/// <summary>
		/// Logs off the user.
		/// </summary>
		/// <returns>Returns an Index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// Retrieve the user entity.
		/// </summary>
		/// <returns>Returns a Update Profile model.</returns>
		[HttpGet]
		public ActionResult Manage()
		{
			try
			{
				var userId = Guid.Parse(User.Identity.GetUserId());
				var userEntity = UserUtil.GetUserEntityBySecurityUserKey(this.ImsiClient, userId);
				return View(AccountUtil.ToUpdateProfileModel(this.ImsiClient, userEntity));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Profile;
			return View("Index", "Home");
		}

		/// <summary>
		/// Updates a user's profile.
		/// </summary>
		/// <param name="model">The model containing the user profile information to be updated.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UpdateProfile(UpdateProfileModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var userEntity = UserUtil.GetUserEntityBySecurityUserKey(this.ImsiClient, Guid.Parse(User.Identity.GetUserId()));
					var securityUserInfo = this.AmiClient.GetUser(userEntity.SecurityUser.Key.Value.ToString());

					if (userEntity == null || securityUserInfo == null)
					{
						TempData["error"] = Locale.User + " " + Locale.NotFound;

						return RedirectToAction("Index", "Home");
					}

					UserEntity updatedUserEntity = AccountUtil.ToUpdateUserEntity(model, userEntity);
					SecurityUserInfo securityInfo = AccountUtil.ToSecurityUserInfo(model, userEntity, securityUserInfo, this.AmiClient);

					this.AmiClient.UpdateUser(userEntity.SecurityUserKey.Value, securityInfo);
					this.ImsiClient.Update<UserEntity>(updatedUserEntity);

					TempData["success"] = Locale.User + " " + Locale.Updated + " " + Locale.Successfully;
					return RedirectToAction("Index", "Home");
				}
				catch (Exception e)
				{
					ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				}
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Profile;
			//return View("Manage", model);
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// Disposes of any managed resources
		/// </summary>
		/// <param name="disposing">Parameter that acts as a logic switch</param>
		/// <returns>Returns a dispose object result.</returns>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (userManager != null)
				{
					userManager.Dispose();
					userManager = null;
				}

				if (signInManager != null)
				{
					signInManager.Dispose();
					signInManager = null;
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Redirects based on the supplied url
		/// </summary>
		/// <param name="returnUrl">The return url for redirect.</param>
		/// <returns>Returns an action result.</returns>
		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}

			return RedirectToAction("Index", "Home");
		}
	}
}