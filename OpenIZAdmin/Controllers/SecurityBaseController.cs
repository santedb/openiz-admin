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
 * Date: 2017-4-14
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ApplicationModels;
using OpenIZAdmin.Models.DeviceModels;
using OpenIZAdmin.Models.PolicyModels;
using OpenIZAdmin.Models.RoleModels;
using OpenIZAdmin.Util;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Represents a base controller for security actions.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Controllers.BaseController" />
	[TokenAuthorize]
	public abstract class SecurityBaseController : BaseController
	{
		/// <summary>
		/// Gets all policies.
		/// </summary>
		/// <returns>Returns a list of policies for the application as policy view model instances.</returns>
		protected IEnumerable<PolicyViewModel> GetAllPolicies()
		{
			return this.AmiClient.GetPolicies(r => r.ObsoletionTime == null).CollectionItem.Select(p => new PolicyViewModel(p));
		}

		/// <summary>
		/// Gets the new policies.
		/// </summary>
		/// <param name="policyKeys">The policy keys.</param>
		/// <returns>Returns a list of security policies.</returns>
		/// <exception cref="System.ArgumentNullException">policyKeys</exception>
		private List<SecurityPolicy> GetNewPolicies(IEnumerable<Guid> policyKeys)
		{
			if (policyKeys == null)
			{
				throw new ArgumentNullException(nameof(policyKeys), Locale.ValueCannotBeNull);
			}

			var policies = new List<SecurityPolicy>();

			if (policyKeys.Any())
			{
				policies.AddRange(from key
								in policyKeys
								  where key != Guid.Empty
								  select this.AmiClient.GetPolicies(r => r.Key == key)
								into result
								  where result.CollectionItem.Count != 0
								  select result.CollectionItem.FirstOrDefault()
								into infoResult
								  where infoResult.Policy != null
								  select infoResult.Policy);
			}

			return policies;
		}

		/// <summary>
		/// Converts a <see cref="EditApplicationModel"/> to a <see cref="SecurityApplicationInfo"/>
		/// </summary>
		/// <param name="model">The edit device model to convert.</param>
		/// <param name="appInfo">The security application info for which to apply the changes against.</param>        
		/// <returns>Returns a security device info object.</returns>
		protected SecurityApplicationInfo ToSecurityApplicationInfo(EditApplicationModel model, SecurityApplicationInfo appInfo)
		{
			appInfo.Application.Key = model.Id;
			appInfo.Id = model.Id;
			appInfo.Application.Name = model.ApplicationName;
			appInfo.Name = model.ApplicationName;

			var policyList = this.GetNewPolicies(model.Policies.Select(Guid.Parse));

			if (policyList.Any())
			{
				appInfo.Policies.Clear();
				appInfo.Policies.AddRange(policyList.Select(p => new SecurityPolicyInfo(p)
				{
					Grant = PolicyGrantType.Grant
				}));
			}

			return appInfo;
		}

		/// <summary>
		/// Converts an <see cref="EditDeviceModel"/> instance to a <see cref="SecurityDeviceInfo"/> instance.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="deviceInfo">The device information.</param>
		/// <returns>Returns the converted security device information instance.</returns>
		protected SecurityDeviceInfo ToSecurityDeviceInfo(EditDeviceModel model, SecurityDeviceInfo deviceInfo)
		{
			deviceInfo.Device.Key = model.Id;
			deviceInfo.Device.Name = model.Name;
			deviceInfo.Device.DeviceSecret = model.DeviceSecret;

			var policyList = this.GetNewPolicies(model.Policies.Select(Guid.Parse));

			if (policyList.Any())
			{
				deviceInfo.Policies.Clear();
				deviceInfo.Policies.AddRange(policyList.Select(p => new SecurityPolicyInfo(p)
				{
					Grant = PolicyGrantType.Grant
				}));
			}

			return deviceInfo;
		}

		/// <summary>
		/// Converts an <see cref="EditRoleModel"/> instance to a <see cref="SecurityRoleInfo"/> instance.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="roleInfo">The role information.</param>
		/// <returns>Returns the converted security role info instance.</returns>
		protected SecurityRoleInfo ToSecurityRoleInfo(EditRoleModel model, SecurityRoleInfo roleInfo)
		{
			roleInfo.Role.Description = model.Description;
			roleInfo.Name = model.Name;

			var addPoliciesList = this.GetNewPolicies(model.Policies.Select(Guid.Parse));

			roleInfo.Policies = addPoliciesList.Select(p => new SecurityPolicyInfo(p)
			{
				Grant = PolicyGrantType.Grant
			}).ToList();

			return roleInfo;
		}
	}
}