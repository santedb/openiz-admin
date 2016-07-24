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

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.PlaceModels
{
	public class CreatePlaceModel
	{
		public CreatePlaceModel()
		{
		}

		[Display(Name = "Latitude", ResourceType = typeof(Localization.Resources))]
		public double? Latitude { get; set; }

		[Display(Name = "Longitude", ResourceType = typeof(Localization.Resources))]
		public double? Longitude { get; set; }

		[Display(Name = "Name", ResourceType = typeof(Localization.Resources))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Resources))]
		[StringLength(255, ErrorMessageResourceName = "NameTooLong", ErrorMessageResourceType = typeof(Localization.Resources))]
		public string Name { get; set; }

		public Place ToPlace()
		{
			Place place = new Place();

			place.Lat = this.Latitude;
			place.Lng = this.Longitude;
			place.Names.Add(new EntityName(NameUseKeys.OfficialRecord, this.Name));

			return place;
		}
	}
}