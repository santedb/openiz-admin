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
 * Date: 2017-6-4
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.AppletModels
{
	/// <summary>
	/// Represents an applet publisher view model.
	/// </summary>
	public class AppletPublisherViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AppletPublisherViewModel"/> class.
		/// </summary>
		public AppletPublisherViewModel()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppletPublisherViewModel"/> class.
		/// </summary>
		/// <param name="certificateInfo">The certificate information.</param>
		public AppletPublisherViewModel(X509Certificate2Info certificateInfo)
		{
			this.Issuer = certificateInfo.Issuer;
			this.Subject = certificateInfo.Subject;
			this.ValidFrom = certificateInfo.NotBefore?.ToString(Constants.DateTimeFormatStringWithTimestamp);
			this.ValidTo = certificateInfo.NotAfter?.ToString(Constants.DateTimeFormatStringWithTimestamp);
		}

		/// <summary>
		/// Gets or sets the issuer.
		/// </summary>
		/// <value>The issuer.</value>
		[Display(Name = "Issuer", ResourceType = typeof(Locale))]
		public string Issuer { get; set; }

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>The subject.</value>
		[Display(Name = "Subject", ResourceType = typeof(Locale))]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the valid from.
		/// </summary>
		/// <value>The valid from.</value>
		[Display(Name = "ValidFrom", ResourceType = typeof(Locale))]
		public string ValidFrom { get; set; }

		/// <summary>
		/// Gets or sets the valid to.
		/// </summary>
		/// <value>The valid to.</value>
		[Display(Name = "ValidTo", ResourceType = typeof(Locale))]
		public string ValidTo { get; set; }
	}
}