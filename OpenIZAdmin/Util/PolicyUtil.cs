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
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.PolicyModels;
using OpenIZAdmin.Models.PolicyModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Util
{
    /// <summary>
	/// Provides a utility for managing policies.
	/// </summary>
	public static class PolicyUtil
	{
        /// <summary>
        /// Queries for a policy by key
        /// </summary>
        /// <param name="client">The AMI service client</param>        
        /// /// <param name="key">The policy guid identifier key </param>        
        /// <returns>Returns SecurityPolicyInfo object, null if not found</returns>
        public static SecurityPolicyInfo GetPolicy(AmiServiceClient client, Guid key)
        {            
            try
            {                
                var result = client.GetPolicies(r => r.Key == key);                
                if (result.CollectionItem.Count != 0)
                {
                    return result.CollectionItem.FirstOrDefault();
                }             
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.TraceError("Unable to retrieve policy: {0}", e.StackTrace);
#endif
                Trace.TraceError("Unable to retrieve policy: {0}", e.Message);
            }

            return null;
        }

        /// <summary>
		/// Gets a list of all policies.
		/// </summary>
		/// <param name="client">The <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.</param>
		/// <returns>Returns a IEnumerable PolicyViewModel list.</returns>
        internal static IEnumerable<PolicyViewModel> GetAllPolicies(AmiServiceClient client)
		{
			IEnumerable<PolicyViewModel> viewModels = new List<PolicyViewModel>();

			try
			{
				var policies = client.GetPolicies(p => p.IsPublic == true);

				viewModels = policies.CollectionItem.Select(p => PolicyUtil.ToPolicyViewModel(p));
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve policies: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve policies: {0}", e.Message);
			}

			return viewModels;
		}

        /// <summary>
        /// Converts a <see cref="OpenIZAdmin.Models.PolicyModels.CreatePolicyModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityPolicyInfo"/>.
        /// </summary>
        /// <param name="model">The CreatePolicyModel object to convert.</param>
        /// <returns>Returns a SecurityPolicyInfo model.</returns>
        public static SecurityPolicyInfo ToSecurityPolicy(CreatePolicyModel model)
        {
            SecurityPolicyInfo policy = new SecurityPolicyInfo();

            policy.CanOverride = model.CanOverride;
            policy.Name = model.Name;
            policy.Oid = model.Oid;

            return policy;
        }

        /// <summary>
        /// Converts a <see cref="OpenIZAdmin.Models.PolicyModels.EditPolicyModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityPolicyInfo"/>.
        /// </summary>
        /// <param name="model">The CreatePolicyModel object to convert.</param>
        /// <param name="policyInfo">The SecurityPolicyInfo object to convert.</param>
        /// <returns>Returns a SecurityPolicyInfo model.</returns>
        public static SecurityPolicyInfo ToSecurityPolicy(EditPolicyModel model, SecurityPolicyInfo policyInfo)
        {
            policyInfo.Policy.Name = model.Name;
            policyInfo.Policy.Oid = model.Oid;
            policyInfo.Policy.CanOverride = model.CanOverride;
            SecurityPolicyInfo policy = new SecurityPolicyInfo(new SecurityPolicyInstance(policyInfo.Policy, (PolicyGrantType)model.Grant));            
            
            return policy;
        }

        /// <summary>
        /// Converts a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityPolicyInfo"/> to a <see cref="OpenIZAdmin.Models.PolicyModels.EditPolicyModel"/>.
        /// </summary>        
        /// <param name="policy">The SecurityPolicyInfo object to convert.</param>
        /// <returns>Returns a EditPolicyModel model.</returns>
        public static EditPolicyModel ToEditPolicyModel(SecurityPolicyInfo policy)
        {
            EditPolicyModel viewModel = new EditPolicyModel();

            viewModel.GrantsList.Add(new SelectListItem { Text = Locale.Select, Value = "" });
            viewModel.GrantsList.Add(new SelectListItem { Text = Locale.Deny, Value = "0" });
            viewModel.GrantsList.Add(new SelectListItem { Text = Locale.Elevate, Value = "1" });
            viewModel.GrantsList.Add(new SelectListItem { Text = Locale.Grant, Value = "2" });

            viewModel.CanOverride = policy.CanOverride;            
            viewModel.Grant = (int)policy.Grant;
            viewModel.IsPublic = policy.Policy.IsPublic;
            viewModel.Key = policy.Policy.Key.Value;
            viewModel.Name = policy.Name;
            viewModel.Oid = policy.Oid;                              

            return viewModel;
        }

        /// <summary>
        /// Converts a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityPolicyInfo"/> to a <see cref="OpenIZAdmin.Models.PolicyModels.ViewModels.PolicyViewModel"/>.
        /// </summary>        
        /// <param name="policy">The SecurityPolicyInfo object to convert.</param>
        /// <returns>Returns a PolicyViewModel model.</returns>
        public static PolicyViewModel ToPolicyViewModel(SecurityPolicyInfo policy)
		{
			PolicyViewModel viewModel = new PolicyViewModel();

			viewModel.CanOverride = policy.CanOverride;
			viewModel.Grant = Enum.GetName(typeof(PolicyGrantType), policy.Grant);
			viewModel.IsPublic = policy.Policy.IsPublic;
			viewModel.Key = policy.Policy.Key.Value;
			viewModel.Name = policy.Name;
			viewModel.Oid = policy.Oid;

            if (policy.Policy.ObsoletionTime == null)
                viewModel.IsObsolete = false;
            else
                viewModel.IsObsolete = true;


            return viewModel;
		}

        /// <summary>
        /// Converts a <see cref="OpenIZ.Core.Model.Security.SecurityPolicy"/> to a <see cref="OpenIZAdmin.Models.PolicyModels.ViewModels.PolicyViewModel"/>.
        /// </summary>        
        /// <param name="policy">The SecurityPolicy object to convert.</param>
        /// <returns>Returns a PolicyViewModel model.</returns>
		public static PolicyViewModel ToPolicyViewModel(SecurityPolicy policy)
		{
			PolicyViewModel viewModel = new PolicyViewModel();

			viewModel.CanOverride = policy.CanOverride;			
			viewModel.IsPublic = policy.IsPublic;
			viewModel.Key = policy.Key.Value;
			viewModel.Name = policy.Name;
			viewModel.Oid = policy.Oid;

			return viewModel;
		}        

        /// <summary>
        /// Converts a <see cref="OpenIZ.Core.Model.Security.SecurityPolicyInstance"/> to a <see cref="OpenIZAdmin.Models.PolicyModels.ViewModels.PolicyViewModel"/>.
        /// </summary>        
        /// <param name="policy">The SecurityPolicyInstance object to convert.</param>
        /// <returns>Returns a PolicyViewModel model.</returns>
		public static PolicyViewModel ToPolicyViewModel(SecurityPolicyInstance policy)
		{
			PolicyViewModel viewModel = PolicyUtil.ToPolicyViewModel(policy.Policy);

            viewModel.CanOverride = policy.Policy.CanOverride;
            viewModel.IsPublic = policy.Policy.IsPublic;
            viewModel.Key = policy.Policy.Key.Value;
            viewModel.Name = policy.Policy.Name;
            viewModel.Oid = policy.Policy.Oid;
            viewModel.Grant = Enum.GetName(typeof(PolicyGrantType), policy.GrantType);
            viewModel.IsObsolete = (policy.Policy.ObsoletionTime != null) ? true : false;            

            return viewModel;
		}

        /// <summary>
        /// Verifies a valid string parameter
        /// </summary>
        /// <param name="key">The string to validate</param>        
        /// <returns>Returns true if valid, false if empty or whitespace</returns>
        public static bool IsValidString(string key)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
                return true;
            else
                return false;
        }
    }
}