﻿/*
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

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.PlaceModels
{
    /// <summary>
    /// Class CreatePlaceModel.
    /// </summary>
    public class CreatePlaceModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePlaceModel"/> class.
        /// </summary>
        public CreatePlaceModel()
        {
            this.TypeConcepts = new List<SelectListItem>();
            this.Address = new EditEntityAddressViewModel();
            this.ClassConcept = EntityClassKeys.Place.ToString();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is service delivery location.
        /// </summary>
        /// <value><c>true</c> if this instance is service delivery location; otherwise, <c>false</c>.</value>
        [Display(Name = "IsServiceDeliveryLocation", ResourceType = typeof(Locale))]
        public bool IsServiceDeliveryLocation { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Display(Name = "Name", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(64, ErrorMessageResourceName = "NameLength64", ErrorMessageResourceType = typeof(Locale))]
        [RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the address of the place
        /// </summary>
        public EditEntityAddressViewModel Address { get; set; }

        /// <summary>
        /// Gets or sets the target population.
        /// </summary>
        /// <value>The target population.</value>
        [Display(Name = "TargetPopulation", ResourceType = typeof(Locale))]
        [Range(1, ulong.MaxValue, ErrorMessageResourceName = "TargetPopulationMustBePositive", ErrorMessageResourceType = typeof(Locale))]
        public ulong? TargetPopulation { get; set; }

        /// <summary>
        /// Gets or sets the type concept.
        /// </summary>
        /// <value>The type concept.</value>
        [Required(ErrorMessageResourceName = "TypeConceptRequired", ErrorMessageResourceType = typeof(Locale))]
        [Display(Name = "TypeConcept", ResourceType = typeof(Locale))]
        public string TypeConcept { get; set; }

        /// <summary>
		/// Gets or sets the class concept.
		/// </summary>
		[Required(ErrorMessageResourceName = "ClassConceptRequired", ErrorMessageResourceType = typeof(Locale))]
        [Display(Name = "ClassConcept", ResourceType = typeof(Locale))]
        public string ClassConcept { get; set; }

        /// <summary>
        /// Gets or sets the type concepts.
        /// </summary>
        /// <value>The type concepts.</value>
        public List<SelectListItem> TypeConcepts { get; set; }

        /// <summary>
        /// Classification concepts
        /// </summary>
        public List<SelectListItem> ClassConcepts { get; set; }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>The year.</value>
        [Display(Name = "PopulationYear", ResourceType = typeof(Locale))]
        public string Year { get; set; }

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

        /// <summary>
        /// Converts a <see cref="CreatePlaceModel"/> instance to a <see cref="Place"/> instance.
        /// </summary>
        /// <returns>Returns the converted place instance.</returns>
        public Place ToPlace()
        {
            var place = new Place
            {
                ClassConceptKey = this.IsServiceDeliveryLocation ? EntityClassKeys.ServiceDeliveryLocation : Guid.Parse(this.ClassConcept),
                Key = Guid.NewGuid(),
                Names = new List<EntityName>
                {
                    new EntityName(NameUseKeys.OfficialRecord, this.Name)
                },
                StatusConceptKey = StatusKeys.Active,
                Addresses = new List<EntityAddress>() { Address.Address.ToEntityAddress() }
            };

            Guid typeConceptKey;

            if (Guid.TryParse(this.TypeConcept, out typeConceptKey))
            {
                place.TypeConceptKey = typeConceptKey;
            }

            return place;
        }
    }
}