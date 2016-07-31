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
 * Date: 2016-7-10
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.CertificateModels.ViewModels
{
	/// <summary>
	/// Represents a certificate signing request view model.
	/// </summary>
	public class CertificateSigningRequestViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Models.CertificateModels.ViewModels.CertificateSigningRequestViewModel"/> class.
		/// </summary>
		public CertificateSigningRequestViewModel()
		{
		}

		/// <summary>
		/// Gets or sets the administrative contact email of the signing request.
		/// </summary>
		[Display(Name = "AdministrativeContactEmail", ResourceType = typeof(Localization.Locale))]
		public string AdministrativeContactEmail { get; set; }

		/// <summary>
		/// Gets or sets the administrative contact name of the signing request.
		/// </summary>
		[Display(Name = "AdministrativeContactName", ResourceType = typeof(Localization.Locale))]
		public string AdministrativeContactName { get; set; }

		/// <summary>
		/// Gets or sets the distinguished name of the signing request.
		/// </summary>
		[Display(Name = "DistinguishedName", ResourceType = typeof(Localization.Locale))]
		public string DistinguishedName { get; set; }

		/// <summary>
		/// Gets or sets the submission time of the signing request.
		/// </summary>
		[Display(Name = "SubmissionTime", ResourceType = typeof(Localization.Locale))]
		public DateTime SubmissionTime { get; set; }
	}
}