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


using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Comparer;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.EntityRelationshipModels;
using OpenIZAdmin.Models.OrganizationModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing organizations.
	/// </summary>
	[TokenAuthorize]
	public class OrganizationController : AssociationController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OrganizationController"/> class.
		/// </summary>
		public OrganizationController()
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
				var organization = this.GetEntity<Organization>(id, null);

				if (organization == null)
				{
					this.TempData["error"] = Locale.UnableToRetrieveOrganization;
					return RedirectToAction("Edit", new { id = id, versionId = versionId });
				}

				organization.CreationTime = DateTimeOffset.Now;
				organization.StatusConceptKey = StatusKeys.Active;
				organization.VersionKey = null;

				var updatedOrganization = this.ImsiClient.Update(organization);

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
			var industryConceptSet = this.GetConceptSet(ConceptSetKeys.IndustryCode);

			var model = new CreateOrganizationModel
			{
				IndustryConcepts = industryConceptSet.ToSelectList().ToList()
			};

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
					var organization = this.ImsiClient.Create(model.ToOrganization());

					TempData["success"] = Locale.OrganizationCreatedSuccessfully;

					return RedirectToAction("ViewOrganization", new { id = organization.Key, versionId = organization.VersionKey });
				}
			}
			catch (Exception e)
			{
				
				Trace.TraceError($"Unable to create organization: { e }");
			}

			var industryConceptSet = this.GetConceptSet(ConceptSetKeys.IndustryCode);

			model.IndustryConcepts = industryConceptSet.Concepts.ToSelectList(c => c.Key == Guid.Parse(model.IndustryConcept)).ToList();

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
				var organization = this.GetEntity<Organization>(id);

				if (organization == null)
				{
					this.TempData["error"] = Locale.OrganizationNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				var relationships = new List<EntityRelationship>();

				relationships.AddRange(this.GetEntityRelationships<ManufacturedMaterial>(organization.Key.Value, r => (r.RelationshipTypeKey == EntityRelationshipTypeKeys.ManufacturedProduct || r.RelationshipTypeKey == EntityRelationshipTypeKeys.WarrantedProduct) && r.ObsoleteVersionSequenceId == null).ToList());

				organization.Relationships = relationships.Intersect(organization.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					ExistingRelationships = organization.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList()
				};

				var concepts = new List<Concept>
				{
					this.GetConcept(EntityRelationshipTypeKeys.ManufacturedProduct),
					this.GetConcept(EntityRelationshipTypeKeys.WarrantedProduct)
				};

				model.RelationshipTypes.AddRange(concepts.ToSelectList());

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
					var organization = this.GetEntity<Organization>(model.SourceId);

					if (organization == null)
					{
						this.TempData["error"] = Locale.UnableToCreateRelatedManufacturedMaterial;
						return RedirectToAction("Edit", new { id = model.SourceId });
					}

					organization.Relationships.RemoveAll(r => r.TargetEntityKey == model.TargetId && r.RelationshipTypeKey == Guid.Parse(model.RelationshipType));
					organization.Relationships.Add(new EntityRelationship(Guid.Parse(model.RelationshipType), model.TargetId) { EffectiveVersionSequenceId = organization.VersionSequence, Key = Guid.NewGuid(), Quantity = model.Quantity ?? 0, SourceEntityKey = model.SourceId });

					this.UpdateEntity<Organization>(organization);

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
				this.GetConcept(EntityRelationshipTypeKeys.ManufacturedProduct),
				this.GetConcept(EntityRelationshipTypeKeys.WarrantedProduct)
			};

			model.RelationshipTypes.AddRange(concepts.ToSelectList());

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
				var organization = this.GetEntity<Organization>(id);

				if (organization == null)
				{
					TempData["error"] = Locale.OrganizationNotFound;

					return RedirectToAction("Index");
				}

				this.ImsiClient.Obsolete<Organization>(organization);

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
				var organization = this.GetEntity<Organization>(id);

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

				relationships.AddRange(this.GetEntityRelationships<ManufacturedMaterial>(organization.Key.Value, r => (r.RelationshipTypeKey == EntityRelationshipTypeKeys.ManufacturedProduct || r.RelationshipTypeKey == EntityRelationshipTypeKeys.WarrantedProduct) && r.ObsoleteVersionSequenceId == null).ToList());

				organization.Relationships = relationships.Intersect(organization.Relationships, new EntityRelationshipComparer()).ToList();

				var industryConceptSet = this.GetConceptSet(ConceptSetKeys.IndustryCode);

				var model = new EditOrganizationModel(organization)
				{
					IndustryConcepts = industryConceptSet?.Concepts.ToSelectList().ToList()
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
					var organization = this.GetEntity<Organization>(model.Id, model.VersionKey, m => m.ClassConceptKey == EntityClassKeys.Organization && m.ObsoletionTime == null);

					if (organization == null)
					{
						TempData["error"] = Locale.OrganizationNotFound;

						return RedirectToAction("Index");
					}

					var updatedOrganization = this.ImsiClient.Update<Organization>(model.ToOrganization(organization));

					TempData["success"] = Locale.OrganizationUpdatedSuccessfully;

					return RedirectToAction("ViewOrganization", new { id = updatedOrganization.Key, versionId = updatedOrganization.VersionKey });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update organization: {e}");
			}

			var industryConceptSet = this.GetConceptSet(ConceptSetKeys.IndustryCode);

			model.IndustryConcepts = industryConceptSet?.Concepts.ToSelectList().ToList();

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
			IEnumerable<OrganizationViewModel> results = new List<OrganizationViewModel>();

			if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
			{
				Bundle bundle;

				Expression<Func<Organization, bool>> nameExpression = p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));

				if (searchTerm == "*")
				{
					bundle = this.ImsiClient.Query<Organization>(p => p.ClassConceptKey == EntityClassKeys.Organization);

					foreach (var organization in bundle.Item.OfType<Organization>().LatestVersionOnly())
					{
						organization.TypeConcept = this.GetTypeConcept(organization);
					}

					results = bundle.Item.OfType<Organization>().LatestVersionOnly().Select(p => new OrganizationViewModel(p)).OrderBy(p => p.Name).ToList();
				}
				else
				{
					bundle = this.ImsiClient.Query<Organization>(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.ClassConceptKey == EntityClassKeys.Organization);

					foreach (var organization in bundle.Item.OfType<Organization>().LatestVersionOnly())
					{
						organization.TypeConcept = this.GetTypeConcept(organization);
					}

					results = bundle.Item.OfType<Organization>().Where(nameExpression.Compile()).LatestVersionOnly().Select(p => new OrganizationViewModel(p)).OrderBy(p => p.Name).ToList();
				}
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
				var organization = this.GetEntity<Organization>(id);

				if (organization == null)
				{
					TempData["error"] = Locale.OrganizationNotFound;

					return RedirectToAction("Index");
				}

				var relationships = new List<EntityRelationship>();

				relationships.AddRange(this.GetEntityRelationships<ManufacturedMaterial>(organization.Key.Value, r => (r.RelationshipTypeKey == EntityRelationshipTypeKeys.ManufacturedProduct || r.RelationshipTypeKey == EntityRelationshipTypeKeys.WarrantedProduct) && r.ObsoleteVersionSequenceId == null).ToList());

				organization.Relationships = relationships.Intersect(organization.Relationships, new EntityRelationshipComparer()).ToList();

				return View(new OrganizationViewModel(organization));
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