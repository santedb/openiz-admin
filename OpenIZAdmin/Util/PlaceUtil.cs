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
using OpenIZAdmin.Models.PlaceModels;
using OpenIZAdmin.Models.PlaceModels.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Util
{
	public static class PlaceUtil
	{
		public static Place ToPlace(CreatePlaceModel model)
		{
			Place place = new Place();

			place.Lat = model.Latitude;
			place.Lng = model.Longitude;
			place.Names.Add(new EntityName(NameUseKeys.OfficialRecord, model.Name));

			return place;
		}

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