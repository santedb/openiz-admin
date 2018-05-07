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

namespace OpenIZAdmin.Services.Dataset
{
	/// <summary>
	/// Represents a dataset service.
	/// </summary>
	public interface IDatasetService
	{
		/// <summary>
		/// Converts an identified data instance to a dataset instance.
		/// </summary>
		/// <typeparam name="T">The type of instance to convert to a dataset.</typeparam>
		/// <param name="instance">The instance.</param>
		/// <returns>Returns the dataset.</returns>
		DatasetInstall ConvertToDataset<T>(T instance) where T : IdentifiedData;

		/// <summary>
		/// Converts an entity or derived entity to a dataset instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="type">The type.</param>
		/// <returns>Returns the dataset.</returns>
		DatasetInstall ConvertToDataset(object instance, Type type);

		/// <summary>
		/// Gets the dataset as stream.
		/// </summary>
		/// <param name="datasetInstall">The dataset install.</param>
		/// <returns>Returns the dataset as a <see cref="Stream"/>.</returns>
		Stream GetDatasetAsStream(DatasetInstall datasetInstall);

		/// <summary>
		/// Serializes the specified dataset install.
		/// </summary>
		/// <param name="datasetInstall">The dataset install.</param>
		/// <returns>Returns the dataset install as an XML string.</returns>
		string Serialize(DatasetInstall datasetInstall);
	}
}