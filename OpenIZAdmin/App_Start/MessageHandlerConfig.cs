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
using OpenIZAdmin.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace OpenIZAdmin
{
	/// <summary>
	/// Represents message handling configuration for the application.
	/// </summary>
	public static class MessageHandlerConfig
	{
		/// <summary>
		/// Registers message handlers for the application.
		/// </summary>
		/// <param name="config">The http configuration for which to add the message handlers.</param>
		public static void Register(HttpConfiguration config)
		{
			config.MessageHandlers.Add(new LogMessageHandler());
		}
	}
}