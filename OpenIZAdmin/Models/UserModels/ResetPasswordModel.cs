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
 * Date: 2016-11-21
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.UserModels
{
	/// <summary>
	/// Represents a reset password model.
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
		/// Gets or sets the password confirmation of the model.
		/// </summary>
		[DataType(DataType.Password)]
		[Display(Name = "ConfirmPassword", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "ConfirmPasswordRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		[Compare("Password", ErrorMessageResourceName = "ConfirmPasswordMatch", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string ConfirmPassword { get; set; }

		/// <summary>
		/// Gets or sets the user id of the model.
		/// </summary>
		[Required]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the password of the model.
		/// </summary>
		[DataType(DataType.Password)]
		[Display(Name = "Password", ResourceType = typeof(Localization.Locale))]
		[Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string Password { get; set; }
	}
}