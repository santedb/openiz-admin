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
using OpenIZAdmin.Models.CertificateModels;
using OpenIZAdmin.Models.CertificateModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing certificates.
	/// </summary>
	[Authorize]
	public class CertificateController : Controller
	{
		/// <summary>
		/// The internal reference to the administrative interface endpoint.
		/// </summary>
		private static readonly Uri amiEndpoint = AmiConfig.AmiEndpoint;

		/// <summary>
		/// The internal reference to the <see cref="System.Net.Http.HttpClient"/> instance.
		/// </summary>
		private readonly HttpClient client;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Controllers.CertificateController"/> class.
		/// </summary>
		public CertificateController()
		{
			this.client = new HttpClient();
		}

		/// <summary>
		/// Accepts a certificate signing request.
		/// </summary>
		/// <returns>Returns a view with the accepted certificate signing request.</returns>
		[HttpPost]
		[ActionName("AcceptCsr")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> AcceptCertificateSigningRequestAsync(AcceptCertificateSigningRequestModel model)
		{
			if (ModelState.IsValid)
			{
				var response = await this.client.PutAsync(string.Format("{0}/csr/{1}", amiEndpoint, model.CertificateId), null);

				if (response.IsSuccessStatusCode)
				{
					TempData["success"] = "Certificate signing request successfully accepted";

					return RedirectToAction("Index");
				}
			}

			TempData["error"] = "Unable to accept certificate signing request";

			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("DeleteCertificate")]
		public async Task<ActionResult> DeleteCertificateAsync(DeleteCertificateModel model)
		{
			if (ModelState.IsValid)
			{
				var response = await this.client.DeleteAsync(string.Format("{0}/csr/{1}", amiEndpoint, model.CertificateId));

				if (response.IsSuccessStatusCode)
				{
					TempData["success"] = "Certificate successfully deleted";

					return RedirectToAction("Index");
				}
			}

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
		[ActionName("GetCertificate")]
		public async Task<ActionResult> GetCertificateAsync(string id)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
			{
				var response = await this.client.GetAsync(string.Format("{0}/certificate/{1}", amiEndpoint, id));

				if (response.IsSuccessStatusCode)
				{
					CertificateViewModel viewModel = new CertificateViewModel();

					return View(viewModel);
				}
			}

			TempData["error"] = "Unable to find specified certificate";

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Gets the certificate revocation list.
		/// </summary>
		/// <returns>Returns a view with a list of revoked certificates.</returns>
		[HttpGet]
		[ActionName("CertificateRevocationList")]
		public async Task<ActionResult> GetCertificateRevocationListAsync()
		{
			var response = await this.client.GetAsync(string.Format("{0}/crl", amiEndpoint));

			if (response.IsSuccessStatusCode)
			{
				CertificateRevocationListViewModel viewModel = new CertificateRevocationListViewModel();

				return View(viewModel);
			}

			TempData["error"] = "Unable to retrieve certificate revocation list";

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Gets a list of certificates.
		/// </summary>
		/// <returns>Returns a view with a list of certificates.</returns>
		[HttpGet]
		[ActionName("GetCertificates")]
		public async Task<ActionResult> GetCertificatesAsync()
		{
			var response = await this.client.GetAsync(string.Format("{0}/certificate", amiEndpoint));

			if (response.IsSuccessStatusCode)
			{
				CertificateViewModel viewModel = new CertificateViewModel();

				return View(viewModel);
			}

			TempData["error"] = "Unable to retrieve certificate list";

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Gets a certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request.</param>
		/// <returns>Returns a view with the certificate signing request.</returns>
		[HttpGet]
		[ActionName("GetCertificateSigingRequest")]
		public async Task<ActionResult> GetCertificateSigningRequestAsync(string id)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
			{
				var response = await this.client.GetAsync(string.Format("{0}/csr/{1}", amiEndpoint, id));

				if (response.IsSuccessStatusCode)
				{
					CertificateSigningRequestViewModel viewModel = new CertificateSigningRequestViewModel();

					return View(viewModel);
				}
			}

			TempData["error"] = "Unable to find specified certificate signing request";

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Gets a list of certificate signing requests.
		/// </summary>
		/// <returns>Returns a view with a list of certificate signing requests.</returns>
		[HttpGet]
		[ActionName("GetCertificateSigningRequests")]
		public async Task<ActionResult> GetCertificateSigningRequestsAsync()
		{
			var response = await this.client.GetAsync(string.Format("{0}/csr", amiEndpoint));

			if (response.IsSuccessStatusCode)
			{
				CertificateSigningRequestViewModel viewModel = new CertificateSigningRequestViewModel();

				return View(viewModel);
			}

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
			return View();
		}

		/// <summary>
		/// Rejects a certificate signing request.
		/// </summary>
		/// <returns>Returns a view with the status of the rejection.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult RejectCertificateSigningRequest(RejectCertificateSigningRequestModel model)
		{
			if (ModelState.IsValid)
			{

			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SubmitCertificateSigningRequest(SubmitCertificateSigningRequestModel model)
		{
			if (ModelState.IsValid)
			{

			}

			return View(model);
		}
	}
}