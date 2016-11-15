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

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.DataTypes;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AssigningAuthorityModels;
using OpenIZAdmin.Models.AssigningAuthorityModels.ViewModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for administering policies.
	/// </summary>
	[TokenAuthorize]
	public class AssigningAuthorityController : Controller
	{
		/// <summary>
		/// The internal reference to the <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient client;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Controllers.PolicyController"/> class.
		/// </summary>
		public AssigningAuthorityController()
		{
		}

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected override void Dispose(bool disposing)
		{
			Trace.TraceInformation("{0} disposing", nameof(AssigningAuthorityController));

			this.client?.Dispose();

			base.Dispose(disposing);
		}

		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "AssigningAuthority";
			return View();
		}

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateAssigningAuthorityModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var test = "";

                    var results = this.client.CreateAssigningAuthority(AssigningAuthorityUtil.ToCreateAssigningAuthorityModel(model));

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.TraceError("Unable to delete assigning authority: {0}", e.StackTrace);
#endif
                    Trace.TraceError("Unable to delete assigning authority: {0}", e.Message);
                }
            }

            TempData["error"] = "Assigning authority not found";
            return RedirectToAction("Index");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var restClient = new RestClientService(Constants.AMI);

			restClient.Accept = "application/xml";
			restClient.Credentials = new AmiCredentials(this.User, HttpContext.Request);

			this.client = new AmiServiceClient(restClient);

			base.OnActionExecuting(filterContext);
		}

		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
            IEnumerable<AssigningAuthorityViewModel> assigningAuthorities = new List<AssigningAuthorityViewModel>();
            try
			{
				if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
				{
					var collection = this.client.GetAssigningAuthorities(p => p.Name.Contains(searchTerm) && p.ObsoletionTime==null);
					TempData["searchTerm"] = searchTerm;

					return PartialView("_AssigningAuthoritySearchResultsPartial", collection.CollectionItem.Select(p => AssigningAuthorityUtil.ToAssigningAuthorityViewModel(p)));
				}
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to search assigning authorities: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to search assigning authorities: {0}", e.Message);
			}

			TempData["error"] = "Invalid search, please check your search criteria";
			TempData["searchTerm"] = searchTerm;

			return PartialView("_AssigningAuthoritySearchResultsPartial", assigningAuthorities);
		}



        [HttpGet]
        public ActionResult Delete(string key)
        {
            Guid assigningAuthorityKey = Guid.Empty;
            if(!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key) && Guid.TryParse(key, out assigningAuthorityKey))
            {
                try
                {
                    var assigningAuthority = this.client.GetAssigningAuthorities(m => m.Key == assigningAuthorityKey);
                    var singleAssigningAuthority = assigningAuthority.CollectionItem.SingleOrDefault();

                    singleAssigningAuthority.AssigningAuthority.ObsoletionTime = new DateTimeOffset(DateTime.Now);

                    this.client.UpdateAssigningAuthority(key, singleAssigningAuthority);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.TraceError("Unable to delete assigning authority: {0}", e.StackTrace);
#endif
                    Trace.TraceError("Unable to delete assigning authority: {0}", e.Message);
                }
            }

            TempData["error"] = "Assigning authority not found";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ViewAssigningAuthority(string key)
        {
            Guid assigningAuthorityKey = Guid.Empty;

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key) && Guid.TryParse(key, out assigningAuthorityKey))
            {
                try
                {
                    var assigningAuthority = this.client.GetAssigningAuthorities(m => m.Key==assigningAuthorityKey);

                    object model = null;

                    return View(assigningAuthority.CollectionItem.Select(p => AssigningAuthorityUtil.ToAssigningAuthorityViewModel(p)).SingleOrDefault());
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.TraceError("Unable to find assigning authority: {0}", e.StackTrace);
#endif
                    Trace.TraceError("Unable to find assigning authority: {0}", e.Message);
                }
            }

            TempData["error"] = "Assigning authority not found";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(string key)
        {
            Guid assigningAuthorityKey = Guid.Empty;

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key) && Guid.TryParse(key, out assigningAuthorityKey))
            {
                try
                {
                    var assigningAuthority = this.client.GetAssigningAuthorities(m => m.Key == assigningAuthorityKey);

                    object model = null;

                    return View(assigningAuthority.CollectionItem.Select(p => AssigningAuthorityUtil.ToEditAssigningAuthorityModel(p)).SingleOrDefault());
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.TraceError("Unable to find assigning authority: {0}", e.StackTrace);
#endif
                    Trace.TraceError("Unable to find assigning authority: {0}", e.Message);
                }
            }

            TempData["error"] = "Assigning authority not found";
            return RedirectToAction("Index");
        }
    }
}