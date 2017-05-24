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
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Models.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OpenIZ.Core.Extensions;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenIZAdmin.Models.PlaceModels
{
	/// <summary>
	/// Represents a place view model.
	/// </summary>
	public class PlaceViewModel : EntityViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlaceViewModel"/> class.
		/// </summary>
		public PlaceViewModel()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlaceViewModel"/> class.
		/// </summary>
		/// <param name="place">The place.</param>
		public PlaceViewModel(Place place) : base(place)
		{
			this.IsServiceDeliveryLocation = place.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation ? Locale.Yes : Locale.No;

			if (place.Extensions.Any(e => e.ExtensionTypeKey  == Constants.TargetPopulationExtensionTypeKey))
			{
				try
				{
					var entityExtension = place.Extensions.First(e => e.ExtensionTypeKey == Constants.TargetPopulationExtensionTypeKey);

					entityExtension.ExtensionType = new ExtensionType(Constants.TargetPopulationUrl, typeof(DictionaryExtensionHandler))
					{
						Key = Constants.TargetPopulationExtensionTypeKey
					};
					var targetPopulation = JsonConvert.DeserializeObject<TargetPopulation>(Encoding.UTF8.GetString(entityExtension.ExtensionValueXml));

					this.TargetPopulation = targetPopulation?.Value ?? 0;
					this.TargetPopulationYear = targetPopulation?.Year ?? 0;
				}
				catch (Exception e)
				{
					Trace.TraceError($"Unable to de-serialize the target population extensions: { e }");
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is service delivery location.
		/// </summary>
		/// <value><c>true</c> if this instance is service delivery location; otherwise, <c>false</c>.</value>
		[Display(Name = "IsServiceDeliveryLocation", ResourceType = typeof(Locale))]
		public string IsServiceDeliveryLocation { get; set; }

		/// <summary>
		/// Gets or sets the target population year.
		/// </summary>
		/// <value>The target population year.</value>
		[Display(Name = "PopulationYear", ResourceType = typeof(Locale))]
		public int TargetPopulationYear { get; set; }

		/// <summary>
		/// Gets or sets the target population.
		/// </summary>
		/// <value>The target population.</value>
		[Display(Name = "TargetPopulation", ResourceType = typeof(Locale))]
		public ulong TargetPopulation { get; set; }
	}
}