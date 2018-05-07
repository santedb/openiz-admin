/*
 * Copyright 2016-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2018-5-7
 */

using System.Xml.Serialization;

namespace OpenIZAdmin.Services.Dataset
{
	/// <summary>
	/// Represents a data insert action.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Dataset.DataInstallAction" />
	[XmlType(nameof(DataInsert), Namespace = "http://openiz.org/data")]
	public class DataInsert : DataInstallAction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataInsert"/> class.
		/// </summary>
		public DataInsert()
		{
		}

		/// <summary>
		/// Gets the name of the action.
		/// </summary>
		/// <value>The name of the action.</value>
		public override string ActionName => "Insert";

		/// <summary>
		/// Gets or sets a value indicating whether the insert should be skipped if it exists.
		/// </summary>
		/// <value><c>true</c> if the insert should be skipped if it exists; otherwise, <c>false</c>.</value>
		[XmlAttribute("skipIfExists")]
		public bool SkipIfExists { get; set; }
	}
}