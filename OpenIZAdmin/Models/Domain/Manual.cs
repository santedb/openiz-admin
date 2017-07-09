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
 * Date: 2017-6-4
 */

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenIZAdmin.Models.Domain
{
	/// <summary>
	/// Represents a manual.
	/// </summary>
	public class Manual
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Manual"/> class.
		/// </summary>
		public Manual() : this(Guid.NewGuid())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Manual"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		public Manual(Guid id)
		{
			this.Id = id;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Manual" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="content">The content.</param>
		public Manual(string name, string content) : this(Guid.NewGuid(), name, content)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Manual" /> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="content">The content.</param>
		public Manual(Guid id, string name, string content) : this(id)
		{
			this.Name = name;
			this.Content = content;
		}

		/// <summary>
		/// Gets or sets the content.
		/// </summary>
		/// <value>The content.</value>
		[Required]
		public string Content { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Required]
		[StringLength(50)]
		public string Name { get; set; }
	}
}