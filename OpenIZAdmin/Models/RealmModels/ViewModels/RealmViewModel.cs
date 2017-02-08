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
using System;
using System.ComponentModel.DataAnnotations;
using OpenIZAdmin.Models.Domain;

namespace OpenIZAdmin.Models.RealmModels.ViewModels
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

		public RealmViewModel(Realm realm)
		{
			this.Map(realm);
		}

		[Display(Name = "Address", ResourceType = typeof(Locale))]
		public string Address { get; set; }

		[Display(Name = "Application Id")]
		public string ApplicationId { get; set; }

		[Display(Name = "CreationTime", ResourceType = typeof(Locale))]
		public DateTime CreationTime { get; set; }

		public Guid Id { get; set; }
	}
}