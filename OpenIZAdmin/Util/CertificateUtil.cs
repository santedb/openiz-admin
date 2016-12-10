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
 * Date: 2016-7-30
 */

using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Models.CertificateModels;
using OpenIZAdmin.Models.CertificateModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing certificates.
	/// </summary>
	public static class CertificateUtil
	{
		/// <summary>
		/// Converts a <see cref="SubmissionInfo"/> to a <see cref="CertificateSigningRequestViewModel"/>.
		/// </summary>
		/// <param name="submissionInfo">The <see cref="SubmissionInfo"/> instance to convert.</param>
		/// <returns>Returns a certificate signing request view model.</returns>
		public static CertificateSigningRequestViewModel ToCertificateSigningRequestViewModel(SubmissionInfo submissionInfo)
		{
			var viewModel = new CertificateSigningRequestViewModel
			{
				AdministrativeContactEmail = submissionInfo.EMail,
				AdministrativeContactName = submissionInfo.AdminContact,
				DistinguishedName = submissionInfo.DistinguishedName,
				SubmissionTime = Convert.ToDateTime(submissionInfo.SubmittedWhen)
			};

			return viewModel;
		}

		/// <summary>
		/// Converts a <see cref="SubmitCertificateSigningRequestModel"/> instance to a <see cref="SubmissionRequest"/> instance.
		/// </summary>
		/// <param name="model">The <see cref="SubmitCertificateSigningRequestModel"/> instance to convert.</param>
		/// <returns>Returns a submission request.</returns>
		public static SubmissionRequest ToSubmissionRequest(SubmitCertificateSigningRequestModel model)
		{
			var submissionRequest = new SubmissionRequest
			{
				AdminAddress = model.AdministrativeContactEmail,
				AdminContactName = model.AdministrativeContactName,
				CmcRequest = model.CmcRequest
			};

			return submissionRequest;
		}

		/// <summary>
		/// Gets a list of certificate signing requests.
		/// </summary>
		/// <param name="client">The <see cref="AmiServiceClient"/> instance.</param>
		/// <returns>Returns a list of certificate signing requests.</returns>
		internal static IEnumerable<CertificateSigningRequestViewModel> GetAllCertificateSigningRequests(AmiServiceClient client)
		{
			var certificateSigningRequests = client.GetCertificateSigningRequests(c => c.ResolvedWhen == null);

			var viewModels = certificateSigningRequests.CollectionItem.Select(CertificateUtil.ToCertificateSigningRequestViewModel);

			return viewModels;
		}
	}
}