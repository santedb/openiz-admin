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
 * Date: 2016-7-13
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenIZAdmin.Models.Domain
{
	/// <summary>
	/// Represents a realm.
	/// </summary>
	public class Realm
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Models.Domain.Realm"/> class.
		/// </summary>
		public Realm() : this(DateTime.UtcNow, Guid.NewGuid())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Models.Domain.Realm"/> class
		/// with a specified creation time and id.
		/// </summary>
		public Realm(DateTime creationTime, Guid id)
		{
			this.CreationTime = creationTime;
			this.Id = id;
		}

		/// <summary>
		/// Gets or sets the address of the realm.
		/// </summary>
		[Required]
		[StringLength(255)]
		[Index(IsUnique = true)]
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets the application id.
		/// </summary>
		[Required]
		[StringLength(255)]
		public string ApplicationId { get; set; }

		/// <summary>
		/// Gets or sets the application secret.
		/// </summary>
		[Required]
		[StringLength(255)]
		public string ApplicationSecret { get; set; }

		/// <summary>
		/// Gets or sets the creation time of the realm.
		/// </summary>
		[Required]
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the id of the realm.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the obsoletion time of the realm.
		/// </summary>
		public DateTime? ObsoletionTime { get; set; }

		/// <summary>
		/// Gets or sets the list of users associated with the realm.
		/// </summary>
		public virtual ICollection<ApplicationUser> Users { get; set; }
	}
}