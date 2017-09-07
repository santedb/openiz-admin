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
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZ.Messaging.RISI.Client;
using OpenIZAdmin.Core.Auditing.Controllers;
using OpenIZAdmin.Core.Auditing.Core;
using OpenIZAdmin.Core.Auditing.Entities;
using OpenIZAdmin.Core.Auditing.SecurityEntities;
using OpenIZAdmin.Core.Auditing.Services;
using OpenIZAdmin.Core.Caching;
using OpenIZAdmin.Core.Engine;
using OpenIZAdmin.DAL;
using OpenIZAdmin.DAL.Manuals;
using OpenIZAdmin.Services.Applets;
using OpenIZAdmin.Services.Auditing;
using OpenIZAdmin.Services.Core;
using OpenIZAdmin.Services.Entities.ManufacturedMaterials;
using OpenIZAdmin.Services.Entities.Materials;
using OpenIZAdmin.Services.Entities.Places;
using OpenIZAdmin.Services.EntityRelationships;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Metadata;
using OpenIZAdmin.Services.Metadata.CodeSystems;
using OpenIZAdmin.Services.Metadata.Concepts;
using OpenIZAdmin.Services.Metadata.ExtensionTypes;
using OpenIZAdmin.Services.Reports;
using OpenIZAdmin.Services.Security.Applications;
using OpenIZAdmin.Services.Security.Devices;
using OpenIZAdmin.Services.Security.Policies;
using OpenIZAdmin.Services.Security.Roles;
using OpenIZAdmin.Services.Security.Users;
using OpenIZAdmin.Services.Server;

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
			builder.RegisterType<HttpContextAuditService>().As<ICoreAuditService>().InstancePerLifetimeScope();
			builder.RegisterType<AuthenticationAuditService>().As<IAuthenticationAuditService>().InstancePerLifetimeScope();
			builder.RegisterType<GlobalAuditService>().As<IGlobalAuditService>().InstancePerLifetimeScope();
			builder.RegisterType<SecurityApplicationAuditService>().As<ISecurityEntityAuditService<SecurityApplication>>().InstancePerLifetimeScope();
			builder.RegisterType<SecurityDeviceAuditService>().As<ISecurityEntityAuditService<SecurityDevice>>().InstancePerLifetimeScope();
			builder.RegisterType<SecurityPolicyAuditService>().As<ISecurityPolicyAuditService>().InstancePerLifetimeScope();
			builder.RegisterType<SecurityRoleAuditService>().As<ISecurityEntityAuditService<SecurityRole>>().InstancePerLifetimeScope();
			builder.RegisterType<SecurityUserAuditService>().As<ISecurityEntityAuditService<SecurityUser>>().InstancePerLifetimeScope();
			builder.RegisterType<EntityAuditService>().As<IEntityAuditService>().InstancePerLifetimeScope();

			// register security entity services
			builder.RegisterType<SecurityApplicationService>().As<ISecurityApplicationService>().InstancePerLifetimeScope();
			builder.RegisterType<SecurityDeviceService>().As<ISecurityDeviceService>().InstancePerLifetimeScope();
			builder.RegisterType<SecurityPolicyService>().As<ISecurityPolicyService>().InstancePerLifetimeScope();
			builder.RegisterType<SecurityRoleService>().As<ISecurityRoleService>().InstancePerLifetimeScope();
			builder.RegisterType<SecurityUserService>().As<ISecurityUserService>().InstancePerLifetimeScope();

			// register cache service
			builder.RegisterType<MemoryCacheService>().As<ICacheService>().InstancePerLifetimeScope();

			// register the concept service
			builder.RegisterType<ConceptService>().As<IConceptService>().InstancePerLifetimeScope();

			// register entity concept services
			builder.RegisterType<ManufacturedMaterialConceptService>().As<IManufacturedMaterialConceptService>().InstancePerLifetimeScope();
			builder.RegisterType<MaterialConceptService>().As<IMaterialConceptService>().InstancePerLifetimeScope();
			builder.RegisterType<PlaceConceptService>().As<IPlaceConceptService>().InstancePerLifetimeScope();

			// register user services
			builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();

			// register entity services
			builder.RegisterType<EntityService>().As<IEntityService>().InstancePerLifetimeScope();

			// register metadata services (extension type, code system, assigning authority, etc.)
			builder.RegisterType<CodeSystemService>().As<ICodeSystemService>().InstancePerLifetimeScope();
			builder.RegisterType<ExtensionTypeService>().As<IExtensionTypeService>().InstancePerLifetimeScope();

			// register applet services
			builder.RegisterType<AppletService>().As<IAppletService>().InstancePerLifetimeScope();

			// register entity relationship services
			builder.RegisterType<EntityRelationshipService>().As<IEntityRelationshipService>().InstancePerLifetimeScope();

			// register server information services
			builder.RegisterType<AmiServerInformationService>().As<IAmiServerInformationService>().InstancePerLifetimeScope();
			builder.RegisterType<ImsiServerInformationService>().As<IImsiServerInformationService>().InstancePerLifetimeScope();

			// register entity relationship concept services
			builder.RegisterType<EntityRelationshipConceptService>().As<IEntityRelationshipConceptService>().InstancePerLifetimeScope();

			// register manual services
			builder.RegisterType<ManualService>().As<IManualService>().InstancePerLifetimeScope();

			// register report services
			builder.RegisterType<ReportService>().As<IReportService>().InstancePerLifetimeScope();
		}
	}
}