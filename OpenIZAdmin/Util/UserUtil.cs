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
			var bundle = client.Query<UserEntity>(u => u.SecurityUserKey == userId);

			bundle.Reconstitute();

			var user = bundle.Item.OfType<UserEntity>().FirstOrDefault(u => u.SecurityUserKey == userId);

			return user;
		}

		/// <summary>
		/// Converts a user entity to a edit user model.
		/// </summary>
		/// <param name="userEntity">The user entity to convert to a edit user model.</param>
		/// <returns>Returns a edit user model.</returns>
		public static EditUserModel ToEditUserModel(UserEntity userEntity)
		{
			var model = new EditUserModel
			{
				Email = userEntity.SecurityUser.Email,
				FacilityId = userEntity.Relationships.Where(r => r.RelationshipType.Key == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation).Select(r => r.Key).FirstOrDefault()?.ToString(),
				FamilyNames = userEntity.Names.Where(n => n.NameUse.Key == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value).ToList(),
				GivenNames = userEntity.Names.Where(n => n.NameUse.Key == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value).ToList(),
				Roles = userEntity.SecurityUser.Roles.Select(r => r.Name),
				Username = userEntity.SecurityUser.UserName,
				UserId = userEntity.SecurityUserKey.GetValueOrDefault(Guid.Empty)
			};

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
				IsObsolete = userInfo.User.ObsoletedBy != null,
				LastLoginTime = userInfo.User.LastLoginTime?.DateTime,
				PhoneNumber = userInfo.User.PhoneNumber,
				Roles = userInfo.Roles.Select(RoleUtil.ToRoleViewModel),
				UserId = userInfo.UserId.Value,
				Username = userInfo.UserName
			};


			return viewModel;
		}
	}
}