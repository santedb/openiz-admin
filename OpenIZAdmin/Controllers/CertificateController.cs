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
 * Date: 2016-7-8
 */

using OpenIZ.Core.Model.AMI.Security;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.CertificateModels;
using OpenIZAdmin.Util;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing certificates.
	/// </summary>
	[TokenAuthorize]
	public class CertificateController : BaseController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Controllers.CertificateController"/> class.
		/// </summary>
		public CertificateController()
		{
		}

		/// <summary>
		/// Accepts a certificate signing request.
		/// </summary>
		/// <returns>Returns a view with the accepted certificate signing request.</returns>
		[HttpPost]
		[ActionName("AcceptCsr")]
		[ValidateAntiForgeryToken]
		public ActionResult AcceptCertificateSigningRequest(AcceptCertificateSigningRequestModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					this.AmiClient.AcceptCertificateSigningRequest(model.CertificateId);

					TempData["success"] = Locale.CertificateSigningRequestAccepted;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to accept certificate signing request: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to accept certificate signing request: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.UnableToAcceptCertificateSigningRequest;

			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteCertificate(DeleteCertificateModel model)
		{
			TempData["error"] = Locale.UnableToDelete + " " + Locale.Certificate;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Gets a certificate by id.
		/// </summary>
		/// <param name="id">The id of the certificate to retrieve.</param>
		/// <returns>Returns a view with the specified certificate.</returns>
		[HttpGet]
		[ActionName("Certificate")]
		public ActionResult GetCertificate(string id)
		{
			TempData["error"] = Locale.UnableToFindSpecifiedCertificate;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Gets a list of certificates.
		/// </summary>
		/// <returns>Returns a view with a list of certificates.</returns>
		[HttpGet]
		[ActionName("Certificates")]
		public ActionResult GetCertificates()
		{
			TempData["error"] = Locale.UnableToRetrieveCertificateList;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Gets a list of certificate signing requests.
		/// </summary>
		/// <returns>Returns a view with a list of certificate signing requests.</returns>
		[HttpGet]
		public ActionResult GetCertificateSigningRequests()
		{
			TempData["error"] = Locale.UnableToRetrieveCertificateSigningRequestList;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "Certificate";
			return View(CertificateUtil.GetAllCertificateSigningRequests(this.AmiClient));
		}

		/// <summary>
		/// Rejects a certificate signing request.
		/// </summary>
		/// <returns>Returns a view with the status of the rejection.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult RejectCertificateSigningRequest(RejectCertificateSigningRequestModel model)
		{
			TempData["error"] = Locale.UnableToRejectCertificateSigningRequest;

			return View(model);
		}

		[HttpGet]
		public ActionResult SubmitCertificateSigningRequest()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SubmitCertificateSigningRequest(SubmitCertificateSigningRequestModel model)
		{
			if (ModelState.IsValid)
			{
				SubmissionRequest submissionRequest = CertificateUtil.ToSubmissionRequest(model);

				SubmissionResult result = null;

				try
				{
					result = this.AmiClient.SubmitCertificateSigningRequest(submissionRequest);

					TempData["success"] = Locale.CertificateSigningRequestSuccessfullySubmitted;

					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to submit certificate signing request: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to submit certificate signing request: {0}", e.Message);
				}
			}

			TempData["error"] = Locale.UnableToSubmitCertificateSigningRequest;

			return View(model);
		}

		/// <summary>
		/// Gets the certificate revocation list.
		/// </summary>
		/// <returns>Returns a view with a list of revoked certificates.</returns>
		[HttpGet]
		public ActionResult ViewCertificateRevocationList()
		{
			TempData["error"] = Locale.UnableToRetrieveCertificateRevocationlist;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Gets a certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request.</param>
		/// <returns>Returns a view with the certificate signing request.</returns>
		[HttpGet]
		public ActionResult ViewCertificateSigningRequest(string id)
		{
			TempData["error"] = Locale.UnableToFindSpecifiedCertificateSigningRequest;

			return RedirectToAction("Index");
		}
	}
}