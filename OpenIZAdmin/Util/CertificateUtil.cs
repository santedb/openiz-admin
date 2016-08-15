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
using System.Diagnostics;
using System.Linq;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing certificates.
	/// </summary>
	public static class CertificateUtil
	{
		/// <summary>
		/// Gets a list of certificate signing requests.
		/// </summary>
		/// <param name="client">The <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.</param>
		/// <returns>Returns a list of certificate signing requests.</returns>
		internal static IEnumerable<CertificateSigningRequestViewModel> GetAllCertificateSigningRequests(AmiServiceClient client)
		{
			IEnumerable<CertificateSigningRequestViewModel> viewModels = new List<CertificateSigningRequestViewModel>();

			try
			{
				var certificateSigningRequests = client.GetCertificateSigningRequests(c => c.DistinguishedName != null);

				viewModels = certificateSigningRequests.CollectionItem.Select(c => CertificateUtil.ToCertificateSigningRequestViewModel(c));
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve certificate signing requests: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve certificate signing requests: {0}", e.Message);
			}

			return viewModels;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.AMI.Security.SubmissionInfo"/> to a <see cref="OpenIZAdmin.Models.CertificateModels.ViewModels.CertificateSigningRequestViewModel"/>.
		/// </summary>
		/// <param name="submissionInfo">The submission information to convert.</param>
		/// <returns>Returns a certificate signing request view model.</returns>
		public static CertificateSigningRequestViewModel ToCertificateSigningRequestViewModel(SubmissionInfo submissionInfo)
		{
			CertificateSigningRequestViewModel viewModel = new CertificateSigningRequestViewModel();

			viewModel.AdministrativeContactEmail = submissionInfo.EMail;
			viewModel.AdministrativeContactName = submissionInfo.AdminContact;
			viewModel.DistinguishedName = submissionInfo.DistinguishedName;
			viewModel.SubmissionTime = Convert.ToDateTime(submissionInfo.SubmittedWhen);

			return viewModel;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZAdmin.Models.CertificateModels.SubmitCertificateSigningRequestModel"/> instance
		/// to a <see cref="OpenIZ.Core.Model.AMI.Security.SubmissionRequest"/> instance.
		/// </summary>
		/// <returns>Returns a submission request.</returns>
		public static SubmissionRequest ToSubmissionRequest(SubmitCertificateSigningRequestModel model)
		{
			SubmissionRequest submissionRequest = new SubmissionRequest();

			submissionRequest.AdminAddress = model.AdministrativeContactEmail;
			submissionRequest.AdminContactName = model.AdministrativeContactName;
			submissionRequest.CmcRequest = model.CmcRequest;

			return submissionRequest;
		}
	}
}