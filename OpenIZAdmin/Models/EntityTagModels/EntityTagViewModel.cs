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
 * Date: 2017-4-23
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZAdmin.Models.EntityTagModels
{
	/// <summary>
	/// Represents an entity tag view model.
	/// </summary>
	public class EntityTagViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityTagViewModel"/> class.
		/// </summary>
		public EntityTagViewModel()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityTagViewModel"/> class.
		/// </summary>
		/// <param name="tag">The tag.</param>
		public EntityTagViewModel(EntityTag tag)
		{
			this.EntityId = tag.SourceEntityKey.Value;
			this.TagKey = tag.TagKey;
			this.TagValue = tag.Value;
		}

		/// <summary>
		/// Gets or sets the entity identifier.
		/// </summary>
		/// <value>The entity identifier.</value>
		public Guid EntityId { get; set; }

		/// <summary>
		/// Gets or sets the tag key.
		/// </summary>
		/// <value>The tag key.</value>
		public string TagKey { get; set; }

		/// <summary>
		/// Gets or sets the tag value.
		/// </summary>
		/// <value>The tag value.</value>
		public string TagValue { get; set; }
	}
}