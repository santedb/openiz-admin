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


using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.Domain;
using OpenIZAdmin.Models.RealmModels;
using OpenIZAdmin.Services.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing realms.
	/// </summary>
	[TokenAuthorize]
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
			var bundle = new Bundle();

			var person = new Person
			{
				Key = Guid.NewGuid(),
				Names = new List<EntityName>
				{
					new EntityName(NameUseKeys.OfficialRecord, "smith", "mary")
				}
			};

			var patient = new Patient
			{
				Relationships = new List<EntityRelationship>
				{
					new EntityRelationship(EntityRelationshipTypeKeys.Mother, Guid.NewGuid())
				}
			};

			var test = new ActParticipation(ActParticipationKey.RecordTarget, Guid.NewGuid());
			var test1 = new ActParticipation(ActParticipationKey.Location, Guid.Parse("880d2a08-8e94-402b-84b6-cb3bc0a576a9"));
			var test2 = new ActParticipation(ActParticipationKey.Performer, Guid.Parse("6ba47c4c-ec52-46a9-b2d9-ad3104ef238f"));
			var test3 = new ActParticipation(ActParticipationKey.Authororiginator, Guid.Parse("6ba47c4c-ec52-46a9-b2d9-ad3104ef238f"));
			var test4 = new ActParticipation(ActParticipationKey.Consumable, Guid.Parse("a14dd78e-1f68-40e0-a59f-cd1e64f720b8"));
			var test5 = new ActParticipation(ActParticipationKey.Consumable, Guid.Parse("ecda818f-e7b7-466a-9a71-a79eb2241ac9"));

			var act = new SubstanceAdministration
			{
				CreationTime = DateTimeOffset.Now,
				Key = Guid.NewGuid(),
				MoodConceptKey = ActMoodKeys.Eventoccurrence,
				Participations = new List<ActParticipation>
				{
					test,
					test1,
					test2,
					test3,
					test4,
					test5
				}
			};

			bundle.Item.Add(person);
			bundle.Item.Add(patient);
			bundle.Item.Add(act);
			bundle.Item.Add(test);
			bundle.Item.Add(test1);
			bundle.Item.Add(test2);
			bundle.Item.Add(test3);
			bundle.Item.Add(test4);
			bundle.Item.Add(test5);

			var content = JsonConvert.SerializeObject(bundle);

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
			HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			this.Response.Cookies.Remove("access_token");

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
					realm.DeviceId = Environment.MachineName + "-" + Guid.NewGuid().ToString().ToUpper();
					realm.DeviceSecret = Guid.NewGuid().ToString().ToUpper();

					var activeRealms = unitOfWork.RealmRepository.AsQueryable().Where(r => r.ObsoletionTime == null).AsEnumerable();

					foreach (var activeRealm in activeRealms)
					{
						activeRealm.ObsoletionTime = DateTime.UtcNow;
						unitOfWork.RealmRepository.Update(activeRealm);
					}

					unitOfWork.RealmRepository.Add(realm);
					unitOfWork.Save();
				}

				try
				{
					var result = await this.SignInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

					switch (result)
					{
						case SignInStatus.Success:
							using (var amiServiceClient = new AmiServiceClient(new RestClientService(Constants.Ami, this.HttpContext, this.SignInManager.AccessToken)))
							{
								var synchronizers = amiServiceClient.GetRoles(r => r.Name == "SYNCHRONIZERS").CollectionItem.FirstOrDefault();
								var device = amiServiceClient.GetRoles(r => r.Name == "DEVICE").CollectionItem.FirstOrDefault();

								var securityUserInfo = new SecurityUserInfo
								{
									Password = realm.DeviceSecret,
									Roles = new List<SecurityRoleInfo>
									{
										device,
										synchronizers
									},
									UserName = realm.DeviceId,
									User = new SecurityUser
									{
										Key = Guid.NewGuid(),
										UserClass = UserClassKeys.ApplicationUser,
										UserName = realm.DeviceId,
										SecurityHash = Guid.NewGuid().ToString()
									},
								};

								amiServiceClient.CreateUser(securityUserInfo);

								var securityDeviceInfo = new SecurityDeviceInfo
								{
									Device = new SecurityDevice
									{
										DeviceSecret = realm.DeviceSecret,
										Name = realm.DeviceId
									}
								};

								amiServiceClient.CreateDevice(securityDeviceInfo);
							}

							MvcApplication.MemoryCache.Set(RealmConfig.RealmCacheKey, true, ObjectCache.InfiniteAbsoluteExpiration);
							break;

						default:
							// always sign out the user when joining the realm
							SignInManager.AuthenticationManager.SignOut();

							var addedRealm = unitOfWork.RealmRepository.Get(r => r.Address == model.Address).FirstOrDefault();

							if (addedRealm != null)
							{
								unitOfWork.RealmRepository.Delete(addedRealm.Id);
								unitOfWork.Save();
							}

							ModelState.AddModelError("", Locale.IncorrectUsernameOrPassword);

							return View(model);
					}

					this.TempData["success"] = Locale.RealmJoinedSuccessfully;

					return RedirectToAction("Login", "Account");
				}
				catch (Exception e)
				{
					Trace.TraceError($"Unable to join realm: {e}");

					var addedRealm = unitOfWork.RealmRepository.Get(r => r.Address == model.Address).Single();
					unitOfWork.RealmRepository.Delete(addedRealm.Id);
					unitOfWork.Save();
				}
				finally
				{
					// always sign out the user when joining the realm
					HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
				}
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
			try
			{
				if (ModelState.IsValid)
				{
					var realm = unitOfWork.RealmRepository.FindById(model.CurrentRealm.Id);

					if (realm == null)
					{
						TempData["error"] = Locale.RealmNotFound;
						return RedirectToAction("Index");
					}

					MvcApplication.MemoryCache.Set(RealmConfig.RealmCacheKey, false, ObjectCache.InfiniteAbsoluteExpiration);

					using (var amiServiceClient = new AmiServiceClient(new RestClientService(Constants.Ami, this.HttpContext)))
					{
						var currentDevice = amiServiceClient.GetDevices(d => d.Name == realm.DeviceId).CollectionItem.FirstOrDefault(d => d.Name == realm.DeviceId);

						if (currentDevice != null)
						{
							currentDevice.Device.ObsoletedByKey = Guid.Parse(this.User.Identity.GetUserId());
							currentDevice.Device.ObsoletionTime = DateTimeOffset.Now;
							amiServiceClient.UpdateDevice(currentDevice.Id.ToString(), currentDevice);
						}
					}

					realm.ObsoletionTime = DateTime.UtcNow;

					// delete all the local references to the users when leaving a realm to avoid
					// conflicts such as multiple accounts named "administrator" etc.
					unitOfWork.RealmRepository.Delete(realm);
					unitOfWork.Save();

					this.Response.Cookies.Remove("access_token");
					HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

					TempData["success"] = Locale.RealmLeftSuccessfully;

					return RedirectToAction("JoinRealm", "Realm");
				}
			}
			catch (Exception e)
			{
				
				Trace.TraceError($"Unable to leave realm: { e }");
			}

			TempData["error"] = Locale.UnableToLeaveRealm;

			return View(model);
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