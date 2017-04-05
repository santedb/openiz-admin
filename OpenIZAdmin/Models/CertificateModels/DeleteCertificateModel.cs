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

using System.ComponentModel.DataAnnotations;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.CertificateModels
{
	/// <summary>
	/// Class DeleteCertificateModel.
	/// </summary>
	public class DeleteCertificateModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteCertificateModel"/> class.
		/// </summary>
		public DeleteCertificateModel()
		{
		}

		/// <summary>
		/// Gets or sets the certificate identifier.
		/// </summary>
		/// <value>The certificate identifier.</value>
		[Required]
		public string CertificateId { get; set; }

		/// <summary>
		/// Gets or sets the revoke reason.
		/// </summary>
		/// <value>The revoke reason.</value>
		[Display(Name = "RevokeReason", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "RevokeReasonRequired", ErrorMessageResourceType = typeof(Locale))]
		public RevokeReason RevokeReason { get; set; }
	}
}