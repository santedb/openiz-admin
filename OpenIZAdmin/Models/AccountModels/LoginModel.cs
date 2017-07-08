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
 * Date: 2016-7-9
 */

using OpenIZAdmin.Localization;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.AccountModels
{
	/// <summary>
	/// Represents a login model.
	/// </summary>
	public class LoginModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Models.AccountModels.LoginModel"/> class.
		/// </summary>
		public LoginModel()
		{
		}

		/// <summary>
		/// Gets or sets the password of the model.
		/// </summary>
		[DataType(DataType.Password)]
		[Display(Name = "Password", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Locale))]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the username of the model.
		/// </summary>
		[Display(Name = "Username", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "UsernameRequired", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(Constants.RegExBasicHtmlString, ErrorMessageResourceName = "InvalidUsername", ErrorMessageResourceType = typeof(Locale))]
		public string Username { get; set; }
	}
}