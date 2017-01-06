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
 * Date: 2016-7-23
 */

using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Query;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.PlaceModels;
using OpenIZAdmin.Models.PlaceModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Elmah;
using OpenIZ.Core.Model.Constants;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing places.
	/// </summary>
	[TokenAuthorize]
	public class PlaceController : BaseController
	{
		/// <summary>
		/// Displays the create policy view.
		/// </summary>
		/// <returns>Returns the create policy view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// Creates a place.
		/// </summary>
		/// <param name="model">The model containing the information about the place.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreatePlaceModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var place = this.ImsiClient.Create<Place>(PlaceUtil.ToPlace(model));

					return RedirectToAction("ViewPlace", new { key = place.Key, versionKey = place.VersionKey });
				}
				catch (Exception e)
				{
					ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				}
			}

			TempData["error"] = Locale.UnableToCreate + " " + Locale.Place;
			return View(model);
		}

        /// <summary>
        /// Displays the create policy view.
        /// </summary>
        /// <returns>Returns the create policy view.</returns>
        [HttpGet]
        public ActionResult Delete(string key, string versionKey)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
            {
                Guid placeId = Guid.Empty;
                Guid placeVersion = Guid.Empty;

                if (Guid.TryParse(key, out placeId) && Guid.TryParse(versionKey, out placeVersion))
                {
                    List<KeyValuePair<string, object>> query = new List<KeyValuePair<string, object>>();

                    if (placeVersion != Guid.Empty)
                    {
                        query.AddRange(QueryExpressionBuilder.BuildQuery<Place>(c => c.Key == placeId && c.VersionKey == placeVersion));
                    }
                    else
                    {
                        query.AddRange(QueryExpressionBuilder.BuildQuery<Place>(c => c.Key == placeId));
                    }

                    var place = this.ImsiClient.Query<Place>(QueryExpressionParser.BuildLinqExpression<Place>(new NameValueCollection(query.ToArray()))).Item.OfType<Place>().FirstOrDefault();

                    if (place == null)
                    {
                        TempData["error"] = Locale.Place + " " + Locale.NotFound;
                        return RedirectToAction("Index");
                    }

                    this.ImsiClient.Obsolete<Place>(place);

                    return View("Index");
                }
            }
            return View("Index");
        }

        /// <summary>
        /// Retrieves the place entity by id and version key
        /// </summary>
        /// <param name="key">The place identifier.</param>
        /// <param name="versionKey">The place version identifier.</param>
        /// <returns>Returns the place edit view.</returns>
        [HttpGet]
		public ActionResult Edit(string key, string versionKey)
		{
			if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
			{
				Guid placeId = Guid.Empty;
				Guid placeVersion = Guid.Empty;

				if (Guid.TryParse(key, out placeId) && Guid.TryParse(versionKey, out placeVersion))
				{
					List<KeyValuePair<string, object>> query = new List<KeyValuePair<string, object>>();

					if (placeVersion != Guid.Empty)
					{
						query.AddRange(QueryExpressionBuilder.BuildQuery<Place>(c => c.Key == placeId && c.VersionKey == placeVersion));
					}
					else
					{
						query.AddRange(QueryExpressionBuilder.BuildQuery<Place>(c => c.Key == placeId));
					}

					var place = this.ImsiClient.Query<Place>(QueryExpressionParser.BuildLinqExpression<Place>(new NameValueCollection(query.ToArray()))).Item.OfType<Place>().FirstOrDefault();

					if (place == null)
					{
						TempData["error"] = Locale.Place + " " + Locale.NotFound;
						return RedirectToAction("Index");
					}

					return View(PlaceUtil.ToEditPlaceModel(place));
				}
			}

			TempData["error"] = Locale.Place + " " + Locale.NotFound;
			return RedirectToAction("Index");
		}

		/// <summary>
		/// Updates a place.
		/// </summary>
		/// <param name="model">The model containing the place information.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditPlaceModel model)
		{
			if (ModelState.IsValid)
			{
				var place = this.ImsiClient.Update<Place>(PlaceUtil.ToPlace(model));

				return RedirectToAction("ViewPlace", new { key = place.Key, versionKey = place.VersionKey });
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Place;
			return View(model);
		}

		public ActionResult Index()
		{
			TempData["searchType"] = "Place";

			return View(new List<PlaceViewModel>());
		}

		/// <summary>
		/// Searches for a place.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of places which match the search term.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			var placeList = new List<PlaceViewModel>();

			if (ModelState.IsValid)
			{
				var places = this.ImsiClient.Query<Place>(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.ObsoletionTime == null);

				placeList = places.Item.OfType<Place>().Select(PlaceUtil.ToPlaceViewModel).OrderBy(p => p.Name).ToList();
			}

			return PartialView("_PlaceSearchResultsPartial", placeList);
		}

		/// <summary>
		/// Searches for a place.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of places which match the search term.</returns>
		[HttpGet]
		public ActionResult SearchAjax(string searchTerm)
		{
			var placeList = new List<PlaceViewModel>();

			if (ModelState.IsValid)
			{
				var places = this.ImsiClient.Query<Place>(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.ObsoletionTime==null);

				placeList = places.Item.OfType<Place>().Select(PlaceUtil.ToPlaceViewModel).OrderBy(p => p.Name).ToList();
			}

			return Json(placeList, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Searches for a place to view details.
		/// </summary>
		/// <param name="key">The place identifier search string.</param>
		/// <param name="versionKey">The place version identifier.</param>
		/// <returns>Returns a place view that matches the search term.</returns>
		[HttpGet]
		public ActionResult ViewPlace(string key, string versionKey)
		{
			if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
			{
				Guid placeId = Guid.Empty;
				Guid placeVersion = Guid.Empty;

				if (Guid.TryParse(key, out placeId) && Guid.TryParse(versionKey, out placeVersion))
				{
					List<KeyValuePair<string, object>> query = new List<KeyValuePair<string, object>>();

					if (placeVersion != Guid.Empty)
					{
						query.AddRange(QueryExpressionBuilder.BuildQuery<Place>(c => c.Key == placeId && c.VersionKey == placeVersion));
					}
					else
					{
						query.AddRange(QueryExpressionBuilder.BuildQuery<Place>(c => c.Key == placeId));
					}

					var place = this.ImsiClient.Query<Place>(QueryExpressionParser.BuildLinqExpression<Place>(new NameValueCollection(query.ToArray()))).Item.OfType<Place>().FirstOrDefault();

					if (place == null)
					{
						TempData["error"] = Locale.Place + " " + Locale.NotFound;
						return RedirectToAction("Index");
					}

					return View(PlaceUtil.ToPlaceViewModel(place));
				}
			}

			TempData["error"] = Locale.Place + " " + Locale.NotFound;
			return RedirectToAction("Index");
		}
	}
}