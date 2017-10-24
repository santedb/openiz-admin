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
	/// Represents a result detail.
	/// </summary>
	[XmlType(nameof(ResultDetail), Namespace = "http://openiz.org/imsi")]
	public class ResultDetail
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResultDetail"/> class.
		/// </summary>
		public ResultDetail()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResultDetail"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="text">The text.</param>
		public ResultDetail(DetailType type, string text)
		{
			this.Type = type;
			this.Text = text;
		}

		/// <summary>
		/// Gets or sets the type of the result detail.
		/// </summary>
		[XmlAttribute("type")]
		public DetailType Type { get; set; }

		/// <summary>
		/// Gets or sets the text of the result detail.
		/// </summary>
		[XmlText]
		public string Text { get; set; }
	}
}