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
 * Date: 2017-10-24
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace OpenIZAdmin.Services.Http.Model
{
	/// <summary>
	/// Represents a collection of detail type codes.
	/// </summary>
	[XmlType(nameof(DetailType), Namespace = "http://openiz.org/imsi")]
	public enum DetailType
	{
		/// <summary>
		/// Represents an informational detail type.
		/// </summary>
		[XmlEnum("I")]
		Information,

		/// <summary>
		/// Represents a warning detail type.
		/// </summary>
		[XmlEnum("W")]
		Warning,

		/// <summary>
		/// Represents an error detail type.
		/// </summary>
		[XmlEnum("E")]
		Error
	}
}