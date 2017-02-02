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
 * Date: 2016-7-30
 */

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.AccountModels;
using OpenIZAdmin.Models.RoleModels.ViewModels;
using OpenIZAdmin.Models.UserModels;
using OpenIZAdmin.Models.UserModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing users.
	/// </summary>
	public static class UserUtil
	{
		/// <summary>
		/// Gets a security user info.
		/// </summary>
		/// <param name="client">The AMI service client.</param>
		/// <param name="userId">The id of the user to be retrieved.</param>
		/// <returns>Returns a security user.</returns>
		public static SecurityUserInfo GetSecurityUserInfo(AmiServiceClient client, Guid userId)
		{
			return client.GetUser(userId.ToString());
		}

		/// <summary>
		/// Gets a user entity.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
		/// <param name="userName">The username entered to verify doesn't exist in the database.</param>
		/// <returns>Returns a user entity or null if no user entity is found.</returns>
		public static bool CheckForUserName(ImsiServiceClient client, string userName)
		{
			var bundle = client.Query<UserEntity>(u => u.SecurityUser.UserName == userName);

			bundle.Reconstitute();

			var user = bundle.Item.OfType<UserEntity>().FirstOrDefault();

			return user != null;
		}

		/// <summary>
		/// Gets a user entity.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
		/// <param name="securityUserId">The user id of the user to retrieve.</param>
		/// <returns>Returns a user entity or null if no user entity is found.</returns>
		public static UserEntity GetUserEntityBySecurityUserKey(ImsiServiceClient client, Guid securityUserId)
		{
			var bundle = client.Query<UserEntity>(u => u.SecurityUserKey == securityUserId && u.ObsoletionTime == null);

			bundle.Reconstitute();

			return bundle.Item.OfType<UserEntity>().FirstOrDefault(u => u.SecurityUserKey == securityUserId);
		}

        /// <summary>
        /// Creates a new CreateUserModel for the Create User View
        /// </summary>
        /// <param name="imsiClient">The Imsi Service Client client.</param>
        /// <param name="model">The CreateUserModel containing the user information to be assigned.</param>
        /// <param name="userEntity">The user entity to convert to a edit user model.</param>
        /// <returns>Returns a CreateUserModel view.</returns>
        public static UserEntity ToCreateUserEntity(ImsiServiceClient imsiClient, CreateUserModel model, UserEntity userEntity)
		{            
			if (model.Roles.Contains(Constants.CLINICAL_STAFF)) //|| model.Roles.Contains(Constants.VACCINATOR) ||
                //model.Roles.Contains(Constants.MIDDLE_LEVEL_OFFICER) || model.Roles.Contains(Constants.NATIONAL_LEVEL_OFFICER) )
			{
				var provider = imsiClient.Create<Provider>(new Provider { Key = Guid.NewGuid() });

				userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.AssignedEntity, provider)
				{
					SourceEntityKey = userEntity.Key.Value
				});
			}

			if (model.Surnames.Any() || model.GivenNames.Any())
			{
				var name = new EntityName
				{
					NameUse = new Concept
					{
						Key = NameUseKeys.OfficialRecord
					},
					Component = new List<EntityNameComponent>()
				};

				name.Component.AddRange(model.Surnames.Select(n => new EntityNameComponent(NameComponentKeys.Family, n)));
				name.Component.AddRange(model.GivenNames.Select(n => new EntityNameComponent(NameComponentKeys.Given, n)));

				userEntity.Names = new List<EntityName> { name };
			}

			if (model.Facilities != null && model.Facilities.Any())
			{
				userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, Guid.Parse(model.Facilities.First())));
			}

			return userEntity;
		}

		/// <summary>
		/// Creates a new CreateUserModel for the Create User View
		/// </summary>
		/// <param name="imsiClient">The Imsi Service Client client.</param>
		/// <param name="amiClient">The Ami service client.</param>
		/// <param name="userEntity">The user entity to convert to a edit user model.</param>
		/// <returns>Returns a CreateUserModel view.</returns>
		public static CreateUserModel ToCreateUserModel(AmiServiceClient amiClient)//ImsiServiceClient imsiClient, 
		{
			var model = new CreateUserModel();

			model.RolesList.Add(new SelectListItem { Text = "", Value = "" });
			model.RolesList.AddRange(RoleUtil.GetAllRoles(amiClient).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }));

			return model;
		}

		/// <summary>
		/// Converts a user entity to a edit user model.
		/// </summary>
		/// <param name="imsiClient">The Imsi Service Client client.</param>
		/// <param name="amiClient">The Ami service client.</param>
		/// <param name="userEntity">The user entity to convert to a edit user model.</param>
		/// <returns>Returns a edit user model.</returns>
		public static EditUserModel ToEditUserModel(ImsiServiceClient imsiClient, AmiServiceClient amiClient, UserEntity userEntity)
		{
			var securityUserInfo = amiClient.GetUser(userEntity.SecurityUser.Key.Value.ToString());

			var model = new EditUserModel
			{
				Email = userEntity.SecurityUser.Email,
				Surnames = userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value).ToList(),
				GivenNames = userEntity.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value).ToList(),
				UserId = userEntity.SecurityUserKey.GetValueOrDefault(Guid.Empty),
				PhoneNumber = securityUserInfo.User.PhoneNumber
			};

			model.CreationTime = securityUserInfo.User.CreationTime.DateTime;
			model.UserRoles = (securityUserInfo.Roles != null && securityUserInfo.Roles.Any()) ? securityUserInfo.Roles.Select(p => RoleUtil.ToRoleViewModel(p)).OrderBy(q => q.Name).ToList() : new List<RoleViewModel>();

			model.SurnameList.AddRange(model.Surnames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));
			model.GivenNamesList.AddRange(model.GivenNames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));

			var healthFacilityRelationship = userEntity.Relationships.FirstOrDefault(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);

			if (healthFacilityRelationship != null && healthFacilityRelationship.TargetEntityKey != null)
			{
				var place = imsiClient.Get<Place>(healthFacilityRelationship.TargetEntityKey.Value, null) as Place;
				string facilityName = string.Join(" ", place.Names.SelectMany(n => n.Component)?.Select(c => c.Value));

				var facility = new List<FacilitiesModel>();
				facility.Add(new FacilitiesModel(facilityName, place.Key.Value.ToString()));

				model.FacilityList.AddRange(facility.Select(f => new SelectListItem { Text = f.Name, Value = f.Id }));
				model.Facilities.Add(place.Key.Value.ToString());
			}

			model.RolesList.Add(new SelectListItem { Text = "", Value = "" });
			model.RolesList.AddRange(RoleUtil.GetAllRoles(amiClient).Select(r => new SelectListItem { Text = r.Name, Value = r.Id.ToString() }));

			model.Roles = securityUserInfo.Roles.Select(r => r.Id.ToString()).ToList();

			model.PhoneTypeList = AccountUtil.GetPhoneTypeList(imsiClient);
            //default to mobile phone unless entry exists
            if (!string.IsNullOrWhiteSpace(userEntity.SecurityUser.PhoneNumber))
            {
                //need to assign phone type when property is accessible
                model.PhoneType = "";
            }
            else
            {
                //mobile phone - e161f90e-5939-430e-861a-f8e885cc353d	
                model.PhoneType = "e161f90e-5939-430e-861a-f8e885cc353d";
            }

            return model;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZAdmin.Models.UserModels.CreateUserModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityUserInfo"/>.
		/// </summary>
		/// <param name="model">The create user object to convert.</param>
		/// <returns>Returns a SecurityUserInfo model.</returns>
		public static SecurityUserInfo ToSecurityUserInfo(CreateUserModel model)
		{
			var userInfo = new SecurityUserInfo
			{
				Email = model.Email,
				Password = model.Password,
				UserName = model.Username,
				Roles = new List<SecurityRoleInfo>()
			};

			userInfo.Roles.AddRange(model.Roles.Select(r => new SecurityRoleInfo { Name = r }));

			return userInfo;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZAdmin.Models.UserModels.CreateUserModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityUserInfo"/>.
		/// </summary>
		/// <param name="model">The create user object to convert.</param>
		/// <returns>Returns a SecurityUserInfo model.</returns>
		public static SecurityUserInfo ToSecurityUserInfo(UserEntity userEntity)
		{
			var userInfo = new SecurityUserInfo
			{
				//Email = userEntity.SecurityUser.Email,
				//Password = userEntity.SecurityUser.p,
				//UserName = userEntity.SecurityUser.Username,
				Roles = new List<SecurityRoleInfo>()
			};

			if (userEntity.SecurityUser.Roles != null && userEntity.SecurityUser.Roles.Any())
			{
			}

			//userInfo.Roles.AddRange(userEntity.SecurityUser.Roles.Select(r => new SecurityRoleInfo { Name = r }));

			return userInfo;
		}

		/// <summary>
		/// Converts a <see cref="UpdateProfileModel"/> instance to a <see cref="UserEntity"/> instance.
		/// </summary>
		/// <param name="model">The edit user object to convert.</param>
		/// <param name="userEntity">The user entity instance.</param>                
		/// <returns>Returns a UserEntity object with the updated info.</returns>
		public static UserEntity ToUpdateUserEntity(EditUserModel model, UserEntity userEntity)
		{
			if (model.Surnames.Any() || model.GivenNames.Any())
			{
				var name = new EntityName
				{
					NameUse = new Concept
					{
						Key = NameUseKeys.OfficialRecord
					},
					Component = new List<EntityNameComponent>()
				};

				name.Component.AddRange(model.Surnames.Select(n => new EntityNameComponent(NameComponentKeys.Family, n)));
				name.Component.AddRange(model.GivenNames.Select(n => new EntityNameComponent(NameComponentKeys.Given, n)));

				userEntity.Names = new List<EntityName> { name };
			}

			var serviceLocation = userEntity.Relationships.FirstOrDefault(e => e.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);

			if (model.Facilities != null && model.Facilities.Any())
			{
				if (serviceLocation != null)
				{
					userEntity.Relationships.First(e => e.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation).TargetEntityKey = Guid.Parse(model.Facilities.First());
				}
				else
				{
					userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, Guid.Parse(model.Facilities.First())));
				}
			}
			else
			{
				if (serviceLocation != null)
				{
					userEntity.Relationships.RemoveAll(e => e.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);
				}
			}

			//need to strip versionkey so update will work
			userEntity.CreationTime = DateTimeOffset.Now;
			userEntity.VersionKey = null;

			return userEntity;
		}
		/// <summary>
		/// Converts a <see cref="EditUserModel"/> instance to a <see cref="SecurityUserInfo"/> instance.
		/// </summary>
		/// <param name="model">The create user object to convert.</param>
		/// <param name="user">The user entity instance.</param>
		/// <param name="client">The AMI service client instance. </param>
		/// <returns>Returns a security user info model.</returns>
		public static SecurityUserInfo ToSecurityUserInfo(EditUserModel model, UserEntity user, AmiServiceClient client)
		{
			var userInfo = new SecurityUserInfo
			{
				Email = model.Email,
				Roles = new List<SecurityRoleInfo>(),
				User = user.SecurityUser,
				UserId = model.UserId
			};

			userInfo.User.Email = model.Email;
			userInfo.User.PhoneNumber = model.PhoneNumber;

			if (model.Roles.Any())
			{
				foreach (var role in model.Roles.Select(roleId => RoleUtil.GetRole(client, Guid.Parse(roleId))).Where(role => role?.Role != null))
				{
					userInfo.Roles.Add(role);
				}
			}

			return userInfo;
		}

		/// <summary>
		/// Converts a update profile model to a user entity.
		/// </summary>
		/// <param name="model">The model to convert to a user entity.</param>
		/// <returns>Returns a user entity.</returns>
		public static UserEntity ToUserEntity(UpdateProfileModel model)
		{
			var userEntity = new UserEntity
			{
				SecurityUser =
				{
					PhoneNumber = model.PhoneNumber
				}
			};

			return userEntity;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityUserInfo"/> to a <see cref="OpenIZAdmin.Models.UserModels.ViewModels.UserViewModel"/>.
		/// </summary>
		/// <param name="userInfo">The security user info object to convert.</param>
		/// <returns>Returns a user entity model.</returns>
		public static UserViewModel ToUserViewModel(SecurityUserInfo userInfo)
		{
			var viewModel = new UserViewModel
			{
				Email = userInfo.Email,
				HasRoles = (userInfo.Roles != null && userInfo.Roles.Any()) ? true : false,
				IsLockedOut = userInfo.Lockout.GetValueOrDefault(false),
				IsObsolete = (userInfo.User.ObsoletionTime != null) ? true : false,
				LastLoginTime = userInfo.User.LastLoginTime?.DateTime,
				PhoneNumber = userInfo.User.PhoneNumber,
				Roles = userInfo.Roles.Select(RoleUtil.ToRoleViewModel),
				UserId = userInfo.UserId.Value,
				//UserId = userInfo,
				Username = userInfo.UserName
			};

			return viewModel;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityUserInfo"/> to a <see cref="OpenIZAdmin.Models.UserModels.ViewModels.UserViewModel"/>.
		/// </summary>
		/// <param name="client">The Imsi service client for queri.</param>
		/// <param name="userInfo">The security user info object to convert.</param>
		/// <returns>Returns a user entity model.</returns>
		public static UserViewModel ToUserViewModel(ImsiServiceClient client, SecurityUserInfo userInfo)
		{
			UserViewModel viewModel = new UserViewModel();

			viewModel.Email = userInfo.Email;
			viewModel.IsLockedOut = userInfo.Lockout.GetValueOrDefault(false);
			viewModel.IsObsolete = (userInfo.User.ObsoletionTime != null) ? true : false;
			viewModel.LastLoginTime = userInfo.User.LastLoginTime?.DateTime;
			viewModel.PhoneNumber = userInfo.User.PhoneNumber;
			viewModel.Roles = userInfo.Roles.Select(RoleUtil.ToRoleViewModel);
			viewModel.UserId = userInfo.UserId.Value;
			viewModel.Username = userInfo.UserName;

			return viewModel;
		}

		/// <summary>
		/// Gets a list of users.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
		/// <returns>Returns a list of users.</returns>
		internal static IEnumerable<UserViewModel> GetAllUsers(AmiServiceClient client)
		{
			// HACK
			var users = client.GetUsers(u => u.Email != null);

			var viewModels = users.CollectionItem.Select(UserUtil.ToUserViewModel);

			return viewModels;
		}

		/// <summary>
		/// Gets a list of users by assigned role.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
		/// <param name="roleId">The role identifier.</param>
		/// <returns>Returns a list of users.</returns>
		internal static IEnumerable<UserViewModel> GetAllUsersByRole(AmiServiceClient client, Guid roleId)
		{
			var users = client.GetUsers(u => u.Roles.Any(r => r.Key == roleId));

			var viewModels = users.CollectionItem.Select(UserUtil.ToUserViewModel);

			return viewModels;
		}

		/// <summary>
		/// Gets the SYSTEM user.
		/// </summary>
		/// <param name="client">The AMI service client.</param>
		/// <returns>Returns the system user.</returns>
		internal static SecurityUserInfo GetSystemUser(AmiServiceClient client)
		{
			return client.GetUsers(u => u.UserName == "SYSTEM").CollectionItem.FirstOrDefault();
		}
	}
}