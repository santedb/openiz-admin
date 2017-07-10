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
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZ.Messaging.RISI.Client;
using OpenIZAdmin.Core.Caching;
using OpenIZAdmin.Core.Engine;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Services.Entities;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Metadata;

namespace OpenIZAdmin.Dependency
{
	/// <summary>
	/// Represents dependency configuration for the application.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Core.Engine.IDependencyRegistrar" />
	public class DependencyConfig : IDependencyRegistrar
	{
		/// <summary>
		/// Registers the specified builder.
		/// </summary>
		/// <param name="builder">The builder.</param>
		public void Register(ContainerBuilder builder)
		{
			// register controllers
			builder.RegisterControllers(typeof(MvcApplication).Assembly);
			builder.RegisterModule<AutofacWebTypesModule>();

			// register service clients with parameters
			builder.RegisterType<RestClientService>().Named<RestClientService>(Constants.Imsi)
				.WithParameter("endpointName", Constants.Imsi)
				.InstancePerLifetimeScope();

			builder.RegisterType<RestClientService>().Named<RestClientService>(Constants.Ami)
				.WithParameter("endpointName", Constants.Ami)
				.InstancePerLifetimeScope();

			builder.RegisterType<RestClientService>().Named<RestClientService>(Constants.Risi)
				.WithParameter("endpointName", Constants.Risi)
				.InstancePerLifetimeScope();

			// register service clients
			builder.Register(ctx => new AmiServiceClient(ctx.ResolveNamed<RestClientService>(Constants.Ami))).InstancePerLifetimeScope();
			builder.Register(ctx => new ImsiServiceClient(ctx.ResolveNamed<RestClientService>(Constants.Imsi))).InstancePerLifetimeScope();
			builder.Register(ctx => new RisiServiceClient(ctx.ResolveNamed<RestClientService>(Constants.Risi))).InstancePerLifetimeScope();

			// register the database context
			builder.RegisterType<ApplicationDbContext>().InstancePerLifetimeScope();

			// register cache service
			builder.RegisterType<MemoryCacheService>().As<ICacheService>().InstancePerLifetimeScope();

			// register the concept service
			builder.RegisterType<ConceptService>().As<IConceptService>().InstancePerLifetimeScope();

			// register material service
			builder.RegisterType<MaterialService>().As<IEntityService<Material>>().InstancePerLifetimeScope();
			builder.RegisterType<MaterialService>().As<IMaterialService>().InstancePerLifetimeScope();
		}
	}
}