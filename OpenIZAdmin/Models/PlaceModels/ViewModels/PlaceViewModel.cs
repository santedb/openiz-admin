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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;

namespace OpenIZAdmin.Models.PlaceModels.ViewModels
{
	/// <summary>
	/// Represents a place view model.
	/// </summary>
	public class PlaceViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlaceViewModel"/> class.
		/// </summary>
		public PlaceViewModel()
		{
			this.RelatedPlaces = new List<RelatedPlaceModel>();
		}

		public PlaceViewModel(Place place) : this()
		{
			this.CreationTime = place.CreationTime.DateTime;
			this.Key = place.Key.Value;
			this.Name = string.Join(", ", place.Names.SelectMany(e => e.Component).Select(c => c.Value));

			if (place.TypeConcept != null)
			{
				this.Type = string.Join(" ", place.TypeConcept.ConceptNames.Select(c => c.Name));
			}

			var childPlaces = place.Relationships.Where(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Child)
						.Select(r => r.TargetEntity)
						.OfType<Place>()
						.Select(p => new RelatedPlaceModel(p));

			if (childPlaces.Any())
			{
				this.RelatedPlaces.AddRange(childPlaces);
			}

			var parentPlaces = place.Relationships.Where(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent)
									.Select(r => r.TargetEntity)
									.OfType<Place>()
									.Select(p => new RelatedPlaceModel(p));

			if (parentPlaces.Any())
			{
				this.RelatedPlaces.AddRange(parentPlaces);
			}
		}

		/// <summary>
		/// Gets or sets the creation time of the place.
		/// </summary>
		[Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Gets or sets a list of places related to the place.
		/// </summary>
		public List<RelatedPlaceModel> RelatedPlaces { get; set; }

		/// <summary>
		/// Gets or sets key of the place.
		/// </summary>
		public Guid Key { get; set; }

		/// <summary>
		/// Gets or sets the name of the place.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type of the place.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the verion key of the place.
		/// </summary>
		public Guid? VersionKey { get; set; }
	}
}