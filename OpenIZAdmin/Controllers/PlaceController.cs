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

using Microsoft.AspNet.Identity;
using OpenIZ.Core.Extensions;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Comparer;
using OpenIZAdmin.Core.Extensions;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core.Serialization;
using OpenIZAdmin.Models.EntityIdentifierModels;
using OpenIZAdmin.Models.EntityRelationshipModels;
using OpenIZAdmin.Models.PlaceModels;
using OpenIZAdmin.Services.Entities;
using OpenIZAdmin.Services.Entities.Places;
using OpenIZAdmin.Services.EntityRelationships;
using OpenIZAdmin.Services.Metadata.Concepts;
using OpenIZAdmin.Services.Security.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing places.
	/// </summary>
	[TokenAuthorize(Constants.UnrestrictedMetadata)]
	public class PlaceController : Controller
	{
		/// <summary>
		/// The concept service.
		/// </summary>
		private readonly IConceptService conceptService;

		/// <summary>
		/// The entity relationship service
		/// </summary>
		private readonly IEntityRelationshipService entityRelationshipService;

		/// <summary>
		/// The entity service.
		/// </summary>
		private readonly IEntityService entityService;

		/// <summary>
		/// The place concept service.
		/// </summary>
		private readonly IPlaceConceptService placeConceptService;

		/// <summary>
		/// The user service.
		/// </summary>
		private readonly IUserService userService;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlaceController"/> class.
		/// </summary>
		public PlaceController(IConceptService conceptService, IEntityService entityService, IEntityRelationshipService entityRelationshipService, IPlaceConceptService placeConceptService, IUserService userService)
		{
			this.conceptService = conceptService;
			this.entityService = entityService;
			this.entityRelationshipService = entityRelationshipService;
			this.placeConceptService = placeConceptService;
			this.userService = userService;
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
				var place = this.entityService.Get<Place>(id);

				if (place == null)
				{
					this.TempData["error"] = Locale.UnableToRetrievePlace;
					return RedirectToAction("Edit", new { id = id, versionId = versionId });
				}

				place.StatusConceptKey = StatusKeys.Active;

				var updatedPlace = entityService.Activate(place);

				this.TempData["success"] = Locale.PlaceActivatedSuccessfully;

				return RedirectToAction("ViewPlace", new { id = updatedPlace.Key, versionId = updatedPlace.VersionKey });
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to activate place: { e }");
			}

			this.TempData["error"] = Locale.UnableToActivatePlace;

			return RedirectToAction("Edit", new { id = id, versionId = versionId });
		}

		/// <summary>
		/// Associates the area served.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the associate area served view.</returns>
		[HttpGet]
		public ActionResult AssociateAreaServed(Guid id)
		{
			try
			{
				var place = this.entityService.Get<Place>(id);

				if (place == null)
				{
					this.TempData["error"] = Locale.PlaceNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				var relationships = new List<EntityRelationship>();

				// get relationships where I am the target of type dedicated service delivery location
				relationships.AddRange(entityRelationshipService.GetEntityRelationshipsByTarget(place.Key.Value, EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, new List<Type> { typeof(Person) }));

				place.Relationships = relationships.Intersect(place.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					ExistingRelationships = place.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList(),
					SourceClass = place.ClassConceptKey?.ToString()
				};

				var concepts = new List<Concept>
				{
					this.conceptService.GetConcept(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, true)
				};

				model.RelationshipTypes.AddRange(concepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, true));

				return View(model);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to load associate child place view: { e }");
			}

			this.TempData["error"] = Locale.PlaceNotFound;

			return RedirectToAction("Edit", new { id = id });
		}

		/// <summary>
		/// Associates the child place.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the associate child place view.</returns>
		[HttpGet]
		public ActionResult AssociateChildPlace(Guid id)
		{
			try
			{
				var place = this.entityService.Get<Place>(id);

				if (place == null)
				{
					this.TempData["error"] = Locale.PlaceNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				var relationships = new List<EntityRelationship>();

				// get the relationships where I am the source and the relationship type is parent
				relationships.AddRange(entityRelationshipService.GetEntityRelationshipsBySource(place.Key.Value, EntityRelationshipTypeKeys.Parent));

				// get the relationships where I am the target and the relationship type is parent
				relationships.AddRange(entityRelationshipService.GetEntityRelationshipsByTarget(place.Key.Value, EntityRelationshipTypeKeys.Parent));

				place.Relationships = relationships.Intersect(place.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					ExistingRelationships = place.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList(),
					SourceClass = place.ClassConceptKey?.ToString()
				};

				var concepts = new List<Concept>
				{
					this.conceptService.GetConcept(EntityRelationshipTypeKeys.Child, true)
				};

				model.RelationshipTypes.AddRange(concepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), null, true));

				return View(model);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to load associate child place view: { e }");
			}

			this.TempData["error"] = Locale.PlaceNotFound;

			return RedirectToAction("Edit", new { id = id });
		}

		/// <summary>
		/// Associates the dedicated service delivery location.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the associate dedicated service delivery location view.</returns>
		[HttpGet]
		public ActionResult AssociateDedicatedServiceDeliveryLocation(Guid id)
		{
			try
			{
				var place = this.entityService.Get<Place>(id);

				if (place == null)
				{
					this.TempData["error"] = Locale.PlaceNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				var relationships = new List<EntityRelationship>();

				relationships.AddRange(this.entityService.GetEntityRelationships<Place>(place.Key.Value,
					r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Child ||
						r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent ||
						r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation));

				place.Relationships = relationships.Intersect(place.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					ExistingRelationships = place.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList(),
					SourceClass = place.ClassConceptKey?.ToString()
				};

				// JF - THIS NEEDS TO BE CHANGED TO USE A CONCEPT SET
				var concepts = new List<Concept>
				{
					this.conceptService.GetConcept(EntityRelationshipTypeKeys.Child, true),
					this.conceptService.GetConcept(EntityRelationshipTypeKeys.Parent, true),
					this.conceptService.GetConcept(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, true)
				};

				model.RelationshipTypes.AddRange(concepts.ToSelectList(this.HttpContext.GetCurrentLanguage()));

				return View(model);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to load associate child place view: { e }");
			}

			this.TempData["error"] = Locale.PlaceNotFound;

			return RedirectToAction("Edit", new { id = id });
		}

		/// <summary>
		/// Associates the parent place.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the associate parent place view.</returns>
		[HttpGet]
		public ActionResult AssociateParentPlace(Guid id)
		{
			try
			{
				var place = this.entityService.Get<Place>(id);

				if (place == null)
				{
					this.TempData["error"] = Locale.PlaceNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				var relationships = new List<EntityRelationship>();

				// get the relationships where I am the source and the relationship type is parent
				relationships.AddRange(entityRelationshipService.GetEntityRelationshipsBySource(place.Key.Value, EntityRelationshipTypeKeys.Parent));

				// get the relationships where I am the target and the relationship type is parent
				relationships.AddRange(entityRelationshipService.GetEntityRelationshipsByTarget(place.Key.Value, EntityRelationshipTypeKeys.Parent));

				place.Relationships = relationships.Intersect(place.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					ExistingRelationships = place.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList(),
					SourceClass = place.ClassConceptKey?.ToString()
				};

				var concepts = new List<Concept>
				{
					this.conceptService.GetConcept(EntityRelationshipTypeKeys.Parent, true)
				};

				model.RelationshipTypes.AddRange(concepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), null, true));

				return View(model);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to load associate parent place view: { e }");
			}

			this.TempData["error"] = Locale.PlaceNotFound;

			return RedirectToAction("Edit", new { id = id });
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
				IsServiceDeliveryLocation = true
			};

			try
			{
				// get the place type concepts
				model.TypeConcepts.AddRange(this.placeConceptService.GetPlaceTypeConcepts().ToSelectList(this.HttpContext.GetCurrentLanguage()).ToList());

				// order the type concept list
				model.TypeConcepts = model.TypeConcepts.OrderBy(c => c.Text).ToList();
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to load type concepts on the create place page: {e}");
			}

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
			if (model.HasOnlyYearOrPopulation())
			{
				if (string.IsNullOrWhiteSpace(model.Year)) ModelState.AddModelError("Year", Locale.TargetYearRequired);

				if (model.TargetPopulation == null) ModelState.AddModelError("TargetPopulation", Locale.TargetPopulationRequired);
			}

			if (!string.IsNullOrWhiteSpace(model.Year))
			{
				if (model.ConvertToPopulationYear() == 0) ModelState.AddModelError("Year", Locale.PopulationYearInvalidFormat);
			}

			if (ModelState.IsValid)
			{
				try
				{
					var placeToCreate = model.ToPlace();

					if (model.SubmitYearAndPopulation())
					{
						var entityExtension = new EntityExtension
						{
							ExtensionType = new ExtensionType(Constants.TargetPopulationUrl, typeof(DictionaryExtensionHandler))
							{
								Key = Constants.TargetPopulationExtensionTypeKey
							},
							ExtensionValue = new TargetPopulation(model.ConvertPopulationToULong(), model.ConvertToPopulationYear())
						};

						placeToCreate.Extensions.Add(entityExtension);
					}

					placeToCreate.CreatedByKey = Guid.Parse(this.User.Identity.GetUserId());

					var createdPlace = this.entityService.Create(placeToCreate);

					TempData["success"] = Locale.PlaceSuccessfullyCreated;

					return RedirectToAction("ViewPlace", new { id = createdPlace.Key, versionId = createdPlace.VersionKey });
				}
				catch (Exception e)
				{
					Trace.TraceError($"Unable to create place: { e }");
				}
			}

			model.TypeConcepts = new List<SelectListItem>();

			// get the place type concepts
			model.TypeConcepts.AddRange(this.placeConceptService.GetPlaceTypeConcepts().ToSelectList(this.HttpContext.GetCurrentLanguage()).ToList());

			// order the type concept list
			model.TypeConcepts = model.TypeConcepts.OrderBy(c => c.Text).ToList();

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
				var place = this.entityService.Get<Place>(id);

				if (place == null)
				{
					this.TempData["error"] = Locale.PlaceNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				var relationships = new List<EntityRelationship>();

				relationships.AddRange(this.entityService.GetEntityRelationships<Place>(place.Key.Value,
					r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Child ||
						r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent ||
						r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation));

				place.Relationships = relationships.Intersect(place.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					ExistingRelationships = place.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList(),
					SourceClass = place.ClassConceptKey?.ToString()
				};

				// JF - THIS NEEDS TO BE CHANGED TO USE A CONCEPT SET
				var concepts = new List<Concept>
				{
					this.conceptService.GetConcept(EntityRelationshipTypeKeys.Child, true),
					this.conceptService.GetConcept(EntityRelationshipTypeKeys.Parent, true),
					this.conceptService.GetConcept(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, true)
				};

				model.RelationshipTypes.AddRange(concepts.ToSelectList(this.HttpContext.GetCurrentLanguage()));

				return View(model);
			}
			catch (Exception e)
			{
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
					var place = this.entityService.Get<Place>(model.Inverse ? Guid.Parse(model.TargetId) : model.SourceId);

					if (place == null)
					{
						Trace.TraceWarning("Could not locate entity {0}", model.SourceId);
						this.TempData["error"] = Locale.UnableToCreateRelatedPlace;
						return RedirectToAction("Edit", new { id = model.SourceId });
					}

					// if the relationship type is child, find the target place
					// and add a parent relationship to the target place, where the target of that relationship is the current place in context
					if (Guid.Parse(model.RelationshipType) == EntityRelationshipTypeKeys.Child)
					{
						var childPlace = this.entityService.Get<Place>(Guid.Parse(model.TargetId));

						childPlace.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Parent, place.Key)
						{
							Key = Guid.NewGuid(),
							Quantity = model.Quantity ?? 0,
							SourceEntityKey = Guid.Parse(model.TargetId)
						});

						this.entityService.Update(childPlace);
					}
					else if (Guid.Parse(model.RelationshipType) == EntityRelationshipTypeKeys.Parent)
					{
						place.Relationships.RemoveAll(r => r.TargetEntityKey == Guid.Parse(model.TargetId) && r.RelationshipTypeKey == Guid.Parse(model.RelationshipType));
						place.Relationships.Add(new EntityRelationship(Guid.Parse(model.RelationshipType), model.Inverse ? model.SourceId : Guid.Parse(model.TargetId)) { EffectiveVersionSequenceId = place.VersionSequence, Key = Guid.NewGuid(), Quantity = model.Quantity ?? 0, SourceEntityKey = model.SourceId });
					}
					else
					{
						place.Relationships.Add(new EntityRelationship(Guid.Parse(model.RelationshipType), model.Inverse ? model.SourceId : Guid.Parse(model.TargetId)) { EffectiveVersionSequenceId = place.VersionSequence, Key = Guid.NewGuid(), Quantity = model.Quantity ?? 0, SourceEntityKey = model.SourceId });
					}

					var updatedPlace = this.entityService.Update(place);

					this.TempData["success"] = Locale.RelatedPlaceCreatedSuccessfully;

					return RedirectToAction("Edit", new { id = updatedPlace.Key.Value, versionId = updatedPlace.VersionKey });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to create related place: { e }");
			}

			var concepts = new List<Concept>
			{
				this.conceptService.GetConcept(EntityRelationshipTypeKeys.Child, true),
				this.conceptService.GetConcept(EntityRelationshipTypeKeys.Parent, true)
			};

			Guid relationshipType;

			if (Guid.TryParse(model.RelationshipType, out relationshipType))
			{
				model.RelationshipTypes.AddRange(concepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == relationshipType));
			}

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
				var place = this.entityService.Get<Place>(id);

				if (place == null)
				{
					TempData["error"] = Locale.PlaceNotFound;

					return RedirectToAction("Index");
				}

				this.TempData["success"] = Locale.PlaceDeactivatedSuccessfully;

				var result = this.entityService.Obsolete(place);

				return RedirectToAction("ViewPlace", new { id = result.Key, versionId = result.VersionKey });
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to delete place: {e}");
			}

			TempData["error"] = Locale.UnableToDeactivatePlace;

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
				var place = this.entityService.Get<Place>(id, versionId);

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

				var areasServed = new List<EntityRelationship>();
				var dedicatedServiceDeliveryLocations = new List<EntityRelationship>();
				var relationships = new List<EntityRelationship>();

				// get relationships where I am the source and the relationship type is parent
				relationships.AddRange(this.entityRelationshipService.GetEntityRelationshipsBySource(place.Key.Value, EntityRelationshipTypeKeys.Parent));

				// get relationships where I am the target and the relationship type is parent
				relationships.AddRange(this.entityRelationshipService.GetEntityRelationshipsByTarget(place.Key.Value, EntityRelationshipTypeKeys.Parent));

				// get relationships where I am the source and the relationship type is child
				//relationships.AddRange(this.entityRelationshipService.GetEntityRelationshipsBySource(place.Key.Value, EntityRelationshipTypeKeys.Child));

				// if I am not a service delivery location, load the dedicated service delivery locations which I am pointing at
				if (place.ClassConceptKey != EntityClassKeys.ServiceDeliveryLocation)
				{
					// get relationships where I am the source of type dedicated service delivery location
					dedicatedServiceDeliveryLocations.AddRange(entityRelationshipService.GetEntityRelationshipsBySource(place.Key.Value, EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation));
				}
				else
				{
					// get relationships where I am the target of type dedicated service delivery location
					areasServed.AddRange(entityRelationshipService.GetEntityRelationshipsByTarget(place.Key.Value, EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, new List<Type> { typeof(Person) }));
				}

				// get relationships where I am the target of type parent
				relationships.AddRange(entityRelationshipService.GetEntityRelationshipsByTarget(place.Key.Value, EntityRelationshipTypeKeys.Parent));

				// set the relationships where I am the parent to relationship type of child
				// this is for display purposes only
				foreach (var relationship in relationships.Where(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent && r.TargetEntityKey == place.Key))
				{
					relationship.RelationshipType = this.conceptService.GetConcept(EntityRelationshipTypeKeys.Child, true);
				}

				place.Relationships = relationships.Intersect(place.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EditPlaceModel(place)
				{
					AreasServed = areasServed.Select(r => new EntityRelationshipModel(r, place.Type, place.ClassConceptKey?.ToString(), r.TargetEntityKey == place.Key) { Quantity = r.Quantity }).ToList(),
					DedicatedServiceDeliveryLocations = dedicatedServiceDeliveryLocations.Select(r => new EntityRelationshipModel(r, place.Type, place.ClassConceptKey?.ToString(), r.TargetEntityKey == place.Key) { Quantity = r.Quantity }).ToList(),
					UpdatedBy = this.userService.GetUserEntityBySecurityUserKey(place.CreatedByKey.Value)?.GetFullName(NameUseKeys.OfficialRecord)
				};

				// get the place type concepts
				model.TypeConcepts.AddRange(this.placeConceptService.GetPlaceTypeConcepts().ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == place.TypeConceptKey).ToList());

				// order the type concept list
				model.TypeConcepts = model.TypeConcepts.OrderBy(c => c.Text).ToList();

				return View(model);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve place: { e }");
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

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
				if (model.HasOnlyYearOrPopulation())
				{
					if (string.IsNullOrWhiteSpace(model.Year)) ModelState.AddModelError("Year", Locale.TargetYearRequired);

					if (model.TargetPopulation == null) ModelState.AddModelError("TargetPopulation", Locale.TargetPopulationRequired);
				}

				if (!string.IsNullOrWhiteSpace(model.Year))
				{
					if (model.ConvertToPopulationYear() == 0) ModelState.AddModelError("Year", Locale.PopulationYearInvalidFormat);
				}

				if (ModelState.IsValid)
				{
					var place = this.entityService.Get<Place>(model.Id, null, p => p.ObsoletionTime == null);

					if (place == null)
					{
						TempData["error"] = Locale.PlaceNotFound;

						return RedirectToAction("Index");
					}

					// repopulate incase the update fails
					model.Identifiers = place.Identifiers.Select(i => new EntityIdentifierModel(i.Key.Value, place.Key.Value)).ToList();
					model.Relationships = place.Relationships.Select(r => new EntityRelationshipModel(r)).ToList();

					// ensure the type concept list is not null
					if (model.TypeConcepts == null)
					{
						model.TypeConcepts = new List<SelectListItem>();
					}

					// get the place type concepts
					model.TypeConcepts.AddRange(this.placeConceptService.GetPlaceTypeConcepts().ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == place.TypeConceptKey).ToList());

					// order the type concept list
					model.TypeConcepts = model.TypeConcepts.OrderBy(c => c.Text).ToList();

					var placeToUpdate = model.ToPlace(place);

					if (model.SubmitYearAndPopulation())
					{
						placeToUpdate.Extensions.RemoveAll(e => e.ExtensionType.Name == Constants.TargetPopulationUrl);

						var entityExtension = new EntityExtension
						{
							ExtensionType = new ExtensionType(Constants.TargetPopulationUrl, typeof(DictionaryExtensionHandler))
							{
								Key = Constants.TargetPopulationExtensionTypeKey
							},
							ExtensionValue = new TargetPopulation(model.ConvertPopulationToULong(), model.ConvertToPopulationYear())
						};

						placeToUpdate.Extensions.Add(entityExtension);
					}

					placeToUpdate.CreatedByKey = Guid.Parse(this.User.Identity.GetUserId());

					var updatedPlace = this.entityService.Update(placeToUpdate);

					TempData["success"] = Locale.PlaceSuccessfullyUpdated;

					return RedirectToAction("ViewPlace", new { id = updatedPlace.Key, versionId = updatedPlace.VersionKey });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update place: {e}");
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
				var place = this.entityService.Get<Place>(id);

				if (place == null)
				{
					this.TempData["error"] = Locale.PlaceNotFound;
					return RedirectToAction("Edit", new { id = id });
				}

				var entityRelationship = place.Relationships.Find(r => r.Key == entityRelationshipId);

				var model = new EntityRelationshipModel(entityRelationship, place.Type, place.ClassConceptKey?.ToString());

				return View(model);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to edit related place: { e }");
			}

			this.TempData["error"] = Locale.PlaceNotFound;
			return RedirectToAction("Edit", new { id = id });
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
					var place = this.entityService.Get<Place>(model.SourceId);

					if (place == null)
					{
						this.TempData["error"] = Locale.UnableToUpdatePlace;
						return RedirectToAction("Edit", new { id = model.SourceId });
					}

					place.Relationships.RemoveAll(r => r.Key == model.Id);
					place.Relationships.Add(new EntityRelationship(Guid.Parse(model.RelationshipType), Guid.Parse(model.TargetId)));

					var updatedPlace = this.entityService.Update(place);

					return RedirectToAction("Edit", new { id = updatedPlace.Key.Value });
				}
			}
			catch (Exception e)
			{
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

			var typeFilter = new List<SelectListItem>();

			try
			{
				// get the place type concepts
				typeFilter.AddRange(this.placeConceptService.GetPlaceTypeConcepts().ToSelectList(this.HttpContext.GetCurrentLanguage()).ToList());

				// order the type concept list
				typeFilter = typeFilter.OrderBy(c => c.Text).ToList();
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to load type filter for the search place page: {e}");
			}

			TempData["typeFilter"] = typeFilter;

			return View();
		}

		/// <summary>
		/// Searches for a place.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <param name="searchType">Type of the search.</param>
		/// <returns>Returns a list of places which match the search term.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm, string searchType)
		{
			var results = new List<PlaceViewModel>();

			try
			{
				if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
				{
					results = this.entityService.Search<Place>(searchTerm, searchType, new[] { "typeConcept", "address" }).Select(p => new PlaceViewModel(p)).OrderBy(p => p.Name).ToList();
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to search for places: {e}");
			}

			TempData["searchTerm"] = searchTerm;

			return PartialView("_PlaceSearchResultsPartial", results);
		}

		/// <summary>
		/// Searches for an address component.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <param name="classConcept">The class concept.</param>
		/// <returns>Returns a list of address components which match the search term.</returns>
		[HttpGet]
		public ActionResult SearchAddressAjax(string searchTerm, string classConcept)
		{
			var results = new List<PlaceViewModel>();

			if (string.IsNullOrEmpty(searchTerm) || string.IsNullOrWhiteSpace(searchTerm))
			{
				return Json(results, JsonRequestBehavior.AllowGet);
			}

			try
			{
				Guid classConceptKey;

				if (Guid.TryParse(classConcept, out classConceptKey))
				{
					var places = this.entityService.Search<Place>(searchTerm, classConceptKey, 0, 15, new []{ "typeConcept", "address.use" });

					results = places.Select(p => new PlaceViewModel(p)).OrderBy(p => p.Name).ToList();
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to search for addresses: {e}");
			}

			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Searches for a place.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of places which match the search term.</returns>
		[HttpGet]
		public ActionResult SearchAjax(string searchTerm)
		{
			var results = new List<PlaceViewModel>();

			if (string.IsNullOrEmpty(searchTerm) || string.IsNullOrWhiteSpace(searchTerm))
			{
				return Json(results, JsonRequestBehavior.AllowGet);
			}
			try
			{
				var places = this.entityService.Search<Place>(searchTerm, new[] { "typeConcept", "address.use" });

				results = places.Select(p => new PlaceViewModel(p)).OrderBy(p => p.Name).ToList();
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to search for places: {e}");
			}

			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Searches for a dedicated service delivery location.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of dedicated service delivery locations which match the search term.</returns>
		[HttpGet]
		public ActionResult SearchDedicatedServiceDeliveryLocation(string searchTerm)
		{
			var results = new List<PlaceViewModel>();

			if (string.IsNullOrEmpty(searchTerm) || string.IsNullOrWhiteSpace(searchTerm))
			{
				return Json(results, JsonRequestBehavior.AllowGet);
			}
			try
			{
				var places = this.entityService.Search<Place>(searchTerm, EntityClassKeys.ServiceDeliveryLocation, new[] { "typeConcept", "address.use" });

				results = places.Select(p => new PlaceViewModel(p)).OrderBy(p => p.Name).ToList();
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to search for places: {e}");
			}

			return Json(results, JsonRequestBehavior.AllowGet);
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
				var place = this.entityService.Get<Place>(id);

				if (place == null)
				{
					TempData["error"] = Locale.PlaceNotFound;
					return RedirectToAction("Index");
				}

				var areasServed = new List<EntityRelationship>();
				var dedicatedServiceDeliveryLocations = new List<EntityRelationship>();
				var relationships = new List<EntityRelationship>();

				// get relationships where I am the source and the relationship type is parent
				relationships.AddRange(this.entityRelationshipService.GetEntityRelationshipsBySource(place.Key.Value, EntityRelationshipTypeKeys.Parent));

				// get relationships where I am the target and the relationship type is parent
				relationships.AddRange(this.entityRelationshipService.GetEntityRelationshipsByTarget(place.Key.Value, EntityRelationshipTypeKeys.Parent));

				// get relationships where I am the source and the relationship type is child
				//relationships.AddRange(this.entityRelationshipService.GetEntityRelationshipsBySource(place.Key.Value, EntityRelationshipTypeKeys.Child));

				// if I am not a service delivery location, load the dedicated service delivery locations which I am pointing at
				if (place.ClassConceptKey != EntityClassKeys.ServiceDeliveryLocation)
				{
					// get relationships where I am the source of type dedicated service delivery location
					dedicatedServiceDeliveryLocations.AddRange(entityRelationshipService.GetEntityRelationshipsBySource(place.Key.Value, EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation));
				}
				else
				{
					// get relationships where I am the target of type dedicated service delivery location
					areasServed.AddRange(entityRelationshipService.GetEntityRelationshipsByTarget(place.Key.Value, EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, new List<Type> { typeof(Person) }));
				}

				// get relationships where I am the target of type parent
				relationships.AddRange(entityRelationshipService.GetEntityRelationshipsByTarget(place.Key.Value, EntityRelationshipTypeKeys.Parent));

				// set the relationships where I am the parent to relationship type of child
				// this is for display purposes only
				foreach (var relationship in relationships.Where(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent && r.TargetEntityKey == place.Key))
				{
					relationship.RelationshipType = this.conceptService.GetConcept(EntityRelationshipTypeKeys.Child, true);
				}

				place.Relationships = relationships.Intersect(place.Relationships, new EntityRelationshipComparer()).ToList();

				var viewModel = new PlaceViewModel(place)
				{
					AreasServed = areasServed.Select(r => new EntityRelationshipViewModel(r, r.TargetEntityKey == place.Key)).OrderBy(r => r.TargetName).ToList(),
					DedicatedServiceDeliveryLocations = dedicatedServiceDeliveryLocations.Select(r => new EntityRelationshipViewModel(r, r.TargetEntityKey == place.Key)).OrderBy(r => r.TargetName).ToList(),
					UpdatedBy = this.userService.GetUserEntityBySecurityUserKey(place.CreatedByKey.Value)?.GetFullName(NameUseKeys.OfficialRecord)
				};

				return View(viewModel);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve place: { e }");
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

			return RedirectToAction("Index");
		}
	}
}