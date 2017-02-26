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
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.AssigningAuthorityModels
{
	public class EditAssigningAuthorityModel
	{
		public EditAssigningAuthorityModel()
		{
		}

		public EditAssigningAuthorityModel(AssigningAuthorityInfo assigningAuthorityInfo)
		{
			this.Id = assigningAuthorityInfo.Id;
			this.Name = assigningAuthorityInfo.AssigningAuthority.Name;
			this.Oid = assigningAuthorityInfo.AssigningAuthority.Oid;
			this.Url = assigningAuthorityInfo.AssigningAuthority.Url;
			this.DomainName = assigningAuthorityInfo.AssigningAuthority.DomainName;
			this.Description = assigningAuthorityInfo.AssigningAuthority.Description;
		}

		/// <summary>
		/// Gets or sets the description of the assigning authority.
		/// </summary>
		[Display(Name = "Description", ResourceType = typeof(Locale))]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the domain name of the assigning authority.
		/// </summary>
		[Display(Name = "DomainName", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		public string DomainName { get; set; }

		/// <summary>
		/// Gets or sets the id of the assigning authority.
		/// </summary>
		[Required]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the assigning authority.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(50, ErrorMessageResourceName = "NameLength50", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the OID of the assigning authority.
		/// </summary>
		[Display(Name = "Oid", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "OidRequired", ErrorMessageResourceType = typeof(Locale))]
		public string Oid { get; set; }

		/// <summary>
		/// Gets or sets the URL of the assigning authority.
		/// </summary>
		[Display(Name = "Url", ResourceType = typeof(Locale))]
		[Url(ErrorMessageResourceName = "UrlInvalid", ErrorMessageResourceType = typeof(Locale))]
		public string Url { get; set; }

		/// <summary>
		/// Converts a <see cref="EditAssigningAuthorityModel"/> instance to an <see cref="AssigningAuthorityInfo"/> instance.
		/// </summary>
		/// <returns>Returns an <see cref="AssigningAuthorityInfo"/> instance.</returns>
		public AssigningAuthorityInfo ToAssigningAuthorityInfo()
		{
			var assigningAuthorityInfo = new AssigningAuthorityInfo
			{
				Id = this.Id,
				AssigningAuthority = new AssigningAuthority
				{
					Key = this.Id,
					Url = this.Url,
					DomainName = this.DomainName,
					Description = this.Description,
					Oid = this.Oid,
					Name = this.Name
				}
			};

			return assigningAuthorityInfo;
		}
	}
}