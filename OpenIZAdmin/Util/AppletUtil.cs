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
 * Date: 2016-11-19
 */

using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Model.AMI.Applet;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Models.AppletModels;
using OpenIZAdmin.Models.AppletModels.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Represents a utility for managing applets.
	/// </summary>
	public static class AppletUtil
	{
        /// <summary>
		/// Retrieves a specific applet.
		/// </summary>
		/// <param name="client">The AMI service client</param>
        /// <param name="id">The identifier of the applet</param>
		public static AppletManifestInfo GetApplet(AmiServiceClient client, string id)
        {            
            return client.GetApplet(id);             
        }

        /// <summary>
        /// Retrieves the list of applets.
        /// </summary>
        /// <param name="client">The AMI service client</param>
        public static List<AppletViewModel> GetApplets(AmiServiceClient client)
		{
			var applets = client.GetApplets();

            return applets.CollectionItem.Select(ToAppletViewModel).ToList();
		} 

        /// <summary>
		/// Converts a <see cref="AppletManifestInfo"/> to a <see cref="AppletViewModel"/>.
		/// </summary>
		/// <param name="appletInfo">The AppletInfo object to convert.</param>
		/// <returns>Returns a AppletViewModel model.</returns>
		public static AppletViewModel ToAppletViewModel(AppletManifestInfo appletInfo)
        {            
            var viewModel = new AppletViewModel
            {
                Author = appletInfo.AppletManifest.Info.Author,
                Group = appletInfo.AppletManifest.Info.GetGroupName("en"),
                Id = appletInfo.AppletManifest.Info.Id,
                PublicKeyToken = appletInfo.AppletManifest.Info.PublicKeyToken,
                Version = appletInfo.AppletManifest.Info.Version,
                Name = string.Join(", ", appletInfo.AppletManifest.Info.Names.Select(l => l.Value))
            };

            if(appletInfo.AppletManifest.Assets != null && appletInfo.AppletManifest.Assets.Any())
            {
                viewModel.Assets = appletInfo.AppletManifest.Assets.Select(ToAppletAssetModel).OrderBy(q => q.Name).ToList();
            }            

            return viewModel;
        }

        private static AppletAssetModel ToAppletAssetModel(AppletAsset appletAsset)
        {
	        var viewModel = new AppletAssetModel
	        {
		        Manifest = appletAsset.Manifest ?? new AppletManifest(),
		        Name = appletAsset.Name ?? string.Empty,
		        MimeType = appletAsset.MimeType ?? string.Empty,
		        Language = appletAsset.Language ?? string.Empty
	        };


	        if (appletAsset.Policies != null && appletAsset.Policies.Any())
            {
                viewModel.Policies = appletAsset.Policies;
            }
            
            return viewModel;
        }
    }
}