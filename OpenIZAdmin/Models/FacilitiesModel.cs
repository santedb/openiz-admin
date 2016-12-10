/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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

using OpenIZAdmin.Util;

namespace OpenIZAdmin.Models
{
	/// <summary>
	/// Represents a facilities model.
	/// </summary>
	public class FacilitiesModel
	{
		public FacilitiesModel()
		{
		}

		public FacilitiesModel(string name, string id)
		{
			if (CommonUtil.IsValidString(name))
				this.Name = name;
			else
				this.Name = string.Empty;

			if (CommonUtil.IsValidString(id))
				this.Id = id;
			else
				this.Id = string.Empty;
		}

		public string Id { get; set; }
		public string Name { get; set; }
	}
}