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
 * Date: 2016-7-13
 */

using Elmah;
using Microsoft.AspNet.Identity.Owin;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.Domain;
using OpenIZAdmin.Models.RealmModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OpenIZAdmin.Models.RealmModels.ViewModels;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing realms.
	/// </summary>
	[Authorize]
	public class RealmController : Controller
	{
		/// <summary>
		/// The internal reference to the unit of work.
		/// </summary>
		private readonly IUnitOfWork unitOfWork;

		/// <summary>
		/// The internal reference to the sign in manager.
		/// </summary>
		private ApplicationSignInManager signInManager;

		/// <summary>
		/// The internal reference to the user manager.
		/// </summary>
		private ApplicationUserManager userManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Controllers.RealmController"/> class.
		/// </summary>
		public RealmController() : this(new EntityUnitOfWork(new ApplicationDbContext()))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Controllers.RealmController"/> class
		/// with a specified <see cref="OpenIZAdmin.DAL.IUnitOfWork"/> instance.
		/// </summary>
		/// <param name="unitOfWork">The unit of work instance.</param>
		public RealmController(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Controllers.RealmController"/> class
		/// with a specified <see cref="OpenIZAdmin.DAL.ApplicationUserManager"/> instance and a
		/// specified <see cref="OpenIZAdmin.DAL.ApplicationSignInManager"/> instance.
		/// </summary>
		/// <param name="userManager">The user manager instance.</param>
		/// <param name="signInManager">The sign in manager instance.</param>
		public RealmController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			this.UserManager = userManager;
			this.SignInManager = signInManager;
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
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			if (!RealmConfig.IsJoinedToRealm())
			{
				return RedirectToAction("Index", "Home");
			}

			var realm = unitOfWork.RealmRepository.Get(r => r.ObsoletionTime == null).Single();

			return View(new RealmViewModel(realm));
		}

		/// <summary>
		/// Displays the join realm view.
		/// </summary>
		/// <returns>Returns the join realm view.</returns>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult JoinRealm()
		{
			return View();
		}

		/// <summary>
		/// Joins a realm.
		/// </summary>
		/// <param name="model">The model containing the properties to use to join the realm.</param>
		/// <returns>Returns the index view if the realm is joined successfully.</returns>
		[HttpPost]
		[AllowAnonymous]
		[ActionName("JoinRealm")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> JoinRealmAsync(JoinRealmModel model)
		{
			if (ModelState.IsValid)
			{
				model.Address = model.Address.HasTrailingBackSlash() ? model.Address.RemoveTrailingBackSlash() : model.Address;
				model.Address = model.Address.HasTrailingForwardSlash() ? model.Address.RemoveTrailingForwardSlash() : model.Address;

				Realm realm = unitOfWork.RealmRepository.Get(r => r.Address == model.Address && r.ObsoletionTime != null).AsEnumerable().SingleOrDefault();

				// remove any leading or trailing spaces
				model.Address = model.Address.Trim();

				// HACK: the UrlAttribute class thinks that http://localhost is not a valid url...
				if (model.Address.StartsWith("http://localhost"))
				{
					model.Address = model.Address.Replace("http://localhost", "http://127.0.0.1");
				}
				else if (model.Address.StartsWith("https://localhost"))
				{
					model.Address = model.Address.Replace("https://localhost", "https://127.0.0.1");
				}

				// is the user attempting to join a realm which they have already left?
				if (realm != null)
				{
					realm.Map(model);
					realm.ObsoletionTime = null;

					unitOfWork.RealmRepository.Update(realm);
					unitOfWork.Save();
				}
				else
				{
					realm = unitOfWork.RealmRepository.Create();

					realm.Map(model);

					IEnumerable<Realm> activeRealms = unitOfWork.RealmRepository.AsQueryable().Where(r => r.ObsoletionTime == null).AsEnumerable();

					foreach (var activeRealm in activeRealms)
					{
						activeRealm.ObsoletionTime = DateTime.UtcNow;
						unitOfWork.RealmRepository.Update(activeRealm);
					}

					unitOfWork.RealmRepository.Add(realm);
					unitOfWork.Save();
				}

				SignInStatus result = SignInStatus.Failure;

				try
				{
					result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, false, shouldLockout: false);
				}
				catch (Exception e)
				{
					ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));

					var addedRealm = unitOfWork.RealmRepository.Get(r => r.Address == model.Address).Single();
					unitOfWork.RealmRepository.Delete(addedRealm.Id);
					unitOfWork.Save();
				}

				switch (result)
				{
					case SignInStatus.Success:
						Response.Cookies.Add(new HttpCookie("access_token", SignInManager.AccessToken));
						break;

					default:
						var addedRealm = unitOfWork.RealmRepository.Get(r => r.Address == model.Address).FirstOrDefault();

						if (addedRealm != null)
						{
							unitOfWork.RealmRepository.Delete(addedRealm.Id);
							unitOfWork.Save();
						}

						ModelState.AddModelError("", Locale.IncorrectUsernameOrPassword);
						return View(model);
				}

				TempData["success"] = Locale.RealmJoined + " " + Locale.Successfully;

				return RedirectToAction("Index", "Home");
			}

			TempData["error"] = Locale.UnableToJoinRealm;

			return View(model);
		}

		/// <summary>
		/// Displays the leave realm view.
		/// </summary>
		/// <returns>Returns the leave realm view.</returns>
		[HttpGet]
		public ActionResult LeaveRealm()
		{
			var realm = unitOfWork.RealmRepository.Get(r => r.ObsoletionTime == null).FirstOrDefault();

			return View(new LeaveRealmModel(realm));
		}

		/// <summary>
		/// Leaves a realm.
		/// </summary>
		/// <param name="model">The model containing the properties to use to leave the realm</param>
		/// <returns>Returns the index view if the realm is left successfully.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LeaveRealm(LeaveRealmModel model)
		{
			if (ModelState.IsValid)
			{
				Realm realm = unitOfWork.RealmRepository.FindById(model.CurrentRealm.Id);

				if (realm == null)
				{
					TempData["error"] = Locale.Realm + " " + Locale.NotFound;
					return RedirectToAction("Index");
				}

				realm.ObsoletionTime = DateTime.UtcNow;

				unitOfWork.RealmRepository.Delete(realm);
				unitOfWork.Save();

				SignInManager.AuthenticationManager.SignOut();

				TempData["success"] = Locale.RealmLeft + " " + Locale.Successfully;

				return RedirectToAction("Index", "Home");
			}

			TempData["error"] = Locale.UnableToLeaveRealm;

			return View(model);
		}

		/// <summary>
		/// Switches a realm.
		/// </summary>
		/// <param name="realmId">The id of the realm to switch to.</param>
		/// <returns>Returns the login view if the realm is switched successfully.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SwitchRealm(Guid realmId)
		{
			if (realmId != Guid.Empty)
			{
				Realm realm = unitOfWork.RealmRepository.FindById(realmId);

				if (realm == null)
				{
					TempData["error"] = Locale.Realm + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				realm.ObsoletionTime = null;

				Realm currentRealm = unitOfWork.RealmRepository.AsQueryable().Single(r => r.ObsoletionTime == null);

				currentRealm.ObsoletionTime = DateTime.UtcNow;

				unitOfWork.RealmRepository.Update(currentRealm);
				unitOfWork.RealmRepository.Update(realm);
				unitOfWork.Save();

				TempData["success"] = Locale.RealmSwitched + " " + Locale.Successfully;
				HttpContext.GetOwinContext().Authentication.SignOut();

				return RedirectToAction("Login", "Account");
			}

			TempData["error"] = Locale.UnableToSwitchRealm;

			return View();
		}

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected override void Dispose(bool disposing)
		{
			this.unitOfWork?.Dispose();

			this.userManager?.Dispose();
			this.userManager = null;

			this.signInManager?.Dispose();
			this.signInManager = null;

			base.Dispose(disposing);
		}
	}
}