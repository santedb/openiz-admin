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
	/// Service client security configuration.
	/// </summary>
	[XmlType(nameof(ServiceClientSecurity), Namespace = "http://openiz.org/web/configuration")]
	public class ServiceClientSecurity : IRestClientSecurityDescription
	{
		/// <summary>
		/// Gets or sets the ICertificateValidator interface which should be called to validate
		/// certificates
		/// </summary>
		/// <value>The serialization binder type xml.</value>
		[XmlAttribute("certificateValidator")]
		public string CertificateValidatorXml
		{
			get
			{
				return this.CertificateValidator?.GetType().AssemblyQualifiedName;
			}
			set
			{
				this.CertificateValidator = Activator.CreateInstance(Type.GetType(value)) as ICertificateValidator;
			}
		}

		/// <summary>
		/// Gets or sets the certificate validator.
		/// </summary>
		/// <value>The certificate validator.</value>
		[XmlIgnore]
		public ICertificateValidator CertificateValidator { get; set; }

		/// <summary>
		/// Gets or sets the ICredentialProvider
		/// </summary>
		/// <value>The credential provider xml.</value>
		[XmlAttribute("credentialProvider")]
		public string CredentialProviderXml
		{
			get
			{
				return this.CredentialProvider?.GetType().AssemblyQualifiedName;
			}
			set
			{
				this.CredentialProvider = Activator.CreateInstance(Type.GetType(value)) as ICredentialProvider;
			}
		}

		/// <summary>
		/// Security mode
		/// </summary>
		/// <value>The mode.</value>
		[XmlAttribute("mode")]
		public SecurityScheme Mode { get; set; }

		/// <summary>
		/// Gets or sets the credential provider.
		/// </summary>
		/// <value>The credential provider.</value>
		[XmlIgnore]
		public ICredentialProvider CredentialProvider { get; set; }

		/// <summary>
		/// Gets the thumbprint the device should use for authentication
		/// </summary>
		[XmlElement("certificate")]
		public ServiceCertificateConfiguration ClientCertificate { get; set; }

		/// <summary>
		/// Gets or sets the authentication realm this client should verify
		/// </summary>
		/// <value>The auth realm.</value>
		[XmlAttribute("realm")]
		public string AuthRealm { get; set; }

		/// <summary>
		/// Gets the certificate.
		/// </summary>
		IRestClientCertificateDescription IRestClientSecurityDescription.ClientCertificate
		{
			get
			{
				return this.ClientCertificate;
			}
		}

		/// <summary>
		/// Gets or sets the preemptive authentication
		/// </summary>
		public bool PreemptiveAuthentication
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}
	}
}