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
	/// Represents a data obsolete action.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Dataset.DataInstallAction" />
	[XmlType(nameof(DataObsolete), Namespace = "http://openiz.org/data")]
	public class DataObsolete : DataInstallAction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataObsolete"/> class.
		/// </summary>
		public DataObsolete()
		{
		}

		/// <summary>
		/// Gets the name of the action.
		/// </summary>
		/// <value>The name of the action.</value>
		public override string ActionName => "Obsolete";
	}
}