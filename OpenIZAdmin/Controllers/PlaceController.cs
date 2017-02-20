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
 * Date: 2016-7-23
 */

using Elmah;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.PlaceModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing places.
	/// </summary>
	[TokenAuthorize]
	public class PlaceController : BaseController
	{
		/// <summary>
		/// Displays the create place view.
		/// </summary>
		/// <returns>Returns the create place view.</returns>
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
					var place = this.ImsiClient.Create<Place>(model.ToPlace());

					TempData["success"] = Locale.Place + " " + Locale.Successfully + " " + Locale.Created;

					return RedirectToAction("ViewPlace", new { id = place.Key });
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
		/// Displays the create place view.
		/// </summary>
		/// <param name="id">The id of the place to delete.</param>
		/// <returns>Returns the create place view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var place = this.ImsiClient.Get<Place>(id, null) as Place;

				if (place == null)
				{
					TempData["error"] = Locale.Place + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				this.ImsiClient.Obsolete<Place>(place);

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToDelete + " " + Locale.Place;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Gets a place by id.
		/// </summary>
		/// <param name="id">The id of the place to retrieve.</param>
		/// <returns>Returns the place edit view.</returns>
		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			try
			{
				var bundle = this.ImsiClient.Query<Place>(p => p.Key == id && p.ObsoletionTime == null, 0, null, true);

				bundle.Reconstitute();

				var place = bundle.Item.OfType<Place>().FirstOrDefault(p => p.Key == id && p.ObsoletionTime == null);

				if (place == null)
				{
					TempData["error"] = Locale.Place + " " + Locale.NotFound;
					return RedirectToAction("Index");
				}

				return View(new EditPlaceModel(place));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
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
			try
			{
				if (ModelState.IsValid)
				{
					var bundle = this.ImsiClient.Query<Place>(p => p.Key == model.Id && p.ClassConceptKey == EntityClassKeys.Place && p.ObsoletionTime == null, 0, null, true);

					bundle.Reconstitute();

					var place = bundle.Item.OfType<Place>().FirstOrDefault(p => p.Key == model.Id && p.ClassConceptKey == EntityClassKeys.Place && p.ObsoletionTime == null);

					if (place == null)
					{
						TempData["error"] = Locale.Place + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					var updatedPlace = this.ImsiClient.Update<Place>(model.ToPlace(place));

					TempData["success"] = Locale.Place + " " + Locale.Successfully + " " + Locale.Updated;

					return RedirectToAction("ViewPlace", new { id = updatedPlace.Key });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
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
			var viewModels = new List<PlaceViewModel>();

			if (ModelState.IsValid)
			{
				searchTerm = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(searchTerm);

				var bundle = this.ImsiClient.Query<Place>(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.ObsoletionTime == null);

				viewModels = bundle.Item.OfType<Place>().Select(p => new PlaceViewModel(p)).OrderBy(p => p.Name).ToList();
			}

			return PartialView("_PlaceSearchResultsPartial", viewModels);
		}

		/// <summary>
		/// Searches for a place.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of places which match the search term.</returns>
		[HttpGet]
		public ActionResult SearchAjax(string searchTerm)
		{
			var viewModels = new List<PlaceViewModel>();

			if (ModelState.IsValid)
			{
				searchTerm = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(searchTerm);
				var places = this.ImsiClient.Query<Place>(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.ObsoletionTime == null && p.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation);

				viewModels = places.Item.OfType<Place>().Select(p => new PlaceViewModel(p)).OrderBy(p => p.Name).ToList();
			}

			return Json(viewModels, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Searches for a place to view details.
		/// </summary>
		/// <param name="id">The place identifier search string.</param>
		/// <returns>Returns a place view that matches the search term.</returns>
		[HttpGet]
		public ActionResult ViewPlace(Guid id)
		{
			try
			{
				var bundle = this.ImsiClient.Query<Place>(p => p.Key == id, 0, null, true);

				bundle.Reconstitute();

				var place = bundle.Item.OfType<Place>().FirstOrDefault(p => p.Key == id);

				if (place == null)
				{
					TempData["error"] = Locale.Place + " " + Locale.NotFound;
					return RedirectToAction("Index");
				}

				return View(new PlaceViewModel(place));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.Place + " " + Locale.NotFound;
			return RedirectToAction("Index");
		}
	}
}