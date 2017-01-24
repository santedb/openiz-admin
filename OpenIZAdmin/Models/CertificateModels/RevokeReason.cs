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
 * Date: 2016-7-9
 */

namespace OpenIZAdmin.Models.CertificateModels
{
	/// <summary>
	/// Represents a collection of revocation reasons for a certificate or certificate signing request.
	/// </summary>
	public enum RevokeReason : uint
	{
		/// <summary>
		/// The certificate or certificate signing request was revoked for an unspecified reason.
		/// </summary>
		Unspecified = 0x0,

		/// <summary>
		/// The certificate or certificate signing request was revoked because the key is compromised.
		/// </summary>
		KeyCompromise = 0x1,

		/// <summary>
		/// The certificate or certificate signing request was revoked because the CA is compromised.
		/// </summary>
		CaCompromise = 0x2,

		/// <summary>
		/// The certificate or certificate signing request was revoked because the affiliation changed.
		/// </summary>
		AffiliationChange = 0x3,

		/// <summary>
		/// The certificate or certificate signing request was revoked because the certificate or signing request was superseded.
		/// </summary>
		Superseded = 0x4,

		/// <summary>
		/// The certificate or certificate signing request was revoked because the institution ceased operations.
		/// </summary>
		CessationOfOperation = 0x5,

		/// <summary>
		/// The certificate or certificate signing request is on hold.
		/// </summary>
		CertificateHold = 0x6,

		/// <summary>
		/// The certificate or certificate signing request should be reinstated.
		/// </summary>
		Reinstate = 0xFFFFFFFF
	}
}