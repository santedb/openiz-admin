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
 * Date: 2017-4-17
 */

using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.CodeSystemModels
{
	/// <summary>
	/// Represents a model to view a code system.
	/// </summary>
	public class CodeSystemViewModel : IdentifiedViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodeSystemViewModel"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		public CodeSystemViewModel(Guid id) : base(id)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeSystemViewModel"/> class.
		/// </summary>
		/// <param name="codeSystem">The code system.</param>
		public CodeSystemViewModel(CodeSystem codeSystem) : this(codeSystem.Key.Value)
		{
			this.Description = codeSystem.Description;
			this.Domain = codeSystem.Authority;
			this.Name = codeSystem.Name ?? Constants.NotApplicable;
			this.Oid = codeSystem.Oid;
			this.Url = codeSystem.Url;
			this.Version = codeSystem.VersionText;
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[Display(Name = "Description", ResourceType = typeof(Locale))]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>The domain.</value>
		[Display(Name = "Domain", ResourceType = typeof(Locale))]
		public string Domain { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the oid.
		/// </summary>
		/// <value>The oid.</value>
		[Display(Name = "Oid", ResourceType = typeof(Locale))]
		public string Oid { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		[Display(Name = "Url", ResourceType = typeof(Locale))]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>The version.</value>
		[Display(Name = "Version", ResourceType = typeof(Locale))]
		public string Version { get; set; }
	}
}