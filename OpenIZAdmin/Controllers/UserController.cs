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

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.UserModels;
using OpenIZAdmin.Models.UserModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using OpenIZ.Core.Model.Roles;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for administering users.
	/// </summary>
	[TokenAuthorize]
	public class UserController : BaseController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UserController"/> class.
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
			var userKey = Guid.Empty;

			if (CommonUtil.IsValidString(id) && Guid.TryParse(id, out userKey))
			{
				var user = UserUtil.GetSecurityUserInfo(this.AmiClient, userKey);

				if (user == null)
				{
					TempData["error"] = Locale.User + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				user.UserId = userKey;
				user.User.ObsoletedBy = null;
				user.User.ObsoletedByKey = null;
				user.User.ObsoletionTime = null;
				user.User.ObsoletionTimeXml = null;

				this.AmiClient.UpdateUser(userKey, user);

                TempData.Clear();
				TempData["success"] = Locale.User + " " + Locale.Activated + " " + Locale.Successfully;

				return RedirectToAction("Index");
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
			var model = new CreateUserModel();

			model.RolesList.Add(new SelectListItem { Text = "", Value = "" });
			model.RolesList.AddRange(RoleUtil.GetAllRoles(this.AmiClient).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }));

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
				var user = UserUtil.ToSecurityUserInfo(model);

				user = this.AmiClient.CreateUser(user);

				var userEntity = UserUtil.GetUserEntity(this.ImsiClient, user.UserId.Value);

				if (model.Roles.Contains("CLINICAL_STAFF"))
				{
					var provider = this.ImsiClient.Create<Provider>(new Provider { Key = Guid.NewGuid() });

					userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.AssignedEntity, provider)
					{
						SourceEntityKey = userEntity.Key.Value
					});
				}

				this.ImsiClient.Update<UserEntity>(userEntity);

				TempData["success"] = Locale.User + " " + Locale.Created + " " + Locale.Successfully;

                return RedirectToAction("ViewUser", new { id = user.UserId.ToString() });
                
			}

			model.RolesList.Add(new SelectListItem { Text = "", Value = "" });
			model.RolesList.AddRange(RoleUtil.GetAllRoles(this.AmiClient).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }));

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
			if (CommonUtil.IsValidString(id))
			{
				this.AmiClient.DeleteUser(id);

				TempData["success"] = Locale.User + " " + Locale.Deactivated + " " + Locale.Successfully;

				return RedirectToAction("Index");
			}

			TempData["error"] = Locale.UnableToDeactivate + " " + Locale.User;

			return RedirectToAction("Index");
		}

        /// <summary>
        /// Retrieves the user entity by id
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>Returns the user edit view.</returns>
        [HttpGet]
		public ActionResult Edit(string id)
		{
			var userId = Guid.Empty;

			if (CommonUtil.IsValidString(id) && Guid.TryParse(id, out userId))
			{
				var userEntity = UserUtil.GetUserEntity(this.ImsiClient, userId);

				if (userEntity == null)
				{
					TempData["error"] = Locale.User + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var model = UserUtil.ToEditUserModel(this.ImsiClient, this.AmiClient, userEntity);				

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
				var userEntity = UserUtil.GetUserEntity(this.ImsiClient, model.UserId);

				if (userEntity == null)
				{
					TempData["error"] = Locale.User + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var name = new EntityName
				{
					NameUse = new Concept
					{
						Key = NameUseKeys.OfficialRecord
					},
					Component = new List<EntityNameComponent>()
				};

				//--specific to the UserEntity
				if (model.FamilyNames != null && model.FamilyNames.Count > 0)
				{
					name.Component.AddRange(model.FamilyNames.Select(n => new EntityNameComponent(NameComponentKeys.Family, n)));
				}

				if (model.GivenNames != null && model.GivenNames.Count > 0)
				{
					name.Component.AddRange(model.GivenNames.Select(n => new EntityNameComponent(NameComponentKeys.Given, n)));
				}

				userEntity.Names = new List<EntityName> { name };

				var serviceLocation = userEntity.Relationships.FirstOrDefault(e => e.RelationshipType.Key == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);

                if (model.Facilities != null && model.Facilities.Any())
                {
                    if (serviceLocation != null)
                    {
                        userEntity.Relationships.First(e => e.RelationshipType.Key == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation).TargetEntityKey = Guid.Parse(model.Facilities.First());
                    }
                    else
                    {
                        //if (model.Facilities.Any())
                        //{
                            userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, Guid.Parse(model.Facilities.First())));
                        //}
                    }
                }				

				var userInfo = UserUtil.ToSecurityUserInfo(model, userEntity, this.AmiClient);

				this.AmiClient.UpdateUser(userEntity.SecurityUserKey.Value, userInfo);
				this.ImsiClient.Update<UserEntity>(userEntity);

				TempData["success"] = Locale.User + " " + Locale.Updated + " " + Locale.Successfully;

                return RedirectToAction("Edit", new { id = userEntity.Key.ToString() });
            }

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.User;

			return View(model);
		}

		/// <summary>
		/// Displays the Index view
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = Locale.User;
			return View(UserUtil.GetAllUsers(this.AmiClient));
		}

		/// <summary>
		/// Displays the reset password view.
		/// </summary>
		/// <returns>Returns the reset password view.</returns>
		[HttpGet]
		public ActionResult ResetPassword(Guid id)
		{
			var user = this.AmiClient.GetUser(id.ToString());

			if (user == null)
			{
				TempData["error"] = Locale.User + " " + Locale.NotFound;
				return Redirect(Request.UrlReferrer?.ToString());
			}

			var model = new ResetPasswordModel
			{
				UserId = id
			};

			return View(model);
		}

		/// <summary>
		/// Resets the password of a user.
		/// </summary>
		/// <param name="model">The reset password model containing the updated information.</param>
		/// <returns>Returns the reset password view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ResetPassword(ResetPasswordModel model)
		{
			if (ModelState.IsValid)
			{
				var user = this.AmiClient.GetUser(model.UserId.ToString());

				if (user == null)
				{
					TempData["error"] = Locale.User + " " + Locale.NotFound;
					return Redirect(Request.UrlReferrer?.ToString());
				}

				user.Password = model.Password;

				this.AmiClient.UpdateUser(model.UserId, user);

				TempData["success"] = Locale.Password + " " + Locale.Reset + " " + Locale.Successfully;

				return RedirectToAction("Index", "Home");
			}

			return View(model);
		}

		/// <summary>
		/// Searches for a user.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of users which match the search term.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			var users = new List<UserViewModel>();

			if (CommonUtil.IsValidString(searchTerm))
			{
				var collection = this.AmiClient.GetUsers(u => u.UserName.Contains(searchTerm) && u.UserClass == UserClassKeys.HumanUser);

				TempData["searchTerm"] = searchTerm;

				users.AddRange(collection.CollectionItem.Select(UserUtil.ToUserViewModel));

				return PartialView("_UsersPartial", users);
			}

			TempData["error"] = Locale.User + " " + Locale.NotFound;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_UsersPartial", users);
		}

		/// <summary>
		/// Searches for a user.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of users which match the search term.</returns>
		[HttpGet]
		public ActionResult SearchAjax(string searchTerm)
		{
			var userList = new List<UserViewModel>();

			if (CommonUtil.IsValidString(searchTerm))
			{
				var users = this.AmiClient.GetUsers(u => u.UserName.Contains(searchTerm) && u.UserClass == UserClassKeys.HumanUser);

				userList = users.CollectionItem.Select(UserUtil.ToUserViewModel).ToList();
			}

			return Json(userList, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Searches for a user to view details.
		/// </summary>
		/// <param name="id">The user identifier search string.</param>
		/// <returns>Returns a user view that matches the search term.</returns>
		[HttpGet]
		public ActionResult ViewUser(string id)
		{
			var userId = Guid.Empty;

			if (!Guid.TryParse(id, out userId))
			{
				TempData["error"] = Locale.User + " " + Locale.NotFound;

				return RedirectToAction("Index");
			}

			var result = this.AmiClient.GetUser(id);

			if (result == null)
			{
				TempData["error"] = Locale.User + " " + Locale.NotFound;

				return RedirectToAction("Index");
			}

			var viewModel = UserUtil.ToUserViewModel(this.ImsiClient, result);

			var user = UserUtil.GetUserEntity(this.ImsiClient, userId);

			viewModel.Name = string.Join(" ", user.Names.SelectMany(n => n.Component).Select(c => c.Value));

			var healthFacility = user.Relationships.FirstOrDefault(r => r.RelationshipType.Key == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);

			if (healthFacility?.TargetEntityKey != null)
			{
				var place = this.ImsiClient.Get<Place>(healthFacility.TargetEntityKey.Value, null) as Place;

				viewModel.HealthFacility = string.Join(" ", place.Names.SelectMany(n => n.Component).Select(c => c.Value));
			}

			return View(viewModel);
		}
	}
}