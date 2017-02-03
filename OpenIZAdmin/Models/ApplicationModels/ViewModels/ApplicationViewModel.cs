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
 * Date: 2016-7-8
 */

using OpenIZAdmin.Models.PolicyModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.ApplicationModels.ViewModels
{
	/// <summary>
	/// Represents an application view model.
	/// </summary>
	public class ApplicationViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationViewModel"/> class. 
		/// </summary>
		public ApplicationViewModel()
		{
			this.Policies = new List<PolicyViewModel>();
		}

		/// <summary>
		/// Gets or sets the application id of the application.
		/// </summary>
		[Display(Name = "ApplicationId", ResourceType = typeof(Localization.Locale))]
		public string ApplicationId { get; set; }

		/// <summary>
		/// Gets or sets the application name of the application.
		/// </summary>
		[Display(Name = "ApplicationName", ResourceType = typeof(Localization.Locale))]
		public string ApplicationName { get; set; }

		/// <summary>
		/// Gets or sets the creation time of the application.
		/// </summary>
		[Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Gets or sets whether the application has policies associated.
		/// </summary>
		[Display(Name = "HasPolicies", ResourceType = typeof(Localization.Locale))]
		public bool HasPolicies { get; set; }

		/// <summary>
		/// Gets or sets the id of the application.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets whether the application is obsolete.
		/// </summary>
		public bool IsObsolete { get; set; }

		/// <summary>
		/// Gets or sets the list of policies associated with the application.
		/// </summary>
		public List<PolicyViewModel> Policies { get; set; }

		/// <summary>
		/// Gets or sets the updated time of the application.
		/// </summary>
		public DateTime? UpdatedTime { get; set; }
	}
}