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
 * User: nitya
 * Date: 2017-8-8
 */

using System;
using OpenIZ.Core.Interop;
using System.Collections.Generic;
using System.Linq;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.DebugModels.ServerInformationViewModels
{
	/// <summary>
	/// Represents a server information view model.
	/// </summary>
	public class ServerInformationViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServerInformationViewModel"/> class.
		/// </summary>
		public ServerInformationViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServerInformationViewModel"/> class.
		/// </summary>
		/// <param name="serviceOptions">The service options.</param>
		public ServerInformationViewModel(ServiceOptions serviceOptions)
		{
			this.ServerInterfaceVersion = serviceOptions.InterfaceVersion;
			this.Endpoints = serviceOptions.Endpoints.Select(s => new ServerEndpointViewModel(Enum.GetName(typeof(ServiceEndpointType), s.ServiceType), new List<string> { Enum.GetName(typeof(ServiceEndpointCapabilities), s.Capabilities) })).ToList();
			this.Services = serviceOptions.Services.Select(s => new ServerServiceViewModel(s.ResourceName, s.Verbs)).ToList();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServerInformationViewModel"/> class.
		/// </summary>
		/// <param name="serviceOptions">The service options.</param>
		/// <param name="name">The name.</param>
		public ServerInformationViewModel(ServiceOptions serviceOptions, string name) : this(serviceOptions)
		{
			this.Name = name;
		}

		/// <summary>
		/// Gets or sets the endpoints.
		/// </summary>
		/// <value>The endpoints.</value>
		public List<ServerEndpointViewModel> Endpoints { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the server interface version.
		/// </summary>
		/// <value>The server interface version.</value>
		public string ServerInterfaceVersion { get; set; }

		/// <summary>
		/// Gets or sets the services.
		/// </summary>
		/// <value>The services.</value>
		public List<ServerServiceViewModel> Services { get; set; }

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString()
		{
			return $"{Locale.ServerInterfaceVersion}: {this.ServerInterfaceVersion}, {Locale.Endpoints}: {this.Endpoints}: {Locale.Services}: {this.Services}";
		}
	}
}