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
 * Date: 2016-11-14
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.ApplicationModels
{
	/// <summary>
	/// Represents a create application model.
	/// </summary>
	public class CreateApplicationModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateApplicationModel"/> class.
		/// </summary>
		public CreateApplicationModel()
		{
			this.Policies = new List<Guid>();
			this.PolicyList = new List<SelectListItem>();
		}

		/// <summary>
		/// Gets or sets the id of the application.
		/// </summary>
		//[Required]
		public string ApplicationId { get; set; }

		/// <summary>
		/// Gets or sets the name of the application.
		/// </summary>
		[Required]
		[Display(Name = "ApplicationName", ResourceType = typeof(Localization.Locale))]
		public string ApplicationName { get; set; }

		/// <summary>
		/// Gets or sets the secret of the application.
		/// </summary>
		[Required]
		[Display(Name = "ApplicationSecret", ResourceType = typeof(Localization.Locale))]
		public string ApplicationSecret { get; set; }

		/// <summary>
		/// Gets or sets a list of policies associated with the application.
		/// </summary>
		public List<Guid> Policies { get; set; }

		/// <summary>
		/// Gets or sets a list of policies assocated with the application.
		/// </summary>
		public List<SelectListItem> PolicyList { get; set; }
	}
}