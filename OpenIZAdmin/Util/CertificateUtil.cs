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
using OpenIZAdmin.Models.CertificateModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenIZAdmin.Util
{
	public static class CertificateUtil
	{
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

		public static CertificateSigningRequestViewModel ToCertificateSigningRequestViewModel(SubmissionInfo submissionInfo)
		{
			CertificateSigningRequestViewModel viewModel = new CertificateSigningRequestViewModel();

			viewModel.AdministrativeContactEmail = submissionInfo.EMail;
			viewModel.AdministrativeContactName = submissionInfo.AdminContact;
			viewModel.DistinguishedName = submissionInfo.DistinguishedName;
			viewModel.SubmissionTime = Convert.ToDateTime(submissionInfo.SubmittedWhen);

			return viewModel;
		}
	}
}