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
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.MaterialModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Models.ManufacturedMaterialModels;
using OpenIZAdmin.Models.PlaceModels;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing materials.
	/// </summary>
	[TokenAuthorize]
	public class MaterialController : BaseController
	{
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
				var bundle = this.ImsiClient.Query<Material>(p => p.Key == id && p.VersionKey == versionId);

				var material = bundle.Item.OfType<Material>().FirstOrDefault(p => p.Key == id && p.VersionKey == versionId);

				if (material == null)
				{
					this.TempData["error"] = Locale.UnableToRetrieve + " " + Locale.Material;
					return RedirectToAction("Edit", new { id = id, versionId = versionId });
				}

				material.CreationTime = DateTimeOffset.Now;
				material.ObsoletedByKey = null;
				material.ObsoletionTime = null;
				material.VersionKey = null;

				var updatedMaterial = this.ImsiClient.Update(material);

				this.TempData["success"] = Locale.Material + " " + Locale.Activated + " " + Locale.Successfully;

				return RedirectToAction("Edit", new { id = id, versionId = updatedMaterial.VersionKey });
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to activate material: { e }");
			}

			this.TempData["error"] = Locale.UnableToActivate + " " + Locale.Material;

			return RedirectToAction("Edit", new { id = id, versionId = versionId });
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

					TempData["success"] = Locale.Material + " " + Locale.Created + " " + Locale.Successfully;

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

			TempData["error"] = Locale.UnableToCreate + " " + Locale.Material;

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
				var material = this.ImsiClient.Get<Material>(id, null) as Material;

				if (material == null)
				{
					TempData["error"] = Locale.Material + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				this.ImsiClient.Obsolete<Material>(material);

				TempData["success"] = Locale.Material + " " + Locale.Deleted + " " + Locale.Successfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToDelete + " " + Locale.Material;

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
				var bundle = this.ImsiClient.Query<Material>(m => m.Key == id && m.VersionKey == versionId && m.ClassConceptKey == EntityClassKeys.Material, 0, null, true);

				bundle.Reconstitute();

				var material = bundle.Item.OfType<Material>().FirstOrDefault(m => m.Key == id && m.VersionKey == versionId && m.ClassConceptKey == EntityClassKeys.Material);

				if (material == null)
				{
					TempData["error"] = Locale.Material + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var formConcepts = this.GetFormConcepts();
				var quantityConcepts = this.GetQuantityConcepts();
				var typeConcepts = this.GetMaterialTypeConcepts();

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
			}

			TempData["error"] = Locale.Material + " " + Locale.NotFound;

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
					var bundle = this.ImsiClient.Query<Material>(m => m.Key == model.Id && m.ClassConceptKey == EntityClassKeys.Material && m.ObsoletionTime == null, 0, null, true);

					bundle.Reconstitute();

					var material = bundle.Item.OfType<Material>().FirstOrDefault(m => m.Key == model.Id && m.ClassConceptKey == EntityClassKeys.Material && m.ObsoletionTime == null);

					if (material == null)
					{
						TempData["error"] = Locale.Material + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					var updatedMaterial = this.ImsiClient.Update<Material>(model.ToMaterial(material));

					TempData["success"] = Locale.Material + " " + Locale.Updated + " " + Locale.Successfully;

					return RedirectToAction("ViewMaterial", new { id = updatedMaterial.Key, versionId = updatedMaterial.VersionKey });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Material;

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
			return View();
		}

		/// <summary>
		/// Searches for a material.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of materials which match the search term.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<MaterialViewModel> results = new List<MaterialViewModel>();

			if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
			{
				var bundle = this.ImsiClient.Query<Material>(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.ClassConceptKey == EntityClassKeys.Material);

				var maxVersionSequence = bundle.Item.OfType<Material>().Where(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.ClassConceptKey == EntityClassKeys.Material).Max(p => p.VersionSequence);

				results = bundle.Item.OfType<Material>().Where(p => p.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && p.VersionSequence == maxVersionSequence).Select(p => new MaterialViewModel(p)).OrderBy(p => p.Name).ToList();
			}

			TempData["searchTerm"] = searchTerm;

			return PartialView("_MaterialSearchResultsPartial", results);
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
				var bundle = this.ImsiClient.Query<Material>(m => m.Key == id && m.VersionKey == versionId && m.ClassConceptKey == EntityClassKeys.Material, 0, null, true);

				bundle.Reconstitute();

				var material = bundle.Item.OfType<Material>().FirstOrDefault(m => m.Key == id && m.VersionKey == versionId && m.ClassConceptKey == EntityClassKeys.Material);

				if (material == null)
				{
					TempData["error"] = Locale.Material + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				material.FormConcept = this.GetConcept(material.FormConceptKey);
				material.QuantityConcept = this.GetConcept(material.QuantityConceptKey);
				material.TypeConcept = this.GetConcept(material.TypeConceptKey);

				for (var i = 0; i < material.Relationships.Count(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.ManufacturedProduct && r.TargetEntity == null && r.TargetEntityKey.HasValue); i++)
				{
					material.Relationships[i].TargetEntity = this.ImsiClient.Get<ManufacturedMaterial>(material.Relationships[i].TargetEntityKey.Value, null) as ManufacturedMaterial;
				}

				return View(new MaterialViewModel(material));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
				Trace.TraceError($"Unable to retrieve material");
			}

			TempData["error"] = Locale.Material + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}
	}
}