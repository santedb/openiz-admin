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
		/// Gets a place by key.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
		/// <param name="key">The key of the place to be retrieved.</param>
		/// <returns>Returns a place.</returns>
		public static Place GetPlace(ImsiServiceClient client, Guid key)
		{
			var bundle = client.Query<Place>(p => p.Key == key && p.ObsoletionTime == null);

			bundle.Reconstitute();

			var place = bundle.Item.OfType<Place>().FirstOrDefault();

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
			var bundle = client.Query<Place>(p => p.IsMobile == false && p.ObsoletionTime == null, offset, count);

			var places = bundle.Item.OfType<Place>();

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
			var place = new Place
			{
				Key = model.Key
				
			};

			place.Names.Add(new EntityName(NameUseKeys.OfficialRecord, model.Name));

			return place;
		}

		/// <summary>
		/// Converts a <see cref="Place"/> instance to a <see cref="EditPlaceModel"/> instance.
		/// </summary>
		/// <param name="place">The place instance to convert.</param>
		/// <returns>Returns an edit place model.</returns>
		public static EditPlaceModel ToEditPlaceModel(Place place)
		{
			var model = new EditPlaceModel
			{
				Key = place.Key.Value,
				Name = string.Join(", ", place.Names.SelectMany(e => e.Component).Select(c => c.Value))
			};

			return model;
		}

		/// <summary>
		/// Converts a <see cref="Place"/> instance to a <see cref="PlaceViewModel"/> instance.
		/// </summary>
		/// <param name="place">The place to convert.</param>
		/// <returns>Returns a place view model.</returns>
		public static PlaceViewModel ToPlaceViewModel(Place place)
		{
			var viewModel = new PlaceViewModel(place);


			var childPlaces = place.Relationships.Where(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Child)
									.Select(r => r.TargetEntity)
									.OfType<Place>()
									.Select(p => new RelatedPlaceModel(p));

			if (childPlaces.Any())
			{
				viewModel.RelatedPlaces.AddRange(childPlaces);
			}

			var parentPlaces = place.Relationships.Where(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent)
									.Select(r => r.TargetEntity)
									.OfType<Place>()
									.Select(p => new RelatedPlaceModel(p));

			if (parentPlaces.Any())
			{
				viewModel.RelatedPlaces.AddRange(parentPlaces);
			}

			return viewModel;
		}
	}
}