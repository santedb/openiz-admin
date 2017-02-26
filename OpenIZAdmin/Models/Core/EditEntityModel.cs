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
 * Date: 2017-2-20
 */

using OpenIZ.Core.Model.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.Core
{
	/// <summary>
	/// Represents an edit entity model.
	/// </summary>
	public abstract class EditEntityModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditEntityModel"/> class.
		/// </summary>
		protected EditEntityModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditEntityModel"/> class
		/// with a specific <see cref="Entity"/> instance.
		/// </summary>
		/// <param name="entity">The <see cref="Entity"/> instance.</param>
		protected EditEntityModel(Entity entity)
		{
			this.Id = entity.Key.Value;
		}

		/// <summary>
		/// Gets or sets the id of the entity.
		/// </summary>
		[Required]
		public Guid Id { get; set; }
	}
}