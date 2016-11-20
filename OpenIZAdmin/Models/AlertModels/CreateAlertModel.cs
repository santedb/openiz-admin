﻿/*
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
 * Date: 2016-11-16
 */
using OpenIZAdmin.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.AlertModels
{
	/// <summary>
	/// Represents a model to create an alert.
	/// </summary>
	public class CreateAlertModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateAlertModel"/> class.
		/// </summary>
		public CreateAlertModel()
		{

		}

		/// <summary>
		/// Gets or sets the message of the alert.
		/// </summary>
		[Display(Name = "Message", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "MessageRequired", ErrorMessageResourceType = typeof(Locale))]
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the priority of the alert.
		/// </summary>
		[Display(Name = "Priority", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "PriorityRequired", ErrorMessageResourceType = typeof(Locale))]
		public int Priority { get; set; }

		/// <summary>
		/// Gets or sets the list of priorities.
		/// </summary>
		public List<SelectListItem> PriorityList { get; set; }

		/// <summary>
		/// Gets or sets the subject of the alert.
		/// </summary>
		[Display(Name = "Subject", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "SubjectRequired", ErrorMessageResourceType = typeof(Locale))]
		public string Subject { get; set; }
	}
}