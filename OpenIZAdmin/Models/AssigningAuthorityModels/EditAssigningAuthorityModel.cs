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
 * Date: 2016-8-14
 */

using OpenIZ.Core.Model.AMI.DataTypes;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.AssigningAuthorityModels
{
	/// <summary>
	/// Represents an edit assigning authority model.
	/// </summary>
	public class EditAssigningAuthorityModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditAssigningAuthorityModel"/> class.
		/// </summary>
		public EditAssigningAuthorityModel()
		{
			AddConcepts = new List<string>();
			AuthorityScopeList = new List<AuthorityScopeViewModel>();
			ConceptList = new List<SelectListItem>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditAssigningAuthorityModel"/> class.
		/// </summary>
		/// <param name="assigningAuthorityInfo">The assigning authority information.</param>
		public EditAssigningAuthorityModel(AssigningAuthorityInfo assigningAuthorityInfo) : this()
		{
			this.Id = assigningAuthorityInfo.Id;
			this.IsUnique = assigningAuthorityInfo.AssigningAuthority.IsUnique ? Locale.Yes : Locale.No;
			this.Name = assigningAuthorityInfo.AssigningAuthority.Name;
			this.Oid = assigningAuthorityInfo.AssigningAuthority.Oid;
			this.Url = assigningAuthorityInfo.AssigningAuthority.Url;
			this.DomainName = assigningAuthorityInfo.AssigningAuthority.DomainName;
			this.Description = assigningAuthorityInfo.AssigningAuthority.Description;
			this.ValidationRegex = assigningAuthorityInfo.AssigningAuthority.ValidationRegex;
		}

		/// <summary>
		/// Gets or sets the list of Concepts to add
		/// </summary>
		/// <value>The add concepts.</value>
		[Display(Name = "AddConcepts", ResourceType = typeof(Locale))]
		public List<string> AddConcepts { get; set; }

		/// <summary>
		/// Gets or sets the authority scopes.
		/// </summary>
		/// <value>The scopes assigned.</value>
		public List<AuthorityScopeViewModel> AuthorityScopeList { get; set; }

		/// <summary>
		/// Gets or sets the concept list from the search parameters from the ajax search method
		/// </summary>
		/// <value>The concept list.</value>
		public List<SelectListItem> ConceptList { get; set; }

		/// <summary>
		/// Gets or sets the description of the assigning authority.
		/// </summary>
		[Display(Name = "Description", ResourceType = typeof(Locale))]
		[StringLength(4000, ErrorMessageResourceName = "DescriptionLength4000", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the domain name of the assigning authority.
		/// </summary>
		[Display(Name = "DomainName", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(32, ErrorMessageResourceName = "DomainNameLength32", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
		public string DomainName { get; set; }

		/// <summary>
		/// Gets or sets the id of the assigning authority.
		/// </summary>
		[Required]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is unique.
		/// </summary>
		/// <value><c>true</c> if this instance is unique; otherwise, <c>false</c>.</value>
		[Display(Name = "IsUnique", ResourceType = typeof(Locale))]
		public string IsUnique { get; set; }

		/// <summary>
		/// Gets or sets the name of the assigning authority.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(50, ErrorMessageResourceName = "NameLength50", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the OID of the assigning authority.
		/// </summary>
		[Display(Name = "Oid", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "OidRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(256, ErrorMessageResourceName = "OidLength256", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(Constants.RegExOidValidation, ErrorMessageResourceName = "OidValidationErrorMessage", ErrorMessageResourceType = typeof(Locale))]
		public string Oid { get; set; }

		/// <summary>
		/// Gets or sets the URL of the assigning authority.
		/// </summary>
		[Display(Name = "Url", ResourceType = typeof(Locale))]
		[Url(ErrorMessageResourceName = "UrlInvalid", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(250, ErrorMessageResourceName = "UrlLength250", ErrorMessageResourceType = typeof(Locale))]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the validation regex.
		/// </summary>
		/// <value>The validation regex.</value>
		[Display(Name = "ValidationRegex", ResourceType = typeof(Locale))]
		[StringLength(64, ErrorMessageResourceName = "RegexLength64", ErrorMessageResourceType = typeof(Locale))]
		public string ValidationRegex { get; set; }

		/// <summary>
		/// Checks of the selected concept is already in the authority scope list
		/// </summary>
		/// <returns>Returns true if the selected concept exists, false if not found</returns>
		public bool HasSelectedAuthorityScope(AssigningAuthority authorityInfo)
		{
			return AddConcepts.Any() && authorityInfo.AuthorityScope.Any(scope => scope.Key.ToString().Equals(AddConcepts[0]));
		}

		/// <summary>
		/// Converts a <see cref="EditAssigningAuthorityModel"/> instance to an <see cref="AssigningAuthorityInfo"/> instance.
		/// </summary>
		/// <returns>Returns an <see cref="AssigningAuthorityInfo"/> instance.</returns>
		public AssigningAuthority ToAssigningAuthorityInfo(AssigningAuthority authorityInfo)
		{
			authorityInfo.Url = this.Url;
			authorityInfo.DomainName = this.DomainName;
			authorityInfo.Description = this.Description;
			authorityInfo.Oid = this.Oid;
			authorityInfo.Name = this.Name;
			authorityInfo.ValidationRegex = this.ValidationRegex;

			if (!this.AddConcepts.Any()) return authorityInfo;

			foreach (var concept in AddConcepts)
			{
				Guid id;
				if (Guid.TryParse(concept, out id))
				{
					if (authorityInfo.AuthorityScopeXml == null)
					{
						authorityInfo.AuthorityScopeXml = new List<Guid>();
					}

					authorityInfo.AuthorityScopeXml.Add(id);
				}
			}

			return authorityInfo;
		}
	}
}