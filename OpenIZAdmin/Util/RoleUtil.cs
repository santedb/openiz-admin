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
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Models.PolicyModels.ViewModels;
using OpenIZAdmin.Models.RoleModels;
using OpenIZAdmin.Models.RoleModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing roles.
	/// </summary>
	public static class RoleUtil
	{
		/// <summary>
		/// Converts a <see cref="SecurityRoleInfo"/> to a <see cref="EditRoleModel"/>.
		/// </summary>
		/// <param name="client">The Ami Service Client.</param>
		/// <param name="role">The SecurityRoleInfo object to convert.</param>
		/// <returns>Returns a EditRoleModel model.</returns>
		public static EditRoleModel ToEditRoleModel(AmiServiceClient client, SecurityRoleInfo role)
		{
			var viewModel = new EditRoleModel
			{
				Description = role.Role.Description,
				Id = role.Id.ToString(),
				Name = role.Role.Name,
				RolePolicies = role.Policies.Select(p => new PolicyViewModel(p)).OrderBy(q => q.Name).ToList()
			};

			if (viewModel.RolePolicies.Any())
			{
				viewModel.Policies = viewModel.RolePolicies.Select(p => p.Id.ToString()).ToList();
			}

			viewModel.PoliciesList.Add(new SelectListItem { Text = "", Value = "" });
			viewModel.PoliciesList.AddRange(CommonUtil.GetAllPolicies(client).Select(r => new SelectListItem { Text = r.Name, Value = r.Id.ToString() }).OrderBy(q => q.Text));

			return viewModel;
		}

		/// <summary>
		/// Converts a <see cref="EditRoleModel"/> to a <see cref="SecurityRoleInfo"/>.
		/// </summary>
		/// <param name="model">The EditRoleModel object to convert.</param>
		/// <returns>Returns a SecurityRoleInfo model.</returns>
		public static SecurityRoleInfo ToSecurityRoleInfo(AmiServiceClient amiClient, EditRoleModel model, SecurityRoleInfo roleInfo)
		{
			roleInfo.Role.Description = model.Description;
			roleInfo.Name = model.Name;

			var addPoliciesList = CommonUtil.GetNewPolicies(amiClient, model.Policies);

			roleInfo.Policies = addPoliciesList.Select(p => new SecurityPolicyInfo(p)
			{
				Grant = PolicyGrantType.Grant
			}).ToList();

			return roleInfo;
		}

		/// <summary>
		/// Gets a list of all roles.
		/// </summary>
		/// <param name="client">The <see cref="AmiServiceClient"/> instance.</param>
		/// <returns>Returns a IEnumerable RoleViewModel list.</returns>
		internal static IEnumerable<RoleViewModel> GetAllRoles(AmiServiceClient client)
		{
			return client.GetRoles(r => r.ObsoletionTime == null).CollectionItem.Select(r => new RoleViewModel(r));
		}
	}
}