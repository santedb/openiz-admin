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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Models.AlertModels
{
	/// <summary>
	/// Represents a flag for an alert message.
	/// </summary>
	public enum AlertMessageFlags
	{
		/// <summary>
		/// Just a normal alert
		/// </summary>
		None = 0x0,

		/// <summary>
		/// Indicates the message requires some immediate action!
		/// </summary>
		Alert = 0x1,

		/// <summary>
		/// Indicates whether someone has acknowledged the alert
		/// </summary>
		Acknowledged = 0x2,

		/// <summary>
		/// Indicates the alert is high priority but doesn't require immediate action
		/// </summary>
		HighPriority = 0x4,

		/// <summary>
		/// Indicates the alert is a system alert
		/// </summary>
		System = 0x8,

		/// <summary>
		/// Indicates the alert is transient and shouldn't be persisted
		/// </summary>
		Transient = 0x10,

		HighPriorityAlert = HighPriority | Alert
	}
}