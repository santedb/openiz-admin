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
 * Date: 2016-7-13
 */

using OpenIZAdmin.Models.RealmModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenIZAdmin.Models.Domain
{
	public class Realm
	{
		public Realm() : this(DateTime.UtcNow, Guid.NewGuid())
		{

		}

		public Realm(DateTime creationTime, Guid id)
		{
			this.CreationTime = creationTime;
			this.Id = id;
		}

		[Required]
		[StringLength(255)]
		public string Address { get; set; }

		[Url]
		[Required]
		public string AmiAuthEndpoint { get; set; }

		[Url]
		[Required]
		[StringLength(255)]
		[Index(IsUnique = true)]
		public string AmiEndpoint { get; set; }

		[Required]
		[StringLength(255)]
		public string ApplicationId { get; set; }

		[Required]
		[StringLength(255)]
		public string ApplicationSecret { get; set; }

		[Required]
		public DateTime CreationTime { get; set; }

		[StringLength(255)]
		public string Description { get; set; }

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		public DateTime? ObsoletionTime { get; set; }

		public virtual ICollection<ApplicationUser> Users { get; set; }
	}
}