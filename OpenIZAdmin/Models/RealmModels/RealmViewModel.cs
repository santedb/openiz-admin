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
 * Date: 2016-7-15
 */

using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.RealmModels
{
	/// <summary>
	/// Represents a realm view model.
	/// </summary>
	public class RealmViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RealmViewModel"/> class.
		/// </summary>
		public RealmViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RealmViewModel"/> class.
		/// </summary>
		/// <param name="realm">The realm.</param>
		public RealmViewModel(Realm realm)
		{
			this.Map(realm);
		}

		/// <summary>
		/// Gets or sets the address.
		/// </summary>
		/// <value>The address.</value>
		[Display(Name = "Address", ResourceType = typeof(Locale))]
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets the application identifier.
		/// </summary>
		/// <value>The application identifier.</value>
		[Display(Name = "Application Id")]
		public string ApplicationId { get; set; }

		/// <summary>
		/// Gets or sets the creation time.
		/// </summary>
		/// <value>The creation time.</value>
		[Display(Name = "CreationTime", ResourceType = typeof(Locale))]
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public Guid Id { get; set; }
	}
}