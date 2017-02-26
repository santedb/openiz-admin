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

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZAdmin.Models.Core;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.ApplicationModels
{
	/// <summary>
	/// Represents an application view model.
	/// </summary>
	public class ApplicationViewModel : SecurityViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationViewModel"/> class.
		/// </summary>
		public ApplicationViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationViewModel"/> class
		/// with a specific <see cref="SecurityApplicationInfo"/> instance.
		/// </summary>
		/// <param name="securityApplicationInfo">The <see cref="SecurityApplicationInfo"/> instance.</param>
		public ApplicationViewModel(SecurityApplicationInfo securityApplicationInfo) : base(securityApplicationInfo)
		{
			this.ApplicationName = securityApplicationInfo.Name;
		}

		/// <summary>
		/// Gets or sets the application name of the application.
		/// </summary>
		[Display(Name = "ApplicationName", ResourceType = typeof(Localization.Locale))]
		public string ApplicationName { get; set; }
	}
}