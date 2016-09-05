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
using OpenIZ.Core.Model.Constants;
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
	public static class UserUtil
	{
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
		/// Gets a user entity.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
		/// <param name="userId">The user id of the user to retrieve.</param>
		/// <param name="versionKey">The version key of the user.</param>
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

			return model;
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
			//List<EntityNameComponent> patientNames = new List<EntityNameComponent>();
			//if (this.GivenNames != null)
			//{
			//	patientNames.AddRange(this.GivenNames.Select(x => new OpenIZ.Core.Model.Entities.EntityNameComponent(NameComponentKeys.Given, x)).ToList());
			//}
			//if (this.FamilyNames != null)
			//{
			//	patientNames.AddRange(this.FamilyNames.Select(x => new OpenIZ.Core.Model.Entities.EntityNameComponent(NameComponentKeys.Family, x)).ToList());
			//}

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
			viewModel.InvalidLoginAttempts = userInfo.User.InvalidLoginAttempts;
			viewModel.IsLockedOut = userInfo.Lockout.GetValueOrDefault(false);
			viewModel.LastLoginTime = userInfo.User.LastLoginTime?.DateTime;
			viewModel.PhoneNumber = userInfo.User.PhoneNumber;
			viewModel.Roles = userInfo.Roles.Select(r => RoleUtil.ToRoleViewModel(r));
			viewModel.UserId = userInfo.UserId.Value;
			viewModel.Username = userInfo.UserName;

			return viewModel;
		}
	}
}