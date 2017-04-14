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
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
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
		/// The health facility mnemonic.
		/// </summary>
		private readonly string healthFacilityMnemonic = ConfigurationManager.AppSettings["HealthFacilityTypeConceptMnemonic"];

		/// <summary>
		/// The place type mnemonic.
		/// </summary>
		private readonly string placeTypeMnemonic = ConfigurationManager.AppSettings["PlaceTypeConceptMnemonic"];

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
		/// Forces the load concepts.
		/// </summary>
		/// <param name="conceptSet">The concept set.</param>
		/// <returns>Returns the concept set with the nested loaded concepts.</returns>
		private ConceptSet ForceLoadConcepts(ConceptSet conceptSet)
		{
			Expression<Func<Concept, bool>> nameExpression = c => c.ConceptNames.Any() || c.Mnemonic == null;

			// HACK: force load missing concept names and mnemonics
			for (var i = 0; i < conceptSet.Concepts.Count(nameExpression.Compile()); i++)
			{
				var concept = conceptSet.Concepts.Where(nameExpression.Compile()).ToArray()[i];
				conceptSet.Concepts.Where(nameExpression.Compile()).ToArray()[i] = this.ImsiClient.Get<Concept>(concept.Key.Value, null) as Concept;
			}

			return conceptSet;
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
		protected T GetEntity<T>(Guid id, Guid? versionId, Expression<Func<T, bool>> expression = null) where T : Entity
		{
			T result;

			if (expression == null)
			{
				result = this.ImsiClient.Get<T>(id, versionId) as T;
			}
			else
			{
				var bundle = this.ImsiClient.Query<T>(m => m.Key == id && m.VersionKey == versionId && expression.Compile().Invoke(m), 0, null, true);

				bundle.Reconstitute();

				result = bundle.Item.OfType<T>().FirstOrDefault(m => m.Key == id && m.VersionKey == versionId && expression.Compile().Invoke(m));
			}

			return result;
		}

		/// <summary>
		/// Gets the entity relationship concept set.
		/// </summary>
		/// <returns>Returns the entity relationship type concept set.</returns>
		protected ConceptSet GetEntityRelationshipTypeConceptSet()
		{
			var conceptSet = MvcApplication.MemoryCache.Get(ConceptSetKeys.EntityRelationshipType.ToString()) as ConceptSet;

			if (conceptSet == null)
			{
				conceptSet = this.ImsiClient.Get<ConceptSet>(ConceptSetKeys.EntityRelationshipType, null) as ConceptSet;

				if (conceptSet != null)
				{
					MvcApplication.MemoryCache.Set(ConceptSetKeys.EntityRelationshipType.ToString(), conceptSet, MvcApplication.CacheItemPolicy);
				}
			}

			return conceptSet;
		}

		/// <summary>
		/// Gets the form concepts.
		/// </summary>
		/// <returns>IEnumerable&lt;Concept&gt;.</returns>
		protected IEnumerable<Concept> GetFormConcepts()
		{
			var concepts = MvcApplication.MemoryCache.Get(ConceptClassKeys.Form.ToString());

			if (concepts == null)
			{
				var bundle = this.ImsiClient.Query<Concept>(c => c.ClassKey == ConceptClassKeys.Form && c.ObsoletionTime == null);

				bundle.Reconstitute();

				concepts = bundle.Item.OfType<Concept>().Where(c => c.ClassKey == ConceptClassKeys.Form && c.ObsoletionTime == null);

				MvcApplication.MemoryCache.Set(new CacheItem(ConceptClassKeys.Form.ToString(), concepts), MvcApplication.CacheItemPolicy);
			}

			return concepts as IEnumerable<Concept>;
		}

		/// <summary>
		/// Gets the industry code concept set.
		/// </summary>
		/// <returns>Returns a concept set.</returns>
		protected ConceptSet GetIndustryCodeConceptSet()
		{
			var bundle = this.ImsiClient.Query<ConceptSet>(c => c.Key == ConceptSetKeys.IndustryCode && c.ObsoletionTime == null);

			bundle.Reconstitute();

			return bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Key == ConceptSetKeys.IndustryCode && c.ObsoletionTime == null);
		}

		/// <summary>
		/// Gets the type concepts.
		/// </summary>
		/// <returns>IEnumerable&lt;Concept&gt;.</returns>
		protected IEnumerable<Concept> GetMaterialTypeConcepts()
		{
			var concepts = MvcApplication.MemoryCache.Get(ConceptClassKeys.Material.ToString()) as IEnumerable<Concept>;

			if (concepts == null)
			{
				var bundle = this.ImsiClient.Query<Concept>(c => c.ClassKey == ConceptClassKeys.Material && c.ObsoletionTime == null);

				bundle.Reconstitute();

				concepts = bundle.Item.OfType<Concept>().Where(c => c.ClassKey == ConceptClassKeys.Material && c.ObsoletionTime == null);

				MvcApplication.MemoryCache.Set(new CacheItem(ConceptClassKeys.Material.ToString(), concepts.ToList()), MvcApplication.CacheItemPolicy);
			}

			return concepts;
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
				var bundle = this.ImsiClient.Query<ConceptSet>(c => c.Mnemonic == Constants.TelecomAddressUse, 0, null, "concept");

				bundle.Reconstitute();

				conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Mnemonic == Constants.TelecomAddressUse);

				if (conceptSet != null)
				{
					conceptSet = ForceLoadConcepts(conceptSet);
				}

				MvcApplication.MemoryCache.Set(new CacheItem(ConceptSetKeys.TelecomAddressUse.ToString(), conceptSet), MvcApplication.CacheItemPolicy);
			}

			return conceptSet;
		}

		/// <summary>
		/// Gets the place type concepts.
		/// </summary>
		/// <returns>IEnumerable&lt;Concept&gt;.</returns>
		protected IEnumerable<Concept> GetPlaceTypeConcepts()
		{
			var typeConcepts = new List<Concept>();

			if (!string.IsNullOrEmpty(this.healthFacilityMnemonic) && !string.IsNullOrWhiteSpace(this.healthFacilityMnemonic))
			{
				typeConcepts.AddRange(this.GetConceptSet(this.healthFacilityMnemonic).Concepts);
			}

			if (!string.IsNullOrEmpty(this.placeTypeMnemonic) && !string.IsNullOrWhiteSpace(this.placeTypeMnemonic))
			{
				typeConcepts.AddRange(this.GetConceptSet(this.placeTypeMnemonic).Concepts);
			}

			if (!typeConcepts.Any())
			{
				typeConcepts.AddRange(this.ImsiClient.Query<Concept>(m => m.ClassKey == ConceptClassKeys.Other && m.ObsoletionTime == null).Item.OfType<Concept>().Where(m => m.ClassKey == ConceptClassKeys.Other && m.ObsoletionTime == null));
			}

			return typeConcepts;
		}

		/// <summary>
		/// Gets the quantity concepts.
		/// </summary>
		/// <returns>IEnumerable&lt;Concept&gt;.</returns>
		protected IEnumerable<Concept> GetQuantityConcepts()
		{
			var concepts = MvcApplication.MemoryCache.Get(ConceptClassKeys.UnitOfMeasure.ToString()) as IEnumerable<Concept>;

			if (concepts == null)
			{
				var bundle = this.ImsiClient.Query<Concept>(c => c.ClassKey == ConceptClassKeys.UnitOfMeasure && c.ObsoletionTime == null);

				bundle.Reconstitute();

				concepts = bundle.Item.OfType<Concept>().Where(c => c.ClassKey == ConceptClassKeys.UnitOfMeasure && c.ObsoletionTime == null);

				MvcApplication.MemoryCache.Set(new CacheItem(ConceptClassKeys.UnitOfMeasure.ToString(), concepts.ToList()), MvcApplication.CacheItemPolicy);
			}

			return concepts;
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
		protected virtual bool IsValidKey(string key)
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