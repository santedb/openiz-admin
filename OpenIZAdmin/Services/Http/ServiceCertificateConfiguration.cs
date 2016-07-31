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
using System;
using System.Xml.Serialization;

namespace OpenIZAdmin.Services.Http
{
	/// <summary>
	/// Represents a service certificate configuration.
	/// </summary>
	[XmlType(nameof(ServiceCertificateConfiguration), Namespace = "http://openiz.org/mobile/configuration")]
	public class ServiceCertificateConfiguration : IRestClientCertificateDescription
	{
		/// <summary>
		/// Gets or sets the type of the find.
		/// </summary>
		/// <value>The type of the find.</value>
		[XmlAttribute("x509FindType")]
		public String FindType { get; set; }

		/// <summary>
		/// Gets or sets the find value.
		/// </summary>
		/// <value>The find value.</value>
		[XmlAttribute("findValue")]
		public String FindValue { get; set; }

		/// <summary>
		/// Gets or sets the store location.
		/// </summary>
		/// <value>The store location.</value>
		[XmlAttribute("storeLocation")]
		public String StoreLocation { get; set; }

		/// <summary>
		/// Gets or sets the name of the store.
		/// </summary>
		/// <value>The name of the store.</value>
		[XmlAttribute("storeName")]
		public String StoreName { get; set; }
	}
}