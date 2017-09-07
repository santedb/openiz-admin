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
 * Date: 2017-9-7
 */

using OpenIZ.Core.Model.RISI;
using OpenIZ.Messaging.RISI.Client;
using OpenIZAdmin.Services.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenIZAdmin.Services.Reports
{
	/// <summary>
	/// Represents a report service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.RisiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.Reports.IReportService" />
	public class ReportService : RisiServiceBase, IReportService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportService"/> class.
		/// </summary>
		/// <param name="client">The client.</param>
		public ReportService(RisiServiceClient client) : base(client)
		{
		}

		/// <summary>
		/// Downloads the report source.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns a <see cref="Stream" /> containing the report source.</returns>
		public Stream DownloadReportSource(Guid key)
		{
			return this.Client.GetReportSource(key);
		}

		/// <summary>
		/// Gets all report definitions.
		/// </summary>
		/// <returns>Returns a list of report definitions.</returns>
		public IEnumerable<ReportDefinition> GetAllReportDefinitions()
		{
			return this.Client.GetReportDefinitions().Items;
		}
	}
}