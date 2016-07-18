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
 * Date: 2016-7-11
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

namespace OpenIZAdmin.Services
{
	public class RestClient : IDisposable
	{
		private readonly HttpClient client;
		private readonly Uri baseUrl;
		private MediaTypeFormatter mediaTypeFormatter;

		public RestClient(string baseUrl, Credentials credentials) : this(new Uri(baseUrl), credentials)
		{
		}

		public RestClient(Uri baseUrl, Credentials credentials, MediaTypeFormatter mediaTypeFormatter = null)
		{
			this.baseUrl = baseUrl;

			if (credentials == null)
			{
				throw new ArgumentNullException(string.Format("{0} cannot be null", nameof(credentials)));
			}

			this.Credentials = credentials;

			this.client = new HttpClient();

			foreach (var header in this.Credentials.GetHttpHeaders())
			{
				client.DefaultRequestHeaders.Add(header.Key, header.Value);
			}

			this.mediaTypeFormatter = mediaTypeFormatter ?? new XmlMediaTypeFormatter { UseXmlSerializer = true };
		}

		public Credentials Credentials { get; }

		/// <summary>
		/// Create the query string from a list of query parameters.
		/// </summary>
		/// <param name="query">The query parameters.</param>
		/// <returns>Returns a string of query parameters.</returns>
		public static string CreateQueryString(params KeyValuePair<string, object>[] query)
		{
			string queryString = string.Empty;
			foreach (var kv in query)
			{
				queryString += String.Format("{0}={1}", kv.Key, Uri.EscapeDataString(kv.Value.ToString()));

				if (!kv.Equals(query.Last()))
				{
					queryString += "&";
				}
			}

			return queryString;
		}

		public async Task<HttpResponseMessage> DeleteAsync(string path)
		{
			return await this.client.DeleteAsync(string.Format("{0}/{1}", this.baseUrl, path));
		}

		public async Task<HttpResponseMessage> DeleteAsync(string path, KeyValuePair<string, object> parameters)
		{
			var response = await client.DeleteAsync(string.Format("{0}/{1}?{2}", this.baseUrl, path, CreateQueryString(parameters)));

			return response;
		}

		public async Task<T> DeleteAsync<T>(string path, KeyValuePair<string, object> parameters) where T : class
		{
			var response = await client.DeleteAsync(string.Format("{0}/{1}?{2}", this.baseUrl, path, CreateQueryString(parameters)));

			return await response.Content.ReadAsAsync<T>(new List<MediaTypeFormatter> { this.mediaTypeFormatter });
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.client?.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~RestClient() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion

		public async Task<HttpResponseMessage> GetAsync(string path)
		{
			return await this.client.GetAsync(string.Format("{0}/{1}", this.baseUrl, path));
		}

		public async Task<T> GetAsync<T>(string path) where T : class
		{
			return await this.GetAsync<T>(path, new Dictionary<string, object>());
		}

		public async Task<T> GetAsync<T>(string path, IDictionary<string, object> parameters) where T : class
		{
			HttpResponseMessage response;

			if (parameters.Count == 0)
			{
				response = await client.GetAsync(string.Format("{0}/{1}", this.baseUrl, path));
			}
			else
			{
				response = await client.GetAsync(string.Format("{0}/{1}?{2}", this.baseUrl, path, CreateQueryString(parameters.ToArray())));
			}

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsAsync<T>(new List<MediaTypeFormatter> { this.mediaTypeFormatter });
			}
			else
			{
				return null;
			}
		}

		public async Task<TResult> PostAsync<T, TResult>(string path, IDictionary<string, object> parameters, T content) where TResult : class
		{
			HttpResponseMessage response;

			if (parameters.Count == 0)
			{
				response = await this.client.PostAsync<T>(string.Format("{0}/{1}", this.baseUrl, path), content, this.mediaTypeFormatter);
			}
			else
			{
				response = await this.client.PostAsync<T>(string.Format("{0}/{1}?{2}", this.baseUrl, path, CreateQueryString(parameters.ToArray())), content, this.mediaTypeFormatter);
			}

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsAsync<TResult>(new List<MediaTypeFormatter> { this.mediaTypeFormatter });
			}
			else
			{
				return null;
			}
		}

		public async Task<TResult> PostAsync<T, TResult>(string path, T content) where TResult : class
		{
			return await this.PostAsync<T, TResult>(path, new Dictionary<string, object>(), content);
		}

		public async Task<HttpResponseMessage> PostAsync<T>(string path, T content)
		{
			return await this.client.PostAsync<T>(string.Format("{0}/{1}", this.baseUrl, path), content, this.mediaTypeFormatter);
		}

		public async Task<HttpResponseMessage> PutAsync(string path)
		{
			return await this.client.PutAsync(string.Format("{0}/{1}", this.baseUrl, path), new StringContent(string.Empty));
		}

		public async Task<TResult> PutAsync<T, TResult>(string path, IDictionary<string, object> parameters, T content) where TResult : class
		{
			HttpResponseMessage response;

			if (parameters.Count == 0)
			{
				response = await this.client.PutAsync<T>(string.Format("{0}/{1}", this.baseUrl, path), content, this.mediaTypeFormatter);
			}
			else
			{
				response = await client.PostAsync<T>(string.Format("{0}/{1}?{2}", this.baseUrl, path, CreateQueryString(parameters.ToArray())), content, this.mediaTypeFormatter);
			}

			return await response.Content.ReadAsAsync<TResult>();
		}

		public async Task<TResult> PutAsync<T, TResult>(string path, T content) where TResult : class
		{
			return await this.PutAsync<T, TResult>(path, new Dictionary<string, object>(), content);
		}
	}
}