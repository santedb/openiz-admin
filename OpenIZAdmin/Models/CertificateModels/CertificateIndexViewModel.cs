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

using System.Collections.Generic;

namespace OpenIZAdmin.Models.CertificateModels
{
	/// <summary>
	/// Represents a certificate index view model.
	/// </summary>
	public class CertificateIndexViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CertificateIndexViewModel"/> class.
		/// </summary>
		public CertificateIndexViewModel()
		{
			this.CertificateRevocations = new List<CertificateRevocationListViewModel>();
			this.Certificates = new List<CertificateViewModel>();
			this.CertificateSigningRequests = new List<CertificateSigningRequestViewModel>();
		}

		/// <summary>
		/// Gets or sets the list of certificates revocations.
		/// </summary>
		public IEnumerable<CertificateRevocationListViewModel> CertificateRevocations { get; set; }

		/// <summary>
		/// Gets or sets the list of certificates.
		/// </summary>
		public IEnumerable<CertificateViewModel> Certificates { get; set; }

		/// <summary>
		/// Gets or sets the list of certificate signing requests.
		/// </summary>
		public IEnumerable<CertificateSigningRequestViewModel> CertificateSigningRequests { get; set; }
	}
}