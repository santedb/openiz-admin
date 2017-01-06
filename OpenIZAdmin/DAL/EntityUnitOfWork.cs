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
 * Date: 2016-7-14
 */

using OpenIZAdmin.Models.Domain;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Elmah;

namespace OpenIZAdmin.DAL
{
	/// <summary>
	/// Defines repositories for accessing data.
	/// </summary>
	public class EntityUnitOfWork : IUnitOfWork
	{
		/// <summary>
		/// The application database context.
		/// </summary>
		private ApplicationDbContext context;

		#region Repositories

		/// <summary>
		/// The internal reference to the realm repository.
		/// </summary>
		private IRepository<Realm> realmRepository;

		/// <summary>
		/// The internal reference to the application user repository.
		/// </summary>
		private IRepository<ApplicationUser> userRepository;

		#endregion Repositories

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.DAL.EntityUnitOfWork"/> class.
		/// </summary>
		public EntityUnitOfWork()
			: this(new ApplicationDbContext())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.DAL.EntityUnitOfWork"/> class with a specified context.
		/// </summary>
		/// <param name="context">The application context.</param>
		public EntityUnitOfWork(ApplicationDbContext context)
		{
			this.context = context;
		}

		/// <summary>
		/// Save any pending changes to the database.
		/// </summary>
		public void Save()
		{
			context.SaveChanges();
		}

		/// <summary>
		/// Save any pending changes to the database.
		/// </summary>
		/// <returns>Returns a task.</returns>
		public async Task SaveAsync()
		{
			await context.SaveChangesAsync();
		}

		#region Repositories

		/// <summary>
		/// The repository for accessing realms.
		/// </summary>
		public IRepository<Realm> RealmRepository
		{
			get
			{
				if (this.realmRepository == null)
				{
					this.realmRepository = new EntityRepository<Realm>(context);
				}

				return realmRepository;
			}
		}

		/// <summary>
		/// The repository for accessing users.
		/// </summary>
		public IRepository<ApplicationUser> UserRepository
		{
			get
			{
				if (this.userRepository == null)
				{
					this.userRepository = new EntityRepository<ApplicationUser>(context);
				}

				return userRepository;
			}
		}

		#endregion Repositories

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Is the process currently disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.context?.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~EntityUnitOfWork() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}