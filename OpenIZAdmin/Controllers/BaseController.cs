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
		/// <returns>Concept.</returns>
		protected Concept GetConcept(Guid key)
		{
			var concept = MvcApplication.MemoryCache.Get(key.ToString()) as Concept;

			if (concept == null)
			{
				var bundle = this.ImsiClient.Query<Concept>(c => c.Key == key && c.ObsoletionTime == null);

				bundle.Reconstitute();

				concept = bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Key == key && c.ObsoletionTime == null);

				if (concept != null)
				{
					MvcApplication.MemoryCache.Set(new CacheItem(concept.Key?.ToString(), concept), MvcApplication.CacheItemPolicy);
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
		/// <param name="key">The key.</param>
		/// <returns>Concept.</returns>
		protected Concept GetConcept(Guid? key)
		{
			Concept concept = null;

			if (key.HasValue && key.Value != Guid.Empty)
			{
				concept = this.GetConcept(key.Value);
			}

			return concept;
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
		/// <returns>ConceptSet.</returns>
		protected ConceptSet GetConceptSet(Guid key)
		{
			var conceptSet = MvcApplication.MemoryCache.Get(key.ToString()) as ConceptSet;

			if (conceptSet == null)
			{
				var bundle = this.ImsiClient.Query<ConceptSet>(c => c.Key == key && c.ObsoletionTime == null);

				bundle.Reconstitute();

				conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Key == key && c.ObsoletionTime == null);

				if (conceptSet != null)
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
		/// <returns>ConceptSet.</returns>
		protected ConceptSet GetConceptSet(string mnemonic)
		{
			var conceptSet = MvcApplication.MemoryCache.Get(mnemonic) as ConceptSet;

			if (conceptSet == null)
			{
				var bundle = this.ImsiClient.Query<ConceptSet>(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);

				bundle.Reconstitute();

				conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);

				if (conceptSet != null)
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

			this.Request.Cookies.Add(new HttpCookie("access_token", deviceIdentity.AccessToken));

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
				query = o => o.Key == id && o.VersionKey == versionId && expression.Compile().Invoke(o as T);
			}
			else if (versionId.HasValue && versionId.Value != Guid.Empty)
			{
				query = o => o.Key == id && o.VersionKey == versionId;
			}

			var bundle = this.ImsiClient.Query<T>(query, 0, null, true);

			bundle.Reconstitute();

			var entity = bundle.Item.OfType<T>().Where(query.Compile()).LatestVersionOnly().FirstOrDefault(query.Compile().Invoke);

			if (entity.TypeConceptKey.HasValue && entity.TypeConceptKey != Guid.Empty)
			{
				entity.TypeConcept = this.ImsiClient.Get<Concept>(entity.TypeConceptKey.Value, null) as Concept;
			}

			return entity;
		}

		/// <summary>
		/// Gets the entity relationships.
		/// </summary>
		/// <typeparam name="TSourceType">The type of the source type.</typeparam>
		/// <typeparam name="TTargetType">The type of the t target type.</typeparam>
		/// <param name="id">The identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="entityExpression">The entity expression.</param>
		/// <param name="entityRelationshipExpression">The entity relationship expression.</param>
		/// <returns>Returns a list of entity relationships for a given entity.</returns>
		protected IEnumerable<EntityRelationship> GetEntityRelationships<TSourceType, TTargetType>(Guid id, Guid? versionId = null, Expression<Func<TSourceType, bool>> entityExpression = null, Expression < Func<EntityRelationship, bool>> entityRelationshipExpression = null) where TSourceType : Entity where TTargetType : Entity
		{
			var entity = this.GetEntity<TSourceType>(id, versionId, entityExpression);

			Expression<Func<EntityRelationship, bool>> expression = r => r.TargetEntity == null && r.TargetEntityKey.HasValue && r.TargetEntityKey.Value != Guid.Empty;

			if (entityRelationshipExpression != null)
			{
				var body = Expression.AndAlso(entityRelationshipExpression.Body, Expression.Invoke(expression, entityRelationshipExpression.Parameters[0]));

				expression = Expression.Lambda<Func<EntityRelationship, bool>>(body, entityRelationshipExpression.Parameters);
			}

			foreach (var entityRelationship in entity.Relationships.Where(expression.Compile()))
			{
				entityRelationship.TargetEntity = this.ImsiClient.Get<TTargetType>(entityRelationship.TargetEntityKey.Value, null) as TTargetType;
			}

			return entity.Relationships;
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
				var bundle = this.ImsiClient.Query<ConceptSet>(c => c.Mnemonic == Constants.TelecomAddressUse, 0, null, new string[] { "concept" });

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

			return bundle.Item.OfType<UserEntity>().FirstOrDefault(u => u.SecurityUserKey == securityUserId);
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