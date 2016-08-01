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
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Models.UserModels;
using OpenIZAdmin.Models.UserModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenIZAdmin.Util
{
	internal static class UserUtil
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

		internal static SecurityUserInfo ToSecurityUserInfo(CreateUserModel model)
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

		internal static UserViewModel ToUserViewModel(SecurityUserInfo userInfo)
		{
			UserViewModel viewModel = new UserViewModel();

			viewModel.Email = userInfo.Email;
			viewModel.IsLockedOut = userInfo.Lockout;
			viewModel.Roles = userInfo.Roles.Select(r => RoleUtil.ToRoleViewModel(r));
			viewModel.UserId = userInfo.UserId.Value;
			viewModel.Username = userInfo.UserName;

			return viewModel;
		}
	}
}