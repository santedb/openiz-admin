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

using OpenIZ.Core.Http;
using OpenIZ.Core.Http.Description;
using System;
using System.Xml.Serialization;

namespace OpenIZAdmin.Services.Http
{
	/// <summary>
	/// Service client binding
	/// </summary>
	[XmlType(nameof(ServiceClientBinding), Namespace = "http://openiz.org/web/configuration")]
	public class ServiceClientBinding : IRestClientBindingDescription
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceClientBinding"/> class.
		/// </summary>
		public ServiceClientBinding()
		{
			this.ContentTypeMapper = new DefaultContentTypeMapper();
		}

		/// <summary>
		/// Gets or sets the type which dictates how a body maps to an object.
		/// </summary>
		/// <value>The serialization binder type XML.</value>
		[XmlAttribute("contentTypeMapper")]
		public string ContentTypeMapperXml
		{
			get
			{
				return this.ContentTypeMapper?.GetType().AssemblyQualifiedName;
			}
			set
			{
				this.ContentTypeMapper = Activator.CreateInstance(Type.GetType(value)) as IContentTypeMapper;
			}
		}

		/// <summary>
		/// Content type mapper
		/// </summary>
		/// <value>The content type mapper.</value>
		[XmlIgnore]
		public IContentTypeMapper ContentTypeMapper { get; set; }

		/// <summary>
		/// Gets or sets the security configuration
		/// </summary>
		/// <value>The security.</value>
		[XmlElement("security")]
		public ServiceClientSecurity Security { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ServiceClientBinding"/> is optimized.
		/// </summary>
		/// <value><c>true</c> if optimize; otherwise, <c>false</c>.</value>
		[XmlElement("optimize")]
		public bool Optimize { get; set; }

		/// <summary>
		/// Gets the security description
		/// </summary>
		IRestClientSecurityDescription IRestClientBindingDescription.Security => this.Security;
	}
}