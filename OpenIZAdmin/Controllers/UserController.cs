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
 * Date: 2016-7-17
 */

using Elmah;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
		public ActionResult Activate(Guid id)
		{
			try
			{
				var user = this.AmiClient.GetUser(id.ToString());

				if (user == null)
				{
					TempData["error"] = Locale.UserNotFound;

					return RedirectToAction("Index");
				}

				user.UserId = id;
				user.User.ObsoletedBy = null;
				user.User.ObsoletedByKey = null;
				user.User.ObsoletionTime = null;
				user.User.ObsoletionTimeXml = null;

				this.AmiClient.UpdateUser(id, user);

				TempData.Clear();
				TempData["success"] = Locale.UserActivatedSuccessfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToActivateUser;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the create user view.
		/// </summary>
		/// <returns>Returns the create user view.</returns>
		[HttpGet]
		public ActionResult Create()
		{            
            var model = new CreateUserModel
			{
				RolesList = this.GetAllRoles().ToSelectList("Name", "Name", null, true),
                PhoneTypeList = GetPhoneTypeConceptSet().Concepts.ToSelectList().ToList(),
            PhoneType = TelecomAddressUseKeys.MobileContact.ToString()
            };                        

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
			try
			{
				//hack - check if empty string passed and remove - select2 issue                        
				model.CheckForEmptyRoleAssigned();

				if (!model.Roles.Any()) ModelState.AddModelError("Roles", Locale.RolesRequired);

				if (ModelState.IsValid)
				{
					if (this.UsernameExists(model.Username))
					{
						TempData["error"] = Locale.UserNameExists;
					}
					else
					{
						if (model.GivenNames.Any(n => !this.IsValidNameLength(n)))
						{
							this.ModelState.AddModelError(nameof(model.GivenNames), Locale.GivenNameLength100);
						}

						if (model.Surnames.Any(n => !this.IsValidNameLength(n)))
						{
							this.ModelState.AddModelError(nameof(model.Surnames), Locale.SurnameLength100);
						}

						var user = this.AmiClient.CreateUser(model.ToSecurityUserInfo());

						var userEntity = this.GetUserEntityBySecurityUserKey(user.UserId.Value);

						if (userEntity == null)
						{
							TempData["error"] = Locale.UnableToRetrieveNewUser;
							return RedirectToAction("Index");
						}

						if (model.Roles.Contains(Constants.ClinicalStaff))
						{
							var provider = this.ImsiClient.Create<Provider>(new Provider { Key = Guid.NewGuid() });

							userEntity.Relationships.Add(
								new EntityRelationship(EntityRelationshipTypeKeys.AssignedEntity, provider)
								{
									SourceEntityKey = userEntity.Key.Value
								});
						}

						this.ImsiClient.Update<UserEntity>(model.ToUserEntity(userEntity));

						TempData["success"] = Locale.UserCreatedSuccessfully;

						return RedirectToAction("Edit", new { id = user.UserId.ToString() });

					}
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				TempData["error"] = Locale.UnableToCreateUser;
			}

			model.RolesList = this.GetAllRoles().ToSelectList("Name", "Name", null, true);            
		    model.PhoneTypeList = GetPhoneTypeConceptSet().Concepts.ToSelectList().ToList();

			if (!TempData.ContainsKey("error") || TempData["error"] == null)
			{
				TempData["error"] = Locale.UnableToCreateUser;
			}

			return View(model);
		}

		/// <summary>
		/// Deletes a user.
		/// </summary>
		/// <param name="id">The id of the user to be deleted.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
				this.AmiClient.DeleteUser(id.ToString());

				TempData["success"] = Locale.UserDeactivatedSuccessfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToDeactivateUser;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Retrieves the user entity by id
		/// </summary>
		/// <param name="id">The user identifier.</param>
		/// <returns>Returns the user edit view.</returns>
		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			try
			{
				var userEntity = this.GetUserEntityBySecurityUserKey(id);

				// used as a check for users, incase an imported user doesn't have a user entity
				if (userEntity == null)
				{
					userEntity = this.ImsiClient.Create<UserEntity>(new UserEntity
					{
						Key = Guid.NewGuid(),
						SecurityUserKey = id
					});
				}

				var securityUserInfo = this.AmiClient.GetUser(userEntity.SecurityUserKey.ToString());

				var model = new EditUserModel(userEntity, securityUserInfo)
				{
					RolesList = this.GetAllRoles().ToSelectList("Name", "Id", null, true)
				};

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

					model.Facility = place.Key?.ToString();
					model.FacilityList.AddRange(facility.Select(f => new SelectListItem { Selected = f.Id == model.Facility, Text = f.Name, Value = f.Id }));
				}

				var phoneTypeList = this.GetPhoneTypeConceptSet().Concepts.ToList();

				var userPhoneType = model.ConvertPhoneTypeToGuid();

				model.PhoneTypeList = (userPhoneType != null) ? phoneTypeList.ToSelectList(p => p.Key == userPhoneType).ToList() : phoneTypeList.ToSelectList().ToList();                

                return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

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
			try
			{
				//hack - check if empty string passed and remove -select2 issue
				model.CheckForEmptyRoleAssigned();

				if (model.GivenNames.Any(n => !this.IsValidNameLength(n)))
				{
					this.ModelState.AddModelError(nameof(model.GivenNames), Locale.GivenNameLength100);
				}

				if (model.Surnames.Any(n => !this.IsValidNameLength(n)))
				{
					this.ModelState.AddModelError(nameof(model.Surnames), Locale.SurnameLength100);
				}

				if (!model.Roles.Any())
				{
					ModelState.AddModelError("Roles", Locale.RoleIsRequired);
					var userEntity = this.GetUserEntityBySecurityUserKey(model.Id);
					if (userEntity != null)
					{
						var securityUserInfo = this.AmiClient.GetUser(userEntity.SecurityUserKey.ToString());
						if (securityUserInfo != null) model.Roles = securityUserInfo.Roles.Select(r => r.Id.ToString()).ToList();
					}
				}

				if (ModelState.IsValid)
				{
					var userEntity = this.GetUserEntityBySecurityUserKey(model.Id);

					if (userEntity == null)
					{
						TempData["error"] = Locale.UserNotFound;
						return RedirectToAction("Index");
					}

					var updatedUserEntity = this.ImsiClient.Update<UserEntity>(model.ToUserEntity(userEntity));

					if (updatedUserEntity.SecurityUser == null)
					{
						updatedUserEntity.SecurityUser = this.AmiClient.GetUser(model.Id.ToString())?.User;
					}

					var securityInfo = model.ToSecurityUserInfo(updatedUserEntity);

					if (model.Roles.Any())
					{
						securityInfo.Roles.AddRange(model.Roles.Select(this.AmiClient.GetRole).Where(r => r.Role != null));
					}

					this.AmiClient.UpdateUser(userEntity.SecurityUserKey.Value, securityInfo);

					TempData["success"] = Locale.UserUpdatedSuccessfully;

					return RedirectToAction("ViewUser", new { id = userEntity.SecurityUserKey.ToString() });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

            var phoneTypeList = this.GetPhoneTypeConceptSet().Concepts.ToList();
            var userPhoneType = model.ConvertPhoneTypeToGuid();
            model.PhoneTypeList = (userPhoneType != null) ? phoneTypeList.ToSelectList(p => p.Key == userPhoneType).ToList() : phoneTypeList.ToSelectList().ToList();

            model.RolesList = this.GetAllRoles().ToSelectList("Name", "Id", null, true);

			TempData["error"] = Locale.UnableToUpdateUser;

			return View(model);
		}

		/// <summary>
		/// Displays the Index view
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "User";
			TempData["searchTerm"] = "*";
			return View();
		}

		/// <summary>
		/// Determines whether the name is between 1 and 100 characters.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns><c>true</c> if the name is between 1 and 100 characters; otherwise, <c>false</c>.</returns>
		/// <exception cref="System.ArgumentNullException">name</exception>
		private bool IsValidNameLength(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name), Locale.ValueCannotBeNull);
			}

			return name.Length > 0 && name.Length <= 100;
		}

		/// <summary>
		/// Displays the reset password view.
		/// </summary>
		/// <returns>Returns the reset password view.</returns>
		[HttpGet]
		public ActionResult ResetPassword(Guid id)
		{
			try
			{
				var user = this.AmiClient.GetUser(id.ToString());

				if (user == null)
				{
					TempData["error"] = Locale.UserNotFound;
					return Redirect(Request.UrlReferrer?.ToString());
				}

				var model = new ResetPasswordModel
				{
					Id = id
				};

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UserNotFound;

			return Redirect(Request.UrlReferrer?.ToString());
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
			try
			{
				if (ModelState.IsValid)
				{
					var user = this.AmiClient.GetUser(model.Id.ToString());

					if (user == null)
					{
						TempData["error"] = Locale.UserNotFound;
						return Redirect(Request.UrlReferrer?.ToString());
					}

					// null out the user, since we are only updating the password
					user.User = null;
					user.Password = model.Password;

					this.AmiClient.UpdateUser(model.Id, user);

					TempData["success"] = Locale.PasswordResetSuccessfully;

					return RedirectToAction("Index", "Home");
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToResetPassword;

			return View(model);
		}

		/// <summary>
		/// Searches for a user.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of users which match the search term.</returns>
		[HttpGet]
		[ValidateInput(false)]
		public ActionResult Search(string searchTerm)
		{
			var users = new List<UserViewModel>();

			try
			{
				if (this.IsValidId(searchTerm))
				{
					var results = new List<SecurityUserInfo>();

					results.AddRange(searchTerm == "*" ? this.AmiClient.GetUsers(a => a.UserClass == UserClassKeys.HumanUser).CollectionItem : this.AmiClient.GetUsers(u => u.UserName.Contains(searchTerm) && u.UserClass == UserClassKeys.HumanUser).CollectionItem);

					TempData["searchTerm"] = searchTerm;

					return PartialView("_UsersPartial", results.Select(u => new UserViewModel(u)).OrderBy(a => a.Username));
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UserNotFound;
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

			if (this.IsValidId(searchTerm))
			{
				var users = this.AmiClient.GetUsers(u => u.UserName.Contains(searchTerm) && u.UserClass == UserClassKeys.HumanUser);

				userList = users.CollectionItem.Select(u => new UserViewModel(u)).ToList();
			}

			return Json(userList, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Checks against the server to determine if a username exists.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <returns><c>true</c> If the username exists, <c>false</c> otherwise.</returns>
		private bool UsernameExists(string username)
		{
			return this.AmiClient.GetUsers(u => u.UserName == username).CollectionItem.Any();
		}

		/// <summary>
		/// Searches for a user to view details.
		/// </summary>
		/// <param name="id">The user identifier search string.</param>
		/// <returns>Returns a user view that matches the search term.</returns>
		[HttpGet]
		public ActionResult ViewUser(Guid id)
		{
			try
			{
				var userInfo = this.AmiClient.GetUser(id.ToString());

				if (userInfo == null)
				{
					TempData["error"] = Locale.UserNotFound;
					return RedirectToAction("Index");
				}

				var userEntity = this.GetUserEntityBySecurityUserKey(id);

				// used as a check for users, incase an imported user doesn't have a user entity
				if (userEntity == null)
				{
					userEntity = this.ImsiClient.Create<UserEntity>(new UserEntity
					{
						Key = Guid.NewGuid(),
						SecurityUserKey = id
					});
				}

				var given = userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value).ToList();
				var family = userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value).ToList();

				var viewModel = new UserViewModel(userInfo)
				{
					Name = string.Join(" ", given) + " " + string.Join(" ", family)
				};

				var facilityRelationship = userEntity.Relationships.FirstOrDefault(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);

				var place = facilityRelationship?.TargetEntity as Place;

				if (facilityRelationship?.TargetEntityKey.HasValue == true && place == null)
				{
					place = this.ImsiClient.Get<Place>(facilityRelationship.TargetEntityKey.Value, null) as Place;
				}

				if (place != null)
				{
					viewModel.HealthFacility = string.Join(" ", place.Names.SelectMany(n => n.Component).Select(c => c.Value));
				}

				return View(viewModel);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

			return RedirectToAction("Index");
		}

	}
}