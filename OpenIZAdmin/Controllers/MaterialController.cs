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
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.MaterialModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Web.Mvc;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Comparer;
using OpenIZAdmin.Models.EntityRelationshipModels;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing materials.
	/// </summary>
	[TokenAuthorize]
	public class MaterialController : AssociationController
	{
		/// <summary>
		/// The material types mnemonic.
		/// </summary>
		private const string MaterialTypesMnemonic = "MaterialTypes";

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialController"/> class.
		/// </summary>
		public MaterialController()
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
				var material = this.GetEntity<Material>(id, versionId);

				if (material == null)
				{
					this.TempData["error"] = Locale.UnableToRetrieveMaterial;
					return RedirectToAction("Edit", new { id = id, versionId = versionId });
				}

				material.CreationTime = DateTimeOffset.Now;
				material.StatusConceptKey = StatusKeys.Active;
				material.VersionKey = null;

				var updatedMaterial = this.ImsiClient.Update(material);

				this.TempData["success"] = Locale.MaterialActivatedSuccessfully;

				return RedirectToAction("Edit", new { id = id, versionId = updatedMaterial.VersionKey });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
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
				var material = this.GetEntity<Material>(id);

				if (material == null)
				{
					this.TempData["error"] = Locale.MaterialNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				var relationships = new List<EntityRelationship>();

				relationships.AddRange(this.GetEntityRelationships<Material>(material.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.UsedEntity && r.ObsoleteVersionSequenceId == null).ToList());

				material.Relationships = relationships.Intersect(material.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					RelationshipType = EntityRelationshipTypeKeys.UsedEntity.ToString(),
					ExistingRelationships = material.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList()
				};

				var concepts = new List<Concept>
				{
					this.GetConcept(EntityRelationshipTypeKeys.UsedEntity)
				};

				model.RelationshipTypes.AddRange(concepts.ToSelectList(c => c.Key == EntityRelationshipTypeKeys.UsedEntity));

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to load associate material page: { e }");
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
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

						concepts.Add(this.GetConcept(EntityRelationshipTypeKeys.UsedEntity));

						model.RelationshipTypes.AddRange(concepts.ToSelectList(c => c.Key == EntityRelationshipTypeKeys.UsedEntity));

						return View(model);
					}

					var material = this.GetEntity<Material>(model.SourceId);

					if (material == null)
					{
						this.TempData["error"] = Locale.MaterialNotFound;
						return RedirectToAction("Edit", new { id = model.SourceId });
					}

					material.Relationships.RemoveAll(r => r.TargetEntityKey == model.TargetId && r.RelationshipTypeKey == Guid.Parse(model.RelationshipType));
					material.Relationships.Add(new EntityRelationship(Guid.Parse(model.RelationshipType), model.TargetId) { EffectiveVersionSequenceId = material.VersionSequence, Key = Guid.NewGuid(), Quantity = model.Quantity.Value, SourceEntityKey = model.SourceId });

					this.ImsiClient.Update(material);

					this.TempData["success"] = Locale.MaterialRelatedSuccessfully;

					return RedirectToAction("Edit", new { id = material.Key.Value });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to create related manufactured material: { e }");
			}

			this.TempData["error"] = Locale.UnableToRelateMaterial;

			concepts.Add(this.GetConcept(EntityRelationshipTypeKeys.UsedEntity));
			model.RelationshipTypes.AddRange(concepts.ToSelectList(c => c.Key == EntityRelationshipTypeKeys.UsedEntity));

			return View(model);
		}

		/// <summary>
		/// Displays the create view.
		/// </summary>
		/// <returns>Returns the create view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			var formConcepts = this.GetFormConcepts();
			var quantityConcepts = this.GetQuantityConcepts();
			var typeConcepts = this.GetMaterialTypeConcepts();

			var model = new CreateMaterialModel
			{
				FormConcepts = formConcepts.ToSelectList().ToList(),
				QuantityConcepts = quantityConcepts.ToSelectList().ToList(),
				TypeConcepts = typeConcepts.ToSelectList().ToList()
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
					var material = this.ImsiClient.Create(model.ToMaterial());

					TempData["success"] = Locale.MaterialCreatedSuccessfully;

					return RedirectToAction("ViewMaterial", new { id = material.Key, versionId = material.VersionKey });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			var formConcepts = this.GetFormConcepts();
			var quantityConcepts = this.GetQuantityConcepts();
			var typeConcepts = this.GetMaterialTypeConcepts();

			model.FormConcepts = formConcepts.ToSelectList().ToList();
			model.QuantityConcepts = quantityConcepts.ToSelectList().ToList();
			model.TypeConcepts = typeConcepts.ToSelectList().ToList();

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
				var material = this.GetEntity<Material>(id);

				if (material == null)
				{
					this.TempData["error"] = Locale.MaterialNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				var relationships = new List<EntityRelationship>();

				relationships.AddRange(this.GetEntityRelationships<ManufacturedMaterial>(material.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.ManufacturedProduct && r.ObsoleteVersionSequenceId == null).ToList());

				material.Relationships = relationships.Intersect(material.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					RelationshipType = EntityRelationshipTypeKeys.ManufacturedProduct.ToString(),
					ExistingRelationships = material.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList()
				};

				var concepts = new List<Concept>
				{
					this.GetConcept(EntityRelationshipTypeKeys.ManufacturedProduct)
				};

				model.RelationshipTypes.AddRange(concepts.ToSelectList(c => c.Key == EntityRelationshipTypeKeys.ManufacturedProduct));

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to load create related manufactured material page: { e }");
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
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

						concepts.Add(this.GetConcept(EntityRelationshipTypeKeys.ManufacturedProduct));

						// re-populate the model
						var existingMaterial = this.GetEntity<Material>(model.SourceId);

						var relationships = new List<EntityRelationship>();

						relationships.AddRange(this.GetEntityRelationships<ManufacturedMaterial>(existingMaterial.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.ManufacturedProduct && r.ObsoleteVersionSequenceId == null).ToList());

						existingMaterial.Relationships = relationships.Intersect(existingMaterial.Relationships, new EntityRelationshipComparer()).ToList();

						model.ExistingRelationships = existingMaterial.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList();
						model.RelationshipTypes.AddRange(concepts.ToSelectList(c => c.Key == EntityRelationshipTypeKeys.ManufacturedProduct));

						return View(model);
					}

					var material = this.GetEntity<Material>(model.SourceId);

					if (material == null)
					{
						this.TempData["error"] = Locale.UnableToCreateRelatedManufacturedMaterial;
						return RedirectToAction("Edit", new { id = model.SourceId });
					}

					material.Relationships.RemoveAll(r => r.TargetEntityKey == model.TargetId && r.RelationshipTypeKey == Guid.Parse(model.RelationshipType));
					material.Relationships.Add(new EntityRelationship(Guid.Parse(model.RelationshipType), model.TargetId) { Key = Guid.NewGuid(), Quantity = model.Quantity ?? 0, SourceEntityKey = model.SourceId });

					var updatedMaterial = this.UpdateEntity<Material>(material);

					this.TempData["success"] = Locale.RelatedManufacturedMaterialCreatedSuccessfully;

					return RedirectToAction("Edit", new { id = updatedMaterial.Key.Value, versionId = updatedMaterial.VersionKey });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to create related manufactured material: { e }");
			}

			this.TempData["error"] = Locale.UnableToCreateRelatedManufacturedMaterial;

			concepts.Add(this.GetConcept(EntityRelationshipTypeKeys.ManufacturedProduct));
			model.RelationshipTypes.AddRange(concepts.ToSelectList(c => c.Key == EntityRelationshipTypeKeys.ManufacturedProduct));

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
				var material = this.GetEntity<Material>(id);

				if (material == null)
				{
					TempData["error"] = Locale.MaterialNotFound;

					return RedirectToAction("Index");
				}

				this.ImsiClient.Obsolete<Material>(material);

				TempData["success"] = Locale.MaterialDeletedSuccessfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToDeleteMaterial;

			return RedirectToAction("Index");
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
				var material = this.GetEntity<Material>(id);

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

				var formConcepts = this.GetFormConcepts();
				var quantityConcepts = this.GetQuantityConcepts();
				var typeConcepts = this.GetMaterialTypeConcepts();

				var relationships = new List<EntityRelationship>();

				relationships.AddRange(this.GetEntityRelationships<Material>(material.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.UsedEntity && r.ObsoleteVersionSequenceId == null).ToList());
				relationships.AddRange(this.GetEntityRelationships<ManufacturedMaterial>(material.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.ManufacturedProduct && r.ObsoleteVersionSequenceId == null).ToList());

				material.Relationships = relationships.Intersect(material.Relationships, new EntityRelationshipComparer()).ToList();

				var model = new EditMaterialModel(material)
				{
					FormConcepts = formConcepts.ToSelectList(c => c.Key == material.FormConceptKey).ToList(),
					QuantityConcepts = quantityConcepts.ToSelectList(c => c.Key == material.QuantityConceptKey).ToList(),
					TypeConcepts = typeConcepts.ToSelectList(c => c.Key == material.TypeConceptKey).ToList()
				};

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

			return RedirectToAction("Index");
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
				var material = this.GetEntity<Material>(id);

				if (material == null)
				{
					this.TempData["error"] = Locale.PlaceNotFound;

					return RedirectToAction("Edit", new { id = id });
				}

				for (var i = 0; i < material.Relationships.Count(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.ManufacturedProduct && r.TargetEntity == null && r.TargetEntityKey.HasValue); i++)
				{
					material.Relationships[i].TargetEntity = this.ImsiClient.Get<ManufacturedMaterial>(material.Relationships[i].TargetEntityKey.Value, null) as ManufacturedMaterial;
				}

				var model = new EntityRelationshipModel(Guid.NewGuid(), id)
				{
					ExistingRelationships = material.Relationships.Select(r => new EntityRelationshipViewModel(r)).ToList()
				};

				model.RelationshipTypes.AddRange(this.GetConceptSet(ConceptSetKeys.EntityRelationshipType).Concepts.ToSelectList(c => c.Key == EntityRelationshipTypeKeys.OwnedEntity).ToList());

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
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
					var material = this.GetEntity<Material>(model.SourceId);

					if (material == null)
					{
						this.TempData["error"] = Locale.UnableToCreateRelatedManufacturedMaterial;
						return RedirectToAction("Edit", new { id = model.SourceId });
					}

					material.Relationships.RemoveAll(r => r.Key == model.Id);
					material.Relationships.Add(new EntityRelationship(Guid.Parse(model.RelationshipType), model.TargetId));

					this.UpdateEntity<Material>(material);

					this.TempData["success"] = Locale.RelatedManufacturedMaterialCreatedSuccessfully;

					return RedirectToAction("Edit", new { id = material.Key.Value });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to update related manufactured material: { e }");
			}

			this.TempData["error"] = Locale.UnableToUpdateRelatedManufacturedMaterial;

			return View(model);
		}

		/// <summary>
		/// Gets the form concepts.
		/// </summary>
		/// <returns>Returns a list of material form concepts.</returns>
		private IEnumerable<Concept> GetFormConcepts()
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
		/// Gets the material type concepts.
		/// </summary>
		/// <returns>Returns a list of material type concepts.</returns>
		private IEnumerable<Concept> GetMaterialTypeConcepts()
		{
			var concepts = MvcApplication.MemoryCache.Get(MaterialTypesMnemonic) as IEnumerable<Concept>;

			if (concepts == null)
			{
				var bundle = this.ImsiClient.Query<ConceptSet>(c => c.Mnemonic == MaterialTypesMnemonic && c.ObsoletionTime == null, 0, null, new[] { "concept" });

				bundle.Reconstitute();

				var conceptSet = bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Mnemonic == MaterialTypesMnemonic && c.ObsoletionTime == null);

				if (conceptSet != null)
				{
					concepts = conceptSet.ConceptsXml.Select(c => this.GetConcept(c)).ToList();
					MvcApplication.MemoryCache.Set(new CacheItem(MaterialTypesMnemonic, concepts), MvcApplication.CacheItemPolicy);
				}
			}

			return concepts;
		}

		/// <summary>
		/// Gets the quantity concepts.
		/// </summary>
		/// <returns>Returns a list of material quantity concepts.</returns>
		private IEnumerable<Concept> GetQuantityConcepts()
		{
			var concepts = MvcApplication.MemoryCache.Get(ConceptClassKeys.UnitOfMeasure.ToString());

			if (concepts == null)
			{
				var bundle = this.ImsiClient.Query<Concept>(c => c.ClassKey == ConceptClassKeys.UnitOfMeasure && c.ObsoletionTime == null);

				bundle.Reconstitute();

				concepts = bundle.Item.OfType<Concept>().Where(c => c.ClassKey == ConceptClassKeys.UnitOfMeasure && c.ObsoletionTime == null);

				MvcApplication.MemoryCache.Set(new CacheItem(ConceptClassKeys.UnitOfMeasure.ToString(), concepts), MvcApplication.CacheItemPolicy);
			}

			return concepts as IEnumerable<Concept>;
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
					var material = this.GetEntity<Material>(model.Id, model.VersionKey, m => m.ClassConceptKey == EntityClassKeys.Material && m.ObsoletionTime == null);

					if (material == null)
					{
						TempData["error"] = Locale.MaterialNotFound;

						return RedirectToAction("Index");
					}

					var updatedEntity = this.UpdateEntity<Material>(model.ToMaterial(material));

					TempData["success"] = Locale.MaterialUpdatedSuccessfully;

					return RedirectToAction("ViewMaterial", new {id = updatedEntity.Key, versionId = updatedEntity.VersionKey});
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}
			finally
			{
				MvcApplication.MemoryCache.Remove(model.Id.ToString());
			}

			TempData["error"] = Locale.UnableToUpdateMaterial;

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
					Bundle bundle;

					Expression<Func<Material, bool>> nameExpression = p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));

					if (searchTerm == "*")
					{
						bundle = this.ImsiClient.Query<Material>(p => p.ClassConceptKey == EntityClassKeys.Material, 0, null, false);

						foreach (var material in ListExtensions.LatestVersionOnly(bundle.Item.OfType<Material>()).Where(p => p.ClassConceptKey == EntityClassKeys.Material))
						{
							material.TypeConcept = this.GetTypeConcept(material);
						}

						results = ListExtensions.LatestVersionOnly(bundle.Item.OfType<Material>()).Where(p => p.ClassConceptKey == EntityClassKeys.Material).Select(p => new MaterialViewModel(p)).OrderBy(p => p.Name).ToList();
					}
					else
					{
						Guid materialId;

						if (!Guid.TryParse(searchTerm, out materialId))
						{
							bundle = this.ImsiClient.Query<Material>(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.ClassConceptKey == EntityClassKeys.Material, 0, null, false);

							foreach (var material in ListExtensions.LatestVersionOnly(bundle.Item.OfType<Material>()).Where(p => p.ClassConceptKey == EntityClassKeys.Material))
							{
								material.TypeConcept = this.GetTypeConcept(material);
							}

							results = ListExtensions.LatestVersionOnly(bundle.Item.OfType<Material>().Where(nameExpression.Compile())).Where(p => p.ClassConceptKey == EntityClassKeys.Material).Select(p => new MaterialViewModel(p)).OrderBy(p => p.Name).ToList();
						}
						else
						{
							var material = this.GetEntity<Material>(materialId);

							if (material != null)
							{
								results.Add(new MaterialViewModel(material));
							}
						}
					}
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
				var materials = this.ImsiClient.Query<Material>(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.StatusConceptKey == StatusKeys.Active && p.ClassConceptKey == EntityClassKeys.Material, 0, null, false);

				viewModels = materials.Item.OfType<Material>().Where(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))) && p.StatusConceptKey == StatusKeys.Active && p.ClassConceptKey == EntityClassKeys.Material).Select(p => new MaterialViewModel(p)).OrderBy(p => p.Name).ToList();
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
				var material = this.GetEntity<Material>(id);

				if (material == null)
				{
					TempData["error"] = Locale.MaterialNotFound;

					return RedirectToAction("Index");
				}

				material.FormConcept = this.GetConcept(material.FormConceptKey);
				material.QuantityConcept = this.GetConcept(material.QuantityConceptKey);
				material.TypeConcept = this.GetConcept(material.TypeConceptKey);

				var relationships = new List<EntityRelationship>();

				relationships.AddRange(this.GetEntityRelationships<Material>(material.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.UsedEntity && r.ObsoleteVersionSequenceId == null).ToList());
				relationships.AddRange(this.GetEntityRelationships<ManufacturedMaterial>(material.Key.Value, r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.ManufacturedProduct && r.ObsoleteVersionSequenceId == null).ToList());

				material.Relationships = relationships.Intersect(material.Relationships, new EntityRelationshipComparer()).ToList();

				return View(new MaterialViewModel(material));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve material: {e}");
				this.TempData["error"] = Locale.UnexpectedErrorMessage;
			}

			return RedirectToAction("Index");
		}
	}
}