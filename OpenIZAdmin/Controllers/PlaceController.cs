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
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models.PlaceModels;
using OpenIZAdmin.Models.PlaceModels.ViewModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	[TokenAuthorize]
    public class PlaceController : Controller
    {
		/// <summary>
		/// The internal reference to the <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient client;

		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreatePlaceModel model)
		{
			return View();
		}

		public ActionResult Index()
        {
			try
			{
				var places = this.client.GetPlaces(p => p.IsMobile == false);

				return View(places.CollectionItem.Select(p => new PlaceViewModel(p)).OrderBy(p => p.Name));
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve places: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve places: {0}", e.Message);
			}

			TempData["error"] = "Unable to retrieve place list";

            return View(new List<PlaceViewModel>());
        }

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var restClient = new RestClientService("AMI");

			restClient.Accept = "application/xml";
			restClient.Credentials = new AmiCredentials(this.User, HttpContext.Request);

			this.client = new AmiServiceClient(restClient);

			base.OnActionExecuting(filterContext);
		}

		[HttpGet]
		public ActionResult Search()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Search(SearchPlaceModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var places = this.client.GetPlaces(p => p.Names.SelectMany(n => n.Component).Any(c => c.Value == model.Name));

					return PartialView("_PlaceSearchResultsPartial", places.CollectionItem.Select(p => new PlaceViewModel(p)).OrderBy(p => p.Name));
				}
				catch (Exception e)
				{
#if DEBUG
					Trace.TraceError("Unable to retrieve places: {0}", e.StackTrace);
#endif
					Trace.TraceError("Unable to retrieve places: {0}", e.Message);
				}

				return PartialView("_PlaceSearchResultsPartial", new List<PlaceViewModel>());
			}

			return View(model);
		}

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

					var place = this.client.GetPlaces(QueryExpressionParser.BuildLinqExpression<Place>(new NameValueCollection(query.ToArray()))).CollectionItem.SingleOrDefault();

					if (place == null)
					{
						TempData["error"] = "Place not found";
						return RedirectToAction("Index");
					}

					return View(new PlaceViewModel(place));

				}
			}

			TempData["error"] = "Place not found";
			return RedirectToAction("Index");
		}
    }
}
