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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OpenIZ.Core.Model.Entities;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.PlaceModels
{
	/// <summary>
	/// Represents an edit place model.
	/// </summary>
	public class EditPlaceModel
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
		public EditPlaceModel(Place place) : this()
		{
			this.Key = place.Key.Value;
			this.Name = string.Join(" ", place.Names.SelectMany(n => n.Component).Select(c => c.Value));
		}

		[Required]
		public Guid Key { get; set; }

		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "NameTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the related places select list.
		/// </summary>
		public List<SelectListItem> RelatedPlacesList { get; set; }

		/// <summary>
		/// Gets or sets the related places.
		/// </summary>
		public List<string> RelatedPlaceKeys { get; set; }

		/// <summary>
		/// Gets or sets the list of related place models.
		/// </summary>
		public List<RelatedPlaceModel> RelatedPlaces { get; set; }
	}
}