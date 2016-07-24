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
using System;
using System.Linq;

namespace OpenIZAdmin.Models.PlaceModels.ViewModels
{
	public class PlaceViewModel
	{
		public PlaceViewModel()
		{
		}

		public PlaceViewModel(Place place)
		{
			this.Key = place.Key.Value;
			this.Latitude = place.Lat ?? 0;
			this.Longitude = place.Lng ?? 0;
			this.Name = place.Names.SelectMany(e => e.Component).Select(c => c.Value).Aggregate((a, b) => (a + ", " + b));
			this.VersionKey = place.VersionKey.GetValueOrDefault();
		}

		public Guid Key { get; set; }

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		public string Name { get; set; }

		public Guid? VersionKey { get; set; }
	}
}