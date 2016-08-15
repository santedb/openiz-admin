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
 * Date: 2016-7-23
 */

using OpenIZ.Core.Http;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Services.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OpenIZAdmin.Services.Entity
{
	public class WebEntitySourceProvider : IEntitySourceProvider, IDisposable
	{
		private ImsiServiceClient serviceClient = new ImsiServiceClient(new RestClientService("IMSI"));

		public WebEntitySourceProvider(Credentials credentials)
		{
			this.serviceClient.Client.Credentials = credentials;
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					serviceClient?.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~WebEntitySourceProvider() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		/// <summary>
		/// Dispose of any resources.
		/// </summary>
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support

		public TObject Get<TObject>(Guid? key) where TObject : IdentifiedData, new()
		{
			var response = this.serviceClient.Get<TObject>(key.GetValueOrDefault(Guid.Empty), null);

			return response as TObject;
		}

		public TObject Get<TObject>(Guid? key, Guid? versionKey) where TObject : IdentifiedData, IVersionedEntity, new()
		{
			var response = this.serviceClient.Get<TObject>(key.GetValueOrDefault(Guid.Empty), versionKey);

			return response as TObject;
		}

		public IEnumerable<TObject> Query<TObject>(Expression<Func<TObject, bool>> query) where TObject : IdentifiedData, new()
		{
			var response = this.serviceClient.Query<TObject>(query);

			if (response == null)
			{
				return new List<TObject>().AsEnumerable();
			}

			return response as IEnumerable<TObject>;
		}

		public List<TObject> GetRelations<TObject>(Guid? sourceKey, decimal? sourceVersionSequence) where TObject : IdentifiedData, IVersionedAssociation, new()
		{
			return this.Query<TObject>(o => sourceKey == o.SourceEntityKey && sourceVersionSequence >= o.EffectiveVersionSequenceId && (o.ObsoleteVersionSequenceId == null || sourceVersionSequence < o.ObsoleteVersionSequenceId)).ToList();
		}
	}
}