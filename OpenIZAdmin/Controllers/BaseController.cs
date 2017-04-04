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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Principal;
using System.Web;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using System.Web.Mvc;
using OpenIZAdmin.DAL;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;

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

		/// <summary>
		/// Redirects the response the URL referrer or to the root of the site if no URL referrer is found.
		/// </summary>
		/// <returns>Returns a redirect result.</returns>
		public RedirectResult RedirectToRequestOrHome()
		{
			return this.Redirect(this.Request.UrlReferrer?.ToString() ?? Url.Content("~/"));
		}
	}
}