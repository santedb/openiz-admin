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
 * Date: 2016-7-17
 */

using Microsoft.AspNet.Identity;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.UserModels;
using OpenIZAdmin.Models.UserModels.ViewModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for administering users.
	/// </summary>
	[TokenAuthorize]
	public class UserController : Controller
	{
		/// <summary>
		/// The internal reference to the <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient amiClient;

		/// <summary>
		/// The internal reference to the <see cref="ImsiServiceClient"/> instance.
		/// </summary>
		private ImsiServiceClient imsiClient;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Controllers.UserController"/> class.
		/// </summary>
		public UserController()
		{
		}

		/// <summary>
		/// Activates a user.
		/// </summary>
		/// <param name="id">The id of the user to be activated.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Activate(string id)
		{
			Guid userKey = Guid.Empty;

			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out userKey))
			{
				try
				{
					var user = UserUtil.GetSecurityUserInfo(this.amiClient, userKey);

					if (user == null)
					{
						TempData["error"] = Locale.User + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					user.User.ObsoletedBy = null;
					user.User.ObsoletionTime = null;

					this.amiClient.UpdateUser(userKey, user);

					TempData["success"] = Locale.User + " " + Locale.ActivatedSuccessfully;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to delete user: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to delete user: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.UnableToActivate + " " + Locale.User;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the create user view.
		/// </summary>
		/// <returns>Returns the create user view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			CreateUserModel model = new CreateUserModel();
			model.RolesList.Add(new SelectListItem { Text = "", Value = "" });

			model.RolesList.AddRange(RoleUtil.GetAllRoles(this.amiClient).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }));

			List<SelectListItem> facilityList = new List<SelectListItem>();

			facilityList.Add(new SelectListItem
			{
				Text = "",
				Value = ""
			});

			var places = PlaceUtil.GetPlaces(this.imsiClient, 0, 200);

			facilityList.AddRange(places.Select(p => new SelectListItem { Text = string.Join(" ", p.Names.SelectMany(n => n.Component).Select(c => c.Value)), Value = p.Key.Value.ToString() }));

			model.FacilityList = facilityList.OrderBy(c => c.Text).ToList();

			return View(model);
		}

		/// <summary>
		/// Creates a user.
		/// </summary>
		/// <param name="model">The model containing the information about the user.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateUserModel model)
		{
			if (ModelState.IsValid)
			{
				SecurityUserInfo user = UserUtil.ToSecurityUserInfo(model);

				try
				{
					var result = this.amiClient.CreateUser(user);

					UserEntity userEntity = UserUtil.ToUserEntity(model);

					userEntity.SecurityUser = result.User;

					this.imsiClient.Create<UserEntity>(userEntity);

                    TempData["success"] = Locale.User + " " + Locale.CreatedSuccessfully;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to create user: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to create user: {0}", e.Message);
				}
			}

			model.RolesList.Add(new SelectListItem { Text = "", Value = "" });

			model.RolesList.AddRange(RoleUtil.GetAllRoles(this.amiClient).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }));

            TempData["error"] = Locale.UnableToCreate + " " + Locale.User;

			return View(model);
		}

		/// <summary>
		/// Deletes a user.
		/// </summary>
		/// <param name="id">The id of the user to be deleted.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(string id)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
			{
				try
				{
					this.amiClient.DeleteUser(id);

					TempData["success"] = Locale.User + " " + Locale.DeactivatedSuccessfully;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to delete user: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to delete user: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.UnableToDeactivate + " " + Locale.User;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected override void Dispose(bool disposing)
		{
			Trace.TraceInformation("{0} disposing", nameof(UserController));

			this.amiClient?.Dispose();

			base.Dispose(disposing);
		}

		[HttpGet]
		public ActionResult Edit(string id)
		{
			Guid userId = Guid.Empty;

			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out userId))
			{
				var userEntity = UserUtil.GetUserEntity(this.imsiClient, userId);

				if (userEntity == null)
				{
					TempData["error"] = Locale.User + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				EditUserModel model = UserUtil.ToEditUserModel(userEntity);

				model.FacilityList.Add(new SelectListItem { Text = "", Value = "" });
				model.FacilityList.AddRange(PlaceUtil.GetPlaces(this.imsiClient).Select(p => new SelectListItem { Text = string.Join(" ", p.Names.SelectMany(n => n.Component).Select(c => c.Value)), Value = p.Key.ToString() }));

				model.FacilityList = model.FacilityList.OrderBy(p => p.Text).ToList();

				model.FamilyNameList.AddRange(model.FamilyNames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));
				model.GivenNamesList.AddRange(model.GivenNames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));

				model.RolesList.Add(new SelectListItem { Text = "", Value = "" });
				model.RolesList.AddRange(RoleUtil.GetAllRoles(this.amiClient).Select(r => new SelectListItem { Text = r.Name, Value = r.Id.ToString() }));

				return View(model);
			}

			TempData["error"] = Locale.User + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Updates a user.
		/// </summary>
		/// <param name="model">The model containing the updated user information.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditUserModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var userEntity = UserUtil.GetUserEntity(this.imsiClient, model.UserId);

					if (userEntity == null)
					{
						TempData["error"] = Locale.User + " " + Locale.NotFound;

                        return RedirectToAction("Index");
					}

					EntityName name = new EntityName();

					name.NameUse = new Concept
					{
						Key = NameUseKeys.OfficialRecord
					};

					name.Component = new List<EntityNameComponent>();

					if (model.FamilyNames != null && model.FamilyNames.Count > 0)
					{
						name.Component.AddRange(model.FamilyNames.Select(n => new EntityNameComponent(NameComponentKeys.Family, n)));
					}

					if (model.GivenNames != null && model.GivenNames.Count > 0)
					{
						name.Component.AddRange(model.GivenNames.Select(n => new EntityNameComponent(NameComponentKeys.Given, n)));
					}

					userEntity.Names = new List<EntityName>();

					userEntity.Names.Add(name);

					userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, Guid.Parse(model.FacilityId)));

					this.amiClient.UpdateUser(userEntity.SecurityUserKey.Value, new SecurityUserInfo(userEntity.SecurityUser));
					this.imsiClient.Update<UserEntity>(userEntity);

					TempData["success"] = Locale.User + " " + Locale.UpdatedSuccessfully;

					return Redirect("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to edit user: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to edit user: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.User;

			return View(model);
		}

		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "User";
			return View(UserUtil.GetAllUsers(this.amiClient));
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var amiRestClient = new RestClientService(Constants.AMI);

			amiRestClient.Accept = "application/xml";
			amiRestClient.Credentials = new AmiCredentials(this.User, HttpContext.Request);

			this.amiClient = new AmiServiceClient(amiRestClient);

			var imsiRestClient = new RestClientService(Constants.IMSI);

			imsiRestClient.Accept = "application/xml";
			imsiRestClient.Credentials = new ImsCredentials(this.User, HttpContext.Request);

			this.imsiClient = new ImsiServiceClient(imsiRestClient);

			base.OnActionExecuting(filterContext);
		}

		/// <summary>
		/// Searches for a user.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of users which match the search term.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<UserViewModel> users = new List<UserViewModel>();

			try
			{
				if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
				{
					var collection = this.amiClient.GetUsers(u => u.UserName.Contains(searchTerm) && u.UserClass == UserClassKeys.HumanUser);

					TempData["searchTerm"] = searchTerm;

					return PartialView("_UsersPartial", collection.CollectionItem.Select(u => UserUtil.ToUserViewModel(u)));
				}
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to search users: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to search users: {0}", e.Message);
			}

			TempData["error"] = Locale.User + " " + Locale.NotFound;
            TempData["searchTerm"] = searchTerm;

			return PartialView("_UsersPartial", users);
		}

		[HttpGet]
		public ActionResult ViewUser(string id)
		{
			Guid userId = Guid.Empty;

			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out userId))
			{
				var result = this.amiClient.GetUsers(u => u.Key == userId);

				if (result.CollectionItem.Count == 0)
				{
					TempData["error"] = Locale.User + " " + Locale.NotFound;

                    return RedirectToAction("Index");
				}

				var userViewModel = UserUtil.ToUserViewModel(result.CollectionItem.Single());

				try
				{
					var user = UserUtil.GetUserEntity(this.imsiClient, userViewModel.UserId);

					if (user == null)
					{
						TempData["error"] = Locale.User + " " + Locale.NotFound;

                        return RedirectToAction("Index");
					}

					userViewModel.Name = string.Join(" ", user.Names.SelectMany(n => n.Component).Select(c => c.Value));

					var healthFacility = user.Relationships.Where(r => r.RelationshipType.Key == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation).FirstOrDefault();

					userViewModel.HealthFacility = "N/A";

					//if (healthFacility == null)
					//{
					//	userViewModel.HealthFacility = "N/A";
					//}
					//else
					//{
					//	userViewModel.HealthFacility = string.Join(" ", healthFacility.TargetEntity.Names.SelectMany(n => n.Component).Select(c => c.Value));
					//}

					return View(userViewModel);
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to retrieve user {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to retrieve user: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.User + " " + Locale.NotFound;

            return RedirectToAction("Index");
		}
	}
}