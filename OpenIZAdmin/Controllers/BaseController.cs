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
 * User: khannan
 * Date: 2016-11-19
 */

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using OpenIZ.Core.Model.Collection;
using OpenIZAdmin.Audit;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Models.RoleModels;

namespace OpenIZAdmin.Controllers
{
    /// <summary>
    /// Represents a base controller.
    /// </summary>
    [TokenAuthorize]
	public abstract class BaseController : Controller
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseController"/> class.
		/// </summary>
		protected BaseController()
		{
			TempData.Clear();
		}

		/// <summary>
		/// Gets the <see cref="AmiServiceClient"/> instance.
		/// </summary>
		protected AmiServiceClient AmiClient { get; private set; }

		/// <summary>
		/// Gets the <see cref="ImsiServiceClient"/> instance.
		/// </summary>
		protected ImsiServiceClient ImsiClient { get; private set; }

		/// <summary>
		/// Redirects the response the URL referrer or to the root of the site if no URL referrer is found.
		/// </summary>
		/// <returns>Returns a redirect result.</returns>
		public RedirectResult RedirectToRequestOrHome()
		{
			return this.Redirect(this.Request.UrlReferrer?.ToString() ?? Url.Content("~/"));
		}

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected override void Dispose(bool disposing)
		{
			this.AmiClient?.Dispose();
			this.ImsiClient?.Dispose();
			base.Dispose(disposing);
		}

		/// <summary>
		/// Gets all roles.
		/// </summary>
		/// <returns>Returns a list of all roles as role view model instances.</returns>
		protected IEnumerable<RoleViewModel> GetAllRoles()
		{
			return this.AmiClient.GetRoles(r => r.ObsoletionTime == null).CollectionItem.Select(r => new RoleViewModel(r));
		}

		/// <summary>
		/// Gets the form concept.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the concept for the given key, or null if no concept is found.</returns>
		protected Concept GetConcept(Guid? key)
		{
			if (!key.HasValue || key == Guid.Empty)
			{
				return null;
			}

			var concept = MvcApplication.MemoryCache.Get(key.ToString()) as Concept;

			if (concept == null)
			{
				var bundle = this.ImsiClient.Query<Concept>(c => c.Key == key && c.ObsoletionTime == null);

				bundle.Reconstitute();

				concept = bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Key == key && c.ObsoletionTime == null);

				if (concept != null)
				{
					MvcApplication.MemoryCache.Set(new CacheItem(concept.Key.ToString(), concept), MvcApplication.CacheItemPolicy);
				}
			}

