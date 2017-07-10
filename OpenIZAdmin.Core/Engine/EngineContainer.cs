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

using Autofac;
using System;
using Autofac.Core.Lifetime;
using Autofac.Integration.Mvc;
using System.Web;

namespace OpenIZAdmin.Core.Engine
{
	/// <summary>
	/// Represents an engine container.
	/// </summary>
	public class EngineContainer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EngineContainer"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		public EngineContainer(IContainer container)
		{
			this.Container = container;
		}

		/// <summary>
		/// Gets the container.
		/// </summary>
		/// <value>The container.</value>
		public IContainer Container { get; }

		/// <summary>
		/// Resolves this instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>T.</returns>
		public T Resolve<T>() where T : class
		{
			var scope = GetLifetimeScope();
			return scope.Resolve<T>();
		}

		/// <summary>
		/// Resolves the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>System.Object.</returns>
		public object Resolve(Type type)
		{
			var scope = GetLifetimeScope();
			return scope.Resolve(type);
		}

		/// <summary>
		/// Gets the lifetime scope.
		/// </summary>
		/// <returns>ILifetimeScope.</returns>
		public ILifetimeScope GetLifetimeScope()
		{
			try
			{
				if (HttpContext.Current != null)
					return AutofacDependencyResolver.Current.RequestLifetimeScope;

				return this.Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
			}
			catch (Exception)
			{
				return this.Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
			}
		}
	}
}