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
 * Date: 2016-8-15
 */

using OpenIZ.Core.Model.Roles;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models.ProviderModels;
using OpenIZAdmin.Models.ProviderModels.ViewModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using System.Linq;
using OpenIZAdmin.Localization;
using OpenIZ.Messaging.AMI.Client;

namespace OpenIZAdmin.Controllers
{
	[TokenAuthorize]
	public class ProviderController : Controller
	{
        /// <summary>
		/// The internal reference to the <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient amiClient;

        /// <summary>
        /// The internal reference to the <see cref="ImsiServiceClient"/> instance.
        /// </summary>
        private ImsiServiceClient imsiClient;

        [HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateProviderModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var provider = this.imsiClient.Create<Provider>(ProviderUtil.ToProvider(model));

					TempData["success"] = "Provider created successfully";
					return RedirectToAction("ViewProvider", new { key = provider.Key, versionKey = provider.VersionKey });
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to create provider: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to create provider: {0}", e.Message);
				}
			}

			TempData["error"] = "Unable to create provider";
			return View(model);
		}

		[HttpGet]
		public ActionResult Edit(string key, string versionKey)
		{
            Guid providerKey = Guid.Empty;
            Guid providerVersionKey = Guid.Empty;

            //if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key) && Guid.TryParse(key, out userId))
            if (ProviderUtil.IsValidString(key) && Guid.TryParse(key, out providerKey) && ProviderUtil.IsValidString(versionKey) && Guid.TryParse(versionKey, out providerVersionKey))
            {
                var providerEntity = ProviderUtil.GetProviderEntity(this.imsiClient, key, versionKey);

                if (providerEntity == null)
                {
                    TempData["error"] = Locale.UserNotFound;

                    return RedirectToAction("Index");
                }

                EditProviderModel model = ProviderUtil.ToEditProviderModel(providerEntity);

                //model.FacilityList.Add(new SelectListItem { Text = "", Value = "" });
                //model.FacilityList.AddRange(PlaceUtil.GetPlaces(this.imsiClient).Select(p => new SelectListItem { Text = string.Join(" ", p.Names.SelectMany(n => n.Component).Select(c => c.Value)), Value = p.Key.ToString() }));

                //model.FacilityList = model.FacilityList.OrderBy(p => p.Text).ToList();

                //model.FamilyNameList.AddRange(model.FamilyNames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));
                //model.GivenNamesList.AddRange(model.GivenNames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));

                //model.RolesList.Add(new SelectListItem { Text = "", Value = "" });
                //model.RolesList.AddRange(RoleUtil.GetAllRoles(this.amiClient).Select(r => new SelectListItem { Text = r.Name, Value = r.Id.ToString() }));

                return View(model);
            }

            TempData["error"] = Locale.UserNotFound;

            return RedirectToAction("Index");
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditProviderModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var provider = this.imsiClient.Update<Provider>(ProviderUtil.ToProvider(model));

					TempData["success"] = "Provider updated successfully";
					return RedirectToAction("ViewProvider", new { key = provider.Key, versionKey = provider.VersionKey });
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to update provider: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to update provider: {0}", e.Message);
				}
			}

			TempData["error"] = "Unable to update provider";
			return View(model);
		}

		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "Provider";
			return View();
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var restClient = new RestClientService("IMSI");

			restClient.Accept = "application/xml";
			restClient.Credentials = new ImsCredentials(this.User, HttpContext.Request);

			this.imsiClient = new ImsiServiceClient(restClient);

			base.OnActionExecuting(filterContext);
		}

		[HttpGet]        
        public ActionResult ViewProvider(string key, string versionKey)
		{
			Guid providerKey = Guid.Empty;
			Guid providerVersionKey = Guid.Empty;

			//if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key) && Guid.TryParse(key, out providerKey) &&
			//	!string.IsNullOrEmpty(versionKey) && !string.IsNullOrWhiteSpace(versionKey) && Guid.TryParse(versionKey, out providerVersionKey))
            if (ProviderUtil.IsValidString(key) && Guid.TryParse(key, out providerKey) && ProviderUtil.IsValidString(versionKey) && Guid.TryParse(versionKey, out providerVersionKey))
                {
				try
				{
					//var provider = this.imsiClient.Get<Provider>(providerKey, providerVersionKey);
                    var provider = this.imsiClient.Get<Provider>(providerKey, null);

                    object model = null;

					return View(model);
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to view provider: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to view provider: {0}", e.Message);
				}
			}

			TempData["error"] = "Provider not found";
			return RedirectToAction("Index");
		}


        [HttpGet]
        public ActionResult Search(string searchTerm)
        {
            IEnumerable<ProviderViewModel> provider = new List<ProviderViewModel>();

            try
            {
                if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
                {                                        
                    var collection = this.imsiClient.Query<Provider>(i => i.Names.Any(x => x.Component.Any(r => r.Value.Contains(searchTerm))));                    

                    TempData["searchTerm"] = searchTerm;

                    return PartialView("_ProviderSearchResultsPartial", collection.Item.OfType<Provider>().Select(u => ProviderUtil.ToProviderViewModel(u)));                    
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.TraceError("Unable to search Providers: {0}", e.StackTrace);
#endif
                Trace.TraceError("Unable to search Providers: {0}", e.Message);
            }

            TempData["error"] = "Invalid search, please check your search criteria";
            TempData["searchTerm"] = searchTerm;

            return PartialView("_ProviderSearchResultsPartial", provider);
        }
        

    }
}