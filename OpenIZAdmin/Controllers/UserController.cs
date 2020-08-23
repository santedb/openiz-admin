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

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for administering users.
	/// </summary>
	[TokenAuthorize(Constants.AlterIdentity)]
	public class UserController : BaseController
	{
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
				user.Lockout = false;

				var updated = this.AmiClient.UpdateUser(id, user);

				if (updated.User == null)
				{
					updated.User = this.AmiClient.GetUser(id.ToString())?.User;
				}

				TempData.Clear();
				TempData["success"] = Locale.UserActivatedSuccessfully;

				return RedirectToAction("ViewUser", new { id });
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to activate user: {e}");
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
				PhoneTypeList = this.GetPhoneTypeConceptSet().Concepts.ToSelectList(this.HttpContext.GetCurrentLanguage()).ToList(),
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
				// remove null or empty strings from the roles list
				model.Roles.RemoveAll(r => string.IsNullOrEmpty(r) || string.IsNullOrWhiteSpace(r));

				if (UsernameExists(model.Username)) ModelState.AddModelError(nameof(CreateUserModel.Username), Locale.UserNameExists);

				if (!model.Roles.Any()) ModelState.AddModelError(nameof(CreateUserModel.Roles), Locale.RolesRequired);

				if (!model.IsValidNameLength(model.GivenName)) this.ModelState.AddModelError(nameof(model.GivenName), Locale.GivenNameLength100);

				if (!model.IsValidNameLength(model.Surname)) this.ModelState.AddModelError(nameof(model.Surname), Locale.SurnameLength100);

				if (ModelState.IsValid)
				{
					var user = this.AmiClient.CreateUser(model.ToSecurityUserInfo());

					var userEntity = this.GetUserEntityBySecurityUserKey(user.UserId.Value);

					// create the provider association if the user has a facility
					if (model.Facility != null)
					{
						var provider = this.ImsiClient.Create<Provider>(new Provider { Key = Guid.NewGuid() });

						userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.AssignedEntity, provider)
						{
							SourceEntityKey = userEntity.Key.Value
						});
					}

					this.ImsiClient.Update<UserEntity>(model.ToUserEntity(userEntity));

					TempData["success"] = Locale.UserCreatedSuccessfully;

					return RedirectToAction("ViewUser", new { id = user.UserId.ToString() });
				}
			}
			catch (Exception e)
			{
				TempData["error"] = Locale.UnableToCreateUser;
				Trace.TraceError($"Unable to create user: {e}");
			}

			model.RolesList = this.GetAllRoles().ToSelectList("Name", "Name", null, true);
			model.PhoneTypeList = this.GetPhoneTypeConceptSet().Concepts.ToSelectList(this.HttpContext.GetCurrentLanguage()).ToList();

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
				Trace.TraceError($"Unable to delete user: {e}");
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

				if (securityUserInfo == null)
				{
					this.TempData["error"] = Locale.UserNotFound;

					return RedirectToAction("Index");
				}

				var model = new EditUserModel(userEntity, securityUserInfo);

				model = BuildEditModelMetaData(model, userEntity, model.IsObsolete);

				return View(model);
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
				Trace.TraceError($"Unable to retrieve user: {e}");
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
			UserEntity userEntity = null;
			try
			{
				userEntity = this.GetUserEntityBySecurityUserKey(model.Id);

				if (userEntity == null)
				{
					TempData["error"] = Locale.UserNotFound;
					return RedirectToAction("Index");
				}

				ValidateEditUserModel(model, userEntity);

				if (ModelState.IsValid)
				{
					UpdateUser(model, userEntity);
					TempData["success"] = Locale.UserUpdatedSuccessfully;
					return RedirectToAction("ViewUser", new { id = userEntity.SecurityUserKey.ToString() });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update user: {e}");
			}

			if (userEntity != null)
			{
				model = BuildEditModelMetaData(model, userEntity, model.IsObsolete);
			}

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
		/// Displays the reset password view.
		/// </summary>
		/// <param name="id">The identifier.</param>
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
				Trace.TraceError($"Unable to retrieve user: {e}");
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
					user.Lockout = false;
					user.Password = model.Password;

					var updated = this.AmiClient.UpdateUser(model.Id, user);

					if (updated.User == null)
					{
						updated.User = this.AmiClient.GetUser(model.Id.ToString())?.User;
					}

					TempData["success"] = Locale.PasswordResetSuccessfully;

					return RedirectToAction("Edit", new { id = user.UserId.Value });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to reset password: {e}");
			}

			TempData["error"] = Locale.UnableToResetPassword;

			return View(model);
		}

		/// <summary>
		/// Searches for a user.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <param name="role">The role Id to filter on.</param>
		/// <param name="facility">The facility Id to filter on.</param>
		/// <returns>Returns a list of users which match the search term.</returns>
		[HttpGet]
		[ValidateInput(false)]
		public ActionResult Search(string searchTerm, Guid? role = null, Guid? facility = null)
		{
			var users = new List<UserViewModel>();

			try
			{
				if (this.IsValidId(searchTerm))
				{
					var results = new List<SecurityUserInfo>();

                    OpenIZ.Core.Model.Query.NameValueCollection nvc = new OpenIZ.Core.Model.Query.NameValueCollection();
					nvc.Add("userClass", UserClassKeys.HumanUser.ToString());


					if (role != null) nvc.Add("role.id", role.ToString());
                    if (facility != null) nvc.Add("userEntity.relationship[DedicatedServiceDeliveryLocation].target", facility.ToString());
                    if (!string.IsNullOrEmpty(searchTerm) && searchTerm != "*") nvc.Add("userName", $"~*{searchTerm}*");

					results.AddRange(this.AmiClient.GetUsers(QueryExpressionParser.BuildLinqExpression<SecurityUser>(nvc, null, false)).CollectionItem);

					TempData["searchTerm"] = searchTerm;

					return PartialView("_UsersPartial", results.Select(u => new UserViewModel(u)).OrderBy(a => a.Username));
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve users: {e}");
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

			try
			{
				if (this.IsValidId(searchTerm))
				{
					var users = this.AmiClient.GetUsers(u => u.UserName.Contains(searchTerm) && u.UserClass == UserClassKeys.HumanUser);

					userList = users.CollectionItem.Select(u => new UserViewModel(u)).ToList();
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve users: {e}");
			}

			return Json(userList, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Unlocks a user.
		/// </summary>
		/// <param name="id">The id of the user profile to be unlocked.</param>
		/// <returns>Returns the view user view.</returns>
		[HttpGet]
		public ActionResult UnlockAccount(Guid id)
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
				user.User = null;
				user.Lockout = false;

				var result = this.AmiClient.UpdateUser(id, user);

				if (result.User == null)
				{
					result.User = this.AmiClient.GetUser(id.ToString())?.User;
				}

				TempData.Clear();
				TempData["success"] = Locale.UserUnlockedSuccessfully;

				return RedirectToAction("ViewUser", new { id = result.UserId.Value });
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update user: {e}");
			}

			TempData["error"] = Locale.UnableToUnlockUser;

			return RedirectToAction("Index");
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

				// used as a check for users, in-case an imported user doesn't have a user entity
				if (userEntity == null)
				{
					userEntity = this.ImsiClient.Create<UserEntity>(new UserEntity
					{
						Key = Guid.NewGuid(),
						SecurityUserKey = id
					});
				}

				var currentLanguageCode = this.HttpContext.GetCurrentLanguage() ?? "en";

				var viewModel = new UserViewModel(userEntity, userInfo);

				if (userEntity.Telecoms.Any())
				{
					var telecom = userEntity.Telecoms.First();

					var phoneType = Locale.NotApplicable;

					if (telecom.AddressUse == null && telecom.AddressUseKey != null)
					{
						var concept = this.GetConcept(telecom.AddressUseKey.Value);

						if (concept != null)
						{
							phoneType = concept.ConceptNames?.Any(cn => cn.Language == currentLanguageCode) == true ? string.Join(" ", concept.ConceptNames.Where(cn => cn.Language == currentLanguageCode).Select(n => n.Name)) : concept.Mnemonic;
						}
					}
					else
					{
						if (telecom.AddressUse == null)
						{
							telecom.AddressUse = this.GetConcept(telecom.AddressUseKey.Value);
						}

						phoneType = telecom.AddressUse.ConceptNames?.Any(cn => cn.Language == currentLanguageCode) == true ? string.Join(" ", telecom.AddressUse.ConceptNames.Where(cn => cn.Language == currentLanguageCode).Select(n => n.Name)) : telecom.AddressUse.Mnemonic;
					}

					viewModel.PhoneType = phoneType;
				}

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
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
				Trace.TraceError($"Unable to retrieve user: {e}");
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Populates the EditUserModel.
		/// </summary>
		/// <param name="model">The EditUserModel instance</param>
		/// <param name="userEntity">The UserEntity object</param>
		/// <param name="isObsolete">if set to <c>true</c> [is obsolete].</param>
		/// <returns>Returns an <see cref="EditUserModel" /> model instance.</returns>
		private EditUserModel BuildEditModelMetaData(EditUserModel model, UserEntity userEntity, bool isObsolete)
		{
			model.IsObsolete = isObsolete;
			model.CreateLanguageList();

			model.RolesList = this.GetAllRoles().ToSelectList("Name", "Id", null, true);

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
		/// Checks against the server to determine if a username exists.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <returns><c>true</c> If the username exists, <c>false</c> otherwise.</returns>
		private bool UsernameExists(string username)
		{
			return this.AmiClient.GetUsers(u => u.UserName == username).CollectionItem.Any();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult RemoveRole(Guid roleId, Guid userId)
		{
			var securityUserInfo = this.AmiClient.GetUser(userId.ToString());

			if (securityUserInfo == null)
			{
				TempData["error"] = Locale.UserNotFound;
				return RedirectToAction("Index");
			}

			securityUserInfo.Roles = securityUserInfo.Roles.Where(r => r.Id != roleId).ToList();

			var userEntity = this.GetUserEntityBySecurityUserKey(userId);
			var model = new EditUserModel(userEntity, securityUserInfo);

			model = BuildEditModelMetaData(model, userEntity, model.IsObsolete);

			try
			{
				ValidateEditUserModel(model, userEntity);

				if (ModelState.IsValid)
				{
					UpdateUser(model, userEntity);
					TempData["success"] = Locale.UserUpdatedSuccessfully;
					return RedirectToAction("ViewRole", "Role", new { id = roleId });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update user: {e}");
			}

			TempData["error"] = Locale.UnableToUpdateUser;
			return RedirectToAction("ViewRole", "Role", new { id = roleId });
		}

		[HttpPost]
		public ActionResult AddRoleForUsers(Guid roleId, List<Guid> userIds)
		{

			var securityRoleInfo = this.AmiClient.GetRole(roleId.ToString());
			if (securityRoleInfo == null)
			{
				TempData["error"] = Locale.RoleNotFound;
				return RedirectToAction("Index");
			}

			try
			{
				foreach (Guid id in userIds)
				{
					var securityUserInfo = this.AmiClient.GetUser(id.ToString());
					if (securityUserInfo == null)
					{
						TempData["error"] = Locale.UserNotFound;
						return RedirectToAction("Index");
					}

					if (!securityUserInfo.Roles.Contains(securityRoleInfo))
						securityUserInfo.Roles.Add(securityRoleInfo);

					var userEntity = this.GetUserEntityBySecurityUserKey(id);
					var model = new EditUserModel(userEntity, securityUserInfo);
				
					ValidateEditUserModel(model, userEntity);

					if (ModelState.IsValid)
						UpdateUser(model, userEntity);
				}

				TempData["success"] = Locale.RoleUpdatedSuccessfully;
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update users: {e}");
				TempData["error"] = Locale.UnableToUpdateUser;
			} 
				
			return RedirectToAction("ViewRole", "Role", new { id = roleId });
		}

		private void UpdateUser(EditUserModel model, UserEntity userEntity)
		{
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

			var updated = this.AmiClient.UpdateUser(userEntity.SecurityUserKey.Value, securityInfo);

			if (updated.User == null)
			{
				updated.User = this.AmiClient.GetUser(model.Id.ToString())?.User;
			}
		}

		private void ValidateEditUserModel(EditUserModel model, UserEntity userEntity)
        {
			// remove null or empty strings from the roles list
			model.Roles.RemoveAll(r => string.IsNullOrEmpty(r) || string.IsNullOrWhiteSpace(r));

			if (!model.IsValidNameLength(model.GivenName))
				this.ModelState.AddModelError(nameof(model.GivenName), Locale.GivenNameLength100);

			if (!model.IsValidNameLength(model.Surname))
				this.ModelState.AddModelError(nameof(model.Surname), Locale.SurnameLength100);
			

			if (!model.Roles.Any())
			{
				ModelState.AddModelError("Roles", Locale.RoleIsRequired);

				//restore roles on error
				var securityUserInfo = this.AmiClient.GetUser(userEntity.SecurityUserKey.ToString());
				if (securityUserInfo != null) model.Roles = securityUserInfo.Roles.Select(r => r.Id.ToString()).ToList();
			}
		}
	}
}