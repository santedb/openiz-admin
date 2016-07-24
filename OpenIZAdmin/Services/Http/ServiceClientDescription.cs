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
 * Date: 2016-7-23
 */
using OpenIZ.Core.Http.Description;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OpenIZAdmin.Services.Http
{
	/// <summary>
	/// A service client represent a single client to a service.
	/// </summary>
	[XmlType(nameof(ServiceClientDescription), Namespace = "http://openiz.org/web/configuration")]
	public class ServiceClientDescription : IRestClientDescription
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Services.Http.ServiceClientDescription"/> class.
		/// </summary>
		public ServiceClientDescription()
		{
			this.Endpoint = new List<ServiceClientEndpoint>();
		}

		/// <summary>
		/// The endpoints of the client.
		/// </summary>
		/// <value>The endpoint.</value>
		[XmlElement("endpoints")]
		public List<ServiceClientEndpoint> Endpoint { get; set; }

		/// <summary>
		/// Gets or sets the name of the service client.
		/// </summary>
		/// <value>The name.</value>
		[XmlAttribute("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the binding for the service client.
		/// </summary>
		/// <value>The binding.</value>
		[XmlElement("binding")]
		public ServiceClientBinding Binding { get; set; }

		/// <summary>
		/// Gets the endpoints for the service client configuration.
		/// </summary>
		List<IRestClientEndpointDescription> IRestClientDescription.Endpoint
		{
			get
			{
				return this.Endpoint.OfType<IRestClientEndpointDescription>().ToList();
			}
		}

		/// <summary>
		/// Gets the binding for the service client configuration.
		/// </summary>
		IRestClientBindingDescription IRestClientDescription.Binding
		{
			get
			{
				return this.Binding;
			}
		}
	}
}