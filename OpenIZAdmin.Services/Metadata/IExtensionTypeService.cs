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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZAdmin.Services.Metadata
{
	/// <summary>
	/// Represents an extension type service.
	/// </summary>
	public interface IExtensionTypeService
	{
		/// <summary>
		/// Gets an extension type by key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the extension type for the give key.</returns>
		ExtensionType Get(Guid key);

		/// <summary>
		/// Creates the specified extension type.
		/// </summary>
		/// <param name="extensionType">Type of the extension.</param>
		/// <returns>Returns the created extension type.</returns>
		ExtensionType Create(ExtensionType extensionType);

		/// <summary>
		/// Searches for an extension type using a given search term.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of extension types which match the given search term.</returns>
		IEnumerable<ExtensionType> Search(string searchTerm);
	}
}
