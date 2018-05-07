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
	/// Represents a data association action.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Dataset.DataInstallAction" />
	[XmlType(nameof(DataAssociation), Namespace = "http://openiz.org/data")]
	public class DataAssociation : DataInstallAction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataAssociation" /> class.
		/// </summary>
		public DataAssociation()
		{
		}

		/// <summary>
		/// Gets the action name
		/// </summary>
		/// <value>The name of the action.</value>
		public override string ActionName => "Add";

		/// <summary>
		/// Gets or sets the name of the property.
		/// </summary>
		/// <value>The name of the property.</value>
		[XmlAttribute("property")]
		public string PropertyName { get; set; }
	}
}