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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.AccountModels
{
	/// <summary>
	/// Represents a forgot password model.
	/// </summary>
	public class ForgotPasswordModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ForgotPasswordModel"/> class.
		/// </summary>
		public ForgotPasswordModel()
		{
			this.TfaMechanisms = new List<TfaMechanismModel>();
		}

		/// <summary>
		/// Gets or sets the tfa mechanism.
		/// </summary>
		/// <value>The tfa mechanism.</value>
		[Required(ErrorMessageResourceName = "TfaMechanismRequired", ErrorMessageResourceType = typeof(Locale))]
		public Guid TfaMechanism { get; set; }

		/// <summary>
		/// Gets or sets the tfa mechanisms.
		/// </summary>
		/// <value>The tfa mechanisms.</value>
		public List<TfaMechanismModel> TfaMechanisms { get; set; }

		/// <summary>
		/// The username of the user.
		/// </summary>
		[Display(Name = "Username", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "UsernameRequired", ErrorMessageResourceType = typeof(Locale))]		
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the verification.
		/// </summary>
		/// <value>The verification.</value>
		[Display(Name = "Verification", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "VerificationRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(256, ErrorMessageResourceName = "VerificationLength256", ErrorMessageResourceType = typeof(Locale))]
		public string Verification { get; set; }
	}
}