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
 * Date: 2017-7-9
 */

using System.Runtime.CompilerServices;

namespace OpenIZAdmin.Core.Engine
{
	/// <summary>
	/// Represents the OpenIZ web engine context.
	/// </summary>
	public class OpenIZAdminEngineContext
	{
		/// <summary>
		/// Gets the current.
		/// </summary>
		/// <value>The current.</value>
		public static IOpenIZAdminEngine Current => Singleton<IOpenIZAdminEngine>.Current ?? (Singleton<IOpenIZAdminEngine>.Current = Initialize());

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <returns>IOpenIZWebEngine.</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static IOpenIZAdminEngine Initialize()
		{
			if (Singleton<IOpenIZAdminEngine>.Current != null)
				return Singleton<IOpenIZAdminEngine>.Current;

			Singleton<IOpenIZAdminEngine>.Current = new OpenIZAdminEngine();
			Singleton<IOpenIZAdminEngine>.Current.Initialize();

			return Singleton<IOpenIZAdminEngine>.Current;
		}
	}
}