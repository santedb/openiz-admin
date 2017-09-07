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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace OpenIZAdmin.Core.Engine
{
	/// <summary>
	/// Represents an OpenIZ admin engine.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Core.Engine.IOpenIZAdminEngine" />
	public class OpenIZAdminEngine : IOpenIZAdminEngine
	{
		/// <summary>
		/// The container manager.
		/// </summary>
		private EngineContainer containerManager;

		/// <summary>
		/// Gets loadable types in an assembly.
		/// Works around <see cref="ReflectionTypeLoadException"/> by filtering out types that could not be loaded.
		/// </summary>
		/// <param name="assembly">The assembly to load the types from.</param>
		/// <returns></returns>
		public static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
		{
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				return e.Types.Where(t => t != null);
			}
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public void Initialize()
		{
			var builder = new ContainerBuilder();

			var dependencyRegistrars = AppDomain.CurrentDomain.GetAssemblies().SelectMany(GetLoadableTypes)
				.Where(type => typeof(IDependencyRegistrar).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
				.ToList();

			foreach (var registrarType in dependencyRegistrars)
			{
				var registrar = (IDependencyRegistrar)Activator.CreateInstance(registrarType);
				registrar.Register(builder);
			}

			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
			containerManager = new EngineContainer(container);
		}

		/// <summary>
		/// Resolves this instance.
		/// </summary>
		/// <typeparam name="T">The type of instance to resolve.</typeparam>
		/// <returns>Returns an instance of the type to resolve.</returns>
		public T Resolve<T>() where T : class
		{
			return containerManager.Resolve<T>();
		}

		/// <summary>
		/// Resolves the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>Returns an instance of the type to resolve.</returns>
		public object Resolve(Type type)
		{
			return containerManager.Resolve(type);
		}
	}
}