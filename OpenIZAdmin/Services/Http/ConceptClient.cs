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
 * Date: 2017-3-25
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using OpenIZ.Core.Http;
using OpenIZ.Core.Interop;
using OpenIZ.Core.Interop.Clients;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Patch;
using OpenIZ.Core.Model.Query;

namespace OpenIZAdmin.Services.Http
{
	/// <summary>
	/// Represents the concept service client.
	/// </summary>
	public class ConceptClient : ServiceClientBase, IDisposable
	{
		/// <summary>
		/// The memory cache.
		/// </summary>
		private readonly MemoryCache MemoryCache = MvcApplication.MemoryCache;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptClient"/> class
		/// with a specific <see cref="IRestClient"/> instance.
		/// </summary>
		/// <param name="client">The <see cref="IRestClient"/> instance.</param>
		public ConceptClient(IRestClient client) : base(client)
		{
			this.Client.Accept = client.Accept ?? "application/xml";
		}

		/// <summary>
		/// Gets the concept.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Concept.</returns>
		public Concept GetConcept(Guid key)
		{
			// Resource name
			var resourceName = typeof(Concept).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

			// URL
			var url = new StringBuilder(resourceName);

			url.AppendFormat("/{0}", key);

			var concept = MvcApplication.MemoryCache.Get(key.ToString()) as Concept;

			if (concept == null)
			{
				// Optimize?
				if (this.Client.Description.Binding.Optimize)
				{
					var bundle = this.Client.Get<Bundle>(url.ToString(), new KeyValuePair<string, object>("_bundle", "true"));
					bundle.Reconstitute();
					concept = bundle.Entry as Concept;
				}
				else
				{
					concept = this.Client.Get<Concept>(url.ToString());
				}

				if (concept != null)
				{
					this.MemoryCache.Set(new CacheItem(concept.Key?.ToString(), concept), new CacheItemPolicy {SlidingExpiration = new TimeSpan(0, 0, 5, 0), Priority = CacheItemPriority.Default});
				}
			}

			return concept;
		}

		/// <summary>
		/// Gets the concepts by concept class.
		/// </summary>
		/// <param name="conceptClass">The concept class.</param>
		/// <returns>IEnumerable&lt;Concept&gt;.</returns>
		public IEnumerable<Concept> GetConceptsByConceptClass(Guid conceptClass)
		{
			var concepts = MvcApplication.MemoryCache.Get(conceptClass.ToString()) as IEnumerable<Concept>;

			if (concepts == null || concepts?.Any() == false)
			{
				var bundle = this.Query<Concept>(c => c.ClassKey == conceptClass && c.ObsoletionTime == null, 0, null, false);

				bundle.Reconstitute();

				concepts = bundle.Item.OfType<Concept>().Where(c => c.ClassKey == conceptClass && c.ObsoletionTime == null);

				this.MemoryCache.Set(new CacheItem(conceptClass.ToString(), concepts), new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 0, 5, 0), Priority = CacheItemPriority.Default });
			}

			return concepts;
		}

		/// <summary>
		/// Gets the concept set.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>ConceptSet.</returns>
		public ConceptSet GetConceptSet(Guid key)
		{
			// Resource name
			var resourceName = typeof(ConceptSet).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

			// URL
			var url = new StringBuilder(resourceName);

			url.AppendFormat("/{0}", key);

			var conceptSet = MvcApplication.MemoryCache.Get(key.ToString()) as ConceptSet;

			if (conceptSet == null)
			{
				// Optimize?
				if (this.Client.Description.Binding.Optimize)
				{
					var bundle = this.Client.Get<Bundle>(url.ToString(), new KeyValuePair<string, object>("_bundle", "true"));
					bundle.Reconstitute();
					conceptSet = bundle.Entry as ConceptSet;
				}
				else
				{
					conceptSet = this.Client.Get<ConceptSet>(url.ToString());
				}

				if (conceptSet != null)
				{
					this.MemoryCache.Set(new CacheItem(conceptSet.Key?.ToString(), conceptSet), new CacheItemPolicy {SlidingExpiration = new TimeSpan(0, 0, 5, 0), Priority = CacheItemPriority.Default});
				}
			}

			return conceptSet;
		}

		/// <summary>
		/// Performs a query.
		/// </summary>
		/// <typeparam name="TModel">The type of object to query.</typeparam>
		/// <param name="query">The query parameters as a LINQ expression.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query results.</param>
		/// <param name="all">Whether the query should return all nested properties.</param>
		/// <returns>Returns a Bundle containing the data.</returns>
		public Bundle Query<TModel>(Expression<Func<TModel, bool>> query, int offset, int? count, bool all) where TModel : IdentifiedData
		{
			// Map the query to HTTP parameters
			var queryParms = QueryExpressionBuilder.BuildQuery(query, true).ToList();

			queryParms.Add(new KeyValuePair<string, object>("_offset", offset));

			if (count.HasValue)
			{
				queryParms.Add(new KeyValuePair<string, object>("_count", count));
			}

			if (all)
			{
				queryParms.Add(new KeyValuePair<string, object>("_all", true));
			}

			// Resource name
			string resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

			// The IMSI uses the XMLName as the root of the request
			var retVal = this.Client.Get<Bundle>(resourceName, queryParms.ToArray());

			// Return value
			return retVal;
		}

		/// <summary>
		/// Performs a query.
		/// </summary>
		/// <typeparam name="TModel">The type of object to query.</typeparam>
		/// <param name="query">The query parameters as a LINQ expression.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query results.</param>
		/// <param name="expandProperties">An property traversal for which to expand upon.</param>
		/// <returns>Returns a Bundle containing the data.</returns>
		public Bundle Query<TModel>(Expression<Func<TModel, bool>> query, int offset, int? count, string expandProperties = null) where TModel : IdentifiedData
		{
			// Map the query to HTTP parameters
			var queryParms = QueryExpressionBuilder.BuildQuery(query, true).ToList();

			queryParms.Add(new KeyValuePair<string, object>("_offset", offset));

			if (count.HasValue)
			{
				queryParms.Add(new KeyValuePair<string, object>("_count", count));
			}

			if (!string.IsNullOrEmpty(expandProperties) && !string.IsNullOrWhiteSpace(expandProperties))
			{
				queryParms.Add(new KeyValuePair<string, object>("_expand", expandProperties));
			}

			// Resource name
			string resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

			// The IMSI uses the XMLName as the root of the request
			var retVal = this.Client.Get<Bundle>(resourceName, queryParms.ToArray());

			// Return value
			return retVal;
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
					this.Client?.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ImsiServiceClient() {
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
	}
}