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
 * Date: 2016-7-23
 */

using Elmah;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.EntityRelationshipModels;
using OpenIZAdmin.Models.PlaceModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using OpenIZ.Core.Extensions;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing places.
	/// </summary>
	[TokenAuthorize]
	public class PlaceController : BaseController
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
		/// Initializes a new instance of the <see cref="PlaceController"/> class.
		/// </summary>
		public PlaceController()
		{
		}

		/// <summary>
		/// Activates the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>ActionResult.</returns>
		public ActionResult Activate(Guid id, Guid? versionId)
		{
			try
			{
				var place = this.GetEntity<Place>(id, null);

				if (place == null)
				{
					this.TempData["error"] = Locale.UnableToRetrievePlace;
					return RedirectToAction("Edit", new { id = id, versionId = versionId });
				}

				place.CreationTime = DateTimeOffset.Now;
				place.VersionKey = null;
				place.StatusConceptKey = StatusKeys.Active;

				var updatedPlace = this.ImsiClient.Update(place);

				this.TempData["success"] = Locale.PlaceActivatedSuccessfully;

				return RedirectToAction("Edit", new { id = id, versionId = updatedPlace.VersionKey });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to activate place: { e }");
			}

			this.TempData["error"] = Locale.UnableToActivatePlace;

			return RedirectToAction("Edit", new { id = id, versionId = versionId });
		}

		/// <summary>
		/// Displays the create place view.
		/// </summary>
		/// <returns>Returns the create place view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			var model = new CreatePlaceModel
			{
				TypeConcepts = this.GetPlaceTypeConcepts().ToSelectList().ToList()
			};

			return View(model);
		}

		/// <summary>
		/// Creates a place.
		/// </summary>
		/// <param name="model">The model containing the information about the place.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreatePlaceModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var targetPopulationExtensionType = this.ImsiClient.Get<ExtensionType>(Constants.TargetPopulationExtensionTypeKey, null) as ExtensionType;

					var placeToCreate = model.ToPlace();

					var entityExtension = new EntityExtension
					{
						ExtensionType = targetPopulationExtensionType,
						ExtensionValue = Convert.ToDecimal(model.TargetPopulation)
					};

					placeToCreate.Extensions.Add(entityExtension);

					var createdPlace = this.ImsiClient.Create<Place>(placeToCreate);

					TempData["success"] = Locale.PlaceSuccessfullyCreated;

					return RedirectToAction("ViewPlace", new { id = createdPlace.Key, versionId = createdPlace.VersionKey });
				}
				catch (Exception e)
				{
					ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
					Trace.TraceError($"Unable to create place: { e }");
				}
			}

			model.TypeConcepts = this.GetPlaceTypeConcepts().ToSelectList().ToList();

			this.TempData["error"] = Locale.UnableToCreatePlace;

			return View(model);
		}

		/// <summary>
		/// Creates the related place.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>ActionResult.</returns>
		[HttpGet]
		public ActionResult CreateRelatedPlace(Guid id)
		{
			try
			{
				var place = this.GetEntity<Place>(id);

				if (place == null)
				{
					this.TempData["error"] = Locale.PlaceNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					ExistingRelationships = place.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList()
				};

				model.RelationshipTypes.AddRange(this.GetConceptSet(ConceptSetKeys.EntityRelationshipType).Concepts.ToSelectList().ToList());

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to create related place: { e }");
			}

			this.TempData["error"] = Locale.PlaceNotFound;

			return RedirectToAction("Edit", new { id = id });
		}

		/// <summary>
		/// Creates the related place.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateRelatedPlace(EntityRelationshipModel model)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					var place = this.GetEntity<Place>(model.SourceId);

					if (place == null)
					{
						this.TempData["error"] = Locale.UnableToCreateRelatedPlace;
						return RedirectToAction("Edit", new { id = model.SourceId });
					}

					place.Relationships.RemoveAll(r => r.TargetEntityKey == model.TargetId && r.RelationshipTypeKey == Guid.Parse(model.RelationshipType));
					place.Relationships.Add(new EntityRelationship(Guid.Parse(model.RelationshipType), model.TargetId) { EffectiveVersionSequenceId = place.VersionSequence, Key = Guid.NewGuid(), Quantity = model.Quantity ?? 0, SourceEntityKey = model.SourceId });

					this.ImsiClient.Update(place);

					this.TempData["success"] = Locale.RelatedPlaceCreatedSuccessfully;

					return RedirectToAction("Edit", new { id = place.Key.Value });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to create related place: { e }");
			}

			model.RelationshipTypes.AddRange(this.GetConceptSet(ConceptSetKeys.EntityRelationshipType).Concepts.SkipWhile(c => c.Key != EntityRelationshipTypeKeys.Child || c.Key != EntityRelationshipTypeKeys.Parent).ToSelectList().ToList());

			this.TempData["error"] = Locale.UnableToCreateRelatedPlace;

			return View(model);
		}

		/// <summary>
		/// Displays the create place view.
		/// </summary>
		/// <param name="id">The id of the place to delete.</param>
		/// <returns>Returns the create place view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var place = this.GetEntity<Place>(id);

				if (place == null)
				{
					TempData["error"] = Locale.PlaceNotFound;

					return RedirectToAction("Index");
				}

				this.TempData["success"] = Locale.PlaceDeletedSuccessfully;

				this.ImsiClient.Obsolete<Place>(place);

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToDeletePlace;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Gets a place by id.
		/// </summary>
		/// <param name="id">The id of the place to retrieve.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>Returns the place edit view.</returns>
		[HttpGet]
		public ActionResult Edit(Guid id, Guid? versionId)
		{
			try
			{
				var place = this.GetEntity<Place>(id, null);

				if (place == null)
				{
					TempData["error"] = Locale.PlaceNotFound;
					return RedirectToAction("Index");
				}

				if (place.Tags.Any(t => t.TagKey == Constants.ImportedDataTag && t.Value?.ToLower() == "true"))
				{
					this.TempData["warning"] = Locale.RecordMustBeVerifiedBeforeEditing;
					return RedirectToAction("ViewPlace", new { id, versionId });
				}

				place.Relationships = this.GetEntityRelationships<Place, Place>(place.Key.Value, place.VersionKey.Value, null,
					r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Child ||
						r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent ||
						r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation).ToList();

				var model = new EditPlaceModel(place)
				{
					TypeConcepts = this.GetPlaceTypeConcepts().ToSelectList(t => t.Key == place.TypeConceptKey).ToList()
				};

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve place: { e }");
			}

			TempData["error"] = Locale.PlaceNotFound;
			return RedirectToAction("Index");
		}

		/// <summary>
		/// Updates a place.
		/// </summary>
		/// <param name="model">The model containing the place information.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditPlaceModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var bundle = this.ImsiClient.Query<Place>(p => p.Key == model.Id && p.ObsoletionTime == null, 0, null, true);

					bundle.Reconstitute();

					var place = bundle.Item.OfType<Place>().FirstOrDefault(p => p.Key == model.Id && p.ObsoletionTime == null);

					if (place == null)
					{
						TempData["error"] = Locale.PlaceNotFound;

						return RedirectToAction("Index");
					}

					model.TypeConcepts = this.GetPlaceTypeConcepts().ToSelectList(t => t.Key == place.TypeConceptKey).ToList();

					var placeToUpdate = model.ToPlace(place);

					var targetPopulationExtensionType = this.ImsiClient.Get<ExtensionType>(Constants.TargetPopulationExtensionTypeKey, null) as ExtensionType;

					var entityExtension = new EntityExtension
					{
						ExtensionType = targetPopulationExtensionType,
						ExtensionValue = Convert.ToDecimal(model.TargetPopulation)
					};

					placeToUpdate.Extensions.Add(entityExtension);

					var updatedPlace = this.ImsiClient.Update<Place>(placeToUpdate);

					TempData["success"] = Locale.PlaceSuccessfullyUpdated;

					return RedirectToAction("ViewPlace", new { id = updatedPlace.Key, versionId = updatedPlace.VersionKey });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			this.TempData["error"] = Locale.UnableToUpdatePlace;

			return View(model);
		}

		/// <summary>
		/// Edits the related place.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="entityRelationshipId">The entity relationship identifier.</param>
		/// <returns>ActionResult.</returns>
		[HttpGet]
		public ActionResult EditRelatedPlace(Guid id, Guid entityRelationshipId)
		{
			try
			{
				var place = this.ImsiClient.Get<Place>(id, null) as Place;

				if (place == null)
				{
					this.TempData["error"] = Locale.PlaceNotFound;
					return RedirectToAction("Edit", new { id = id });
				}

				var entityRelationship = place.Relationships.Find(r => r.Key == entityRelationshipId);

				var model = new EntityRelationshipModel(entityRelationship, place.Type);

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to edit related place: { e }");
			}

			this.TempData["error"] = Locale.PlaceNotFound;
			return RedirectToAction("Edit", new { id = id });
		}

		/// <summary>
		/// Gets the place type concepts.
		/// </summary>
		/// <returns>IEnumerable&lt;Concept&gt;.</returns>
		private IEnumerable<Concept> GetPlaceTypeConcepts()
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
		/// Edits the related place.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditRelatedPlace(EntityRelationshipModel model)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					var place = this.ImsiClient.Get<Place>(model.SourceId, null) as Place;

					if (place == null)
					{
						this.TempData["error"] = Locale.UnableToUpdatePlace;
						return RedirectToAction("Edit", new { id = model.SourceId });
					}

					place.Relationships.RemoveAll(r => r.Key == model.Id);
					place.Relationships.Add(new EntityRelationship(Guid.Parse(model.RelationshipType), model.TargetId));

					var updatedPlace = this.ImsiClient.Update<Place>(place);

					return RedirectToAction("Edit", new { id = updatedPlace.Key.Value });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to edit related place: { e }");
			}

			return View(model);
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>ActionResult.</returns>
		public ActionResult Index()
		{
			TempData["searchType"] = "Place";
            TempData["searchTerm"] = "*";
            return View();
		}

		/// <summary>
		/// Searches for a place.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of places which match the search term.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			var results = new List<PlaceViewModel>();

			if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
			{
				Bundle bundle;

				Expression<Func<Place, bool>> nameExpression = p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));

				if (searchTerm == "*")
				{
					bundle = this.ImsiClient.Query<Place>(p => p.ObsoletionTime == null, 0, null, new[] { "typeConcept" });

					foreach (var place in bundle.Item.OfType<Place>().LatestVersionOnly())
					{
						place.TypeConcept = this.GetTypeConcept(place);
					}

					results = bundle.Item.OfType<Place>().LatestVersionOnly().Select(p => new PlaceViewModel(p)).OrderBy(p => p.Name).ToList();
				}
				else
				{
					Guid placeId;

					if (!Guid.TryParse(searchTerm, out placeId))
					{
						bundle = this.ImsiClient.Query<Place>(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))), 0, null, new[] { "typeConcept" });

						foreach (var place in bundle.Item.OfType<Place>().LatestVersionOnly())
						{
							place.TypeConcept = this.GetTypeConcept(place);
						}

						results = bundle.Item.OfType<Place>().Where(nameExpression.Compile()).LatestVersionOnly().Select(p => new PlaceViewModel(p)).OrderBy(p => p.Name).ToList();
					}
					else
					{
						var place = this.GetEntity<Place>(placeId);

						if (place != null)
						{
							results.Add(new PlaceViewModel(place));
						}
					}
				}
			}

			TempData["searchTerm"] = searchTerm;

			return PartialView("_PlaceSearchResultsPartial", results);
		}

		/// <summary>
		/// Searches for a place.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of places which match the search term.</returns>
		[HttpGet]
		public ActionResult SearchAjax(string searchTerm)
		{
			var viewModels = new List<PlaceViewModel>();

		    if (!ModelState.IsValid) return Json(viewModels, JsonRequestBehavior.AllowGet);

		    //var places = this.ImsiClient.Query<Place>(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.ObsoletionTime == null && p.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation);
		    var places = this.ImsiClient.Query<Place>(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.ObsoletionTime == null && p.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation);
		    //viewModels = places.Item.OfType<Place>().Where(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))) && p.ObsoletionTime == null && p.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation).LatestVersionOnly().Select(p => new PlaceViewModel(p)).OrderBy(p => p.Name).ToList();
		    viewModels = places.Item.OfType<Place>().LatestVersionOnly().Select(p => new PlaceViewModel(p)).OrderBy(p => p.Name).ToList();

		    return Json(viewModels, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Searches for a place to view details.
		/// </summary>
		/// <param name="id">The place identifier search string.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>Returns a place view that matches the search term.</returns>
		[HttpGet]
		public ActionResult ViewPlace(Guid id, Guid? versionId)
		{
			try
			{
				var place = this.GetEntity<Place>(id, null);

				if (place == null)
				{
					TempData["error"] = Locale.PlaceNotFound;
					return RedirectToAction("Index");
				}

				place.Relationships = this.GetEntityRelationships<Place, Place>(place.Key.Value, place.VersionKey.Value, null, 
					r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Child || 
						r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent || 
						r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation).ToList();

				return View(new PlaceViewModel(place));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve place: { e }");
			}

			TempData["error"] = Locale.PlaceNotFound;

			return RedirectToAction("Index");
		}
	}
}