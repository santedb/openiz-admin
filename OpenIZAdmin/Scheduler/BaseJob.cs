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
 * User: khannan
 * Date: 2017-3-13
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Principal;
using System.Web;
using OpenIZ.Core.Http;
using OpenIZ.Core.Interop.Clients;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;

namespace OpenIZAdmin.Scheduler
{
	/// <summary>
	/// Represents a base scheduler job.
	/// </summary>
	public abstract class BaseJob
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseJob"/> class.
		/// </summary>
		protected BaseJob()
		{
			
		}

		/// <summary>
		/// Gets the memory cache.
		/// </summary>
		/// <value>The memory cache.</value>
		protected MemoryCache MemoryCache => MvcApplication.MemoryCache;

		/// <summary>
		/// Gets an authenticated service client instance.
		/// </summary>
		/// <typeparam name="T">The type of service client to create.</typeparam>
		/// <param name="endpointName">Name of the endpoint.</param>
		/// <returns>Returns a new instance of the type, or null.</returns>
		/// <exception cref="System.ArgumentNullException">If the endpoint name is null.</exception>
		protected virtual T GetServiceClient<T>(string endpointName) where T : ServiceClientBase
		{
			if (string.IsNullOrEmpty(endpointName) || string.IsNullOrWhiteSpace(endpointName))
			{
				throw new ArgumentNullException($"The parameter {endpointName} cannot be null");
			}

			var deviceIdentity = ApplicationSignInManager.LoginAsDevice();

			if (deviceIdentity == null)
			{
				return null;
			}

			var restClientService = new RestClientService(endpointName)
			{
				Credentials = new AmiCredentials(new GenericPrincipal(deviceIdentity, null), deviceIdentity.AccessToken)
			};

			return Activator.CreateInstance(typeof(T), restClientService) as T;
		}
	}
}