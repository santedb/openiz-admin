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
using OpenIZAdmin.Models.MaterialModels;
using OpenIZAdmin.Services.Entities;
using OpenIZAdmin.Services.Entities.Materials;
using OpenIZAdmin.Services.Metadata.Concepts;
using OpenIZAdmin.Services.Security.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using OpenIZAdmin.Services.Dataset;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing materials.
	/// </summary>
	[TokenAuthorize(Constants.UnrestrictedMetadata)]
	public class MaterialController : Controller
	{
		/// <summary>
		/// The materials cache key.
		/// </summary>
		private const string MaterialsCacheKey = "Materials";

		/// <summary>
		/// The concept service
		/// </summary>
		private readonly IConceptService conceptService;

		/// <summary>
		/// The dataset service.
		/// </summary>
		private readonly IDatasetService datasetService;

		/// <summary>
		/// The material service.
		/// </summary>
		private readonly IEntityService entityService;

		/// <summary>
		/// The material concept service.
		/// </summary>
		private readonly IMaterialConceptService materialConceptService;

		/// <summary>
		/// The user service.
		/// </summary>
		private readonly IUserService userService;

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialController"/> class.
		/// </summary>
		public MaterialController()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialController" /> class.
		/// </summary>
		/// <param name="conceptService">The concept service.</param>
		/// <param name="datasetService">The dataset service.</param>
		/// <param name="entityService">The entity service.</param>
		/// <param name="materialConceptService">The material concept service.</param>
		/// <param name="userService">The user service.</param>
		public MaterialController(IConceptService conceptService, IDatasetService datasetService, IEntityService entityService, IMaterialConceptService materialConceptService, IUserService userService)
		{
			this.conceptService = conceptService;
			this.datasetService = datasetService;
			this.entityService = entityService;
			this.materialConceptService = materialConceptService;
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
				var material = this.entityService.Get<Material>(id, versionId);

				if (material == null)
				{
					this.TempData["error"] = Locale.UnableToRetrieveMaterial;
					return RedirectToAction("Edit", new { id = id, versionId = versionId });
				}

				var updatedMaterial = entityService.Activate(material);

				this.TempData["success"] = Locale.MaterialActivatedSuccessfully;

				return RedirectToAction("Edit", new { id = id, versionId = updatedMaterial.VersionKey });
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to activate material: { e }");
			}

			this.TempData["error"] = Locale.UnableToActivateMaterial;

			return RedirectToAction("Edit", new { id = id, versionId = versionId });
		}

		/// <summary>
		/// Associates a material to another material.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the associate material page.</returns>
		public ActionResult AssociateMaterial(Guid id)
		{
			try
			{
				var material = this.entityService.Get<Material>(id);

				if (material == null)
				{
					this.TempData["error"] = Locale.MaterialNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				var relationships = new List<EntityRelationship>();

				relationships.AddRange(this.entityService.GetEntityRelationships<Material>(material.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.UsedEntity && r.ObsoleteVersionSequenceId == null).ToList());

				material.Relationships = relationships.Intersect(material.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					RelationshipType = EntityRelationshipTypeKeys.UsedEntity.ToString(),
					ExistingRelationships = material.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList(),
					TargetList = this.BuildMaterialSelectList(material)
				};

				var concepts = new List<Concept>
				{
					this.conceptService.GetConcept(EntityRelationshipTypeKeys.UsedEntity, true)
				};

				model.RelationshipTypes.AddRange(concepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == EntityRelationshipTypeKeys.UsedEntity));

				return View(model);
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
				Trace.TraceError($"Unable to load associate material page: { e }");
			}

			return RedirectToAction("Edit", new { id = id });
		}

		/// <summary>
		/// Associates the material.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AssociateMaterial(EntityRelationshipModel model)
		{
			var concepts = new List<Concept>();
			Material material = null;

			try
			{
				if (this.ModelState.IsValid)
				{
					// HACK: manually validating the quantity field, since for this particular page the quantity is required
					// but it feels like overkill to literally create the same model for the purpose of making only 1 property
					// required.
					if (!model.Quantity.HasValue)
					{
						this.ModelState.AddModelError(nameof(model.Quantity), Locale.QuantityRequired);
						this.TempData["error"] = Locale.QuantityRequired;

						concepts.Add(this.conceptService.GetConcept(EntityRelationshipTypeKeys.UsedEntity));

						model.RelationshipTypes.AddRange(concepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == EntityRelationshipTypeKeys.UsedEntity));

						return View(model);
					}

					material = this.entityService.Get<Material>(model.SourceId);

					if (material == null)
					{
						this.TempData["error"] = Locale.MaterialNotFound;
						return RedirectToAction("Edit", new { id = model.SourceId });
					}

					material.Relationships.RemoveAll(r => r.TargetEntityKey == Guid.Parse(model.TargetId) && r.RelationshipTypeKey == Guid.Parse(model.RelationshipType));
					material.Relationships.Add(new EntityRelationship(Guid.Parse(model.RelationshipType), Guid.Parse(model.TargetId)) { EffectiveVersionSequenceId = material.VersionSequence, Key = Guid.NewGuid(), Quantity = model.Quantity.Value, SourceEntityKey = model.SourceId });

					this.entityService.Update(material);

					this.TempData["success"] = Locale.MaterialRelatedSuccessfully;

					return RedirectToAction("Edit", new { id = material.Key.Value });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to create related manufactured material: { e }");
			}

			this.TempData["error"] = Locale.UnableToRelateMaterial;

			model.TargetList = this.BuildMaterialSelectList(material);

			concepts.Add(this.conceptService.GetConcept(EntityRelationshipTypeKeys.UsedEntity));
			model.RelationshipTypes.AddRange(concepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == EntityRelationshipTypeKeys.UsedEntity));

			return View(model);
		}

		/// <summary>
		/// Displays the create view.
		/// </summary>
		/// <returns>Returns the create view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			var formConcepts = this.materialConceptService.GetFormConcepts();
			var quantityConcepts = this.materialConceptService.GetQuantityConcepts();
			var typeConcepts = this.materialConceptService.GetMaterialTypeConcepts();

			var language = this.HttpContext.GetCurrentLanguage();

			var model = new CreateMaterialModel
			{
				FormConcepts = formConcepts.ToSelectList(language).ToList(),
				QuantityConcepts = quantityConcepts.ToSelectList(language).ToList(),
				TypeConcepts = typeConcepts.ToSelectList(language).ToList()
			};

			return View(model);
		}

		/// <summary>
		/// Creates a material.
		/// </summary>
		/// <param name="model">The model containing the information to create a material.</param>
		/// <returns>Returns the created material.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateMaterialModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var materialToCreate = model.ToMaterial();

					materialToCreate.CreatedByKey = Guid.Parse(this.User.Identity.GetUserId());

					var material = entityService.Create(materialToCreate);

					TempData["success"] = Locale.MaterialCreatedSuccessfully;

					return RedirectToAction("ViewMaterial", new { id = material.Key, versionId = material.VersionKey });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to create material: {e}");
			}

			var formConcepts = this.materialConceptService.GetFormConcepts();
			var quantityConcepts = this.materialConceptService.GetQuantityConcepts();
			var typeConcepts = this.materialConceptService.GetMaterialTypeConcepts();

			var language = this.HttpContext.GetCurrentLanguage();

			model.FormConcepts = formConcepts.ToSelectList(language).ToList();
			model.QuantityConcepts = quantityConcepts.ToSelectList(language).ToList();
			model.TypeConcepts = typeConcepts.ToSelectList(language).ToList();

			TempData["error"] = Locale.UnableToCreateMaterial;

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
				var material = this.entityService.Get<Material>(id);

				if (material == null)
				{
					this.TempData["error"] = Locale.MaterialNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				var relationships = new List<EntityRelationship>();

				relationships.AddRange(this.entityService.GetEntityRelationships<ManufacturedMaterial>(material.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Instance && r.ObsoleteVersionSequenceId == null).ToList());

				material.Relationships = relationships.Intersect(material.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					RelationshipType = EntityRelationshipTypeKeys.Instance.ToString(),
					ExistingRelationships = material.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList()
				};

				var concepts = new List<Concept>
				{
					this.conceptService.GetConcept(EntityRelationshipTypeKeys.Instance)
				};

				model.RelationshipTypes.AddRange(concepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == EntityRelationshipTypeKeys.Instance));

				return View(model);
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
				Trace.TraceError($"Unable to load create related manufactured material page: { e }");
			}

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
			var concepts = new List<Concept>();

			try
			{
				if (this.ModelState.IsValid)
				{
					// HACK: manually validating the quantity field, since for this particular page the quantity is required
					// but it feels like overkill to literally create the same model for the purpose of making only 1 property
					// required.
					if (!model.Quantity.HasValue)
					{
						this.ModelState.AddModelError(nameof(model.Quantity), Locale.QuantityRequired);
						this.TempData["error"] = Locale.QuantityRequired;

						concepts.Add(this.conceptService.GetConcept(EntityRelationshipTypeKeys.Instance, true));

						// re-populate the model
						var existingMaterial = this.entityService.Get<Material>(model.SourceId);

						var relationships = new List<EntityRelationship>();

						relationships.AddRange(this.entityService.GetEntityRelationships<ManufacturedMaterial>(existingMaterial.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Instance && r.ObsoleteVersionSequenceId == null).ToList());

						existingMaterial.Relationships = relationships.Intersect(existingMaterial.Relationships, new EntityRelationshipComparer()).ToList();

						model.ExistingRelationships = existingMaterial.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList();
						model.RelationshipTypes.AddRange(concepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == EntityRelationshipTypeKeys.Instance));

						return View(model);
					}

					var material = this.entityService.Get<Material>(model.SourceId);

					if (material == null)
					{
						this.TempData["error"] = Locale.UnableToCreateRelatedManufacturedMaterial;
						return RedirectToAction("Edit", new { id = model.SourceId });
					}

					material.Relationships.RemoveAll(r => r.TargetEntityKey == Guid.Parse(model.TargetId) && r.RelationshipTypeKey == Guid.Parse(model.RelationshipType));
					material.Relationships.Add(new EntityRelationship(Guid.Parse(model.RelationshipType), Guid.Parse(model.TargetId)) { Key = Guid.NewGuid(), Quantity = model.Quantity ?? 0, SourceEntityKey = model.SourceId });

					var updatedMaterial = this.entityService.Update(material);

					this.TempData["success"] = Locale.RelatedManufacturedMaterialCreatedSuccessfully;

					return RedirectToAction("Edit", new { id = updatedMaterial.Key.Value, versionId = updatedMaterial.VersionKey });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to create related manufactured material: { e }");
			}

			this.TempData["error"] = Locale.UnableToCreateRelatedManufacturedMaterial;

			concepts.Add(this.conceptService.GetConcept(EntityRelationshipTypeKeys.Instance, true));
			model.RelationshipTypes.AddRange(concepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == EntityRelationshipTypeKeys.Instance));

			return View(model);
		}

		/// <summary>
		/// Deletion of a material.
		/// </summary>
		/// <param name="id">The id of the material.</param>
		/// <returns>Returns the to the material search page.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var material = this.entityService.Get<Material>(id);

				if (material == null)
				{
					TempData["error"] = Locale.MaterialNotFound;

					return RedirectToAction("Index");
				}

				this.entityService.Deactivate(material);

				this.TempData["success"] = Locale.MaterialDeactivatedSuccessfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to deactivate material: {e}");
			}

			TempData["error"] = Locale.UnableToDeleteMaterial;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Downloads as dataset.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the material as a dataset instance.</returns>
		[HttpGet]
		public ActionResult DownloadAsDataset(Guid id)
		{
			try
			{
				var material = this.entityService.Get<Material>(id);

				if (material == null)
				{
					this.TempData["error"] = Locale.MaterialNotFound;
					return RedirectToAction("ViewMaterial", "Material", id);
				}

				var stream = this.datasetService.GetDatasetAsStream(this.datasetService.ConvertToDataset(material));
				string name;

				if (material.Names.Any(n => n.NameUseKey == NameUseKeys.Assigned))
				{
					name = string.Join(" ", material.Names.Where(n => n.NameUseKey == NameUseKeys.Assigned).SelectMany(n => n.Component).Select(c => c.Value));
				}
				else if (material.Names.Any(n => n.NameUseKey == NameUseKeys.OfficialRecord))
				{
					name = string.Join(" ", material.Names.Where(n => n.NameUseKey == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Select(c => c.Value));
				}
				else
				{
					name = string.Join(" ", material.Names.SelectMany(n => n.Component).Select(c => c.Value));
				}

				var contentDisposition = new ContentDisposition
				{
					FileName = $"Material-{name} {id}.xml",
					Inline = false
				};

				this.Response.AppendHeader("Content-Disposition", contentDisposition.ToString());

				return File(stream, MediaTypeNames.Text.Xml);
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
				Trace.TraceError($"Unable to download material: {e}");
			}

			return RedirectToAction("ViewMaterial", "Material", new { id, versionId = Guid.Empty });
		}

		/// <summary>
		/// Edit for material.
		/// </summary>
		/// <param name="id">The key of the material.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>Returns the edited material.</returns>
		[HttpGet]
		public ActionResult Edit(Guid id, Guid? versionId)
		{
			try
			{
				var material = entityService.Get<Material>(id, null, a => a.ClassConceptKey == EntityClassKeys.Material);

				if (material == null)
				{
					TempData["error"] = Locale.MaterialNotFound;

					return RedirectToAction("Index");
				}

				if (material.Tags.Any(t => t.TagKey == Constants.ImportedDataTag && t.Value?.ToLower() == "true"))
				{
					this.TempData["warning"] = Locale.RecordMustBeVerifiedBeforeEditing;
					return RedirectToAction("ViewMaterial", new { id, versionId });
				}

				var formConcepts = this.materialConceptService.GetFormConcepts();
				var quantityConcepts = this.materialConceptService.GetQuantityConcepts();
				var typeConcepts = this.materialConceptService.GetMaterialTypeConcepts();

				var relationships = new List<EntityRelationship>();

				foreach (var relationship in material.Relationships)
				{
					var rel = relationship;

					// only load the relationships which need data to be loaded
					if (relationship.RelationshipType == null || relationship.TargetEntity == null)
					{
						rel = entityService.GetEntityRelationship(relationship.Key.Value);
					}

					relationships.Add(rel);
				}

				material.Relationships = relationships;

				var model = new EditMaterialModel(material)
				{
					FormConcepts = formConcepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == material.FormConceptKey).ToList(),
					QuantityConcepts = quantityConcepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == material.QuantityConceptKey).ToList(),
					TypeConcepts = typeConcepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == material.TypeConceptKey).ToList(),
					UpdatedBy = this.userService.GetUserEntityBySecurityUserKey(material.CreatedByKey.Value)?.GetFullName(NameUseKeys.OfficialRecord)
				};

				return View(model);
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
				Trace.TraceError($"Unable to retrieve material: {e}");
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Edit for material.
		/// </summary>
		/// <param name="model">The model containing the information of the edit material.</param>
		/// <returns>Returns the edit for a material.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditMaterialModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var material = this.entityService.Get<Material>(model.Id, model.VersionKey, m => m.ClassConceptKey == EntityClassKeys.Material && m.ObsoletionTime == null);

					if (material == null)
					{
						TempData["error"] = Locale.MaterialNotFound;

						return RedirectToAction("Index");
					}

					var materialToUpdate = model.ToMaterial(material);

					materialToUpdate.CreatedByKey = Guid.Parse(this.User.Identity.GetUserId());

					var updatedEntity = this.entityService.Update(materialToUpdate);

					TempData["success"] = Locale.MaterialUpdatedSuccessfully;

					return RedirectToAction("ViewMaterial", new { id = updatedEntity.Key, versionId = updatedEntity.VersionKey });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update material: {e}");
			}
			finally
			{
				MvcApplication.MemoryCache.Remove(model.Id.ToString());
			}

			TempData["error"] = Locale.UnableToUpdateMaterial;

			return View(model);
		}

		/// <summary>
		/// Edits the related manufactured material.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>ActionResult.</returns>
		[HttpGet]
		public ActionResult EditRelatedManufacturedMaterial(Guid id)
		{
			try
			{
				var material = this.entityService.Get<Material>(id);

				if (material == null)
				{
					this.TempData["error"] = Locale.PlaceNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				for (var i = 0; i < material.Relationships.Count(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Instance && r.TargetEntity == null && r.TargetEntityKey.HasValue); i++)
				{
					material.Relationships[i].TargetEntity = this.entityService.Get<ManufacturedMaterial>(material.Relationships[i].TargetEntityKey.Value, null) as ManufacturedMaterial;
				}

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					ExistingRelationships = material.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList()
				};

				model.RelationshipTypes.AddRange(this.conceptService.GetConceptSet(ConceptSetKeys.EntityRelationshipType).Concepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == EntityRelationshipTypeKeys.OwnedEntity).ToList());

				return View(model);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve related manufactured material: { e }");
			}

			this.TempData["error"] = Locale.PlaceNotFound;

			return RedirectToAction("Edit", new { id = id });
		}

		/// <summary>
		/// Edits the related manufactured material.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>ActionResult.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditRelatedManufacturedMaterial(EntityRelationshipModel model)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					var material = this.entityService.Get<Material>(model.SourceId);

					if (material == null)
					{
						this.TempData["error"] = Locale.UnableToCreateRelatedManufacturedMaterial;
						return RedirectToAction("Edit", new { id = model.SourceId });
					}

					material.Relationships.RemoveAll(r => r.Key == model.Id);
					material.Relationships.Add(new EntityRelationship(Guid.Parse(model.RelationshipType), Guid.Parse(model.TargetId)));

					this.entityService.Update(material);

					this.TempData["success"] = Locale.RelatedManufacturedMaterialCreatedSuccessfully;

					return RedirectToAction("Edit", new { id = material.Key.Value });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update related manufactured material: { e }");
			}

			this.TempData["error"] = Locale.UnableToUpdateRelatedManufacturedMaterial;

			return View(model);
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "Material";
			TempData["searchTerm"] = "*";
			return View();
		}

		/// <summary>
		/// Searches for a material.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of materials which match the search term.</returns>
		[HttpGet]
		[ValidateInput(false)]
		public ActionResult Search(string searchTerm)
		{
			var results = new List<MaterialViewModel>();

			try
			{
				if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
				{
					results = entityService.Search<Material>(searchTerm).Select(p => new MaterialViewModel(p)).OrderBy(p => p.Name).ToList();
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to search for material: {e}");
			}

			TempData["searchTerm"] = searchTerm;

			return PartialView("_MaterialSearchResultsPartial", results);
		}

		/// <summary>
		/// Searches for a manufactured material.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of manufactured materials which match the search term.</returns>
		[HttpGet]
		public ActionResult SearchAjax(string searchTerm)
		{
			var viewModels = new List<MaterialViewModel>();

			if (ModelState.IsValid)
			{
				var materials = this.entityService.Query<Material>(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.StatusConceptKey == StatusKeys.Active && p.ClassConceptKey == EntityClassKeys.Material, 0, null, false);

				viewModels.AddRange(materials.Select(p => new MaterialViewModel(p)).OrderBy(p => p.Name));
			}

			return Json(viewModels, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// View for material.
		/// </summary>
		/// <param name="id">The id of the material.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>Returns the view for a material.</returns>
		[HttpGet]
		public ActionResult ViewMaterial(Guid id, Guid? versionId)
		{
			try
			{
				var material = this.entityService.Get<Material>(id);

				if (material == null)
				{
					TempData["error"] = Locale.MaterialNotFound;

					return RedirectToAction("Index");
				}

				material.FormConcept = this.conceptService.GetConcept(material.FormConceptKey, true);
				material.QuantityConcept = this.conceptService.GetConcept(material.QuantityConceptKey, true);
				material.TypeConcept = this.conceptService.GetConcept(material.TypeConceptKey, true);

				var relationships = new List<EntityRelationship>();

				foreach (var relationship in material.Relationships)
				{
					var rel = relationship;

					// only load the relationships which need data to be loaded
					if (relationship.RelationshipType == null || relationship.TargetEntity == null)
					{
						rel = entityService.GetEntityRelationship(relationship.Key.Value);
					}

					relationships.Add(rel);
				}

				material.Relationships = relationships;

				var viewModel = new MaterialViewModel(material)
				{
					UpdatedBy = this.userService.GetUserEntityBySecurityUserKey(material.CreatedByKey.Value)?.GetFullName(NameUseKeys.OfficialRecord)
				};

				return View(viewModel);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve material: {e}");
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Builds the material select list.
		/// </summary>
		/// <param name="material">The material.</param>
		/// <returns>Returns a select list of material values to use to associate materials to each other.</returns>
		private List<SelectListItem> BuildMaterialSelectList(Material material)
		{
			var selectList = new List<SelectListItem>
			{
				new SelectListItem
				{
					Text = string.Empty,
					Value = string.Empty
				}
			};

			var filterIds = new List<Guid>
			{
				material.Key.Value
			};

			filterIds.AddRange(material.Relationships.Select(r => r.TargetEntityKey.Value).ToArray());

			var materials = this.GetMaterials(filterIds.ToArray());

			foreach (var item in materials)
			{
				var selectListItem = new SelectListItem
				{
					Value = item.Key.ToString()
				};

				Guid? nameUseKey = null;

				if (item.Names?.Any(n => n.NameUseKey == NameUseKeys.Assigned) == true)
				{
					nameUseKey = NameUseKeys.Assigned;
				}
				else if (item.Names?.Any(n => n.NameUseKey == NameUseKeys.OfficialRecord) == true)
				{
					nameUseKey = NameUseKeys.OfficialRecord;
				}
				else
				{
					// get the first name component value
					selectListItem.Text = item.Names?.FirstOrDefault()?.Component?.FirstOrDefault()?.Value;
				}

				if (nameUseKey != null)
				{
					selectListItem.Text = item.Names.First(n => n.NameUseKey == nameUseKey).Component.FirstOrDefault()?.Value;
				}

				selectList.Add(selectListItem);
			}

			return selectList.OrderBy(t => t.Text).ToList();
		}

		/// <summary>
		/// Gets the materials.
		/// </summary>
		/// <param name="filterIds">The filter ids.</param>
		/// <returns>Returns a list of materials.</returns>
		private IEnumerable<Material> GetMaterials(params Guid[] filterIds)
		{
			var materials = MvcApplication.MemoryCache.Get(MaterialsCacheKey) as IEnumerable<Material>;

			if (materials == null)
			{
				materials = this.entityService.Query<Material>(r => r.ClassConceptKey == EntityClassKeys.Material && r.StatusConceptKey == StatusKeys.Active && r.ObsoletionTime == null);

				MvcApplication.MemoryCache.Set(MaterialsCacheKey, materials, MvcApplication.CacheItemPolicy);
			}

			if (filterIds.Any())
			{
				materials = materials.Where(m => !filterIds.Any(f => f == m.Key));
			}

			return materials;
		}
	}
}