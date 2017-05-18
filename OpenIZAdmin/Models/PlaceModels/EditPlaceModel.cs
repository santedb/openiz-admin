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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using OpenIZ.Core.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core.Serialization;

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
			    try
			    {
                    var extension = place.Extensions.First(e => e.ExtensionTypeKey == Constants.TargetPopulationExtensionTypeKey);
                    var data = JsonConvert.DeserializeObject<TargetPopulation>(Encoding.UTF8.GetString(extension.ExtensionValueXml));
                    this.TargetPopulation = data.Value;
                    this.Year = data.Year.ToString();
                }
			    catch (Exception e)
			    {
			        Console.WriteLine(e);
                    Trace.TraceError($"Unable to retrieve place.Extensions: { e }");                    
                }				
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
		//[Required(ErrorMessageResourceName = "TargetPopulationRequired", ErrorMessageResourceType = typeof(Locale))]
		[Range(1, ulong.MaxValue, ErrorMessageResourceName = "TargetPopulationMustBePositive", ErrorMessageResourceType = typeof(Locale))]
		public ulong? TargetPopulation { get; set; }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>The year.</value>
        [Display(Name = "PopulationYear", ResourceType = typeof(Locale))]
        //[Required(ErrorMessageResourceName = "YearRequired", ErrorMessageResourceType = typeof(Locale))]
        public string Year { get; set; }

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
			place.ClassConceptKey = this.IsServiceDeliveryLocation ? EntityClassKeys.ServiceDeliveryLocation : EntityClassKeys.Place;
			place.CreationTime = DateTimeOffset.Now;
			place.Extensions.RemoveAll(e => e.ExtensionType.Name == Constants.TargetPopulationUrl);
			place.Names.RemoveAll(n => n.NameUseKey == NameUseKeys.OfficialRecord);
			place.Names.Add(new EntityName(NameUseKeys.OfficialRecord, this.Name));
			place.TypeConceptKey = Guid.Parse(this.TypeConcept);
			place.VersionKey = null;

			return place;
		}

        /// <summary>
        /// Converts the string year to an int
        /// </summary>
        /// <returns>Returns the year as an int or 0 if unsuccessful.</returns>
        public ulong ConvertPopulationToULong()
        {
            if (TargetPopulation == null) return 0;

            return (ulong)TargetPopulation;
        }

        /// <summary>
        /// Converts the string year to an int
        /// </summary>
        /// <returns>Returns the year as an int or 0 if unsuccessful.</returns>
        public int ConvertToPopulationYear()
        {
            //if (string.IsNullOrWhiteSpace(Year)) return 0;

            int year;

            if (int.TryParse(Year, out year) && (year >= 1900 && year <= 2100)) return year;

            return 0;
        }

        /// <summary>
        /// Checks if year and population are entered
        /// </summary>
        /// <returns>Returns true if both contain entries or both are empty.</returns>
        public bool HasOnlyYearOrPopulation()
        {
            if (string.IsNullOrWhiteSpace(Year) && TargetPopulation != null) return true;

            return !string.IsNullOrWhiteSpace(Year) && TargetPopulation == null;
        }

        /// <summary>
        /// Checks if year and population are entered
        /// </summary>
        /// <returns>Returns true if both contain entries.</returns>
        public bool SubmitYearAndPopulation()
        {
            if (TargetPopulation == null) return false;

            return !string.IsNullOrWhiteSpace(Year) && TargetPopulation > 0;
        }
    }
}