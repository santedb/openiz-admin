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
			this.Assets = new List<AppletViewAssetModel>();
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
            //this.Assets = appletManifestInfo.AppletInfo..Assets.Select(a => new AppletViewAssetModel(a)).OrderBy(q => q.Name).ToList();
            //this.Assets = appletManifestInfo.AppletInfo.Assets.Select(a => new AppletViewAssetModel(a)).OrderBy(q => q.Name).ToList();

            //this.Author = appletManifestInfo.AppletManifest.Info.Author;
            //this.Group = appletManifestInfo.AppletManifest.Info.GetGroupName("en");
            //this.Id = appletManifestInfo.AppletManifest.Info.Id;
            //this.PublicKeyToken = appletManifestInfo.AppletManifest.Info.PublicKeyToken;
            //this.Version = appletManifestInfo.AppletManifest.Info.Version;
            //this.Name = string.Join(", ", appletManifestInfo.AppletManifest.Info.Names.Select(l => l.Value));
            //this.Assets = appletManifestInfo.AppletManifest.Assets.Select(a => new AppletViewAssetModel(a)).OrderBy(q => q.Name).ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppletViewModel"/> class
        /// with a specific author, group, id, name, and version.
        /// </summary>
        /// <param name="author">The author of the applet.</param>
        /// <param name="group">The group of the applet.</param>
        /// <param name="id">The id of the applet.</param>
        /// <param name="name">The name of the applet.</param>
        /// <param name="version">The version of the applet.</param>
        public AppletViewModel(string author, string group, string id, string name, string version)
		{
			this.Author = author;
			this.Group = group;
			this.Id = id;
			this.Name = name;
			this.Version = version;
		}

		/// <summary>
		/// Gets or sets the author of the applet.
		/// </summary>
		public int AssetCount => this.Assets?.Count ?? 0;

		/// <summary>
		/// Gets or sets the list with the associated assets
		/// </summary>
		public List<AppletViewAssetModel> Assets { get; set; }

		/// <summary>
		/// Gets or sets the author of the applet.
		/// </summary>
		[Display(Name = "Author", ResourceType = typeof(Localization.Locale))]
		public string Author { get; set; }

		/// <summary>
		/// Gets or sets the group of the applet.
		/// </summary>
		[Display(Name = "Group", ResourceType = typeof(Localization.Locale))]
		public string Group { get; set; }

		/// <summary>
		/// Gets or sets the id of the applet.
		/// </summary>
		[Display(Name = "Id", ResourceType = typeof(Localization.Locale))]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the applet.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the key token string
		/// </summary>
		public string PublicKeyToken { get; set; }

		/// <summary>
		/// Gets or sets the version of the applet.
		/// </summary>
		[Display(Name = "Version", ResourceType = typeof(Localization.Locale))]
		public string Version { get; set; }
	}
}