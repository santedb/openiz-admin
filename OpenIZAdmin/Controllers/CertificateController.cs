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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	[Authorize]
    public class CertificateController : Controller
	{
		public CertificateController()
		{

		}

		/// <summary>
		/// Accepts a certificate signing request.
		/// </summary>
		/// <returns>Returns a view with the accepted certificate signing request.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AcceptCertificateSigningRequest()
		{
			return View();
		}

		[HttpGet]
		public ActionResult Certificate(string id)
		{
			return View();
		}

		[HttpGet]
		public ActionResult CertificateRevocationList()
		{
			return View();
		}

		/// <summary>
		/// Gets a list of certificates
		/// </summary>
		/// <returns>Returns a view with a list of certificates.</returns>
		[HttpGet]
		public ActionResult Certificates()
		{
			return View();
		}

		/// <summary>
		/// Gets a certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request.</param>
		/// <returns>Returns a view with the certificate signing request.</returns>
		[HttpGet]
		public ActionResult CertificateSigningRequest(string id)
		{
			return View();
		}

		/// <summary>
		/// Gets a list of certificate signing requests.
		/// </summary>
		/// <returns>Returns a view with a list of certificate signing requests.</returns>
		[HttpGet]
		public ActionResult CertificateSigningRequests()
		{
			return View();
		}

		[HttpGet]
		public ActionResult DeleteCertificate()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteCertificate(DeleteCertificateModel model)
		{
			if (ModelState.IsValid)
			{

			}

			return View(model);
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