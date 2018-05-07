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

using OpenIZ.Core.Model;
using System;
using System.IO;
using System.Xml.Serialization;

namespace OpenIZAdmin.Services.Dataset
{
	/// <summary>
	/// Class DatasetService.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Dataset.IDatasetService" />
	public class DatasetService : IDatasetService
	{
		/// <summary>
		/// Converts an identified data instance to a dataset instance.
		/// </summary>
		/// <typeparam name="T">The type of instance to convert to a dataset.</typeparam>
		/// <param name="instance">The instance.</param>
		/// <returns>Returns the dataset.</returns>
		public DatasetInstall ConvertToDataset<T>(T instance) where T : IdentifiedData
		{
			return ConvertToDatasetInternal(instance, typeof(T));
		}

		/// <summary>
		/// Converts an entity or derived entity to a dataset instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="type">The type.</param>
		/// <returns>Returns the dataset.</returns>
		public DatasetInstall ConvertToDataset(object instance, Type type)
		{
			return ConvertToDatasetInternal(instance, type);
		}

		/// <summary>
		/// Gets the dataset as stream.
		/// </summary>
		/// <param name="datasetInstall">The dataset install.</param>
		/// <returns>Returns the dataset as a <see cref="Stream" />.</returns>
		public Stream GetDatasetAsStream(DatasetInstall datasetInstall)
		{
			var content = this.Serialize(datasetInstall);

			var stream = new MemoryStream();

			var streamWriter = new StreamWriter(stream);

			streamWriter.Write(content);
			streamWriter.Flush();
			stream.Position = 0;

			return stream;
		}

		/// <summary>
		/// Serializes the specified dataset install.
		/// </summary>
		/// <param name="datasetInstall">The dataset install.</param>
		/// <returns>Returns the dataset install as an XML string.</returns>
		public string Serialize(DatasetInstall datasetInstall)
		{
			var serializer = new XmlSerializer(typeof(DatasetInstall));

			string result = null;

			using (var stringWriter = new StringWriter())
			{
				serializer.Serialize(stringWriter, datasetInstall);
				result = stringWriter.ToString();
			}

			return result;
		}

		/// <summary>
		/// Converts an entity or derived entity to a dataset instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="type">The type.</param>
		/// <returns>Returns the dataset.</returns>
		/// <exception cref="ArgumentException">Thrown if the type does not inherit from <see cref="IdentifiedData"/>.</exception>
		private DatasetInstall ConvertToDatasetInternal(object instance, Type type)
		{
			if (!type.IsSubclassOf(typeof(IdentifiedData)))
			{
				throw new ArgumentException($"The type {type} must be a derived type of {typeof(IdentifiedData)}");
			}

			var datasetInstall = new DatasetInstall(Guid.NewGuid().ToString());

			datasetInstall.Action.Add(new DataUpdate
			{
				InsertIfNotExists = true,
				Element = (IdentifiedData)instance,
				IgnoreErrors = false
			});

			return datasetInstall;
		}
	}
}