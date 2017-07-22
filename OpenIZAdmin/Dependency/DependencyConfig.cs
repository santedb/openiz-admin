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
using Autofac.Integration.Mvc;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZ.Messaging.RISI.Client;
using OpenIZAdmin.Core.Auditing.Controllers;
using OpenIZAdmin.Core.Auditing.SecurityEntities;
using OpenIZAdmin.Core.Auditing.Services;
using OpenIZAdmin.Core.Caching;
using OpenIZAdmin.Core.Engine;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Services.Auditing;
using OpenIZAdmin.Services.Entities;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Metadata;
using OpenIZAdmin.Services.Security;
using System.Web;
using OpenIZAdmin.Core.Auditing.Core;
using OpenIZAdmin.Core.Auditing.Entities;
using OpenIZAdmin.Services.Core;
using OpenIZAdmin.Services.Entities.Materials;

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

			// register the audit service
			builder.RegisterType<AuditService>().As<IAuditService>().InstancePerLifetimeScope();

			// register additional auditing services
			builder.RegisterType<HttpContextAuditService>().As<ICoreAuditService>().WithParameter("context", HttpContext.Current).InstancePerLifetimeScope();
			builder.RegisterType<AuthenticationAuditService>().As<IAuthenticationAuditService>().WithParameter("context", HttpContext.Current).InstancePerLifetimeScope();
			builder.RegisterType<GlobalAuditService>().As<IGlobalAuditService>().WithParameter("context", HttpContext.Current).InstancePerLifetimeScope();
			builder.RegisterType<SecurityApplicationAuditService>().As<ISecurityEntityAuditService<SecurityApplication>>().WithParameter("context", HttpContext.Current).InstancePerLifetimeScope();
			builder.RegisterType<SecurityDeviceAuditService>().As<ISecurityEntityAuditService<SecurityDevice>>().WithParameter("context", HttpContext.Current).InstancePerLifetimeScope();
			builder.RegisterType<SecurityRoleAuditSerivce>().As<ISecurityEntityAuditService<SecurityRole>>().WithParameter("context", HttpContext.Current).InstancePerLifetimeScope();
			builder.RegisterType<SecurityUserAuditService>().As<ISecurityEntityAuditService<SecurityUser>>().WithParameter("context", HttpContext.Current).InstancePerLifetimeScope();
			builder.RegisterType<EntityAuditService>().As<IEntityAuditService>().WithParameter("context", HttpContext.Current).InstancePerLifetimeScope();

			// register security entity services
			builder.RegisterType<SecurityUserService>().As<ISecurityUserService>().InstancePerLifetimeScope();

			// register cache service
			builder.RegisterType<MemoryCacheService>().As<ICacheService>().InstancePerLifetimeScope();

			// register the concept service
			builder.RegisterType<ConceptService>().As<IConceptService>().InstancePerLifetimeScope();

			// register material concept services
			builder.RegisterType<MaterialConceptService>().As<IMaterialConceptService>().InstancePerLifetimeScope();

			// register user services
			builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();

			// register entity services
			builder.RegisterType<EntityService>().As<IEntityService>().InstancePerLifetimeScope();

			// register extension type services
			builder.RegisterType<ExtensionTypeService>().As<IExtensionTypeService>().InstancePerLifetimeScope();
		}
	}
}