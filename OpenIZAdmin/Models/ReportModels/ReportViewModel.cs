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
 * User: Nityan
 * Date: 2017-4-17
 */

using OpenIZ.Core.Model.RISI;
using System;

namespace OpenIZAdmin.Models.ReportModels
{
	/// <summary>
	/// Represents a report view model.
	/// </summary>
	public class ReportViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportViewModel"/> class.
		/// </summary>
		public ReportViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportViewModel" /> class.
		/// </summary>
		/// <param name="reportDefinition">The report definition.</param>
		public ReportViewModel(ReportDefinition reportDefinition)
		{
			this.Id = reportDefinition.Key.Value;
			this.Name = reportDefinition.Name;
			this.Description = reportDefinition.Description;
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }
	}
}