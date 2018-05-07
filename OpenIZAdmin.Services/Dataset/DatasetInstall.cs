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

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OpenIZAdmin.Services.Dataset
{
	/// <summary>
	/// Represents a dataset.
	/// </summary>
	[XmlRoot("dataset", Namespace = "http://openiz.org/data")]
	[XmlType(nameof(DatasetInstall), Namespace = "http://openiz.org/data")]
	public class DatasetInstall
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DatasetInstall"/> class.
		/// </summary>
		public DatasetInstall()
		{
			this.Action = new List<DataInstallAction>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DatasetInstall"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		public DatasetInstall(string id) : this()
		{
			this.Id = id;
		}

		/// <summary>
		/// Gets or sets the action.
		/// </summary>
		/// <value>The action.</value>
		[XmlElement("insert", Type = typeof(DataInsert))]
		[XmlElement("obsolete", Type = typeof(DataObsolete))]
		[XmlElement("update", Type = typeof(DataUpdate))]
		public List<DataInstallAction> Action { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[XmlAttribute("id")]
		public string Id { get; set; }

		/// <summary>
		/// Loads the specified file to dataset.
		/// </summary>
		/// <param name="datasetFile">The dataset file.</param>
		/// <returns>Returns the loaded dataset.</returns>
		public static DatasetInstall Load(string datasetFile)
		{
			using (var fs = File.OpenRead(datasetFile))
			{
				var xs = new XmlSerializer(typeof(DatasetInstall));
				return xs.Deserialize(fs) as DatasetInstall;
			}
		}
	}
}