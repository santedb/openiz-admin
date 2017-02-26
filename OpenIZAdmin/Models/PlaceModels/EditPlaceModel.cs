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
 * User: khannan
 * Date: 2016-8-14
 */

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Models.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.PlaceModels
{
	/// <summary>
	/// Represents an edit place model.
	/// </summary>
	public class EditPlaceModel : EditEntityModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditPlaceModel"/> class.
		/// </summary>
		public EditPlaceModel()
		{
			this.RelatedPlaces = new List<RelatedPlaceModel>();
			this.RelatedPlaceKeys = new List<string>();
			this.RelatedPlacesList = new List<SelectListItem>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditPlaceModel"/> class
		/// with a specific <see cref="Place"/> instance.
		/// </summary>
		/// <param name="place">The <see cref="Place"/> instance.</param>
		public EditPlaceModel(Place place) : base(place)
		{
			this.Name = string.Join(" ", place.Names.SelectMany(n => n.Component).Select(c => c.Value));
		}

		/// <summary>
		/// Gets or sets the name of the place.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(64, ErrorMessageResourceName = "NameLength64", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the related places.
		/// </summary>
		public List<string> RelatedPlaceKeys { get; set; }

		/// <summary>
		/// Gets or sets the list of related place models.
		/// </summary>
		public List<RelatedPlaceModel> RelatedPlaces { get; set; }

		/// <summary>
		/// Gets or sets the related places select list.
		/// </summary>
		public List<SelectListItem> RelatedPlacesList { get; set; }

		/// <summary>
		/// Converts a <see cref="EditPlaceModel"/> instance to a <see cref="Place"/> instance.
		/// </summary>
		/// <param name="place">The <see cref="Place"/> instance.</param>
		/// <returns>Returns a <see cref="Place"/> instance.</returns>
		public Place ToPlace(Place place)
		{
			place.Names.RemoveAll(n => n.NameUseKey == NameUseKeys.OfficialRecord);
			place.Names.Add(new EntityName(NameUseKeys.OfficialRecord, this.Name));

			return place;
		}
	}
}