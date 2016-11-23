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
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Models.AccountModels;
using OpenIZAdmin.Models.UserModels;
using OpenIZAdmin.Models.UserModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
			IEnumerable<UserViewModel> viewModels = new List<UserViewModel>();

			try
			{
				// HACK
				var users = client.GetUsers(u => u.Email != null);

				viewModels = users.CollectionItem.Select(u => UserUtil.ToUserViewModel(u));
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve users: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve users: {0}", e.Message);
			}

			return viewModels;
		}

        /// <summary>
		/// Gets a list of users by assigned role.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
        /// <param name="id">The role identifier.</param>
		/// <returns>Returns a list of users.</returns>
		internal static IEnumerable<UserViewModel> GetAllUsersByRole(AmiServiceClient client, string id)
        {
            IEnumerable<UserViewModel> viewModels = new List<UserViewModel>();

            try
            {
                // HACK
                var users = client.GetUsers(u => u.Email != null);

                viewModels = users.CollectionItem.Select(u => UserUtil.ToUserViewModel(u));
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.TraceError("Unable to retrieve users: {0}", e.StackTrace);
#endif
                Trace.TraceError("Unable to retrieve users: {0}", e.Message);
            }

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
			SecurityUserInfo user = null;

			try
			{
				user = client.GetUser(userId.ToString());
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve security user: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve security user: {0}", e.Message);
			}

			return user;
		}

		/// <summary>
		/// Gets a user entity.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
		/// <param name="userId">The user id of the user to retrieve.</param>
		/// <returns>Returns a user entity or null if no user entity is found.</returns>
		public static UserEntity GetUserEntity(ImsiServiceClient client, Guid userId)
		{
			UserEntity user = null;

			try
			{
				var bundle = client.Query<UserEntity>(u => u.SecurityUserKey == userId);

				bundle.Reconstitute();

				user = bundle.Item.OfType<UserEntity>().Cast<UserEntity>().FirstOrDefault();
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve user entity: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve user entity: {0}", e.Message);
			}

			return user;
		}

		/// <summary>
		/// Converts a user entity to a edit user model.
		/// </summary>
		/// <param name="userEntity">The user entity to convert to a edit user model.</param>
		/// <returns>Returns a edit user model.</returns>
		public static EditUserModel ToEditUserModel(UserEntity userEntity)
		{
			EditUserModel model = new EditUserModel();

			model.Email = userEntity.SecurityUser.Email;
			model.FacilityId = userEntity.Relationships.Where(r => r.RelationshipType.Key == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation).Select(r => r.Key).FirstOrDefault()?.ToString();
			model.FamilyNames = userEntity.Names.Where(n => n.NameUse.Key == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentType.Key == NameComponentKeys.Family).Select(c => c.Value).ToList();
			model.GivenNames = userEntity.Names.Where(n => n.NameUse.Key == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentType.Key == NameComponentKeys.Given).Select(c => c.Value).ToList();

			model.Roles = userEntity.SecurityUser.Roles.Select(r => r.Name);
			model.Username = userEntity.SecurityUser.UserName;
			model.UserId = userEntity.SecurityUserKey.GetValueOrDefault(Guid.Empty);

			return model;
		}

		public static UserEntity ToUserEntity(CreateUserModel model)
		{
			UserEntity userEntity = new UserEntity();

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

			return userEntity;
		}

		/// <summary>
		/// Converts a update profile model to a user entity.
		/// </summary>
		/// <param name="model">The model to convert to a user entity.</param>
		/// <returns>Returns a user entity.</returns>
		public static UserEntity ToUserEntity(UpdateProfileModel model)
		{
			UserEntity userEntity = new UserEntity();

			userEntity.SecurityUser.PhoneNumber = model.PhoneNumber;

			return userEntity;
		}

		public static SecurityUserInfo ToSecurityUserInfo(CreateUserModel model)
		{
			SecurityUserInfo userInfo = new SecurityUserInfo
			{
				Email = model.Email,
				Password = model.Password,
				UserName = model.Username
			};

			userInfo.Roles = new List<SecurityRoleInfo>();

			userInfo.Roles.AddRange(model.Roles.Select(r => new SecurityRoleInfo { Name = r }));

			return userInfo;
		}

		public static UserViewModel ToUserViewModel(SecurityUserInfo userInfo)
		{
			UserViewModel viewModel = new UserViewModel();

			viewModel.Email = userInfo.Email;
			viewModel.IsLockedOut = userInfo.Lockout.GetValueOrDefault(false);
			viewModel.IsObsolete = userInfo.User.ObsoletedBy != null;
			viewModel.LastLoginTime = userInfo.User.LastLoginTime?.DateTime;
			viewModel.PhoneNumber = userInfo.User.PhoneNumber;
			viewModel.Roles = userInfo.Roles.Select(r => RoleUtil.ToRoleViewModel(r));
			viewModel.UserId = userInfo.UserId.Value;
			viewModel.Username = userInfo.UserName;

			return viewModel;
		}
	}
}