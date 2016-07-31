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

using Newtonsoft.Json;
using OpenIZ.Core.Http.Description;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Models.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
		/// Gets or sets the proxy address.
		/// </summary>
		/// <value>The proxy address.</value>
		[XmlElement("proxyAddress"), JsonProperty("proxyAddress")]
		public String ProxyAddress { get; set; }

		/// <summary>
		/// Represents a service client
		/// </summary>
		/// <value>The client.</value>
		[XmlElement("clients")]
		public List<ServiceClientDescription> Clients { get; set; }

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
		/// Gets or sets the rest client implementation
		/// </summary>
		/// <value>The type of the rest client.</value>
		[XmlIgnore]
		public Type RestClientType { get; set; }
	}

	internal class InternalConfiguration
	{
		/// <summary>
		/// Gets the service client configuration.
		/// </summary>
		/// <returns>Returns the service client configuration.</returns>
		internal static ServiceClientConfigurationSection GetServiceClientConfiguration()
		{
			Realm realm = null;

			using (IUnitOfWork unitOfWork = new EntityUnitOfWork(new ApplicationDbContext()))
			{
				realm = unitOfWork.RealmRepository.Get(r => r.ObsoletionTime == null).SingleOrDefault();
			}

			if (realm == null)
			{
				return new ServiceClientConfigurationSection();
			}

			ServiceClientConfigurationSection configurationSection = new ServiceClientConfigurationSection
			{
				Clients = new System.Collections.Generic.List<ServiceClientDescription>
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
						Endpoint = new System.Collections.Generic.List<ServiceClientEndpoint>
						{
							new ServiceClientEndpoint
							{
								Address = realm.AmiAuthEndpoint
							}
						},
						Name = "ACS"
					},
					new ServiceClientDescription
					{
						Binding = new ServiceClientBinding
						{
							Security = new ServiceClientSecurity
							{
								Mode = SecurityScheme.Bearer
							},
							Optimize = false
						},
						Endpoint = new System.Collections.Generic.List<ServiceClientEndpoint>
						{
							new ServiceClientEndpoint
							{
								Address = realm.AmiEndpoint
							}
						},
						Name = "AMI"
					},
					new ServiceClientDescription
					{
						Binding = new ServiceClientBinding
						{
							Security = new ServiceClientSecurity
							{
								Mode = SecurityScheme.Basic
							}
						},
						Endpoint = new System.Collections.Generic.List<ServiceClientEndpoint>
						{
							new ServiceClientEndpoint
							{
								Address = realm.Address + "/imsi"
							}
						},
						Name = "IMSI"
					}
				}
			};

			return configurationSection;
		}
	}
}