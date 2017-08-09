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

using OpenIZAdmin.Localization;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Models.DebugModels.ServerInformationViewModels
{
	/// <summary>
	/// Represents a server endpoint view model.
	/// </summary>
	public class ServerEndpointViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServerEndpointViewModel"/> class.
		/// </summary>
		public ServerEndpointViewModel()
		{
			this.Capabilities = new List<string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServerEndpointViewModel"/> class.
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		public ServerEndpointViewModel(string serviceType) : this()
		{
			this.ServiceType = serviceType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServerEndpointViewModel"/> class.
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="capabilities">The capabilities.</param>
		public ServerEndpointViewModel(string serviceType, List<string> capabilities) : this(serviceType)
		{
			this.Capabilities = capabilities;
		}

		/// <summary>
		/// Gets or sets the capabilities.
		/// </summary>
		/// <value>The capabilities.</value>
		public List<string> Capabilities { get; set; }

		/// <summary>
		/// Gets or sets the type of the service.
		/// </summary>
		/// <value>The type of the service.</value>
		public string ServiceType { get; set; }

		/// <summary>
		/// Returns the server endpoint information.
		/// </summary>
		/// <returns>Returns the server endpoint information.</returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.ServiceType) || string.IsNullOrWhiteSpace(this.ServiceType))
			{
				this.ServiceType = Locale.NotApplicable;
			}

			this.Capabilities.RemoveAll(c => string.IsNullOrEmpty(c) || string.IsNullOrWhiteSpace(c));

			return $"{Locale.ServiceType}: {this.ServiceType}, {Locale.Capabilities}: {string.Join(", ", this.Capabilities.OrderBy(c => c))}";
		}
	}
}