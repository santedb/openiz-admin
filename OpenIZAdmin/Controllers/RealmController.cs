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
 * Date: 2016-7-13
 */

using OpenIZAdmin.DAL;
using OpenIZAdmin.Extensions;
using System.Linq;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.Domain;
using Microsoft.AspNet.Identity.Owin;
using OpenIZAdmin.Models.RealmModels;
using System.Web.Mvc;
using System.Collections.Generic;
using System;
using OpenIZAdmin.Models.RealmModels.ViewModels;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Microsoft.Owin.Security;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OpenIZAdmin.Controllers
{
	[Authorize]
	public class RealmController : Controller
	{
		private ApplicationSignInManager signInManager;
		private ApplicationUserManager userManager;
		private readonly IUnitOfWork unitOfWork;

		public RealmController() : this(new EntityUnitOfWork(new ApplicationDbContext()))
		{
		}

		public RealmController(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public RealmController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			this.UserManager = userManager;
			this.SignInManager = signInManager;
		}

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

		protected override void Dispose(bool disposing)
		{
			this.unitOfWork?.Dispose();

			this.userManager?.Dispose();
			this.userManager = null;

			this.signInManager?.Dispose();
			this.signInManager = null;

			base.Dispose(disposing);
		}

		private SwitchRealmViewModel GenerateSwitchRealmViewModel(Realm realm)
		{
			SwitchRealmViewModel viewModel = new SwitchRealmViewModel();

			viewModel.Map(realm);

			return viewModel;
		}

		private RealmViewModel GenerateRealmViewModel(Realm realm)
		{
			RealmViewModel viewModel = new RealmViewModel();

			viewModel.Map(realm);

			return viewModel;
		}

		[HttpGet]
		public ActionResult Index()
		{
			Realm realm = unitOfWork.RealmRepository.Get(r => r.ObsoletionTime == null).Single();

			return View(this.GenerateRealmViewModel(realm));
		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult JoinRealm()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ActionName("JoinRealm")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> JoinRealmAsync(JoinRealmModel model)
		{
			if (ModelState.IsValid)
			{

				Realm realm = unitOfWork.RealmRepository.Create();

				realm.AmiAuthEndpoint = model.Address + (model.Address.HasTrailingForwardSlash() ? "auth/oauth2_token" : "/auth/oauth2_token");
				realm.AmiEndpoint = model.Address + (model.Address.HasTrailingForwardSlash() ? "ami" : "/ami");
				realm.Map(model);

				IEnumerable<Realm> activeRealms = unitOfWork.RealmRepository.AsQueryable().Where(r => r.ObsoletionTime == null);

				foreach (var activeRealm in activeRealms)
				{
					activeRealm.ObsoletionTime = DateTime.UtcNow;
					unitOfWork.RealmRepository.Update(activeRealm);
					unitOfWork.Save();
				}

				unitOfWork.RealmRepository.Add(realm);
				unitOfWork.Save();

				var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, false, shouldLockout: false);

				switch (result)
				{
					case SignInStatus.Success:
						Response.Cookies.Add(new HttpCookie("access_token", SignInManager.AccessToken));
						break;
					default:
						var addedRealm = unitOfWork.RealmRepository.Get(r => r.Address == model.Address).Single();
						unitOfWork.RealmRepository.Delete(addedRealm.Id);
						unitOfWork.Save();

						ModelState.AddModelError("", "Incorrect Username or Password");
						return View(model);
				}

				TempData["success"] = "Realm saved successfully";

				return RedirectToAction("Index", "Home");
			}

			TempData["error"] = "Unable to save realm";

			return View(model);
		}

		[HttpGet]
		public ActionResult LeaveRealm()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LeaveRealm(LeaveRealmModel model)
		{
			if (ModelState.IsValid)
			{
				Realm realm = unitOfWork.RealmRepository.FindById(model.RealmId);

				if (realm == null)
				{
					TempData["error"] = "Realm not found";
					return RedirectToAction("Index");
				}

				realm.ObsoletionTime = DateTime.UtcNow;

				unitOfWork.RealmRepository.Update(realm);
				unitOfWork.Save();

				TempData["error"] = "Realm left successfully";

				return RedirectToAction("Index");
			}

			TempData["error"] = "Unable to leave realm";

			return View(model);
		}

		[HttpGet]
		public ActionResult SwitchRealm()
		{
			IEnumerable<Realm> realms = unitOfWork.RealmRepository.Get(r => r.ObsoletionTime != null);

			IEnumerable<SwitchRealmViewModel> realmViewModels = realms.Select(r => this.GenerateSwitchRealmViewModel(r));

			return View(realmViewModels);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SwitchRealm(SwitchRealmModel model)
		{
			if (ModelState.IsValid)
			{
				Realm realm = unitOfWork.RealmRepository.FindById(model.RealmId);

				if (realm == null)
				{
					TempData["error"] = "Realm not found";

					return RedirectToAction("Index");
				}

				realm.ObsoletionTime = null;

				Realm currentRealm = unitOfWork.RealmRepository.AsQueryable().Single(r => r.ObsoletionTime == null);

				currentRealm.ObsoletionTime = DateTime.UtcNow;

				unitOfWork.RealmRepository.Update(currentRealm);
				unitOfWork.RealmRepository.Update(realm);
				unitOfWork.Save();

				TempData["success"] = "Realm switched successfully";
				return RedirectToAction("Index");
			}

			TempData["error"] = "Unable to switch realm";

			return View(model);
		}

		[HttpGet]
		public ActionResult UpdateRealmSettings()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UpdateRealmSettings(UpdateRealmModel model)
		{
			return View();
		}
	}
}