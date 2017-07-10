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

using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Core.Auditing.Core;
using OpenIZAdmin.Core.Auditing.Entities;
using OpenIZAdmin.Core.Caching;
using OpenIZAdmin.Services.Core;
using OpenIZAdmin.Services.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OpenIZAdmin.Services.Entities
{
	/// <summary>
	/// Represents a material service.
	/// </summary>
	/// <seealso cref="Material" />
	public class MaterialService : EntityServiceBase<Material>, IMaterialService
	{
		/// <summary>
		/// The material types mnemonic.
		/// </summary>
		private const string MaterialTypesMnemonic = "MaterialTypes";

		/// <summary>
		/// The cache service.
		/// </summary>
		private readonly ICacheService cacheService;

		/// <summary>
		/// The concept service.
		/// </summary>
		private readonly IConceptService conceptService;

		/// <summary>
		/// The core audit service.
		/// </summary>
		private readonly ICoreAuditService coreAuditService;

		/// <summary>
		/// The entity audit service.
		/// </summary>
		private readonly IEntityAuditService entityAuditService;

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialService" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="cacheService">The cache service.</param>
		/// <param name="conceptService">The concept service.</param>
		/// <param name="coreAuditService">The core audit service.</param>
		public MaterialService(ImsiServiceClient client, ICacheService cacheService, IConceptService conceptService, ICoreAuditService coreAuditService, IEntityAuditService entityAuditService) : base(client, conceptService, coreAuditService, entityAuditService)
		{
			this.cacheService = cacheService;
			this.conceptService = conceptService;
			this.coreAuditService = coreAuditService;
			this.entityAuditService = entityAuditService;
		}

		/// <summary>
		/// Gets the form concepts.
		/// </summary>
		/// <returns>Returns a list of form concepts.</returns>
		public IEnumerable<Concept> GetFormConcepts()
		{
			return cacheService.Get(ConceptClassKeys.Form.ToString(), () =>
			{
				var bundle = this.Client.Query<Concept>(c => c.ClassKey == ConceptClassKeys.Form && c.ObsoletionTime == null);

				bundle.Reconstitute();

				return bundle.Item.OfType<Concept>().Where(c => c.ClassKey == ConceptClassKeys.Form && c.ObsoletionTime == null);
			});
		}

		/// <summary>
		/// Gets the material type concepts.
		/// </summary>
		/// <returns>Returns a list of concepts which are a part of the material type concept set.</returns>
		public IEnumerable<Concept> GetMaterialTypeConcepts()
		{
			return this.cacheService.Get<IEnumerable<Concept>>(MaterialTypesMnemonic, () =>
			{
				var bundle = this.Client.Query<ConceptSet>(c => c.Mnemonic == MaterialTypesMnemonic && c.ObsoletionTime == null, 0, null, new[] { "concept" });

				bundle.Reconstitute();

				var conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Mnemonic == MaterialTypesMnemonic && c.ObsoletionTime == null);

				var concepts = new List<Concept>();

				if (conceptSet != null)
				{
					concepts.AddRange(conceptSet.ConceptsXml.Select(c => this.conceptService.GetConcept(c)).ToList());
				}

				return concepts;
			});
		}

		/// <summary>
		/// Gets the quantity concepts.
		/// </summary>
		/// <returns>Returns a list of quantity concepts.</returns>
		public IEnumerable<Concept> GetQuantityConcepts()
		{
			return cacheService.Get(ConceptClassKeys.UnitOfMeasure.ToString(), () =>
			{
				var bundle = this.Client.Query<Concept>(c => c.ClassKey == ConceptClassKeys.UnitOfMeasure && c.ObsoletionTime == null);

				bundle.Reconstitute();

				return bundle.Item.OfType<Concept>().Where(c => c.ClassKey == ConceptClassKeys.UnitOfMeasure && c.ObsoletionTime == null);
			});
		}

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		public override IEnumerable<Material> Query(Expression<Func<Material, bool>> expression)
		{
			var materials = new List<Material>();

			try
			{
				var bundle = this.Client.Query(expression, 0, null, true, null);

				bundle.Reconstitute();

				materials.AddRange(bundle.Item.OfType<Material>().Where(expression.Compile()));

				this.entityAuditService.AuditQueryEntity(OutcomeIndicator.Success, materials);
			}
			catch (Exception e)
			{
				coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, this.entityAuditService.QueryEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return materials;
		}

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="all">if set to <c>true</c> load all the nested properties of the entity.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		public override IEnumerable<Material> Query(Expression<Func<Material, bool>> expression, int offset, int? count, bool all = false)
		{
			var materials = new List<Material>();

			try
			{
				var bundle = this.Client.Query(expression, offset, count, all, null);

				bundle.Reconstitute();

				materials.AddRange(bundle.Item.OfType<Material>().Where(expression.Compile()));

				this.entityAuditService.AuditQueryEntity(OutcomeIndicator.Success, materials);
			}
			catch (Exception e)
			{
				coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, this.entityAuditService.QueryEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return materials;
		}

		/// <summary>
		/// Queries for an entity with a given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="expandProperties">The expand properties.</param>
		/// <returns>Returns a list of entities which match the given expression.</returns>
		public override IEnumerable<Material> Query(Expression<Func<Material, bool>> expression, int offset, int? count, string[] expandProperties)
		{
			var materials = new List<Material>();

			try
			{
				var bundle = this.Client.Query(expression, offset, count, expandProperties, null);

				bundle.Reconstitute();

				materials.AddRange(bundle.Item.OfType<Material>().Where(expression.Compile()));

				this.entityAuditService.AuditQueryEntity(OutcomeIndicator.Success, materials);
			}
			catch (Exception e)
			{
				coreAuditService.AuditGenericError(OutcomeIndicator.EpicFail, this.entityAuditService.QueryEntityAuditCode, EventIdentifierType.ApplicationActivity, e);
				throw;
			}

			return materials;
		}
	}
}