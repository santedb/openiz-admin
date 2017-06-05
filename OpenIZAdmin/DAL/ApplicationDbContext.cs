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
 * Date: 2016-7-10
 */

using Microsoft.AspNet.Identity.EntityFramework;
using OpenIZAdmin.Models.Domain;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace OpenIZAdmin.DAL
{
	/// <summary>
	/// Represents the database context for the application.
	/// </summary>
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.DAL.ApplicationDbContext"/> class.
		/// </summary>
		public ApplicationDbContext()
			: base("OpenIZAdminConnection", throwIfV1Schema: false)
		{
		}

		/// <summary>
		/// Gets or sets the manuals.
		/// </summary>
		/// <value>The manuals.</value>
		public DbSet<Manual> Manuals { get; set; }

		/// <summary>
		/// Gets or sets the Realm database set.
		/// </summary>
		public DbSet<Realm> Realm { get; set; }

		/// <summary>
		/// Creates a database context.
		/// </summary>
		/// <returns>Returns the newly created database context.</returns>
		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		/// <summary>
		/// Overrides the on model creating process for customization purposes.
		/// </summary>
		/// <param name="modelBuilder">The model builder reference.</param>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			base.OnModelCreating(modelBuilder);
		}
	}
}