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
		/// Initializes a new instance of the <see cref="Manual"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="fileSystemPath">The file system path.</param>
		public Manual(string name, string fileSystemPath) : this(Guid.NewGuid(), name, fileSystemPath)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Manual"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="fileSystemPath">The file system path.</param>
		public Manual(Guid id, string name, string fileSystemPath) : this(id)
		{
			this.FileSystemPath = fileSystemPath;
			this.Name = name;
		}

		/// <summary>
		/// Gets or sets the file system path.
		/// </summary>
		/// <value>The file system path.</value>
		[Required]
		[StringLength(255)]
		public string FileSystemPath { get; set; }

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