			return concept;
		}

		/// <summary>
		/// Gets the concept.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="versionKey">The version key.</param>
		/// <returns>Returns the concept or null if no concept is found.</returns>
		protected Concept GetConcept(Guid key, Guid? versionKey)
		{
			return this.ImsiClient.Get<Concept>(key, versionKey) as Concept;
		}

		/// <summary>
		/// Gets the concept.
		/// </summary>
		/// <param name="mnemonic">The mnemonic.</param>
		/// <returns>Returns a concept or null if no concept is found.</returns>
		protected Concept GetConcept(string mnemonic)
		{
			var concept = MvcApplication.MemoryCache.Get(mnemonic) as Concept;

			if (concept == null)
			{
				var bundle = this.ImsiClient.Query<Concept>(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);

				bundle.Reconstitute();

				concept = bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);

				if (concept != null)
				{
					MvcApplication.MemoryCache.Set(new CacheItem(concept.Key?.ToString(), concept), MvcApplication.CacheItemPolicy);
				}
			}

			return concept;
		}

		/// <summary>
		/// Gets the concept set.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="loadFast">if set to <c>true</c> [load fast].</param>
		/// <returns>Returns the concept set for the given key.</returns>
		protected ConceptSet GetConceptSet(Guid key, bool loadFast = false)
		{
			var conceptSet = MvcApplication.MemoryCache.Get(key.ToString()) as ConceptSet;

			if (conceptSet == null)
			{
				var bundle = this.ImsiClient.Query<ConceptSet>(c => c.Key == key && c.ObsoletionTime == null);

				bundle.Reconstitute();

				conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Key == key && c.ObsoletionTime == null);

				if (conceptSet != null && loadFast)
				{
					MvcApplication.MemoryCache.Set(new CacheItem(key.ToString(), conceptSet), MvcApplication.CacheItemPolicy);
				}
			}

			return conceptSet;
		}

		/// <summary>
		/// Gets the concept set.
		/// </summary>
		/// <param name="mnemonic">The mnemonic.</param>
		/// <param name="loadFast">if set to <c>true</c> [load fast].</param>
		/// <returns>Return the concept set for the given concept set mnemonic.</returns>
		protected ConceptSet GetConceptSet(string mnemonic, bool loadFast = false)
		{
			var conceptSet = MvcApplication.MemoryCache.Get(mnemonic) as ConceptSet;

			if (conceptSet == null)
			{
				var bundle = this.ImsiClient.Query<ConceptSet>(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);

				bundle.Reconstitute();

				conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);

				if (conceptSet != null && loadFast)
				{
					MvcApplication.MemoryCache.Set(new CacheItem(conceptSet.Key.ToString(), conceptSet), MvcApplication.CacheItemPolicy);
				}
			}

			return conceptSet;
		}

		/// <summary>
		/// Gets the device service client.
		/// </summary>
		/// <returns>Returns an AMI service client instance or null.</returns>
		protected AmiServiceClient GetDeviceServiceClient()
		{
			var deviceIdentity = ApplicationSignInManager.LoginAsDevice();

			if (deviceIdentity == null)
			{
				return null;
			}

			this.Request.Cookies.Set(new HttpCookie("access_token", deviceIdentity.AccessToken));

			var restClientService = new RestClientService(Constants.Ami)
			{
				Credentials = new AmiCredentials(new GenericPrincipal(deviceIdentity, null), this.Request)
			};

			return new AmiServiceClient(restClientService);
		}

		/// <summary>
		/// Gets the entity.
		/// </summary>
		/// <typeparam name="T">The type of entity.</typeparam>
		/// <param name="id">The identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns the identifier based on the id, version id, and an expression.</returns>
		protected T GetEntity<T>(Guid id, Guid? versionId = null, Expression<Func<T, bool>> expression = null) where T : Entity
		{
			Expression<Func<T, bool>> query = o => o.Key == id;

			if (versionId.HasValue && versionId.Value != Guid.Empty && expression != null)
			{
				query = o => o.Key == id && o.VersionKey == versionId && expression.Compile().Invoke(o);
			}
			else if (versionId.HasValue && versionId.Value != Guid.Empty)
			{
				query = o => o.Key == id && o.VersionKey == versionId;
			}

			T entity = null;

			var bundle = this.ImsiClient.Query<T>(query, 0, null, true);

			bundle.Reconstitute();

			entity = bundle.Item.OfType<T>().Where(query.Compile()).LatestVersionOnly().FirstOrDefault(query.Compile().Invoke);

			if (entity == null)
			{
				return null;
			}

			if (entity.TypeConceptKey.HasValue && entity.TypeConceptKey != Guid.Empty)
			{
				entity.TypeConcept = this.GetConcept(entity.TypeConceptKey.Value);
			}

			return entity;
		}


		/// <summary>
		/// Gets the entity relationships.
		/// </summary>
		/// <typeparam name="TTargetType">The type of the t target type.</typeparam>
		/// <param name="id">The identifier.</param>
		/// <param name="entityRelationshipExpression">The entity relationship expression.</param>
		/// <returns>Returns a list of entity relationships for the given entity.</returns>
		protected IEnumerable<EntityRelationship> GetEntityRelationships<TTargetType>(Guid id, Expression<Func<EntityRelationship, bool>> entityRelationshipExpression = null) where TTargetType : Entity
		{
			Bundle bundle;

			if (entityRelationshipExpression != null)
			{
				bundle = this.ImsiClient.Query<EntityRelationship>(c => c.SourceEntityKey == id && entityRelationshipExpression.Compile().Invoke(c));
			}
			else
			{
				bundle = this.ImsiClient.Query<EntityRelationship>(c => c.SourceEntityKey == id);
			}

			bundle.Reconstitute();

			var entityRelationships = bundle.Item.OfType<EntityRelationship>().ToArray();

			if (entityRelationshipExpression != null)
			{
				entityRelationships = entityRelationships.Where(c => entityRelationshipExpression.Compile().Invoke(c)).ToArray();
			}

			foreach (var entityRelationship in entityRelationships)
			{
				// only load the relationship type concept if the IMS didn't return the relationship type concept of the relationship
				if (entityRelationship.RelationshipType == null && entityRelationship.RelationshipTypeKey.HasValue && entityRelationship.RelationshipTypeKey.Value != Guid.Empty)
				{
					entityRelationship.RelationshipType = this.GetConcept(entityRelationship.RelationshipTypeKey);
				}

				// only load the target entity if the IMS didn't return the target entity by default
				if (entityRelationship.TargetEntity == null && entityRelationship.TargetEntityKey.HasValue && entityRelationship.TargetEntityKey.Value != Guid.Empty)
				{
					entityRelationship.TargetEntity = this.ImsiClient.Get<TTargetType>(entityRelationship.TargetEntityKey.Value, null) as TTargetType;
				}

				// only load the type concept of the target entity if the IMS didn't return the type concept of the target entity
				if (entityRelationship.TargetEntity?.TypeConcept == null && entityRelationship.TargetEntity?.TypeConceptKey.HasValue == true && entityRelationship.TargetEntity?.TypeConceptKey != Guid.Empty)
				{
					entityRelationship.TargetEntity.TypeConcept = this.GetConcept(entityRelationship.TargetEntity.TypeConceptKey.Value);
				}
			}

			return entityRelationships;
		}

		/// <summary>
		/// Gets the phone type concept set.
		/// </summary>
		/// <returns>Returns the concept set which represents a telecommunications address use.</returns>
		protected ConceptSet GetPhoneTypeConceptSet()
		{
			var conceptSet = MvcApplication.MemoryCache.Get(ConceptSetKeys.TelecomAddressUse.ToString()) as ConceptSet;

			if (conceptSet == null)
			{
				var bundle = this.ImsiClient.Query<ConceptSet>(c => c.Mnemonic == Constants.TelecomAddressUse, 0, null, new[] { "concept" });

				bundle.Reconstitute();

				conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Mnemonic == Constants.TelecomAddressUse);

				if (conceptSet != null)
				{
					MvcApplication.MemoryCache.Set(new CacheItem(ConceptSetKeys.TelecomAddressUse.ToString(), conceptSet), MvcApplication.CacheItemPolicy);
				}
			}

			return conceptSet;
		}

		/// <summary>
		/// Gets the type concept.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity">The entity.</param>
		/// <returns>Return the entity with a type concept value.</returns>
		protected Concept GetTypeConcept<T>(T entity) where T : Entity
		{
			Concept typeConcept = null;

			if (entity.TypeConceptKey.HasValue && entity.TypeConceptKey.Value != Guid.Empty)
			{
				typeConcept = MvcApplication.MemoryCache.Get(entity.TypeConceptKey.ToString()) as Concept;

				if (typeConcept == null)
				{
					typeConcept = this.ImsiClient.Get<Concept>(entity.TypeConceptKey.Value, null) as Concept;

					entity.TypeConceptKey = typeConcept.Key;

					MvcApplication.MemoryCache.Set(entity.TypeConceptKey.ToString(), typeConcept, MvcApplication.CacheItemPolicy);
				}
			}

			return typeConcept;
		}

		/// <summary>
		/// Gets the user entity by security user key.
		/// </summary>
		/// <param name="securityUserId">The security user identifier.</param>
		/// <returns>Returns the user entity instance.</returns>
		protected UserEntity GetUserEntityBySecurityUserKey(Guid securityUserId)
		{
			var bundle = this.ImsiClient.Query<UserEntity>(u => u.SecurityUserKey == securityUserId && u.ObsoletionTime == null, 0, null, true);

			bundle.Reconstitute();

			return bundle.Item.OfType<UserEntity>().Where(u => u.SecurityUserKey == securityUserId).LatestVersionOnly().FirstOrDefault();
		}

		/// <summary>
		/// Determines whether [is valid key] [the specified key].
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns><c>true</c> if [is valid key] [the specified key]; otherwise, <c>false</c>.</returns>
		protected virtual bool IsValidId(string key)
		{
			return !string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key);
		}

		/// <summary>
		/// Called when the action is executing.
		/// </summary>
		/// <param name="filterContext">The filter context of the action executing.</param>
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var amiRestClient = new RestClientService(Constants.Ami)
			{
				Accept = Constants.ApplicationXml,
				Credentials = new AmiCredentials(this.User, HttpContext.Request)
			};

			this.AmiClient = new AmiServiceClient(amiRestClient);

			var imsiRestClient = new RestClientService(Constants.Imsi)
			{
				Accept = Constants.ApplicationXml,
				Credentials = new ImsCredentials(this.User, HttpContext.Request)
			};

			this.ImsiClient = new ImsiServiceClient(imsiRestClient);

			base.OnActionExecuting(filterContext);
		}
	}
}