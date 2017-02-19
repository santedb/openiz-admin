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

using OpenIZ.Core.Model.AMI.Security;
using OpenIZAdmin.Localization;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.CertificateModels
{
	/// <summary>
	/// Represents a model to submit a certificate signing request.
	/// </summary>
	public class SubmitCertificateSigningRequestModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Models.CertificateModels.AcceptCertificateSigningRequestModel"/> class.
		/// </summary>
		public SubmitCertificateSigningRequestModel()
		{
		}

		/// <summary>
		/// Gets or sets the email of the administrative contact of the submission request.
		/// </summary>
		[Required]
		[EmailAddress]
		[Display(Name = "AdministrativeContactEmail", ResourceType = typeof(Locale))]
		public string AdministrativeContactEmail { get; set; }

		/// <summary>
		/// Gets or sets the name of the administrative contact of the submission request.
		/// </summary>
		[Required]
		[StringLength(255)]
		[Display(Name = "AdministrativeContactName", ResourceType = typeof(Locale))]
		public string AdministrativeContactName { get; set; }

		/// <summary>
		/// Gets or sets the cmc request of the submission request.
		/// </summary>
		[Required]
		[Display(Name = "CmcRequest", ResourceType = typeof(Locale))]
		public string CmcRequest { get; set; }

		/// <summary>
		/// Converts a <see cref="SubmitCertificateSigningRequestModel"/> instance to a <see cref="SubmissionRequest"/> instance.
		/// </summary>
		/// <returns>Returns a <see cref="SubmissionRequest"/> instance.</returns>
		public SubmissionRequest ToSubmissionRequest()
		{
			return new SubmissionRequest
			{
				AdminAddress = this.AdministrativeContactEmail,
				AdminContactName = this.AdministrativeContactName,
				CmcRequest = this.CmcRequest
			};
		}
	}
}