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
 * User: nitya
 * Date: 2017-8-8
 */

using System.Collections.Generic;
using System.Linq;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.DebugModels.ServerInformationViewModels
{
	/// <summary>
	/// Represents a server service view model.
	/// </summary>
	public class ServerServiceViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServerServiceViewModel"/> class.
		/// </summary>
		public ServerServiceViewModel()
		{
			this.Verbs = new List<string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServerServiceViewModel"/> class.
		/// </summary>
		/// <param name="resourceName">Name of the resource.</param>
		public ServerServiceViewModel(string resourceName) : this()
		{
			this.ResourceName = resourceName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServerServiceViewModel"/> class.
		/// </summary>
		/// <param name="resourceName">Name of the resource.</param>
		/// <param name="verbs">The verbs.</param>
		public ServerServiceViewModel(string resourceName, List<string> verbs) : this(resourceName)
		{
			this.Verbs = verbs;
		}

		/// <summary>
		/// Gets or sets the name of the resource.
		/// </summary>
		/// <value>The name of the resource.</value>
		public string ResourceName { get; set; }

		/// <summary>
		/// Gets or sets the verbs.
		/// </summary>
		/// <value>The verbs.</value>
		public List<string> Verbs { get; set; }

		/// <summary>
		/// Returns the service service information.
		/// </summary>
		/// <returns>Returns the server service information.</returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.ResourceName) || string.IsNullOrWhiteSpace(this.ResourceName))
			{
				this.ResourceName = Locale.NotApplicable;
			}

			this.Verbs.RemoveAll(c => string.IsNullOrEmpty(c) || string.IsNullOrWhiteSpace(c));

			return $"{Locale.ResourceName}: {this.ResourceName}, {Locale.Verbs}: {string.Join(", ", this.Verbs.OrderBy(c => c))}";
		}
	}
}