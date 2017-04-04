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
 * Date: 2016-7-8
 */

using Elmah;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.CertificateModels;
using System;
using System.Diagnostics;
using System.Linq;
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
		/// <param name="model">The model.</param>
		/// <returns>Returns a view with the accepted certificate signing request.</returns>
		[HttpPost]
		[ActionName("AcceptCsr")]
		[ValidateAntiForgeryToken]
		public ActionResult AcceptCertificateSigningRequest(AcceptCertificateSigningRequestModel model)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					this.AmiClient.AcceptCertificateSigningRequest(model.CertificateId);

					TempData["success"] = Locale.CertificateSigningRequestAccepted;

					return RedirectToAction("Index");
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			this.TempData["error"] = Locale.UnableToAcceptCertificateSigningRequest;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Deletes the certificate.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteCertificate(DeleteCertificateModel model)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					this.AmiClient.DeleteCertificate(model.CertificateId, model.RevokeReason);

					this.TempData["success"] = Locale.Certificate + " " + Locale.Deleted + " " + Locale.Successfully;

					return RedirectToAction("Index");
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to delete certificate: { e }");
			}

			this.TempData["error"] = Locale.UnableToDelete + " " + Locale.Certificate;

			return View(model);
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
			try
			{
				var model = new CertificateViewModel(this.AmiClient.GetCertificateSigningRequest(id));

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve certificate: { e }");
			}

			this.TempData["error"] = Locale.UnableToFindSpecifiedCertificate;

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
			try
			{
				var certificates = this.AmiClient.GetCertificateSigningRequests(c => c.RequestID != null);

				var models = certificates.CollectionItem.Select(c => new CertificateViewModel(c));

				return View(models);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve certificate: { e }");
			}

			TempData["error"] = Locale.UnableTo + " " + Locale.Retrieve + " " + Locale.Certificates;

			return this.RedirectToRequestOrHome();
		}

		/// <summary>
		/// Gets a list of certificate signing requests.
		/// </summary>
		/// <returns>Returns a view with a list of certificate signing requests.</returns>
		[HttpGet]
		public ActionResult GetCertificateSigningRequests()
		{
			try
			{
				var certificates = this.AmiClient.GetCertificateSigningRequests(c => c.RequestID != null);

				var models = certificates.CollectionItem.Select(c => new CertificateViewModel(c));

				return View(models);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(this.HttpContext.ApplicationInstance.Context).Log(new Error(e, this.HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve certificate: { e }");
			}

			TempData["error"] = Locale.UnableTo + " " + Locale.Retrieve + " " + Locale.Certificates;

			return this.RedirectToRequestOrHome();
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			var viewModel = new CertificateIndexViewModel();

			TempData["searchType"] = "Certificate";

			try
			{
				var certificateSigningRequests = this.AmiClient.GetCertificateSigningRequests(c => c.ResolvedWhen == null);

				viewModel.CertificateSigningRequests = certificateSigningRequests.CollectionItem.Select(c => new CertificateSigningRequestViewModel(c));

				return View(viewModel);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve certificate signing requests: { e }");
			}

			return View(viewModel);
		}

		/// <summary>
		/// Rejects a certificate signing request.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Returns a view with the status of the rejection.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult RejectCertificateSigningRequest(RejectCertificateSigningRequestModel model)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					this.AmiClient.DeleteCertificate(model.CertificateId, model.RevokeReason);
					//	this.AmiClient.re
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to reject certificate signing requests: { e }");
			}
			TempData["error"] = Locale.UnableToRejectCertificateSigningRequest;

			return View(model);
		}

		/// <summary>
		/// Submits the certificate signing request.
		/// </summary>
		/// <returns>ActionResult.</returns>
		[HttpGet]
		public ActionResult SubmitCertificateSigningRequest()
		{
			return View();
		}

		/// <summary>
		/// Submits the certificate signing request.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SubmitCertificateSigningRequest(SubmitCertificateSigningRequestModel model)
		{
			if (ModelState.IsValid)
			{
				var submissionRequest = model.ToSubmissionRequest();

				try
				{
					var result = this.AmiClient.SubmitCertificateSigningRequest(submissionRequest);

					TempData["success"] = Locale.CertificateSigningRequestSuccessfullySubmitted;

					return RedirectToAction("ViewCertificateSigningRequest", new { id = result.RequestId });
				}
				catch (Exception e)
				{
					ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
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
		public ActionResult ViewCertificateSigningRequest(int id)
		{
			TempData["error"] = Locale.UnableToFindSpecifiedCertificateSigningRequest;

			return RedirectToAction("Index");
		}
	}
}