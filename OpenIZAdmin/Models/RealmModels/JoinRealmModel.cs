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
 * Date: 2016-7-13
 */

using System.ComponentModel.DataAnnotations;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.RealmModels
{
	/// <summary>
	/// Represents a model to join a realm.
	/// </summary>
	public class JoinRealmModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Models.RealmModels.JoinRealmModel"/> class.
		/// </summary>
		public JoinRealmModel()
		{
		}

		/// <summary>
		/// Gets or sets the address of the realm.
		/// </summary>
		[Display(Name = "Address", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "AddressRequired", ErrorMessageResourceType = typeof(Locale))]
		//[StringLength(255, ErrorMessageResourceName = "AddressLength255", ErrorMessageResourceType = typeof(Locale))]
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets the application id of the current application.
		/// </summary>
		[Display(Name = "ApplicationId", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "ApplicationIdRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(255, ErrorMessageResourceName = "ApplicationIdLength255", ErrorMessageResourceType = typeof(Locale))]
		public string ApplicationId { get; set; }

		/// <summary>
		/// Gets or sets the application secret of current application.
		/// </summary>
		[Display(Name = "ApplicationSecret", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "ApplicationSecretRequired", ErrorMessageResourceType = typeof(Locale))]
		//[StringLength(255, ErrorMessageResourceName = "ApplicationSecretLength255", ErrorMessageResourceType = typeof(Locale))]
		public string ApplicationSecret { get; set; }

		/// <summary>
		/// Gets or sets the password used to connect to the realm.
		/// </summary>
		[DataType(DataType.Password)]
		[Display(Name = "Password", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Locale))]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the username used to connect to the realm.
		/// </summary>
		[Display(Name = "Username", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "UsernameRequired", ErrorMessageResourceType = typeof(Locale))]
		public string Username { get; set; }
	}
}