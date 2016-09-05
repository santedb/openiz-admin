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
		/// Gets a list of places from the IMS.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
		/// <returns>Returns a list of places.</returns>
		public static IEnumerable<Place> GetPlaces(ImsiServiceClient client)
		{
			IEnumerable<Place> places = new List<Place>();

			try
			{
				var bundle = client.Query<Place>(p => p.IsMobile == false);

				bundle.Reconstitute();

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
		/// Converts a <see cref="OpenIZAdmin.Models.PlaceModels.CreatePlaceModel"/> to a <see cref="OpenIZ.Core.Model.Entities.Place"/>.
		/// </summary>
		/// <param name="model">The create place model to convert.</param>
		/// <returns>Returns a place.</returns>
		public static Place ToPlace(CreatePlaceModel model)
		{
			Place place = new Place();

			place.Lat = model.Latitude;
			place.Lng = model.Longitude;
			place.Names.Add(new EntityName(NameUseKeys.OfficialRecord, model.Name));

			return place;
		}

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
		/// Converts a <see cref="OpenIZ.Core.Model.Entities.Place"/> to a <see cref="OpenIZAdmin.Models.PlaceModels.ViewModels.PlaceViewModel"/>.
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