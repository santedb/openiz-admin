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
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Comparer;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.EntityRelationshipModels;
using OpenIZAdmin.Models.OrganizationModels;
using OpenIZAdmin.Services.Entities;
using OpenIZAdmin.Services.Entities.Organizations;
using OpenIZAdmin.Services.Metadata.Concepts;
using OpenIZAdmin.Services.Security.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing organizations.
	/// </summary>
	[TokenAuthorize(Constants.UnrestrictedMetadata)]
	public class OrganizationController : Controller
	{
		/// <summary>
		/// The concept service.
		/// </summary>
		private readonly IConceptService conceptService;

		/// <summary>
		/// The entity service.
		/// </summary>
		private readonly IEntityService entityService;

		/// <summary>
		/// The organization concept service.
		/// </summary>
		private readonly IOrganizationConceptService organizationConceptService;

		/// <summary>
		/// The user service.
		/// </summary>
		private readonly IUserService userService;

		/// <summary>
		/// Initializes a new instance of the <see cref="OrganizationController" /> class.
		/// </summary>
		/// <param name="conceptService">The concept service.</param>
		/// <param name="entityService">The entity service.</param>
		/// <param name="organizationConceptService">The organization concept service.</param>
		/// <param name="userService">The user service.</param>
		public OrganizationController(IConceptService conceptService, IEntityService entityService, IOrganizationConceptService organizationConceptService, IUserService userService)
		{
			this.conceptService = conceptService;
			this.entityService = entityService;
			this.organizationConceptService = organizationConceptService;
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
				var organization = this.entityService.Get<Organization>(id);

				if (organization == null)
				{
					this.TempData["error"] = Locale.UnableToRetrieveOrganization;
					return RedirectToAction("Edit", new { id = id, versionId = versionId });
				}

				var updatedOrganization = this.entityService.Activate(organization);

				this.TempData["success"] = Locale.OrganizationActivatedSuccessfully;

				return RedirectToAction("Edit", new { id = id, versionId = updatedOrganization.VersionKey });
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to activate organization: { e }");
			}

			this.TempData["error"] = Locale.UnableToActivateOrganization;

			return RedirectToAction("Edit", new { id = id, versionId = versionId });
		}

		/// <summary>
		/// Displays the create view.
		/// </summary>
		/// <returns>Returns the create view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			var model = new CreateOrganizationModel();

			try
			{
				model.IndustryConcepts = organizationConceptService.GetIndustryConcepts().ToSelectList(this.HttpContext.GetCurrentLanguage()).ToList();
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to load industry concepts: {e}");
			}

			return View(model);
		}

		/// <summary>
		/// Creates a organization.
		/// </summary>
		/// <param name="model">The model containing the information to create a organization.</param>
		/// <returns>Returns the created organization.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateOrganizationModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var organizationToCreate = model.ToOrganization();

					organizationToCreate.CreatedByKey = Guid.Parse(this.User.Identity.GetUserId());

					var organization = this.entityService.Create(organizationToCreate);

					TempData["success"] = Locale.OrganizationCreatedSuccessfully;

					return RedirectToAction("ViewOrganization", new { id = organization.Key, versionId = organization.VersionKey });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to create organization: { e }");
			}

			var industryConceptSet = this.organizationConceptService.GetIndustryConcepts();

			model.IndustryConcepts = industryConceptSet.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == Guid.Parse(model.IndustryConcept)).ToList();

			TempData["error"] = Locale.UnableToCreateOrganization;

			return View(model);
		}

		/// <summary>
		/// Creates the related manufactured material.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>ActionResult.</returns>
		[HttpGet]
		public ActionResult CreateRelatedManufacturedMaterial(Guid id)
		{
			try
			{
				var organization = this.entityService.Get<Organization>(id);

				if (organization == null)
				{
					this.TempData["error"] = Locale.OrganizationNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				var relationships = new List<EntityRelationship>();

				relationships.AddRange(this.entityService.GetEntityRelationships<ManufacturedMaterial>(organization.Key.Value, r => (r.RelationshipTypeKey == EntityRelationshipTypeKeys.Instance || r.RelationshipTypeKey == EntityRelationshipTypeKeys.ManufacturedProduct) && r.ObsoleteVersionSequenceId == null).ToList());

				organization.Relationships = relationships.Intersect(organization.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					ExistingRelationships = organization.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList()
				};

				var concepts = new List<Concept>
				{
					this.conceptService.GetConcept(EntityRelationshipTypeKeys.Instance),
					this.conceptService.GetConcept(EntityRelationshipTypeKeys.ManufacturedProduct)
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
		/// Creates the related manufactured material.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateRelatedManufacturedMaterial(EntityRelationshipModel model)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					var organization = this.entityService.Get<Organization>(model.SourceId);

					if (organization == null)
					{
						this.TempData["error"] = Locale.UnableToCreateRelatedManufacturedMaterial;
						return RedirectToAction("Edit", new { id = model.SourceId });
					}

					organization.Relationships.RemoveAll(r => r.TargetEntityKey == Guid.Parse(model.TargetId) && r.RelationshipTypeKey == Guid.Parse(model.RelationshipType));
					organization.Relationships.Add(new EntityRelationship(Guid.Parse(model.RelationshipType), Guid.Parse(model.TargetId)) { EffectiveVersionSequenceId = organization.VersionSequence, Key = Guid.NewGuid(), Quantity = model.Quantity ?? 0, SourceEntityKey = model.SourceId });

					this.entityService.Update(organization);

					this.TempData["success"] = Locale.RelatedManufacturedMaterialCreatedSuccessfully;

					return RedirectToAction("Edit", new { id = organization.Key.Value });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to create related manufactured material: { e }");
			}

			var concepts = new List<Concept>
			{
				this.conceptService.GetConcept(EntityRelationshipTypeKeys.Instance),
				this.conceptService.GetConcept(EntityRelationshipTypeKeys.ManufacturedProduct)
			};

			model.RelationshipTypes.AddRange(concepts.ToSelectList(this.HttpContext.GetCurrentLanguage()));

			this.TempData["error"] = Locale.UnableToCreateRelatedManufacturedMaterial;

			return View(model);
		}

		/// <summary>
		/// Deletes the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var organization = this.entityService.Get<Organization>(id);

				if (organization == null)
				{
					TempData["error"] = Locale.OrganizationNotFound;

					return RedirectToAction("Index");
				}

				this.entityService.Obsolete(organization);

				this.TempData["success"] = Locale.OrganizationDeactivatedSuccessfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to delete organization: {e}");
			}

			TempData["error"] = Locale.UnableToDeleteOrganization;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Edits the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>ActionResult.</returns>
		[HttpGet]
		public ActionResult Edit(Guid id, Guid? versionId)
		{
			try
			{
				var organization = this.entityService.Get<Organization>(id);

				if (organization == null)
				{
					TempData["error"] = Locale.OrganizationNotFound;

					return RedirectToAction("Index");
				}

				if (organization.Tags.Any(t => t.TagKey == Constants.ImportedDataTag && t.Value?.ToLower() == "true"))
				{
					this.TempData["warning"] = Locale.RecordMustBeVerifiedBeforeEditing;
					return RedirectToAction("ViewOrganization", new { id, versionId });
				}

				var relationships = new List<EntityRelationship>();

				foreach (var relationship in organization.Relationships)
				{
					var rel = relationship;

					// only load the relationships which need data to be loaded
					if (relationship.RelationshipType == null || relationship.TargetEntity == null)
					{
						rel = entityService.GetEntityRelationship(relationship.Key.Value);
					}

					relationships.Add(rel);
				}

				organization.Relationships = relationships;

				var industryConceptSet = this.organizationConceptService.GetIndustryConcepts();

				var model = new EditOrganizationModel(organization)
				{
					IndustryConcepts = industryConceptSet?.ToSelectList(this.HttpContext.GetCurrentLanguage()).ToList(),
					UpdatedBy = this.userService.GetUserEntityBySecurityUserKey(organization.CreatedByKey.Value)?.GetFullName(NameUseKeys.OfficialRecord)
				};

				return View(model);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve organization: {e}");
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Edits the specified model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditOrganizationModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var organization = this.entityService.Get<Organization>(model.Id, model.VersionKey, m => m.ClassConceptKey == EntityClassKeys.Organization && m.ObsoletionTime == null);

					if (organization == null)
					{
						TempData["error"] = Locale.OrganizationNotFound;

						return RedirectToAction("Index");
					}

					var organizationToUpdate = model.ToOrganization(organization);

					organizationToUpdate.CreatedByKey = Guid.Parse(this.User.Identity.GetUserId());

					var updatedOrganization = this.entityService.Update(organizationToUpdate);

					TempData["success"] = Locale.OrganizationUpdatedSuccessfully;

					return RedirectToAction("ViewOrganization", new { id = updatedOrganization.Key, versionId = updatedOrganization.VersionKey });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update organization: {e}");
			}

			var industryConceptSet = this.organizationConceptService.GetIndustryConcepts();

			model.IndustryConcepts = industryConceptSet?.ToSelectList(this.HttpContext.GetCurrentLanguage()).ToList();

			TempData["error"] = Locale.UnableToUpdateOrganization;

			return View(model);
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "Organization";
			TempData["searchTerm"] = "*";
			return View();
		}

		/// <summary>
		/// Searches for a organization.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of organizations which match the search term.</returns>
		[HttpGet]
		[ValidateInput(false)]
		public ActionResult Search(string searchTerm)
		{
			var results = new List<OrganizationViewModel>();

			try
			{
				if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
				{
					results = entityService.Search<Organization>(searchTerm).Select(p => new OrganizationViewModel(p)).OrderBy(p => p.Name).ToList();
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to search for organization: {e}");
			}

			TempData["searchTerm"] = searchTerm;

			return PartialView("_OrganizationSearchResultsPartial", results);
		}

		/// <summary>
		/// View for organization.
		/// </summary>
		/// <param name="id">The id of the organization.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>Returns the view for a organization.</returns>
		[HttpGet]
		public ActionResult ViewOrganization(Guid id, Guid? versionId)
		{
			try
			{
				var organization = this.entityService.Get<Organization>(id);

				if (organization == null)
				{
					TempData["error"] = Locale.OrganizationNotFound;

					return RedirectToAction("Index");
				}

				var relationships = new List<EntityRelationship>();

				foreach (var relationship in organization.Relationships)
				{
					var rel = relationship;

					// only load the relationships which need data to be loaded
					if (relationship.RelationshipType == null || relationship.TargetEntity == null)
					{
						rel = entityService.GetEntityRelationship(relationship.Key.Value);
					}

					relationships.Add(rel);
				}

				organization.Relationships = relationships;

				var viewModel = new OrganizationViewModel(organization)
				{
					UpdatedBy = this.userService.GetUserEntityBySecurityUserKey(organization.CreatedByKey.Value)?.GetFullName(NameUseKeys.OfficialRecord)
				};

				return View(viewModel);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve organization: { e }");
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

			return RedirectToAction("Index");
		}
	}
}