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

using OpenIZ.Core.Applets.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.AppletModels
{
	/// <summary>
	/// Represents an applet asset.
	/// </summary>
	public class AppletDependencyViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AppletDependencyViewModel"/> class.
		/// </summary>
		public AppletDependencyViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppletDependencyViewModel" /> class.
		/// </summary>
		/// <param name="appletName">Name of the applet.</param>
		public AppletDependencyViewModel(AppletName appletName)
		{
			this.Id = appletName.Id;
			this.Version = appletName.Version;
		}

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[Display(Name = "Id", ResourceType = typeof(Locale))]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>The version.</value>
		[Display(Name = "Version", ResourceType = typeof(Locale))]
		public string Version { get; set; }
	}
}