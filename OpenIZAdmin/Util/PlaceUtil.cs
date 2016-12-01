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
 * Date: 2016-7-31
 */

using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Models.PlaceModels;
using OpenIZAdmin.Models.PlaceModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing places.
	/// </summary>
	public static class PlaceUtil
	{
		/// <summary>
		/// Gets a place by key.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
		/// <param name="key">The key of the place to be retrieved.</param>
		/// <returns>Returns a place.</returns>
		public static Place GetPlace(ImsiServiceClient client, Guid key)
		{
			Place place = null;

			try
			{
				var bundle = client.Query<Place>(p => p.Key == key);

				bundle.Reconstitute();

				place = bundle.Item.OfType<Place>().Cast<Place>().FirstOrDefault();
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve place: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve place: {0}", e.Message);
			}

			return place;
		}

		/// <summary>
		/// Gets a list of places from the IMS.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <returns>Returns a list of places.</returns>
		public static IEnumerable<Place> GetPlaces(ImsiServiceClient client, int offset = 0, int? count = null)
		{
			IEnumerable<Place> places = new List<Place>();

			try
			{
				var bundle = client.Query<Place>(p => p.IsMobile == false, offset, count);

				places = bundle.Item.OfType<Place>().Cast<Place>().AsEnumerable();
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve places: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve places: {0}", e.Message);
			}

			return places;
		}

		/// <summary>
		/// Converts a <see cref="CreatePlaceModel"/> instance to a <see cref="Place"/> instance.
		/// </summary>
		/// <param name="model">The create place model instance to convert.</param>
		/// <returns>Returns a place.</returns>
		public static Place ToPlace(CreatePlaceModel model)
		{
			var place = new Place
			{
				Key = Guid.NewGuid(),
				Lat = model.Latitude,
				Lng = model.Longitude
			};

			place.Names.Add(new EntityName(NameUseKeys.OfficialRecord, model.Name));

			return place;
		}

		/// <summary>
		/// Converts an <see cref="EditPlaceModel"/> instance to a <see cref="Place"/> instance.
		/// </summary>
		/// <param name="model">The edit place model instance to convert.</param>
		/// <returns>Returns a place.</returns>
		public static Place ToPlace(EditPlaceModel model)
		{
			Place place = new Place();

			place.Key = model.Id;
			place.Lat = model.Latitude;
			place.Lng = model.Longitude;
			place.Names.Add(new EntityName(NameUseKeys.OfficialRecord, model.Name));
			place.VersionKey = model.VersionId;
			place.StatusConceptKey = StatusKeys.Active;

			return place;
		}
		
		/// <summary>
		/// Converts a <see cref="Place"/> instance to a <see cref="EditPlaceModel"/> instance.
		/// </summary>
		/// <param name="place">The place instance to convert.</param>
		/// <returns>Returns an edit place model.</returns>
		public static EditPlaceModel ToEditPlaceModel(Place place)
		{
			EditPlaceModel model = new EditPlaceModel();

			model.Id = place.Key.Value;
			model.Latitude = place.Lat;
			model.Longitude = place.Lng;
			model.Name = place.Names.SelectMany(n => n.Component).Select(c => c.Value).Aggregate((a, b) => a + ", " + b);
			model.VersionId = place.VersionKey.Value;

			return model;
		}

		/// <summary>
		/// Converts a <see cref="Place"/> instance to a <see cref="PlaceViewModel"/> instance.
		/// </summary>
		/// <param name="place">The place to convert.</param>
		/// <returns>Returns a place view model.</returns>
		public static PlaceViewModel ToPlaceViewModel(Place place)
		{
			PlaceViewModel viewModel = new PlaceViewModel();

			viewModel.CreationTime = place.CreationTime.DateTime;

			viewModel.Details.Add(new DetailedPlaceViewModel
			{
				IsMobile = place.IsMobile,
				Services = new List<string>()
			});

			viewModel.Key = place.Key.Value;
			viewModel.Latitude = place.Lat ?? 0;
			viewModel.Longitude = place.Lng ?? 0;
			viewModel.Name = place.Names.SelectMany(e => e.Component).Select(c => c.Value).Aggregate((a, b) => (a + ", " + b));
			viewModel.VersionKey = place.VersionKey.GetValueOrDefault();

			return viewModel;
		}
	}
}