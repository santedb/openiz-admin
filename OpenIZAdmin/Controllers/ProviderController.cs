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
 * Date: 2016-8-15
 */

using Elmah;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Roles;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ProviderModels;
using OpenIZAdmin.Models.ProviderModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for administering providers.
	/// </summary>
	[TokenAuthorize]
	public class ProviderController : BaseController
	{
		/// <summary>
		/// Displays the create provider view.
		/// </summary>
		/// <returns>Returns the create provider view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// Displays the create view.
		/// </summary>
		/// <param name="model">The model containing the new provider information.</param>
		/// <returns>Returns the Index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateProviderModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var provider = this.ImsiClient.Create<Provider>(ProviderUtil.ToProvider(model));

					TempData["success"] = Locale.Provider + " " + Locale.Created + " " + Locale.Successfully;
					//return RedirectToAction("ViewProvider", new { key = provider.Key, versionKey = provider.VersionKey });
					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
					ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				}
			}

			TempData["error"] = Locale.UnableToCreate + " " + Locale.Provider;
			return View(model);
		}

		/// <summary>
		/// Retrieves the provider entity by id and version key
		/// </summary>
		/// <param name="key">The provider identifier.</param>
		/// <param name="versionKey">The provider version identifier.</param>
		/// <returns>Returns the provider edit view.</returns>
		[HttpGet]
		public ActionResult Edit(string key, string versionKey)
		{
			Guid providerKey = Guid.Empty;
			Guid providerVersionKey = Guid.Empty;

			if (CommonUtil.IsValidString(key) && Guid.TryParse(key, out providerKey) && CommonUtil.IsValidString(versionKey) && Guid.TryParse(versionKey, out providerVersionKey))
			{
				var providerEntity = ProviderUtil.GetProviderEntity(this.ImsiClient, key, versionKey);

				if (providerEntity == null)
				{
					TempData["error"] = Locale.Provider + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				EditProviderModel model = ProviderUtil.ToEditProviderModel(providerEntity);

				return View(model);
			}

			TempData["error"] = Locale.Provider + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Updates a provider.
		/// </summary>
		/// <param name="model">The model containing the provider information.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditProviderModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var provider = this.ImsiClient.Update<Provider>(ProviderUtil.ToProvider(model));

					TempData["success"] = Locale.Provider + " " + Locale.Updated + " " + Locale.Successfully;
					return RedirectToAction("ViewProvider", new { key = provider.Key, versionKey = provider.VersionKey });
				}
				catch (Exception e)
				{
					ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				}
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Provider;
			return View(model);
		}

		/// <summary>
		/// Displays the Index view
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = Locale.Provider;
			return View();
		}

		/// <summary>
		/// Searches for a provider.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of providers which match the search term.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<ProviderViewModel> provider = new List<ProviderViewModel>();

			try
			{
				if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
				{
					var collection = this.ImsiClient.Query<Provider>(i => i.Names.Any(x => x.Component.Any(r => r.Value.Contains(searchTerm)) && i.ClassConceptKey == EntityClassKeys.Provider));

					TempData["searchTerm"] = searchTerm;

					return PartialView("_ProviderSearchResultsPartial", collection.Item.OfType<Provider>().Select(ProviderUtil.ToProviderViewModel));
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.InvalidSearch;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_ProviderSearchResultsPartial", provider);
		}

		/// <summary>
		/// Searches for a provider to view details.
		/// </summary>
		/// <param name="key">The provider identifier search string.</param>
		/// <param name="versionKey">The provider version identifier.</param>
		/// <returns>Returns a provider view that matches the search term.</returns>
		[HttpGet]
		public ActionResult ViewProvider(string key, string versionKey)
		{
			Guid providerKey = Guid.Empty;
			Guid providerVersionKey = Guid.Empty;

			if (CommonUtil.IsValidString(key) && Guid.TryParse(key, out providerKey) && CommonUtil.IsValidString(versionKey) && Guid.TryParse(versionKey, out providerVersionKey))
			{
				try
				{
					var provider = this.ImsiClient.Get<Provider>(providerKey, null);

					object model = null;

					return View(model);
				}
				catch (Exception e)
				{
					ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				}
			}

			TempData["error"] = Locale.Provider + " " + Locale.NotFound;
			return RedirectToAction("Index");
		}
	}
}