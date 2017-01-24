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
 * Date: 2016-7-23
 */

using OpenIZ.Core.Http.Description;
using System;
using System.Xml.Serialization;

namespace OpenIZAdmin.Services.Http
{
	/// <summary>
	/// Represents a single endpoint for use in the service client.
	/// </summary>
	[XmlType(nameof(ServiceClientEndpoint), Namespace = "http://openiz.org/web/configuration")]
	public class ServiceClientEndpoint : IRestClientEndpointDescription
	{
		/// <summary>
		/// The internal reference to the timeout value.
		/// </summary>
		private int timeout;

		/// <summary>
		/// Gets or sets the service client endpoint's address.
		/// </summary>
		/// <value>The address.</value>
		[XmlAttribute("address")]
		public String Address { get; set; }

		/// <summary>
		/// Gets or sets the timeout value for the endpoint.
		/// </summary>
		[XmlAttribute("timeout")]
		public int Timeout
		{
			get
			{
				return this.timeout;
			}

			set
			{
				this.timeout = value;
			}
		}
	}
}