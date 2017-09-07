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
 * Date: 2016-5-31
 */
using OpenIZAdmin.DAL;
using System.Data.Entity.Migrations;

namespace OpenIZAdmin.Migrations
{
	/// <summary>
	/// Represents the database configuration for the application. This class cannot be inherited.
	/// </summary>
	/// <seealso cref="ApplicationDbContext" />
	/// <seealso cref="ApplicationDbContext" />
	internal sealed class Configuration : DbMigrationsConfiguration<OpenIZAdmin.DAL.ApplicationDbContext>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Configuration"/> class.
		/// </summary>
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
			ContextKey = "OpenIZAdmin.DAL.ApplicationDbContext";
		}

		/// <summary>
		/// Runs after upgrading to the latest migration to allow seed data to be updated.
		/// </summary>
		/// <param name="context">Context to be used for updating seed data.</param>
		/// <remarks>Note that the database may already contain seed data when this method runs. This means that
		/// implementations of this method must check whether or not seed data is present and/or up-to-date
		/// and then only make changes if necessary and in a non-destructive way. The
		/// <see cref="M:System.Data.Entity.Migrations.DbSetMigrationsExtensions.AddOrUpdate``1(System.Data.Entity.IDbSet{``0},``0[])" />
		/// can be used to help with this, but for seeding large amounts of data it may be necessary to do less
		/// granular checks if performance is an issue.
		/// If the <see cref="T:System.Data.Entity.MigrateDatabaseToLatestVersion`2" /> database
		/// initializer is being used, then this method will be called each time that the initializer runs.
		/// If one of the <see cref="T:System.Data.Entity.DropCreateDatabaseAlways`1" />, <see cref="T:System.Data.Entity.DropCreateDatabaseIfModelChanges`1" />,
		/// or <see cref="T:System.Data.Entity.CreateDatabaseIfNotExists`1" /> initializers is being used, then this method will not be
		/// called and the Seed method defined in the initializer should be used instead.</remarks>
		protected override void Seed(OpenIZAdmin.DAL.ApplicationDbContext context)
		{
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method
			//  to avoid creating duplicate seed data. E.g.
			//
			//    context.People.AddOrUpdate(
			//      p => p.FullName,
			//      new Person { FullName = "Andrew Peters" },
			//      new Person { FullName = "Brice Lambson" },
			//      new Person { FullName = "Rowan Miller" }
			//    );
			//
		}
	}
}