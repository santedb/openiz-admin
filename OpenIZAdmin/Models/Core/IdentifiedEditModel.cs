﻿/*
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

using System;
using System.ComponentModel.DataAnnotations;

namespace OpenIZAdmin.Models.Core
{
	/// <summary>
	/// Represents an identifiable model.
	/// </summary>
	public abstract class IdentifiedEditModel : IIdentifiable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IdentifiedEditModel"/> class.
		/// </summary>
		protected IdentifiedEditModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IdentifiedEditModel"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		protected IdentifiedEditModel(Guid id)
		{
			this.Id = id;
		}

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[Required]
		public Guid Id { get; set; }
	}
}