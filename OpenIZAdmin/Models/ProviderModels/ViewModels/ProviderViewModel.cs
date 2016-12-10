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
 * Date: 2016-8-15
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.ProviderModels.ViewModels
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ProviderViewModel"/> class.
	/// </summary>
	public class ProviderViewModel
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public ProviderViewModel()
		{
		}

		/// <summary>
		/// Gets or sets the creation date/time of the provider.
		/// </summary>
		[Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
		public DateTimeOffset? CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the key (GUID) of the provider.
		/// </summary>
		[Display(Name = "Key", ResourceType = typeof(Localization.Locale))]
		public Guid? Key { get; set; }

		/// <summary>
		/// Gets or sets the name of the provider.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the specialty of the provider.
		/// </summary>
		[Display(Name = "ProviderSpecialty", ResourceType = typeof(Localization.Locale))]
		public string ProviderSpecialty { get; set; }

		/// <summary>
		/// Gets or sets the user id of the provider.
		/// </summary>
		[Display(Name = "UserId", ResourceType = typeof(Localization.Locale))]
		public string UserId { get; set; }

		/// <summary>
		/// Gets or sets the username of the provider.
		/// </summary>
		[Display(Name = "Username", ResourceType = typeof(Localization.Locale))]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the Version key (GUID) of the provider.
		/// </summary>
		[Display(Name = "VersionKey", ResourceType = typeof(Localization.Locale))]
		public Guid? VersionKey { get; set; }
	}
}