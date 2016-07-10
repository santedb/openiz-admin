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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Tracing;

namespace OpenIZAdmin.MessageHandlers
{
	/// <summary>
	/// Represents a message handler for logging requests and responses.
	/// </summary>
	public class LogMessageHandler : DelegatingHandler
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.MessageHandlers.LogMessageHandler"/> class.
		/// </summary>
		public LogMessageHandler()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZAdmin.MessageHandlers.LogMessageHandler"/> class with the specified <see cref="System.Net.Http.HttpMessageHandler"/> instance.
		/// </summary>
		/// <param name="innerHandler">The instance of the HTTP message handler.</param>
		protected LogMessageHandler(HttpMessageHandler innerHandler) : base(innerHandler)
		{
		}

		/// <summary>
		/// Sends the request asynchronously.
		/// </summary>
		/// <param name="request">The HTTP request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>Returns a task with the HTTP response message.</returns>
		protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			this.LogMessage(request);
			return await base.SendAsync(request, cancellationToken);
		}

		/// <summary>
		/// Logs a message for a specific HTTP request.
		/// </summary>
		/// <param name="request">The request to log.</param>
		private void LogMessage(HttpRequestMessage request)
		{
#if DEBUG
			Trace.TraceInformation("{0} {1} {2}", new object[] { nameof(LogMessageHandler), request.Method.ToString(), request.RequestUri.PathAndQuery });
#endif
			Trace.TraceInformation("{0} {1} {2}", new object[] { nameof(LogMessageHandler), request.Method.ToString(), request.RequestUri.GetLeftPart(UriPartial.Path) });
		}
	}
}