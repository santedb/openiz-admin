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
 * Date: 2016-7-8
 */

namespace OpenIZAdmin.Models
{
	/// <summary>
	/// Represents a facilities model.
	/// </summary>
	public class FacilityModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FacilityModel"/> class.
		/// </summary>
		public FacilityModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FacilityModel"/> class.
		/// </summary>
		public FacilityModel(string name, string id)
		{
			this.Name = name;
			this.Id = id;
		}

		/// <summary>
		/// Gets or sets the identifier
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the string representation of the name
		/// </summary>
		public string Name { get; set; }
	}
}