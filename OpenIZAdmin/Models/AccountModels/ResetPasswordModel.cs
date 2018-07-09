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
 * Date: 2017-3-28
 */

using OpenIZAdmin.Localization;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.AccountModels
{
	/// <summary>
	/// Represents a model to allow a user to reset their password.
	/// </summary>
	public class ResetPasswordModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResetPasswordModel"/> class.
		/// </summary>
		public ResetPasswordModel()
		{
		}

		/// <summary>
		/// Gets or sets the code.
		/// </summary>
		/// <value>The code.</value>
		[Display(Name = "Code", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "CodeRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(4, ErrorMessageResourceName = "CodeLength4", ErrorMessageResourceType = typeof(Locale))]
		public string Code { get; set; }

		/// <summary>
		/// Gets or sets the confirm password.
		/// </summary>
		/// <value>The confirm password.</value>
		[DataType(DataType.Password)]
		[Display(Name = "ConfirmPassword", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "ConfirmPasswordRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(50, ErrorMessageResourceName = "PasswordLength50", ErrorMessageResourceType = typeof(Locale))]
		[Compare("Password", ErrorMessageResourceName = "ConfirmPasswordMatch", ErrorMessageResourceType = typeof(Locale))]
		public string ConfirmPassword { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		[DataType(DataType.Password)]
		[Display(Name = "Password", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(50, ErrorMessageResourceName = "PasswordLength50", ErrorMessageResourceType = typeof(Locale))]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		/// <value>The user identifier.</value>
		[Required]
		public Guid UserId { get; set; }
	}
}