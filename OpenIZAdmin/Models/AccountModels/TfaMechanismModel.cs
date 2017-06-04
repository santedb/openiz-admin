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
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.AccountModels
{
	/// <summary>
	/// Represents a two factor authentication mechanism model.
	/// </summary>
	public class TfaMechanismModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TfaMechanismModel"/> class.
		/// </summary>
		public TfaMechanismModel()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TfaMechanismModel"/> class.
		/// </summary>
		/// <param name="twoFactorAuthenticationMechanismInfo">The two factor authentication mechanism information.</param>
		public TfaMechanismModel(TfaMechanismInfo twoFactorAuthenticationMechanismInfo)
		{
			this.ChallengeText = twoFactorAuthenticationMechanismInfo.ChallengeText;
			this.Id = twoFactorAuthenticationMechanismInfo.Id;
			this.Name = twoFactorAuthenticationMechanismInfo.Name;
		}

		/// <summary>
		/// Gets or sets the challenge text.
		/// </summary>
		/// <value>The challenge text.</value>
		public string ChallengeText { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [StringLength(256, ErrorMessageResourceName = "NameLength256", ErrorMessageResourceType = typeof(Locale))]
        public string Name { get; set; }
	}
}