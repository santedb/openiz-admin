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
 * User: nitya
 * Date: 2017-9-7
 */

using OpenIZ.Core.Model.RISI;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenIZAdmin.Services.Reports
{
	/// <summary>
	/// Represents a report service.
	/// </summary>
	public interface IReportService
	{
		/// <summary>
		/// Downloads the report source.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns a <see cref="Stream"/> containing the report source.</returns>
		Stream DownloadReportSource(Guid key);

		/// <summary>
		/// Gets all report definitions.
		/// </summary>
		/// <returns>Returns a list of report definitions.</returns>
		IEnumerable<ReportDefinition> GetAllReportDefinitions();
	}
}