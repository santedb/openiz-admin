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
 * Date: 2016-7-30
 */

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Models.AccountModels;
using OpenIZAdmin.Models.UserModels;
using OpenIZAdmin.Models.UserModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		/// <param name="userId">The user id of the user to retrieve.</param>
		/// <returns>Returns a user entity or null if no user entity is found.</returns>
		public static UserEntity GetUserEntity(ImsiServiceClient client, Guid userId)
		{
			var bundle = client.Query<UserEntity>(u => u.SecurityUserKey == userId && u.ObsoletionTime == null);

			bundle.Reconstitute();

			return bundle.Item.OfType<UserEntity>().FirstOrDefault(u => u.SecurityUserKey == userId);
		}

        /// <summary>
        /// Converts a user entity to a edit user model.
        /// </summary>
        /// /// <param name="client">The Ami service client.</param>
        /// <param name="userEntity">The user entity to convert to a edit user model.</param>
        /// <returns>Returns a edit user model.</returns>
        public static EditUserModel ToEditUserModel(AmiServiceClient client, UserEntity userEntity)
        {
	        var securityUserInfo = client.GetUser(userEntity.SecurityUser.Key.Value.ToString());

	        var model = new EditUserModel
	        {
		        Email = userEntity.SecurityUser.Email,
		        Facilities =
		        {
					userEntity.Relationships.Where(r => r.RelationshipType.Key == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation).Select(r => r.Key).FirstOrDefault()?.ToString()
				},
		        FamilyNames = userEntity.Names.Where(n => n.NameUse.Key == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value).ToList(),
		        GivenNames = userEntity.Names.Where(n => n.NameUse.Key == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value).ToList(),
		        Roles = securityUserInfo.Roles.Select(r => r.Name).AsEnumerable(),
		        UserId = userEntity.SecurityUserKey.GetValueOrDefault(Guid.Empty)
	        };

	        model.FamilyNameList.AddRange(model.FamilyNames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));
			model.GivenNamesList.AddRange(model.GivenNames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));

			model.RolesList.Add(new SelectListItem { Text = "", Value = "" });
			model.RolesList.AddRange(RoleUtil.GetAllRoles(client).Select(r => new SelectListItem { Text = r.Name, Value = r.Id.ToString() }));

			return model;
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


            if(userEntity.SecurityUser.Roles != null && userEntity.SecurityUser.Roles.Any())
            {

            }

            //userInfo.Roles.AddRange(userEntity.SecurityUser.Roles.Select(r => new SecurityRoleInfo { Name = r }));

            return userInfo;
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

			if (!model.Roles.Any())
			{
				return userInfo;
			}

			foreach (var role in model.Roles.Select(roleId => RoleUtil.GetRole(client, roleId)).Where(role => role?.Role != null))
			{
				userInfo.Roles.Add(role);
			}

			return userInfo;
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
                IsLockedOut = userInfo.Lockout.GetValueOrDefault(false),
                IsObsolete = (userInfo.User.ObsoletionTime != null) ? true : false,
		        LastLoginTime = userInfo.User.LastLoginTime?.DateTime,
		        PhoneNumber = userInfo.User.PhoneNumber,
		        Roles = userInfo.Roles.Select(RoleUtil.ToRoleViewModel),
		        UserId = userInfo.UserId.Value,
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
    }
}