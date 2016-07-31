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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;

namespace OpenIZAdmin.Services.Http
{
	/// <summary>
	/// Represents a Rest service for accessing Rest web endpoints.
	/// </summary>
	public class RestClientService : RestClientBase
	{
		/// <summary>
		/// The internal reference to the service client configuration section.
		/// </summary>
		private readonly ServiceClientConfigurationSection configuration = InternalConfiguration.GetServiceClientConfiguration();

		/// <summary>
		/// Initializes a new instance <see cref="OpenIZAdmin.Services.Http.RestClientService"/> class.
		/// </summary>
		public RestClientService(string endpointName) : base(InternalConfiguration.GetServiceClientConfiguration().Clients.Find(x => x.Name == endpointName))
		{
			Trace.TraceInformation(string.Format("Current Entity Source: {0}", EntitySource.Current));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.Services.Http.RestClientService"/> class with a specified <see cref="OpenIZ.Core.Http.Description.IRestClientDescription"/> instance.
		/// </summary>
		/// <param name="configuration"></param>
		public RestClientService(IRestClientDescription configuration) : base(configuration)
		{
			Trace.TraceInformation(string.Format("Current Entity Source: {0}", EntitySource.Current));
		}

		/// <summary>
		/// Creates and HTTP request.
		/// </summary>
		/// <param name="resourceName">The name of the resource being accessed.</param>
		/// <param name="query">The query parameters.</param>
		/// <returns></returns>
		protected override WebRequest CreateHttpRequest(string resourceName, params KeyValuePair<string, object>[] query)
		{
			var request = (HttpWebRequest)base.CreateHttpRequest(resourceName, query);

			// Certs?
			//if (this.ClientCertificates != null)
			//	request.ClientCertificates.AddRange(configuration.Client.cli);

			// Compress?
			if (this.Description.Binding.Optimize)
			{
				request.Headers.Add(HttpRequestHeader.ContentEncoding, "deflate gzip");
			}

			// Proxy?
			if (!string.IsNullOrEmpty(configuration.ProxyAddress))
			{
				request.Proxy = new WebProxy(configuration.ProxyAddress);
			}

			return request;
		}

		/// <summary>
		/// Invokes a request.
		/// </summary>
		/// <typeparam name="TBody">The type of the body of the request.</typeparam>
		/// <typeparam name="TResult">The type of the result of the request.</typeparam>
		/// <param name="method">The request method.</param>
		/// <param name="url">The URL of the request.</param>
		/// <param name="contentType">The content type of the request.</param>
		/// <param name="body">The body of the request.</param>
		/// <param name="query">The query parameters of the request.</param>
		/// <returns>Returns the response of the request.</returns>
		protected override TResult InvokeInternal<TBody, TResult>(string method, string url, string contentType, TBody body, params KeyValuePair<string, object>[] query)
		{
			if (string.IsNullOrEmpty(method))
			{
				throw new ArgumentNullException(nameof(method));
			}

			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentNullException(nameof(url));
			}

			var requestObj = this.CreateHttpRequest(url, query);

			if (!string.IsNullOrEmpty(contentType))
			{
				requestObj.ContentType = contentType;
			}

			requestObj.Method = method;

			// Body was provided?
			Credentials triedAuth = null;
			while (triedAuth != this.Credentials)
				try
				{
					// Try assigned credentials
					triedAuth = this.Credentials;
					IBodySerializer serializer = null;

					if (body != null)
					{
						if (contentType == null)
						{
							throw new ArgumentNullException(nameof(contentType));
						}

						serializer = this.Description.Binding.ContentTypeMapper.GetSerializer(contentType, typeof(TBody));

						// Serialize and compress with deflate
						if (this.Description.Binding.Optimize)
						{
							requestObj.Headers.Add(HttpRequestHeader.ContentEncoding, "deflate");
							using (var df = new DeflateStream(requestObj.GetRequestStream(), CompressionMode.Compress))
							{
								serializer.Serialize(df, body);
							}
						}
						else
						{
							serializer.Serialize(requestObj.GetRequestStream(), body);
						}
					}

					// Response
					var response = requestObj.GetResponse();
					var validationResult = this.ValidateResponse(response);
					if (validationResult != ServiceClientErrorType.Valid)
					{
						//this.m_tracer.TraceError("Response failed validation : {0}", validationResult);
						throw new WebException("Response failed validation", null, WebExceptionStatus.Success, response);
					}

					// De-serialize
					serializer = this.Description.Binding.ContentTypeMapper.GetSerializer(response.ContentType, typeof(TResult));

					EntitySource.Current = new EntitySource(new WebEntitySourceProvider());

					// Compression?
					switch (response.Headers[HttpResponseHeader.ContentEncoding])
					{
						case "deflate":
							using (DeflateStream df = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
							{
								return (TResult)serializer.DeSerialize(df);
							}
						case "gzip":
							using (GZipStream df = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
							{
								return (TResult)serializer.DeSerialize(df);
							}
						default:
							return (TResult)serializer.DeSerialize(response.GetResponseStream());
					}
				}
				catch (WebException e)
				{
					//this.m_tracer.TraceError(e.ToString());

					// status
					switch (e.Status)
					{
						case WebExceptionStatus.ProtocolError:

							// Deserialize
							var errorResponse = (e.Response as HttpWebResponse);
							var serializer = this.Description.Binding.ContentTypeMapper.GetSerializer(e.Response.ContentType, typeof(TResult));
							TResult result = (TResult)serializer.DeSerialize(e.Response.GetResponseStream());

							switch (errorResponse.StatusCode)
							{
								case HttpStatusCode.Unauthorized: // Validate the response
									if (this.ValidateResponse(errorResponse) != ServiceClientErrorType.Valid)
									{
										throw new RestClientException<TResult>(result, e, e.Status, e.Response);
									}
									break;

								default:
									throw new RestClientException<TResult>(result, e, e.Status, e.Response);
							}
							break;

						default:
							throw;
					}
				}
			return default(TResult);
		}
	}
}