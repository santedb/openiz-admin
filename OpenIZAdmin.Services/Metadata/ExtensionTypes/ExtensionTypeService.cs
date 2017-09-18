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
 * Date: 2017-7-22
 */

using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Services.Core;
using System;
using System.Collections.Generic;

namespace OpenIZAdmin.Services.Metadata.ExtensionTypes
{
	/// <summary>
	/// Represents an extension type service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.AmiServiceBase" />
	/// <seealso cref="IExtensionTypeService" />
	public class ExtensionTypeService : AmiServiceBase, IExtensionTypeService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionTypeService"/> class.
		/// </summary>
		/// <param name="client">The client.</param>
		public ExtensionTypeService(AmiServiceClient client) : base(client)
		{
		}

		/// <summary>
		/// Creates the specified extension type.
		/// </summary>
		/// <param name="extensionType">Type of the extension.</param>
		/// <returns>Returns the created extension type.</returns>
		public ExtensionType Create(ExtensionType extensionType)
		{
			return this.Client.CreateExtensionType(extensionType);
		}

		/// <summary>
		/// Gets an extension type by key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the extension type for the give key.</returns>
		public ExtensionType Get(Guid key)
		{
			return this.Client.GetExtensionType(key.ToString());
		}

		/// <summary>
		/// Searches for an extension type using a given search term.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of extension types which match the given search term.</returns>
		public IEnumerable<ExtensionType> Search(string searchTerm)
		{
			var results = new List<ExtensionType>();

			if (searchTerm == "*")
			{
				results.AddRange(this.Client.GetExtensionTypes(a => a.Key != null).CollectionItem);
			}
			else
			{
				Guid extensionTypeId;

				if (!Guid.TryParse(searchTerm, out extensionTypeId))
				{
					results.AddRange(this.Client.GetExtensionTypes(a => a.Name.Contains(searchTerm)).CollectionItem);
				}
				else
				{
					var extensionType = this.Client.GetExtensionType(extensionTypeId.ToString());

					if (extensionType != null)
					{
						results.Add(extensionType);
					}
				}
			}

			return results;
		}
	}
}