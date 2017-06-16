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
using System.ComponentModel.DataAnnotations;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.CodeSystemModels
{
	/// <summary>
	/// Represents a create code system model.
	/// </summary>
	public class CreateCodeSystemModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateCodeSystemModel"/> class.
		/// </summary>
		public CreateCodeSystemModel()
		{
		}

		/// <summary>
		/// Gets or sets the Description
		/// </summary>
		[Display(Name = "Description", ResourceType = typeof(Locale))]
		[StringLength(256, ErrorMessageResourceName = "DescriptionLength256", ErrorMessageResourceType = typeof(Locale))]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the Domain
		/// </summary>
		[Display(Name = "Domain", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "DomainRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(50, ErrorMessageResourceName = "DomainLength50", ErrorMessageResourceType = typeof(Locale))]
		public string Domain { get; set; }

		/// <summary>
		/// Gets or sets the description
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(256, ErrorMessageResourceName = "NameLength256", ErrorMessageResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Oid
		/// </summary>
		[Display(Name = "Oid", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "OidRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(64, ErrorMessageResourceName = "OidLength64", ErrorMessageResourceType = typeof(Locale))]
        [RegularExpression(Constants.RegExOidValidation, ErrorMessageResourceName = "OidValidationErrorMessage", ErrorMessageResourceType = typeof(Locale))]
        public string Oid { get; set; }

		/// <summary>
		/// Gets or sets the Url
		/// </summary>
		[Display(Name = "Url", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "UrlRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(256, ErrorMessageResourceName = "UrlLength256", ErrorMessageResourceType = typeof(Locale))]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the Version
		/// </summary>
		[Display(Name = "Version", ResourceType = typeof(Locale))]
		[StringLength(10, ErrorMessageResourceName = "VersionLength10", ErrorMessageResourceType = typeof(Locale))]
		public string Version { get; set; }

		/// <summary>
		/// Converts a <see cref="CreateCodeSystemModel"/> instance to a <see cref="CodeSystem"/> instance.
		/// </summary>
		/// <returns>Returns the converted code system.</returns>
		public CodeSystem ToCodeSystem()
		{
			return new CodeSystem(this.Name, this.Oid, this.Domain)
			{
				Description = this.Description,
				Url = this.Url,
				VersionText = this.Version
			};
		}
	}
}