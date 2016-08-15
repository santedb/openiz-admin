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
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models.CertificateModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
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
	public class CertificateController : Controller
	{
		/// <summary>
		/// The internal reference to the <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient client;

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
					this.client.AcceptCertificateSigningRequest(model.CertificateId);

					TempData["success"] = "Certificate signing request successfully accepted";

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

			TempData["error"] = "Unable to accept certificate signing request";

			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteCertificate(DeleteCertificateModel model)
		{
			//if (ModelState.IsValid)
			//{
			//	var response = await this.client.DeleteAsync(string.Format("/csr/{0}", model.CertificateId));

			//	if (response.IsSuccessStatusCode)
			//	{
			//		TempData["success"] = "Certificate successfully deleted";

			//		return RedirectToAction("Index");
			//	}
			//}

			TempData["error"] = "Unable to delete certificate";

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected override void Dispose(bool disposing)
		{
			Trace.TraceInformation("{0} disposing", nameof(CertificateController));

			this.client?.Dispose();

			base.Dispose(disposing);
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
			//if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
			//{
			//	var response = await this.client.GetAsync(string.Format("/certificate/{0}", id));

			//	if (response.IsSuccessStatusCode)
			//	{
			//		CertificateViewModel viewModel = new CertificateViewModel();

			//		return View(viewModel);
			//	}
			//}

			TempData["error"] = "Unable to find specified certificate";

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
			//var response = await this.client.GetAsync(string.Format("/certificates/"));

			//if (response.IsSuccessStatusCode)
			//{
			//	CertificateViewModel viewModel = new CertificateViewModel();

			//	return View(viewModel);
			//}

			TempData["error"] = "Unable to retrieve certificate list";

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Gets a list of certificate signing requests.
		/// </summary>
		/// <returns>Returns a view with a list of certificate signing requests.</returns>
		[HttpGet]
		public ActionResult GetCertificateSigningRequests()
		{
			//var response = await this.client.GetAsync(string.Format("/csrs/"));

			//if (response.IsSuccessStatusCode)
			//{
			//	CertificateSigningRequestViewModel viewModel = new CertificateSigningRequestViewModel();

			//	return View(viewModel);
			//}

			TempData["error"] = "Unable to retrieve certificate signing request list";

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
			return View(CertificateUtil.GetAllCertificateSigningRequests(this.client));
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var restClient = new RestClientService("AMI");

			restClient.Accept = "application/xml";
			restClient.Credentials = new AmiCredentials(this.User, HttpContext.Request);

			this.client = new AmiServiceClient(restClient);

			base.OnActionExecuting(filterContext);
		}

		/// <summary>
		/// Rejects a certificate signing request.
		/// </summary>
		/// <returns>Returns a view with the status of the rejection.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult RejectCertificateSigningRequest(RejectCertificateSigningRequestModel model)
		{
			//if (ModelState.IsValid)
			//{
			//	var response = await this.client.DeleteAsync(string.Format("/csr/{0}", model.CertificateId));

			//	if (response.IsSuccessStatusCode)
			//	{
			//		TempData["success"] = "Certificate signing request sucessfully rejected";

			//		return RedirectToAction("Index");
			//	}
			//}

			TempData["error"] = "Unable to reject certificate signing request";

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
					result = this.client.SubmitCertificateSigningRequest(submissionRequest);

					TempData["success"] = "Certificate signing request successfully submitted";

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

			TempData["error"] = "Unable to submit certificate signing request";

			return View(model);
		}

		/// <summary>
		/// Gets the certificate revocation list.
		/// </summary>
		/// <returns>Returns a view with a list of revoked certificates.</returns>
		[HttpGet]
		public ActionResult ViewCertificateRevocationList()
		{
			//var response = await this.client.GetAsync(string.Format("/crl"));

			//if (response.IsSuccessStatusCode)
			//{
			//	CertificateRevocationListViewModel viewModel = new CertificateRevocationListViewModel();

			//	return View(viewModel);
			//}

			TempData["error"] = "Unable to retrieve certificate revocation list";

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
			//if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
			//{
			//	var response = await this.client.GetAsync(string.Format("/csr/{0}", id));

			//	if (response.IsSuccessStatusCode)
			//	{
			//		CertificateSigningRequestViewModel viewModel = new CertificateSigningRequestViewModel();

			//		return View(viewModel);
			//	}
			//}

			TempData["error"] = "Unable to find specified certificate signing request";

			return RedirectToAction("Index");
		}
	}
}