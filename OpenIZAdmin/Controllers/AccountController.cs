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
 * User: khannan
 * Date: 2016-5-31
 */

using Elmah;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.AccountModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Services.Http;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations to manage an account.
	/// </summary>
	[TokenAuthorize]
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
			var model = new ChangePasswordModel();

			try
			{
				var securityUser = this.AmiClient.GetUser(this.User.Identity.GetUserId());

				if (securityUser == null)
				{
					// if the user is null yet they are logged in, we need to log them out for security purposes
					TempData["error"] = Locale.User + " " + Locale.NotFound;

					HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
					Response.Cookies.Remove("access_token");

					return RedirectToAction("Login", "Account");
				}

				model.Username = securityUser.UserName;
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
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
			try
			{
				if (ModelState.IsValid)
				{
					var securityUser = this.AmiClient.GetUser(this.User.Identity.GetUserId());

					if (securityUser == null)
					{
						// if the user is null yet they are logged in, we need to log them out for security purposes
						TempData["error"] = Locale.User + " " + Locale.NotFound;

						HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
						Response.Cookies.Remove("access_token");

						return RedirectToAction("Login", "Account");
					}

					var result = await SignInManager.PasswordSignInAsync(model.Username, model.CurrentPassword, false, false);

					if (result == SignInStatus.Success)
					{
						securityUser.User = null;
						securityUser.Password = model.Password;

						this.AmiClient.UpdateUser(securityUser.UserId.Value, securityUser);

						TempData["success"] = Locale.Password + " " + Locale.Updated + " " + Locale.Successfully;

						HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
						Response.Cookies.Remove("access_token");

						return RedirectToAction("Login", "Account");
					}
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Password;

			return View(model);
		}

		/// <summary>
		/// Displays the forgot password view.
		/// </summary>
		/// <returns>Returns the forgot password view.</returns>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			var model = new ForgotPasswordModel();

			try
			{
				// ensure the device/service account is logged out first, so that we have the most recent access token value
				this.HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
				this.Response.Cookies.Remove("access_token");

				var amiServiceClient = GetDeviceServiceClient();

				var twoFactorAuthenticationMechanisms = amiServiceClient.GetTwoFactorMechanisms();

				model.TfaMechanisms = twoFactorAuthenticationMechanisms.CollectionItem.Select(t => new TfaMechanismModel(t)).ToList();
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnableTo + " " + Locale.Retrieve + " " + Locale.ForgotPasswordMechanisms;

				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
			}

			return View(model);
		}

		/// <summary>
		/// Allows the user to receive a code to reset their password.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ForgotPassword(ForgotPasswordModel model)
		{
			var amiServiceClient = GetDeviceServiceClient();

			try
			{
				if (this.ModelState.IsValid)
				{
					amiServiceClient.SendTfaSecret(new TfaRequestInfo
					{
						Purpose = "PasswordReset",
						ResetMechanism = model.TfaMechanism,
						UserName = model.Username,
						Verification = model.Verification
					});

					var user = this.AmiClient.GetUsers(u => u.UserName == model.Username && u.ObsoletionTime == null).CollectionItem.FirstOrDefault();

					if (user == null)
					{
						this.TempData["error"] = Locale.UnableToReset + " " + Locale.Password;
						return RedirectToAction("ForgotPassword");
					}

					var resetPasswordModel = new ResetPasswordModel
					{
						UserId = user.UserId.Value
					};

					this.TempData["success"] = Locale.ResetCodeSent;

					return View("ResetPassword", resetPasswordModel);
				}
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnableTo + " " + Locale.Reset + " " + Locale.Password;

				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
			}

			var twoFactorAuthenticationMechanisms = amiServiceClient.GetTwoFactorMechanisms();

			model.TfaMechanisms = model.TfaMechanisms = twoFactorAuthenticationMechanisms.CollectionItem.Select(t => new TfaMechanismModel(t)).ToList();

			this.TempData["error"] = Locale.UnableToReset + " " + Locale.Password;

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

		/// <summary>
		/// Logs off the user.
		/// </summary>
		/// <returns>Returns an Index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			this.Response.Cookies.Remove("access_token");
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// Resets the password.
		/// </summary>
		/// <returns>ActionResult.</returns>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult ResetPassword()
		{
			return View();
		}

		/// <summary>
		/// Resets the password.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[AllowAnonymous]
		[ActionName("ResetPassword")]
		public async Task<ActionResult> ResetPasswordAsync(ResetPasswordModel model)
		{
			try
			{
				// ensure the device/service account is logged out first, so that we have the most recent access token value
				this.HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
				this.Response.Cookies.Remove("access_token");

				if (this.ModelState.IsValid)
				{
					var amiServiceClient = GetDeviceServiceClient();

					var user = amiServiceClient.GetUser(model.UserId.ToString());

					if (user == null || user?.User?.ObsoletionTime != null)
					{
						this.TempData["error"] = Locale.UnableToReset + " " + Locale.Password;
						return RedirectToAction("ForgotPassword");
					}

					// perform a TFA sign to force the password update
					var result = await this.SignInManager.TfaSignInAsync(user.UserName, null, model.Code);

					if (result == SignInStatus.Success)
					{
						amiServiceClient = new AmiServiceClient(new RestClientService(Constants.Ami, this.HttpContext, this.SignInManager.AccessToken));

						// null out the other properties, since we are only updating the password
						user.User = null;
						user.Lockout = null;
						user.Roles.Clear();

						user.Password = model.Password;

						amiServiceClient.UpdateUser(user.UserId.Value, user);

						this.TempData["success"] = Locale.Password + " " + Locale.Reset + " " + Locale.Successfully;
					}

					switch (result)
					{
						case SignInStatus.Success:
							// force a signout
							this.HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
							Response.Cookies.Remove("access_token");
							return RedirectToAction("Login");
						default:
							ModelState.AddModelError("", Locale.IncorrectUsernameOrPassword);
							break;
					}
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
			}

			this.TempData["error"] = Locale.UnableTo + " " + Locale.Reset + " " + Locale.Password;

			return View("ResetPassword", model);
		}

		/// <summary>
		/// Retrieve the user entity.
		/// </summary>
		/// <returns>Returns a Update Profile model.</returns>
		[HttpGet]
		public ActionResult UpdateProfile()
		{
			try
			{
				var userEntity = UserUtil.GetUserEntityBySecurityUserKey(this.ImsiClient, Guid.Parse(User.Identity.GetUserId()));

				if (userEntity.SecurityUser == null)
				{
					userEntity.SecurityUser = this.AmiClient.GetUser(userEntity.SecurityUserKey.ToString())?.User;
				}

				var model = new UpdateProfileModel(userEntity);

				var facilityRelationship = userEntity.Relationships.FirstOrDefault(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);

				var place = facilityRelationship?.TargetEntity as Place;

				if (facilityRelationship?.TargetEntityKey.HasValue == true && place == null)
				{
					place = this.ImsiClient.Get<Place>(facilityRelationship.TargetEntityKey.Value, null) as Place;
				}

				if (place != null)
				{
					var facility = new List<FacilityModel>
					{
						new FacilityModel(string.Join(" ", place.Names.SelectMany(n => n.Component).Select(c => c.Value)), place.Key?.ToString())
					};

					model.FacilityList.AddRange(facility.Select(f => new SelectListItem { Text = f.Name, Value = f.Id, Selected = f.Id == place.Key?.ToString() }));
					model.Facility = place.Key?.ToString();
				}

				model.PhoneTypeList = AccountUtil.GetPhoneTypeList(this.ImsiClient);
				model.PhoneTypeList = model.PhoneTypeList.Select(p => new SelectListItem { Selected = p.Value == model.PhoneType, Text = p.Text, Value = p.Value }).OrderBy(p => p.Text).ToList();

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToRetrieve + " " + Locale.Profile;

			return Redirect(Request.UrlReferrer.ToString());
		}

		/// <summary>
		/// Updates a user's profile.
		/// </summary>
		/// <param name="model">The user information to update.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UpdateProfile(UpdateProfileModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var userId = Guid.Parse(User.Identity.GetUserId());

					var securityUserInfo = this.AmiClient.GetUser(userId.ToString());
					var userEntity = UserUtil.GetUserEntityBySecurityUserKey(this.ImsiClient, userId);

					if (securityUserInfo == null || userEntity == null)
					{
						TempData["error"] = Locale.User + " " + Locale.NotFound;

						return RedirectToAction("Index", "Home");
					}

					securityUserInfo.User.Email = model.Email;
					securityUserInfo.User.PhoneNumber = model.PhoneNumber;

					this.AmiClient.UpdateUser(userId, securityUserInfo);
					this.ImsiClient.Update<UserEntity>(model.ToUserEntity(userEntity));

					var user = this.UserManager.FindById(userId.ToString());

					user.Language = model.Language;

					this.UserManager.Update(user);

					TempData["success"] = Locale.User + " " + Locale.Updated + " " + Locale.Successfully;

					return RedirectToAction("Index", "Home");
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Profile;

			return View(model);
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