/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-11-14
 */
using OpenIZ.Core.Alert.Alerting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Models.AlertModels.ViewModels
{
	/// <summary>
	/// Represents an alert view model.
	/// </summary>
	public class AlertViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AlertViewModel"/> class.
		/// </summary>
		public AlertViewModel()
		{
		}

		/// <summary>
		/// Gets or sets the flags of the alert.
		/// </summary>
		public AlertMessageFlags Flags { get; set; }

		/// <summary>
		/// Gets or sets the body of the alert.
		/// </summary>
		public string Body { get; set; }

		/// <summary>
		/// Gets or sets the user who created the alert.
		/// </summary>
		public Guid CreatedBy { get; set; }

		/// <summary>
		/// Gets or sets the from section of the alert.
		/// </summary>
		public string From { get; set; }

		/// <summary>
		/// Gets or sets the id of the alert.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the subject of the alert.
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the time of the alert.
		/// </summary>
		public DateTime Time { get; set; }

		/// <summary>
		/// Gets or sets the to section of the alert.
		/// </summary>
		public string To { get; set; }
	}
}