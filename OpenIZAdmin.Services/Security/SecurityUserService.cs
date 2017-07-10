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
 * Date: 2017-7-10
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Services.Core;

namespace OpenIZAdmin.Services.Security
{
	/// <summary>
	/// Represents a security user service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.AmiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.Security.ISecurityUserService" />
	public class SecurityUserService : AmiServiceBase, ISecurityUserService

	{
		public SecurityUserService(AmiServiceClient client) : base(client)
		{
		}

		public SecurityUserInfo ActivateSecurityUser(Guid key)
		{
			throw new NotImplementedException();
		}

		public SecurityUserInfo CreateSecurityUser(SecurityUserInfo userInfo)
		{
			throw new NotImplementedException();
		}

		public SecurityUserInfo GetSecurityUser(Guid key)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<SecurityUserInfo> Query(Expression<Func<SecurityUser, bool>> expression)
		{
			throw new NotImplementedException();
		}
	}
}
