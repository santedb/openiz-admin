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
 * Date: 2016-7-10
 */

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenIZAdmin.Models.Domain
{
	/// <summary>
	/// Represents an application user.
	/// </summary>
	public class ApplicationUser : IdentityUser
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationUser"/> class.
		/// </summary>
		public ApplicationUser()
		{
			this.Language = "en";
		}

		/// <summary>
		/// Gets or sets the language of the application user.
		/// </summary>
		[StringLength(2)]
		public string Language { get; set; }

		/// <summary>
		/// Gets or sets the realm of the application user.
		/// </summary>
		[ForeignKey("RealmId")]
		public virtual Realm Realm { get; set; }

		/// <summary>
		/// Gets or sets the id of the realm of the application user.
		/// </summary>
		[Required]
		public Guid RealmId { get; set; }

		/// <summary>
		/// Generates a user identity.
		/// </summary>
		/// <param name="manager">The <see cref="UserManager{TUser}"/> instance.</param>
		/// <returns>Returns a <see cref="Task{ClaimsIdentity}"/> instance.</returns>
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}
	}
}