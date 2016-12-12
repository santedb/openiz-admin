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
 * Date: 2016-7-8
 */


using OpenIZ.Core.Applets.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Models.AppletModels
{
    /// <summary>
	/// Represents an applet asset.
	/// </summary>
    public class AppletAssetModel
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="AppletAssetModel"/> class.
		/// </summary>
		public AppletAssetModel()
        {
            
        }

        /// <summary>
        /// Gets or sets the Language of the asset.
        /// </summary>
        [Display(Name = "Language", ResourceType = typeof(Localization.Locale))]
        public string Language { get; set; }

        /// <summary>
        /// Gets the or sets the manifest to which the asset belongs
        /// </summary>
        [Display(Name = "Manifest", ResourceType = typeof(Localization.Locale))]
        public AppletManifest Manifest { get; set; }

        /// <summary>
        /// Gets or sets the Mime type of the asset.
        /// </summary>
        [Display(Name = "MimeType", ResourceType = typeof(Localization.Locale))]
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the name of the asset.
        /// </summary>
        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Policies of the asset.
        /// </summary>
        [Display(Name = "Policies", ResourceType = typeof(Localization.Locale))]
        public List<string> Policies { get; set; }
    }
}