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
		/// /// <param name="key">The policy GUID identifier key </param>
		/// <returns>Returns SecurityPolicyInfo object, null if not found</returns>
		public static SecurityPolicyInfo GetPolicy(AmiServiceClient client, Guid key)
		{
			return client.GetPolicy(key.ToString());
		}

		/// <summary>
		/// Gets a list of all policies.
		/// </summary>
		/// <param name="client">The <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.</param>
		/// <returns>Returns a IEnumerable PolicyViewModel list.</returns>
		internal static IEnumerable<PolicyViewModel> GetAllPolicies(AmiServiceClient client)
		{
			var policies = client.GetPolicies(p => p.IsPublic == true);

			var viewModels = policies.CollectionItem.Select(p => new PolicyViewModel(p));

			return viewModels;
		}
	}
}