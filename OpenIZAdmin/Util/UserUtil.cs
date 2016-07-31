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
using System;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Models.RoleModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using OpenIZAdmin.Models.UserModels.ViewModels;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZAdmin.Models.UserModels;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Extensions;
using OpenIZ.Core.Model.Constants;

namespace OpenIZAdmin.Util
{
	public static class UserUtil
	{
		public static IEnumerable<UserViewModel> GetAllUsers(AmiServiceClient client)
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
				User = new SecurityUser
				{
					Entities = new List<Person>()
				},
				UserName = model.Username
			};

			userInfo.User.Entities.Add(new Person
			{
				Names = new List<EntityName>
				{
					new EntityName(NameUseKeys.Legal, model.LastName, new string[] { model.FirstName })
				}
			});

			userInfo.Roles = new List<SecurityRoleInfo>();

			userInfo.Roles.AddRange(model.Roles.Select(r => new SecurityRoleInfo { Name = r }));

			return userInfo;
		}

		public static UserViewModel ToUserViewModel(SecurityUserInfo userInfo)
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