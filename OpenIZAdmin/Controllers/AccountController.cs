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

using MARC.HI.EHRS.SVC.Auditing.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.AccountModels;
using OpenIZAdmin.Models.Audit;
using OpenIZAdmin.Security;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OpenIZAdmin.Core.Auditing.Controllers;
using OpenIZAdmin.Models.AlertModels;
using OpenIZ.Core.Model.AMI.Alerting;
using OpenIZ.Core.Alert.Alerting;

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
		/// The audit service.
		/// </summary>
		private readonly IAuthenticationAuditService auditService;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountController"/> class.
		/// </summary>
		public AccountController()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountController" /> class.
		/// </summary>
		/// <param name="auditService">The audit service.</param>
		public AccountController(IAuthenticationAuditService auditService)
		{
			this.auditService = auditService;
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
					TempData["error"] = Locale.UserNotFound;

					HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
					Response.Cookies.Remove("access_token");

					return RedirectToAction("Login", "Account");
				}

				model.Username = securityUser.UserName;
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to display change password view: {e}");
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
						TempData["error"] = Locale.UserNotFound;

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

						TempData["success"] = Locale.PasswordUpdatedSuccessfully;

						HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
						Response.Cookies.Remove("access_token");

						return RedirectToAction("Login", "Account");
					}
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to change password: {e}");
			}

			TempData["error"] = Locale.UnableToUpdatePassword;

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
				this.TempData["error"] = Locale.UnableToRetrieveForgotPasswordMechanisms;

				Trace.TraceError($"Unable to display forgot password view: {e}");
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

			var userId = Guid.Empty;

			var resetPasswordModel = new ResetPasswordModel
			{
				UserId = userId
			};

			try
			{
				amiServiceClient.SendTfaSecret(new TfaRequestInfo
				{
					Purpose = "PasswordReset",
					ResetMechanism = model.TfaMechanism,
					UserName = model.Username,
					Verification = model.Verification
				});

				var user = this.AmiClient.GetUsers(u => u.UserName == model.Username && u.ObsoletionTime == null).CollectionItem.FirstOrDefault();

				// here, we don't care if the user is null, since throwing an error if the user in null
				// could indicate that the user doesn't exist to a potentially malicious user
				if (user == null)
				{
					user = new SecurityUserInfo(new SecurityUser
					{
						Key = Guid.NewGuid()
					});
				}

				resetPasswordModel.UserId = user.UserId.Value;

				this.TempData["success"] = Locale.ResetCodeSent;
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to send TFA mechanism: {e}");
			}

			return View("ResetPassword", resetPasswordModel);
		}

        /// <summary>
		/// Gets the alerts.
		/// </summary>
		/// <param name="all">if set to <c>true</c> [all].</param>
		/// <returns>Returns a list of alerts for the current user, including alerts marked for "everyone".</returns>
		private IEnumerable<AlertMessageInfo> GetAlerts()
        {


            var amiServiceClient = GetDeviceServiceClient();

            var alerts = amiServiceClient.GetAlerts(a => a.To.Contains("everyone") ).CollectionItem.ToList();
            alerts = alerts.Where(a => a.AlertMessage.ObsoletionTime == null && a.AlertMessage.Flags == AlertMessageFlags.System).ToList();

            return alerts.OrderByDescending(o => o.AlertMessage.CreationTime);
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

			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

            // Get alerts for login screen
            var models = new List<AlertViewModel>();

            try
            {
                ViewBag.ReturnUrl = returnUrl;
                models.AddRange(this.GetAlerts().Select(a => new AlertViewModel(a)));
                return View(new LoginModel(models));
            }
            catch (Exception e)
            {
                Trace.TraceError($"Unable to retrieve alerts: {e}");
            }
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
			var result = SignInStatus.Failure;

			try
			{
				if (!ModelState.IsValid)
				{
					return View(model);
				}

				result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, false, shouldLockout: false);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to login: {e}");
			}

			switch (result)
			{
				case SignInStatus.Success:
					try
					{
						// set the credentials
						this.ImsiClient.Client.Credentials = new AmiCredentials(this.User, this.SignInManager.AccessToken);

						var user = this.GetUserEntityBySecurityUserKey(Guid.Parse(SignInManager.AuthenticationManager.AuthenticationResponseGrant.Identity.GetUserId()));

						if (user != null)
						{
							// Default to english
							var languageCode = LocalizationConfig.LanguageCode.English;

							var language = user.LanguageCommunication.FirstOrDefault(u => u.IsPreferred);
							if (language != null)
							{
								languageCode = language.LanguageCode;
							}

							Response.Cookies.Add(new HttpCookie(LocalizationConfig.LanguageCookieName, languageCode));
						}

						this.auditService.AuditLogin(model.Username, RealmConfig.GetCurrentRealm().DeviceId);
					}
					catch (Exception e)
					{
						Trace.TraceError($"Unable to set the users default language, reverting to english: {e}");
					}

					Response.Cookies.Add(new HttpCookie("access_token", SignInManager.AccessToken));
					return RedirectToLocal(returnUrl);

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
			try
			{
				this.TempData.Clear();

				HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

				this.Response.Cookies.Remove("access_token");

				this.auditService.AuditLogOff(this.User, RealmConfig.GetCurrentRealm().DeviceId);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to logoff: {e}");
			}

			return RedirectToAction("Login", "Account");
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
						this.TempData["error"] = Locale.UnableToResetPassword;
						return RedirectToAction("ForgotPassword");
					}

					// perform a TFA sign to force the password update
					var result = await this.SignInManager.TfaSignInAsync(user.UserName, null, model.Code);

					if (result == SignInStatus.Success)
					{
						amiServiceClient = new AmiServiceClient(new RestClientService(Constants.Ami, this.HttpContext, this.SignInManager.AccessToken));

						// null out the other properties, since we are only updating the password
						user.User = null;
						user.Lockout = false;
						user.Roles.Clear();

						user.Password = model.Password;

						amiServiceClient.UpdateUser(user.UserId.Value, user);

						this.TempData["success"] = Locale.PasswordResetSuccessfully;
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
				Trace.TraceError($"Unable to reset password: {e}");
			}

			this.TempData["error"] = Locale.UnableToResetPassword;

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
				var userEntity = this.GetUserEntityBySecurityUserKey(Guid.Parse(this.User.Identity.GetUserId()));

				if (userEntity.SecurityUser == null)
				{
					userEntity.SecurityUser = this.AmiClient.GetUser(userEntity.SecurityUserKey.ToString())?.User;
				}

				var model = new UpdateProfileModel(userEntity);

				model = BuildUpdateModelMetaData(model, userEntity);

				return View(model);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to display update profile view: {e}");
			}

			TempData["error"] = Locale.UnableToRetrieveProfile;

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
			UserEntity userEntity = null;
			try
			{
				var userId = Guid.Parse(User.Identity.GetUserId());

				var securityUserInfo = this.AmiClient.GetUser(userId.ToString());
				userEntity = this.GetUserEntityBySecurityUserKey(userId);

				if (securityUserInfo == null || userEntity == null)
				{
					TempData["error"] = Locale.UserNotFound;

					return RedirectToAction("Index", "Home");
				}

				if (!model.IsValidNameLength(model.GivenName))
				{
					this.ModelState.AddModelError(nameof(model.GivenName), Locale.GivenNameLength100);
				}

				if (!model.IsValidNameLength(model.Surname))
				{
					this.ModelState.AddModelError(nameof(model.Surname), Locale.SurnameLength100);
				}

				if (ModelState.IsValid)
				{
					securityUserInfo.User.Email = model.Email;
					securityUserInfo.User.PhoneNumber = model.PhoneNumber;

					this.AmiClient.UpdateUser(userId, securityUserInfo);
					var updatedUser = this.ImsiClient.Update<UserEntity>(model.ToUserEntity(userEntity));

					var language = updatedUser.LanguageCommunication.FirstOrDefault(u => u.IsPreferred);

					var code = LocalizationConfig.LanguageCode.English;

					switch (language?.LanguageCode)
					{
						// only swahili is currently supported
						case LocalizationConfig.LanguageCode.Swahili:
							code = LocalizationConfig.LanguageCode.Swahili;
							break;

						default:
							break;
					}

					Response.Cookies.Add(new HttpCookie(LocalizationConfig.LanguageCookieName, code));

					TempData["success"] = Locale.ProfileUpdatedSuccessfully;

					return RedirectToAction("Index", "Home");
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update profile: {e}");
			}

			if (userEntity != null)
			{
				model = BuildUpdateModelMetaData(model, userEntity);
			}

			TempData["error"] = Locale.UnableToUpdateProfile;

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
		/// Populates the UpdateProfileModel.
		/// </summary>
		/// <param name="model">The UpdateProfileModel instance </param>
		/// <param name="userEntity">The UserEntity object</param>
		/// <returns>Returns an <see cref="UpdateProfileModel"/> model instance.</returns>
		private UpdateProfileModel BuildUpdateModelMetaData(UpdateProfileModel model, UserEntity userEntity)
		{
			model.CreateLanguageList();

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

			var phoneTypes = this.GetPhoneTypeConceptSet().Concepts.ToList();

			Guid phoneType;
			model.PhoneTypeList = this.IsValidId(model.PhoneType) && Guid.TryParse(model.PhoneType, out phoneType) ? phoneTypes.ToSelectList(this.HttpContext.GetCurrentLanguage(), p => p.Key == phoneType).ToList() : phoneTypes.ToSelectList(this.HttpContext.GetCurrentLanguage()).ToList();

			if (userEntity.Telecoms.Any())
			{
				//can have more than one contact - default to show mobile
				if (userEntity.Telecoms.Any(t => t.AddressUseKey == TelecomAddressUseKeys.MobileContact))
				{
					model.PhoneNumber = userEntity.Telecoms.First(t => t.AddressUseKey == TelecomAddressUseKeys.MobileContact).Value;
					model.PhoneType = TelecomAddressUseKeys.MobileContact.ToString();
				}
				else
				{
					model.PhoneNumber = userEntity.Telecoms.FirstOrDefault()?.Value;
					model.PhoneType = userEntity.Telecoms.FirstOrDefault()?.AddressUseKey?.ToString();
				}
			}
			else
			{
				//Default to Mobile - requirement
				model.PhoneType = TelecomAddressUseKeys.MobileContact.ToString();
			}

			return model;
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