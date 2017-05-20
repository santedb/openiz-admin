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
 * Date: 2017-5-19
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.ExtensionTypeModels
{
	/// <summary>
	/// Represents an extension type view model.
	/// </summary>
	public class ExtensionTypeViewModel : IdentifiedViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionTypeViewModel"/> class.
		/// </summary>
		public ExtensionTypeViewModel()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionTypeViewModel"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		public ExtensionTypeViewModel(Guid id) : base(id)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionTypeViewModel"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="creationTime">The creation time.</param>
		public ExtensionTypeViewModel(Guid id, DateTimeOffset creationTime) : base(id, creationTime)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionTypeViewModel"/> class.
		/// </summary>
		/// <param name="extensionType">Type of the extension.</param>
		public ExtensionTypeViewModel(ExtensionType extensionType) : base(extensionType.Key.Value, extensionType.CreationTime)
		{
			this.Name = extensionType.Name;
			this.HandlerClass = extensionType.ExtensionHandler.Name;
		}

		/// <summary>
		/// Gets or sets the handler class.
		/// </summary>
		/// <value>The handler class.</value>
		[Display(Name = "HandlerClass", ResourceType = typeof(Locale))]
		public string HandlerClass { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		public string Name { get; set; }
	}
}