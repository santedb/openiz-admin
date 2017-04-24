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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.Entities;

namespace OpenIZAdmin.Models.Core
{
	/// <summary>
	/// Represents a verify entity model.
	/// </summary>
	public class VerifyEntityModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VerifyEntityModel"/> class.
		/// </summary>
		public VerifyEntityModel()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="VerifyEntityModel" /> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="type">The type.</param>
		/// <value>The unique identifier.</value>
		public VerifyEntityModel(Guid id, string type)
		{
			this.Id = id;
			this.Type = type;
		}

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[Required]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		[Required]
		public string Type { get; set; }
	}
}