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
using OpenIZAdmin.Models.Core;
using System;
using System.ComponentModel.DataAnnotations;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.CodeSystemModels
{
	/// <summary>
	/// Represents an edit code system model.
	/// </summary>
	public class EditCodeSystemModel : MetadataEditModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditCodeSystemModel"/> class.
		/// </summary>
		public EditCodeSystemModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditCodeSystemModel"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		public EditCodeSystemModel(Guid id) : base(id)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditCodeSystemModel"/> class.
		/// </summary>
		/// <param name="codeSystem">The code system.</param>
		public EditCodeSystemModel(CodeSystem codeSystem) : base(codeSystem.Key.Value)
		{
			this.Description = codeSystem.Description;
			this.Domain = codeSystem.Authority;
			this.Name = codeSystem.Name;
			this.Oid = codeSystem.Oid;
			this.Url = codeSystem.Url;
			this.Version = codeSystem.VersionText;
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[Display(Name = "Description", ResourceType = typeof(Locale))]
		[StringLength(256, ErrorMessageResourceName = "DescriptionLength256", ErrorMessageResourceType = typeof(Locale))]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>The domain.</value>
		[Display(Name = "Domain", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "DomainRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(50, ErrorMessageResourceName = "DomainLength50", ErrorMessageResourceType = typeof(Locale))]
		public string Domain { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(256, ErrorMessageResourceName = "NameLength256", ErrorMessageResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the oid.
		/// </summary>
		/// <value>The oid.</value>
		[Display(Name = "Oid", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "OidRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(64, ErrorMessageResourceName = "OidLength64", ErrorMessageResourceType = typeof(Locale))]
        [RegularExpression(Constants.RegExOidValidation, ErrorMessageResourceName = "OidValidationErrorMessage", ErrorMessageResourceType = typeof(Locale))]
        public string Oid { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		[Display(Name = "Url", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "UrlRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(256, ErrorMessageResourceName = "UrlLength256", ErrorMessageResourceType = typeof(Locale))]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>The version.</value>
		[Display(Name = "Version", ResourceType = typeof(Locale))]
		[StringLength(10, ErrorMessageResourceName = "VersionLength10", ErrorMessageResourceType = typeof(Locale))]
		public string Version { get; set; }

		/// <summary>
		/// Converts a <see cref="CreateCodeSystemModel"/> instance to a <see cref="CodeSystem"/> instance.
		/// </summary>
		/// <returns>Returns the converted code system.</returns>
		public CodeSystem ToCodeSystem(CodeSystem codeSystem)
		{
			codeSystem.Description = this.Description;
			codeSystem.Name = this.Name;
			codeSystem.Authority = this.Domain;
			codeSystem.Oid = this.Oid;
			codeSystem.Url = this.Url;
			codeSystem.VersionText = this.Version;

			return codeSystem;
		}
	}
}