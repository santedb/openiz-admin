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
 * Date: 2017-8-8
 */
using OpenIZ.Core.Interop;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Services.Core;

namespace OpenIZAdmin.Services.Server
{
	/// <summary>
	/// Represents a server information service for the IMSI service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.ImsiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.Server.IServerInformationService" />
	public class ImsiServerInformationService : ImsiServiceBase, IImsiServerInformationService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImsiServerInformationService"/> class.
		/// </summary>
		/// <param name="client">The client.</param>
		public ImsiServerInformationService(ImsiServiceClient client) : base(client)
		{
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name => "Immunization Management Service Interface";

		/// <summary>
		/// Gets the server information.
		/// </summary>
		/// <returns>Returns the server information.</returns>
		public ServiceOptions GetServerInformation()
		{
			return this.Client.Options();
		}
	}
}
