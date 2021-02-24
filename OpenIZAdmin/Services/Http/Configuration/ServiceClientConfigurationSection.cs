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

using Newtonsoft.Json;
using OpenIZ.Core.Http.Description;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace OpenIZAdmin.Services.Http.Configuration
{
	/// <summary>
	/// Represents a service client configuration section.
	/// </summary>
	[XmlType(nameof(ServiceClientConfigurationSection), Namespace = "http://openiz.org/web/configuration"), JsonObject(nameof(ServiceClientConfigurationSection))]
	public class ServiceClientConfigurationSection : ConfigurationSection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Services.Http.Configuration.ServiceClientConfigurationSection"/> class.
		/// </summary>
		public ServiceClientConfigurationSection()
		{
			this.Clients = new List<ServiceClientDescription>();
		}

		/// <summary>
		/// Represents a service client
		/// </summary>
		/// <value>The client.</value>
		[XmlElement("clients")]
		public List<ServiceClientDescription> Clients { get; set; }

		/// <summary>
		/// Gets or sets the proxy address.
		/// </summary>
		/// <value>The proxy address.</value>
		[XmlElement("proxyAddress"), JsonProperty("proxyAddress")]
		public String ProxyAddress { get; set; }

		/// <summary>
		/// Gets or sets the rest client implementation
		/// </summary>
		/// <value>The type of the rest client.</value>
		[XmlIgnore]
		public Type RestClientType { get; set; }

		/// <summary>
		/// Gets or sets the type which is to be used for rest clients
		/// </summary>
		/// <value>The rest client type xml.</value>
		[XmlAttribute("clientType")]
		public String RestClientTypeXml
		{
			get
			{
				return this.RestClientType?.AssemblyQualifiedName;
			}
			set
			{
				this.RestClientType = Type.GetType(value);
			}
		}

		/// <summary>
		/// Converts the service client configuration section to an XML representation.
		/// </summary>
		/// <returns>Returns an XML representation of the service client configuration section.</returns>
		public override string ToString()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ServiceClientConfigurationSection));

			StringWriter stringWriter = new StringWriter();

			serializer.Serialize(stringWriter, this);

			return stringWriter.ToString();
		}
	}

	internal class InternalConfiguration
	{
		/// <summary>
		/// Gets the service client configuration.
		/// </summary>
		/// <returns>Returns the service client configuration.</returns>
		internal static ServiceClientConfigurationSection GetServiceClientConfiguration()
		{
			var realm = RealmConfig.GetCurrentRealm();

			if (realm == null)
			{
				return new ServiceClientConfigurationSection();
			}

			ServiceClientConfigurationSection configurationSection = new ServiceClientConfigurationSection
			{
				Clients = new List<ServiceClientDescription>
				{
					new ServiceClientDescription
					{
						Binding = new ServiceClientBinding
						{
							Security = new ServiceClientSecurity
							{
								Mode = SecurityScheme.Basic
							}
						},
						Endpoint = new List<ServiceClientEndpoint>
						{
							new ServiceClientEndpoint
							{
								Address = $"{realm.Address}/auth/oauth2_token",
								Timeout = 30000
							}
						},
						Name = Constants.Acs
					},
					new ServiceClientDescription
					{
						Binding = new ServiceClientBinding
						{
							Security = new ServiceClientSecurity
							{
								Mode = SecurityScheme.Bearer
							}
						},
						Endpoint = new List<ServiceClientEndpoint>
						{
							new ServiceClientEndpoint
							{
								Address = $"{realm.Address}/ami",
								Timeout = 30000
							}
						},
						Name = Constants.Ami
					},
					new ServiceClientDescription
					{
						Binding = new ServiceClientBinding
						{
							Security = new ServiceClientSecurity
							{
								Mode = SecurityScheme.Bearer
							},
							Optimize = true
						},
						Endpoint = new List<ServiceClientEndpoint>
						{
							new ServiceClientEndpoint
							{
								Address = $"{realm.Address}/imsi",
								Timeout = 120000
							}
						},
						Name = Constants.Imsi
					},
					new ServiceClientDescription
					{
						Binding = new ServiceClientBinding
						{
							Security = new ServiceClientSecurity
							{
								Mode = SecurityScheme.Bearer
							},
							Optimize = true
						},
						Endpoint = new List<ServiceClientEndpoint>
						{
							new ServiceClientEndpoint
							{
								Address = $"{realm.Address}/risi",
								Timeout = 30000
							}
						},
						Name = Constants.Risi
					}
				}
			};

			return configurationSection;
		}
	}
}