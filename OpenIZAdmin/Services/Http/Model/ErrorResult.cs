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

using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OpenIZAdmin.Services.Http.Model
{
	/// <summary>
	/// Represents an error result.
	/// </summary>
	/// <seealso cref="OpenIZ.Core.Model.IdentifiedData" />
	[XmlType(nameof(ErrorResult), Namespace = "http://openiz.org/imsi")]
	[XmlRoot(nameof(ErrorResult), Namespace = "http://openiz.org/imsi")]
	public class ErrorResult : IdentifiedData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorResult"/> class.
		/// </summary>
		public ErrorResult()
		{
			this.Details = new List<ResultDetail>();
		}

		/// <summary>
		/// Gets or sets the details of the result.
		/// </summary>
		[XmlElement("detail")]
		public List<ResultDetail> Details { get; set; }

		/// <summary>
		/// Gets the date this was modified.
		/// </summary>
		public override DateTimeOffset ModifiedOn => DateTimeOffset.Now;

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this error result.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this error result.</returns>
		public override string ToString()
		{
			return string.Join("\r\n", Details.Select(o => $">> {o.Type} : {o.Text}"));
		}
	}
}