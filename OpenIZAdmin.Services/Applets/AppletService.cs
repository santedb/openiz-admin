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
 * User: khannan
 * Date: 2017-8-3
 */

using OpenIZ.Core.Model.AMI.Applet;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Services.Core;
using System.Collections.Generic;

namespace OpenIZAdmin.Services.Applets
{
	/// <summary>
	/// Represents an applet service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.AmiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.Applets.IAppletService" />
	public class AppletService : AmiServiceBase, IAppletService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AppletService"/> class.
		/// </summary>
		/// <param name="client">The client.</param>
		public AppletService(AmiServiceClient client) : base(client)
		{
		}

		/// <summary>
		/// Gets all applets.
		/// </summary>
		/// <returns>Returns a list of all the applets in the system.</returns>
		public IEnumerable<AppletManifestInfo> GetAllApplets()
		{
			return this.Client.GetApplets().CollectionItem;
		}

		/// <summary>
		/// Gets the applet.
		/// </summary>
		/// <param name="appletId">The applet identifier.</param>
		/// <returns>Returns the applet with the given id, or null if no applet is found.</returns>
		public AppletManifestInfo GetApplet(string appletId)
		{
			return this.Client.GetApplet(appletId);
		}
	}
}