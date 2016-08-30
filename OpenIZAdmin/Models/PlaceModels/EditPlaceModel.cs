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
 * User: khannan
 * Date: 2016-8-14
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.PlaceModels
{
	public class EditPlaceModel
	{
		public EditPlaceModel()
		{
		}

		[Required]
		public Guid Id { get; set; }

		[Display(Name = "Latitude", ResourceType = typeof(Localization.Locale))]
		public double? Latitude { get; set; }

		[Display(Name = "Longitude", ResourceType = typeof(Localization.Locale))]
		public double? Longitude { get; set; }

		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[StringLength(255, ErrorMessageResourceName = "NameTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		[Required]
		public Guid VersionId { get; set; }
	}
}