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
 * User: khannan
 * Date: 2016-5-31
 */

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AccountModels;
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
	public class AccountController : Controller
	{
		/// <summary>
		/// The internal reference to the <see cref="AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient amiClient;

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
		/// Changes the password of a user.
		/// </summary>
		/// <param name="model">The model containing the users new password.</param>
		/// <returns>Returns an action result.</returns>
		[HttpPost]
		public ActionResult ChangePassword(ChangePasswordModel model)
		{
			if (ModelState.IsValid)
			{
				var userId = Guid.Parse(User.Identity.GetUserId());

				try
				{
					var user = this.amiClient.GetUser(userId.ToString());

					if (user != null && !user.Lockout.GetValueOrDefault(false))
					{
						user.Password = model.Password;
						user = this.amiClient.UpdateUser(userId, user);
					}

					TempData["success"] = Locale.PasswordChangedSuccessfully;

					return RedirectToAction("Manage");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to change user's password", e.StackTrace);
#endif
					Trace.TraceError("Unable to change user's password", e.Message);
				}
			}

			TempData["error"] = Locale.UnableToChangePassword;

			return RedirectToAction("Manage");
		}

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
		/// Displays the login view.
		/// </summary>
		/// <param name="returnUrl">The return url for an unauthenticated user.</param>
		/// <returns></returns>
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			if (User.Identity.IsAuthenticated && RealmConfig.IsJoinedToRealm())
			{
				return RedirectToAction("Index", "Home");
			}
			else if (!RealmConfig.IsJoinedToRealm())
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

				case SignInStatus.LockedOut:
					return View("Lockout");

				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });

				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", Locale.IncorrectUsernameOrPassword);
					return View(model);
			}
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public ActionResult Manage()
		{
			return View();
		}

		public ActionResult Manage(ManageModel model)
		{
			if (ModelState.IsValid)
			{
			}

			TempData["error"] = Locale.UnableToUpdateProfile;
			return View(model);
		}

		#region Helpers

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		#endregion Helpers
	}
}