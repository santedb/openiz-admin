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
 * Date: 2016-7-10
 */

using OpenIZ.Core.Model.AMI.Security;
using System;
using System.Security.Cryptography.X509Certificates;

namespace OpenIZAdmin.Models.CertificateModels
{
	/// <summary>
	/// Represents a certificate view model.
	/// </summary>
	public class CertificateViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CertificateViewModel"/> class.
		/// </summary>
		public CertificateViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CertificateViewModel"/> class
		/// with a specified <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/> instance.
		/// </summary>
		public CertificateViewModel(X509Certificate2 certificate)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CertificateViewModel"/> class.
		/// </summary>
		/// <param name="submissionInfo">The submission information.</param>
		public CertificateViewModel(SubmissionInfo submissionInfo)
		{
			this.Id = submissionInfo.RequestID;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CertificateViewModel"/> class.
		/// </summary>
		/// <param name="submissionResult">The submission result.</param>
		public CertificateViewModel(SubmissionResult submissionResult)
		{
		}

		/// <summary>
		/// Gets or sets the id of the certificate.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the issuer of the certificate.
		/// </summary>
		public string Issuer { get; set; }

		/// <summary>
		/// Gets or sets the time which the certificate is valid until.
		/// </summary>
		public DateTime NotAfter { get; set; }

		/// <summary>
		/// Gets or sets the time which the certificate is valid from.
		/// </summary>
		public DateTime NotBefore { get; set; }

		/// <summary>
		/// Gets or sets the subject of the certificate.
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the thumbprint of the certificate.
		/// </summary>
		public string Thumbprint { get; set; }
	}
}