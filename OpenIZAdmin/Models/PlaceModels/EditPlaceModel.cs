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
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Models.Core;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;

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
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditPlaceModel"/> class
		/// with a specific <see cref="Place"/> instance.
		/// </summary>
		/// <param name="place">The <see cref="Place"/> instance.</param>
		public EditPlaceModel(Place place) : base(place)
		{
			this.IsServiceDeliveryLocation = place.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation;
			this.Name = string.Join(" ", place.Names.SelectMany(n => n.Component).Select(c => c.Value));

			if (place.Extensions.Any(e => e.ExtensionTypeKey == Constants.TargetPopulationExtensionTypeKey))
			{
				this.TargetPopulation = BitConverter.ToInt64(place.Extensions.First(e => e.ExtensionTypeKey == Constants.TargetPopulationExtensionTypeKey).ExtensionValueXml, 0);
			}

			this.TypeConcepts = new List<SelectListItem>();
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is service delivery location.
		/// </summary>
		/// <value><c>true</c> if this instance is service delivery location; otherwise, <c>false</c>.</value>
		[Display(Name = "IsServiceDeliveryLocation", ResourceType = typeof(Locale))]
		public bool IsServiceDeliveryLocation { get; set; }

		/// <summary>
		/// Gets or sets the name of the place.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(64, ErrorMessageResourceName = "NameLength64", ErrorMessageResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the target population.
		/// </summary>
		/// <value>The target population.</value>
		[Display(Name = "TargetPopulation", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "TargetPopulation", ErrorMessageResourceType = typeof(Locale))]
		public long TargetPopulation { get; set; }

		/// <summary>
		/// Gets or sets the type concept.
		/// </summary>
		/// <value>The type concept.</value>
		[Display(Name = "TypeConcept", ResourceType = typeof(Locale))]
		public string TypeConcept { get; set; }

		/// <summary>
		/// Gets or sets the type concepts.
		/// </summary>
		/// <value>The type concepts.</value>
		public List<SelectListItem> TypeConcepts { get; set; }

		/// <summary>
		/// Converts a <see cref="EditPlaceModel"/> instance to a <see cref="Place"/> instance.
		/// </summary>
		/// <param name="place">The <see cref="Place"/> instance.</param>
		/// <returns>Returns a <see cref="Place"/> instance.</returns>
		public Place ToPlace(Place place)
		{
			place.CreationTime = DateTimeOffset.Now;
			place.Extensions.RemoveAll(e => e.ExtensionType.Name == Constants.TargetPopulationUrl);
			place.Names.RemoveAll(n => n.NameUseKey == NameUseKeys.OfficialRecord);
			place.Names.Add(new EntityName(NameUseKeys.OfficialRecord, this.Name));
			place.TypeConceptKey = Guid.Parse(this.TypeConcept);
			place.VersionKey = null;

			return place;
		}
	}
}