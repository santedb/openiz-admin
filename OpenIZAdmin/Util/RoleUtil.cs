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
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Models.PolicyModels.ViewModels;
using OpenIZAdmin.Models.RoleModels;
using OpenIZAdmin.Models.RoleModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		/// Gets a list of all roles.
		/// </summary>
		/// <param name="client">The <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.</param>
		/// <returns>Returns a IEnumerable RoleViewModel list.</returns>
		internal static IEnumerable<RoleViewModel> GetAllRoles(AmiServiceClient client)
		{
			return client.GetRoles(r => r.ObsoletionTime == null).CollectionItem.Select(RoleUtil.ToRoleViewModel);
		}

        /// <summary>
        /// Queries for a role by key
        /// </summary>
        /// <param name="client">The AMI service client</param>        
        /// /// <param name="roleId">The role guid identifier</param>        
        /// <returns>Returns SecurityRoleInfo object, null if not found</returns>
        public static SecurityRoleInfo GetRole(AmiServiceClient client, Guid roleId)
        {
            try
            {                
                var roles = client.GetRoles(r => r.Key == roleId);
                if (roles.CollectionItem.Count != 0)
                {
                    return roles.CollectionItem.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.TraceError("Unable to retrieve role: {0}", e.StackTrace);
#endif
                Trace.TraceError("Unable to retrieve role: {0}", e.Message);
            }

            return null;
        }

        /// <summary>
        /// Queries for a specific role by role id
        /// </summary>
        /// <param name="client">The AMI service client</param>        
        /// /// <param name="id">The role identifier</param>        
        /// <returns>Returns SecurityRoleInfo object, null if not found</returns>
        public static SecurityRoleInfo GetRole(AmiServiceClient client, string id)
        {
            try
            {
                var role = client.GetRole(id);
                if (role != null)
                {
                    return role;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.TraceError("Unable to retrieve role: {0}", e.StackTrace);
#endif
                Trace.TraceError("Unable to retrieve role: {0}", e.Message);
            }

            return null;
        }

        /// <summary>
        /// Converts a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityRoleInfo"/> to a <see cref="OpenIZAdmin.Models.RoleModels.EditRoleModel"/>.
        /// </summary>        
        /// <param name="client">The Ami Service Client.</param>
        /// <param name="role">The SecurityRoleInfo object to convert.</param>
        /// <returns>Returns a EditRoleModel model.</returns>
        public static EditRoleModel ToEditPolicyModel(AmiServiceClient client, SecurityRoleInfo role)
        {
            EditRoleModel viewModel = new EditRoleModel();           

            viewModel.Description = role.Role.Description;
            viewModel.Id = role.Id.ToString();
            viewModel.Name = role.Role.Name;            
            //viewModel.Policies = role.Policies.Select(p => PolicyUtil.ToPolicyViewModel(p.Policy)).ToList();

            if (viewModel.Policies != null && viewModel.Policies.Count() > 0)
                viewModel.RolePolicies = viewModel.Policies.Select(p => PolicyUtil.ToPolicyViewModel(p)).OrderBy(q => q.Name).ToList();
            else
                viewModel.RolePolicies = new List<PolicyViewModel>();

            viewModel.PoliciesList.Add(new SelectListItem { Text = "", Value = "" });
            viewModel.PoliciesList.AddRange(CommonUtil.GetAllPolicies(client).Select(r => new SelectListItem { Text = r.Name, Value = r.Key.ToString() }));



            return viewModel;
        }

        /// <summary>
        /// Converts a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityRoleInfo"/> to a <see cref="OpenIZAdmin.Models.RoleModels.ViewModels.RoleViewModel"/>.
        /// </summary>
        /// <param name="roleInfo">The SecurityRoleInfo object to convert.</param>
        /// <returns>Returns a RoleViewModel model.</returns>
        public static RoleViewModel ToRoleViewModel(SecurityRoleInfo roleInfo)
		{
			RoleViewModel viewModel = new RoleViewModel();

			viewModel.Description = roleInfo.Role.Description;
			viewModel.Id = roleInfo.Id.Value;
			viewModel.Name = roleInfo.Name;
            viewModel.HasPolicies = (roleInfo.Role.Policies.Any()) ? true : false;
            viewModel.IsObsolete = (roleInfo.Role.ObsoletionTime != null) ? true : false; //CommonUtil.IsObsolete(roleInfo.Role.ObsoletionTime);

            if (roleInfo.Role.Policies != null)
                viewModel.Policies = roleInfo.Role.Policies.Select(p => PolicyUtil.ToPolicyViewModel(p)).OrderBy(q => q.Name).ToList();
            else
                viewModel.Policies = new List<PolicyViewModel>();

            return viewModel;
		}

        /// <summary>
        /// Converts a <see cref="OpenIZAdmin.Models.RoleModels.CreateRoleModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityRoleInfo"/>.
        /// </summary>
        /// <param name="model">The CreateRoleModel object to convert.</param>
        /// <returns>Returns a SecurityRoleInfo model.</returns>
		public static SecurityRoleInfo ToSecurityRoleInfo(CreateRoleModel model)
		{
			SecurityRoleInfo roleInfo = new SecurityRoleInfo();

			roleInfo.Role = new SecurityRole();

			roleInfo.Role.Description = model.Description;
			roleInfo.Name = model.Name;

			return roleInfo;
		}

        /// <summary>
        /// Converts a <see cref="OpenIZAdmin.Models.RoleModels.EditRoleModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityRoleInfo"/>.
        /// </summary>
        /// <param name="model">The EditRoleModel object to convert.</param>
        /// <returns>Returns a SecurityRoleInfo model.</returns>
		public static SecurityRoleInfo ToSecurityRoleInfo(EditRoleModel model)
		{
			SecurityRoleInfo roleInfo = new SecurityRoleInfo();

			roleInfo.Role = new SecurityRole();

			roleInfo.Role.Description = model.Description;
			roleInfo.Name = model.Name;

			return roleInfo;
		}

        ///// <summary>
        ///// Checks if a device has policies
        ///// </summary>
        ///// <param name="pList">A list with the policies applied to the role</param>        
        ///// <returns>Returns true if policies exist, false if no policies exist</returns>
        //private static bool HasPolicies(List<SecurityPolicyInstance> pList)
        //{
        //    if (pList != null && pList.Count() > 0)
        //        return true;
        //    else
        //        return false;
        //}

        ///// <summary>
        ///// Checks if a device is active or inactive
        ///// </summary>
        ///// <param name="date">A DateTimeOffset object</param>        
        ///// <returns>Returns true if active, false if inactive</returns>
        //private static bool IsActiveStatus(DateTimeOffset? date)
        //{
        //    if (date != null)
        //        return true;
        //    else
        //        return false;
        //}

        ///// <summary>
        ///// Checks if an application is active or inactive
        ///// </summary>
        ///// <param name="date">A DateTimeOffset object</param>        
        ///// <returns>Returns true if active, false if inactive</returns>
        //private static bool IsObsolete(DateTimeOffset? date)
        //{
        //    if (date == null)
        //        return false;
        //    else
        //        return true;
        //}

        ///// <summary>
        ///// Verifies a valid string parameter
        ///// </summary>
        ///// <param name="key">The string to validate</param>        
        ///// <returns>Returns true if valid, false if empty or whitespace</returns>
        //public static bool IsValidString(string key)
        //{
        //    if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
        //        return true;
        //    else
        //        return false;
        //}       
    }
}