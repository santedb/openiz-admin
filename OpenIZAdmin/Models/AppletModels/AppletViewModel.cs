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

using OpenIZ.Core.Model.AMI.Applet;
using OpenIZAdmin.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OpenIZAdmin.Models.AppletModels
{
	/// <summary>
	/// Represents an applet.
	/// </summary>
	public class AppletViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AppletViewModel"/> class.
		/// </summary>
		public AppletViewModel()
		{
			this.Dependencies = new List<AppletDependencyViewModel>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppletViewModel"/> class.
		/// </summary>
		/// <param name="appletManifestInfo">The applet manifest information.</param>
		public AppletViewModel(AppletManifestInfo appletManifestInfo) : this()
		{
			this.Author = appletManifestInfo.AppletInfo.Author;
			this.Group = appletManifestInfo.AppletInfo.GetGroupName("en");
			this.Id = appletManifestInfo.AppletInfo.Id;
			this.PublicKeyToken = appletManifestInfo.AppletInfo.PublicKeyToken;
			this.Version = appletManifestInfo.AppletInfo.Version;
			this.Name = string.Join(", ", appletManifestInfo.AppletInfo.Names.Select(l => l.Value));

			if (appletManifestInfo.AppletInfo.Dependencies?.Any() == true)
			{
				this.Dependencies = appletManifestInfo.AppletInfo.Dependencies.Select(a => new AppletDependencyViewModel(a)).ToList();
			}

			if (appletManifestInfo.PublisherData != null)
			{
				this.PublisherViewModel = new AppletPublisherViewModel(appletManifestInfo.PublisherData);
			}
		}

		/// <summary>
		/// Gets or sets the author of the applet.
		/// </summary>
		[Display(Name = "Author", ResourceType = typeof(Locale))]
		public string Author { get; set; }

		/// <summary>
		/// Gets or sets the dependencies.
		/// </summary>
		/// <value>The dependencies.</value>
		public List<AppletDependencyViewModel> Dependencies { get; set; }

		/// <summary>
		/// Gets or sets the group of the applet.
		/// </summary>
		[Display(Name = "Group", ResourceType = typeof(Locale))]
		public string Group { get; set; }

		/// <summary>
		/// Gets or sets the id of the applet.
		/// </summary>
		[Display(Name = "Id", ResourceType = typeof(Locale))]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the applet.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the key token string
		/// </summary>
		[Display(Name = "PublicKeyToken", ResourceType = typeof(Locale))]
		public string PublicKeyToken { get; set; }

		/// <summary>
		/// Gets or sets the publisher view model.
		/// </summary>
		/// <value>The publisher view model.</value>
		public AppletPublisherViewModel PublisherViewModel { get; set; }

		/// <summary>
		/// Gets or sets the version of the applet.
		/// </summary>
		[Display(Name = "Version", ResourceType = typeof(Locale))]
		public string Version { get; set; }
	}
}