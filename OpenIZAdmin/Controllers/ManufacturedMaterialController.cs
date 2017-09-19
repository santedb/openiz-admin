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
 * Date: 2017-3-27
 */

using Microsoft.AspNet.Identity;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ManufacturedMaterialModels;
using OpenIZAdmin.Services.Entities;
using OpenIZAdmin.Services.Entities.ManufacturedMaterials;
using OpenIZAdmin.Services.Entities.Materials;
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
	/// Provides operations for managing manufactured materials.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Controllers.BaseController" />
	[TokenAuthorize(Constants.UnrestrictedMetadata)]
	public class ManufacturedMaterialController : Controller
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
		/// The manufactured material concept service.
		/// </summary>
		private readonly IManufacturedMaterialConceptService manufacturedMaterialConceptService;

		/// <summary>
		/// The material concept service.
		/// </summary>
		private readonly IMaterialConceptService materialConceptService;

		/// <summary>
		/// The user service.
		/// </summary>
		private readonly IUserService userService;

		/// <summary>
		/// Initializes a new instance of the <see cref="ManufacturedMaterialController"/> class.
		/// </summary>
		public ManufacturedMaterialController(IConceptService conceptService, IEntityService entityService, IMaterialConceptService materialConceptService, IManufacturedMaterialConceptService manufacturedMaterialConceptService, IUserService userService)
		{
			this.conceptService = conceptService;
			this.entityService = entityService;
			this.materialConceptService = materialConceptService;
			this.manufacturedMaterialConceptService = manufacturedMaterialConceptService;
			this.userService = userService;
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
			var typeConcepts = this.manufacturedMaterialConceptService.GetMaterialTypeConcepts();

			var language = this.HttpContext.GetCurrentLanguage();

			var model = new CreateManufacturedMaterialModel
			{
				FormConcepts = formConcepts.ToSelectList(language).ToList(),
				QuantityConcepts = quantityConcepts.ToSelectList(language).ToList(),
				TypeConcepts = typeConcepts.ToSelectList(language).ToList()
			};

			return View(model);
		}

		/// <summary>
		/// Creates a manufactured material.
		/// </summary>
		/// <param name="model">The model containing the information to create a manufactured material.</param>
		/// <returns>Returns the created manufactured material.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateManufacturedMaterialModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var manufacturedMaterialToCreate = model.ToManufacturedMaterial();

					manufacturedMaterialToCreate.CreatedByKey = Guid.Parse(this.User.Identity.GetUserId());

					var material = entityService.Create(manufacturedMaterialToCreate);

					TempData["success"] = Locale.ManufacturedMaterialCreatedSuccessfully;

					return RedirectToAction("ViewManufacturedMaterial", new { id = material.Key, versionId = material.VersionKey });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to create manufactured material: {e}");
			}

			var formConcepts = this.materialConceptService.GetFormConcepts();
			var quantityConcepts = this.materialConceptService.GetQuantityConcepts();
			var typeConcepts = this.materialConceptService.GetMaterialTypeConcepts();

			var language = this.HttpContext.GetCurrentLanguage();

			model.FormConcepts = formConcepts.ToSelectList(language).ToList();
			model.QuantityConcepts = quantityConcepts.ToSelectList(language).ToList();
			model.TypeConcepts = typeConcepts.ToSelectList(language).ToList();

			TempData["error"] = Locale.UnableToCreateManufacturedMaterial;

			return View(model);
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
				var manufacturedMaterial = entityService.Get<ManufacturedMaterial>(id, null, a => a.ClassConceptKey == EntityClassKeys.ManufacturedMaterial);

				if (manufacturedMaterial == null)
				{
					TempData["error"] = Locale.ManufacturedMaterialNotFound;

					return RedirectToAction("Index");
				}

				if (manufacturedMaterial.Tags.Any(t => t.TagKey == Constants.ImportedDataTag && t.Value?.ToLower() == "true"))
				{
					this.TempData["warning"] = Locale.RecordMustBeVerifiedBeforeEditing;
					return RedirectToAction("ViewManufacturedMaterial", new { id, versionId });
				}

				var formConcepts = this.materialConceptService.GetFormConcepts();
				var quantityConcepts = this.materialConceptService.GetQuantityConcepts();
				var typeConcepts = this.manufacturedMaterialConceptService.GetMaterialTypeConcepts();

				//var relationships = new List<EntityRelationship>();

				//relationships.AddRange(this.entityService.GetEntityRelationships<Material>(material.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.UsedEntity && r.ObsoleteVersionSequenceId == null).ToList());
				//relationships.AddRange(this.entityService.GetEntityRelationships<ManufacturedMaterial>(material.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Instance && r.ObsoleteVersionSequenceId == null).ToList());

				//manufacturedMaterial.Relationships = relationships.Intersect(manufacturedMaterial.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EditManufacturedMaterialModel(manufacturedMaterial)
				{
					FormConcepts = formConcepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == manufacturedMaterial.FormConceptKey).ToList(),
					QuantityConcepts = quantityConcepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == manufacturedMaterial.QuantityConceptKey).ToList(),
					TypeConcepts = typeConcepts.ToSelectList(this.HttpContext.GetCurrentLanguage(), c => c.Key == manufacturedMaterial.TypeConceptKey).ToList(),
					UpdatedBy = this.userService.GetUserEntityBySecurityUserKey(manufacturedMaterial.CreatedByKey.Value)?.GetFullName(NameUseKeys.OfficialRecord)
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
		public ActionResult Edit(EditManufacturedMaterialModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var manufacturedMaterial = this.entityService.Get<ManufacturedMaterial>(model.Id, model.VersionKey, m => m.ClassConceptKey == EntityClassKeys.ManufacturedMaterial && m.ObsoletionTime == null);

					if (manufacturedMaterial == null)
					{
						TempData["error"] = Locale.ManufacturedMaterialNotFound;

						return RedirectToAction("Index");
					}

					var manufacturedMaterialToUpdate = model.ToManufacturedMaterial(manufacturedMaterial);

					manufacturedMaterialToUpdate.CreatedByKey = Guid.Parse(this.User.Identity.GetUserId());

					var updatedEntity = this.entityService.Update(manufacturedMaterialToUpdate);

					TempData["success"] = Locale.ManufacturedMaterialUpdatedSuccessfully;

					return RedirectToAction("ViewManufacturedMaterial", new { id = updatedEntity.Key, versionId = updatedEntity.VersionKey });
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update manufactured material: {e}");
			}
			finally
			{
				MvcApplication.MemoryCache.Remove(model.Id.ToString());
			}

			TempData["error"] = Locale.UnableToUpdateManufacturedMaterial;

			return View(model);
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			TempData["searchType"] = "ManufacturedMaterial";
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
			var results = new List<ManufacturedMaterialViewModel>();

			try
			{
				if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
				{
					results = entityService.Search<ManufacturedMaterial>(searchTerm).Select(p => new ManufacturedMaterialViewModel(p)).OrderBy(p => p.Name).ToList();
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to search for manufactured material: {e}");
			}

			TempData["searchTerm"] = searchTerm;

			return PartialView("_ManufacturedMaterialSearchResultsPartial", results);
		}

		/// <summary>
		/// Searches for a manufactured material.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of manufactured materials which match the search term.</returns>
		[HttpGet]
		public ActionResult SearchAjax(string searchTerm)
		{
			var results = new List<ManufacturedMaterialViewModel>();

			try
			{
				if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
				{
					results = entityService.Search<ManufacturedMaterial>(searchTerm).Select(p => new ManufacturedMaterialViewModel(p)).OrderBy(p => p.Name).ToList();
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to search for manufactured material: {e}");
			}

			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Views the manufactured material.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>ActionResult.</returns>
		[HttpGet]
		public ActionResult ViewManufacturedMaterial(Guid id, Guid? versionId)
		{
			try
			{
				var manufacturedMaterial = this.entityService.Get<ManufacturedMaterial>(id);

				if (manufacturedMaterial == null)
				{
					TempData["error"] = Locale.MaterialNotFound;

					return RedirectToAction("Index");
				}

				manufacturedMaterial.FormConcept = this.conceptService.GetConcept(manufacturedMaterial.FormConceptKey, true);
				manufacturedMaterial.QuantityConcept = this.conceptService.GetConcept(manufacturedMaterial.QuantityConceptKey, true);
				manufacturedMaterial.TypeConcept = this.conceptService.GetConcept(manufacturedMaterial.TypeConceptKey, true);

				//var relationships = new List<EntityRelationship>();

				//relationships.AddRange(this.entityService.GetEntityRelationships<Material>(manufacturedMaterial.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.UsedEntity && r.ObsoleteVersionSequenceId == null).ToList());
				//relationships.AddRange(this.entityService.GetEntityRelationships<ManufacturedMaterial>(manufacturedMaterial.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Instance && r.ObsoleteVersionSequenceId == null).ToList());

				//manufacturedMaterial.Relationships = relationships.Intersect(manufacturedMaterial.Relationships, new EntityRelationshipComparer()).ToList();

				var viewModel = new ManufacturedMaterialViewModel(manufacturedMaterial)
				{
					UpdatedBy = this.userService.GetUserEntityBySecurityUserKey(manufacturedMaterial.CreatedByKey.Value)?.GetFullName(NameUseKeys.OfficialRecord)
				};

				return View(viewModel);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve manufactured material: {e}");
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

			return RedirectToAction("Index");
		}
	}
}