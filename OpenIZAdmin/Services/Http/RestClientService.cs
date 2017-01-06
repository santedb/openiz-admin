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
using OpenIZ.Core.Http.Description;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZAdmin.Services.Entity;
using OpenIZAdmin.Services.Http.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Net;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;

namespace OpenIZAdmin.Services.Http
{
	/// <summary>
	/// Represents a REST service for accessing REST web endpoints.
	/// </summary>
	public class RestClientService : RestClientBase
	{
		/// <summary>
		/// The internal reference to the service client configuration section.
		/// </summary>
		//private readonly ServiceClientConfigurationSection configuration = InternalConfiguration.GetServiceClientConfiguration();

		// Poor man's cache
		private static readonly ConcurrentDictionary<string, Lazy<ServiceClientDescription>> endpoints = new ConcurrentDictionary<string, Lazy<ServiceClientDescription>>();

		/// <summary>
		/// Initializes a new instance <see cref="RestClientService"/> class
		/// with a specified endpoint name.
		/// </summary>
		/// <param name="endpointName">The name of the endpoint to use in the configuration.</param>
		public RestClientService(string endpointName) : base(endpoints.GetOrAdd(endpointName, 
			(key) => new Lazy<ServiceClientDescription>(
				() => InternalConfiguration.GetServiceClientConfiguration().Clients.Find(x => x.Name == key))).Value)
		{
			Trace.TraceInformation("Current Entity Source: {0}", EntitySource.Current.Provider.GetType().Name);

			this.Requesting += (o, e) =>
			{
				EntitySource.Current = new EntitySource(new WebEntitySourceProvider(this.Credentials));
			};
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RestClientService"/> class
		/// with a specified <see cref="IRestClientDescription"/> instance.
		/// </summary>
		/// <param name="configuration">The configuration instance.</param>
		public RestClientService(IRestClientDescription configuration) : base(configuration)
		{
			Trace.TraceInformation("Current Entity Source: {0}", EntitySource.Current.Provider.GetType().Name);
		}

		/// <summary>
		/// Invokes a request.
		/// </summary>
		/// <typeparam name="TBody">The type of the body of the request.</typeparam>
		/// <typeparam name="TResult">The type of the result of the request.</typeparam>
		/// <param name="method">The request method.</param>
		/// <param name="url">The URL of the request.</param>
		/// <param name="contentType">The content type of the request.</param>
		/// <param name="requestHeaders">Additional headers for the request.</param>
		/// <param name="body">The body of the request.</param>
		/// <param name="query">The query parameters of the request.</param>
		/// <returns>Returns the response of the request.</returns>
		protected override TResult InvokeInternal<TBody, TResult>(string method, string url, string contentType, WebHeaderCollection requestHeaders, out WebHeaderCollection responseHeaders, TBody body, NameValueCollection query)
		{

			if (String.IsNullOrEmpty(method))
				throw new ArgumentNullException(nameof(method));
			//if (String.IsNullOrEmpty(url))
			//    throw new ArgumentNullException(nameof(url));

			// Three times:
			// 1. With provided credential
			// 2. With challenge
			// 3. With challenge again
			for (int i = 0; i < 2; i++)
			{
				// Credentials provided ?

				var requestObj = this.CreateHttpRequest(url, query) as HttpWebRequest;

				if (!string.IsNullOrEmpty(contentType))
				{
					requestObj.ContentType = contentType;
				}
				requestObj.Method = method;

				// Additional headers
				if (requestHeaders != null)
				{
					foreach (var hdr in requestHeaders.AllKeys)
					{
						if (hdr == "If-Modified-Since")
						{
							requestObj.IfModifiedSince = DateTime.Parse(requestHeaders[hdr]);
						}
						else
						{
							requestObj.Headers.Add(hdr, requestHeaders[hdr]);
						}
					}
				}

				// Body was provided?
				try
				{

					// Try assigned credentials
					IBodySerializer serializer = null;
					if (body != null)
					{
						// GET Stream, 
						Stream requestStream = null;
						Exception requestException = null;

						try
						{
							//requestStream = requestObj.GetRequestStream();
							var requestTask = requestObj.GetRequestStreamAsync().ContinueWith(r =>
							{
								if (r.IsFaulted)
									requestException = r.Exception.InnerExceptions.First();
								else
									requestStream = r.Result;
							});

							if (!requestTask.Wait(this.Description.Endpoint[0].Timeout))
							{
								throw new TimeoutException();
							}
							else if (requestException != null) throw requestException;

							if (contentType == null && typeof(TResult) != typeof(Object))
								throw new ArgumentNullException(nameof(contentType));

							serializer = this.Description.Binding.ContentTypeMapper.GetSerializer(contentType, typeof(TBody));
							(body as IdentifiedData)?.SetDelayLoad(false);
							// Serialize and compress with deflate
							if (this.Description.Binding.Optimize)
							{
								requestObj.Headers.Add("Content-Encoding", "deflate");
								using (var df = new DeflateStream(requestStream, CompressionMode.Compress))
									serializer.Serialize(df, body);
							}
							else
								serializer.Serialize(requestStream, body);
						}
						finally
						{
							if (requestStream != null)
								requestStream.Dispose();
						}
					}

					// Response
					HttpWebResponse response = null;
					Exception responseError = null;

					try
					{
						var responseTask = requestObj.GetResponseAsync().ContinueWith(r =>
						{
							if (r.IsFaulted)
								responseError = r.Exception.InnerExceptions.First();
							else
								response = r.Result as HttpWebResponse;
						});
						if (!responseTask.Wait(this.Description.Endpoint[0].Timeout))
							throw new TimeoutException();
						else
						{
							if (responseError != null)
							{
								if (((responseError as WebException)?.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotModified)
								{
									responseHeaders = response?.Headers;
									return default(TResult);
								}
								else
									throw responseError;
							}
						}

						responseHeaders = response.Headers;
						var validationResult = this.ValidateResponse(response);
						if (validationResult != ServiceClientErrorType.Valid)
						{
							Trace.TraceError("Response failed validation : {0}", validationResult);
							throw new WebException("Response validation failed", null, WebExceptionStatus.Success, response);
						}

						// No content - does the result want a pointer maybe?
						if (response.StatusCode == HttpStatusCode.NoContent)
						{

							return default(TResult);
						}
						else
						{
							// De-serialize
							var responseContentType = response.ContentType;
							if (String.IsNullOrEmpty(responseContentType))
								return default(TResult);

							if (responseContentType.Contains(";"))
								responseContentType = responseContentType.Substring(0, responseContentType.IndexOf(";"));

							if (response.StatusCode == HttpStatusCode.NotModified)
								return default(TResult);

							serializer = this.Description.Binding.ContentTypeMapper.GetSerializer(responseContentType, typeof(TResult));

							TResult retVal = default(TResult);

							Trace.TraceInformation("Deserializing type: {0}", typeof(TResult).Name);

							// Compression?
							switch (response.Headers[HttpResponseHeader.ContentEncoding])
							{
								case "deflate":
									using (DeflateStream df = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
										retVal = (TResult)serializer.DeSerialize(df);
									break;
								case "gzip":
									using (GZipStream df = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
										retVal = (TResult)serializer.DeSerialize(df);
									break;
								default:
									retVal = (TResult)serializer.DeSerialize(response.GetResponseStream());
									break;
							}

							(retVal as IdentifiedData)?.SetDelayLoad(true);
							return retVal;
						}
					}
					finally
					{
						response?.Dispose();
					}
				}
				catch (TimeoutException e)
				{
					Trace.TraceError("Request timed out:{0}", e);
					throw;
				}
				catch (WebException e)
				{
					Trace.TraceError(e.ToString());

					// status
					switch (e.Status)
					{
						case WebExceptionStatus.ProtocolError:

							// Deserialize
							TResult result = default(TResult);
							var errorResponse = (e.Response as HttpWebResponse);
							var responseContentType = errorResponse.ContentType;
							if (responseContentType.Contains(";"))
								responseContentType = responseContentType.Substring(0, responseContentType.IndexOf(";"));
							var serializer = this.Description.Binding.ContentTypeMapper.GetSerializer(responseContentType, typeof(TResult));
							try
							{
								switch (errorResponse.Headers[HttpResponseHeader.ContentEncoding])
								{
									case "deflate":
										using (DeflateStream df = new DeflateStream(errorResponse.GetResponseStream(), CompressionMode.Decompress))
											result = (TResult)serializer.DeSerialize(df);
										break;
									case "gzip":
										using (GZipStream df = new GZipStream(errorResponse.GetResponseStream(), CompressionMode.Decompress))
											result = (TResult)serializer.DeSerialize(df);
										break;
									default:
										result = (TResult)serializer.DeSerialize(errorResponse.GetResponseStream());
										break;
								}
							}
							catch (Exception dse)
							{
								Trace.TraceError("Could not de-serialize error response! {0}", dse);
							}

							switch (errorResponse.StatusCode)
							{
								case HttpStatusCode.Unauthorized: // Validate the response
									if (this.ValidateResponse(errorResponse) != ServiceClientErrorType.Valid)
										throw new RestClientException<TResult>(
											result,
											e,
											e.Status,
											e.Response);

									break;
								default:
									throw new RestClientException<TResult>(
										result,
										e,
										e.Status,
										e.Response);
							}
							break;
						default:
							throw;
					}
				}

			}

			responseHeaders = new WebHeaderCollection();
			return default(TResult);
		}
	}
}