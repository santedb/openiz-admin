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

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.MaterialModels;
using OpenIZAdmin.Models.MaterialModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
		/// Displays the create view.
		/// </summary>
		/// <returns>Returns the create view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			var model = new CreateMaterialModel();

			var formConcepts = ImsiClient.Query<Concept>(m => m.Class.Key == ConceptClassKeys.Form && m.ObsoletionTime == null);

			model.FormConcepts.AddRange(formConcepts.Item.OfType<Concept>().Select(c => new SelectListItem { Text = c.Mnemonic, Value = c.Key.Value.ToString() }));

			var quantityConcepts = ImsiClient.Query<Concept>(m => m.Class.Key == ConceptClassKeys.UnitOfMeasure && m.ObsoletionTime == null);

			model.QuantityConcepts.AddRange(quantityConcepts.Item.OfType<Concept>().Select(c => new SelectListItem { Text = c.Mnemonic, Value = c.Key.Value.ToString() }));

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
			if (ModelState.IsValid)
			{
				var material = new Material
				{
					Key = Guid.NewGuid(),
					Names = new List<EntityName>
					{
						new EntityName(NameUseKeys.OfficialRecord, model.Name)
					},
					FormConcept = new Concept
					{
						Key = Guid.Parse(model.FormConcept),
					},
					QuantityConcept = new Concept
					{
						Key = Guid.Parse(model.QuantityConcept),
					}
				};

				this.ImsiClient.Create(material);

				TempData["success"] = Locale.Material + " " + Locale.Created + " " + Locale.Successfully;

				return RedirectToAction("Index");
			}

			TempData["error"] = Locale.UnableToCreate + " " + Locale.Material;

			return View(model);
		}

		/// <summary>
		/// Deletion of a material.
		/// </summary>
		/// <param name="key">The key of the material.</param>
		/// <param name="versionKey">The version key of the material.</param>
		/// <returns>Returns the to the material search page.</returns>
		[HttpGet]
		public ActionResult Delete(Guid key, Guid versionKey)
		{
			if (key != Guid.Empty)
			{
				var material = this.ImsiClient.Get<Material>(key, versionKey) as Material;
				this.ImsiClient.Obsolete<Material>(material);
				return View("Index");
			}

			TempData["error"] = Locale.UnableToDelete + " " + Locale.Material;

			return View("Index");
		}

		/// <summary>
		/// Edit for material.
		/// </summary>
		/// <param name="key">The key of the material.</param>
		/// <param name="versionKey">The version key of the material.</param>
		/// <returns>Returns the edited material.</returns>
		[HttpGet]
		public ActionResult Edit(Guid key, Guid versionKey)
		{
			if (key != Guid.Empty)
			{
				var material = this.ImsiClient.Get<Material>(key, versionKey) as Material;

				if (material == null)
				{
					TempData["error"] = Locale.Material + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var model = new EditMaterialModel
				{
					Name = material.Names.FirstOrDefault().Component.FirstOrDefault().Value
				};

				var formConcepts = ImsiClient.Query<Concept>(m => m.Class.Key == ConceptClassKeys.Form && m.ObsoletionTime == null);

				model.FormConcepts.AddRange(formConcepts.Item.OfType<Concept>().Select(c => new SelectListItem { Text = c.Mnemonic, Selected = material.FormConcept.Key == c.Key, Value = c.Key.Value.ToString() }));

				var quantityConcepts = ImsiClient.Query<Concept>(m => m.Class.Key == ConceptClassKeys.UnitOfMeasure && m.ObsoletionTime == null);

				model.QuantityConcepts.AddRange(quantityConcepts.Item.OfType<Concept>().Select(c => new SelectListItem { Text = c.Mnemonic, Selected = material.QuantityConcept.Key == c.Key, Value = c.Key.Value.ToString() }));

				return View(model);
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
			if (ModelState.IsValid)
			{
				var material = this.ImsiClient.Get<Material>(model.Key, model.VersionKey) as Material;

				if (material == null)
				{
					TempData["error"] = Locale.Material + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				material.Names = new List<EntityName>
				{
					new EntityName(NameUseKeys.OfficialRecord, model.Name)
				};

				material.FormConcept = new Concept
				{
					Key = Guid.Parse(model.FormConcept),
				};

				material.QuantityConcept = new Concept
				{
					Key = Guid.Parse(model.QuantityConcept),
				};

				var result = this.ImsiClient.Update<Material>(material);

				TempData["success"] = Locale.Material + " " + Locale.Updated + " " + Locale.Successfully;

				return RedirectToAction("ViewMaterial", new { key = result.Key, versionKey = result.VersionKey });
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Material;

			return RedirectToAction("Index");
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
			IEnumerable<MaterialSearchResultViewModel> results = new List<MaterialSearchResultViewModel>();

			if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
			{
				var bundle = this.ImsiClient.Query<Material>(m => m.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && m.ClassConceptKey == EntityClassKeys.Material && m.ObsoletionTime == null);

				TempData["searchTerm"] = searchTerm;

				return PartialView("_MaterialSearchResultsPartial", bundle.Item.OfType<Material>().Select(MaterialUtil.ToMaterialSearchResultViewModel));
			}

			TempData["error"] = Locale.Material + " " + Locale.NotFound;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_MaterialSearchResultsPartial", results);
		}

		/// <summary>
		/// View for material.
		/// </summary>
		/// <param name="key">The key of the material.</param>
		/// <param name="versionKey">The version key of the material.</param>
		/// <returns>Returns the view for a material.</returns>
		[HttpGet]
		public ActionResult ViewMaterial(Guid key, Guid versionKey)
		{
			var material = this.ImsiClient.Get<Material>(key, versionKey) as Material;

			if (material == null)
			{
				TempData["error"] = Locale.Material + " " + Locale.NotFound;

				return RedirectToAction("Index");
			}

			var model = new MaterialViewModel
			{
				Key = key,
				Name = string.Join(" ", material.Names.SelectMany(n => n.Component).Select(c => c.Value)),
				FormConcept = material.FormConcept.Mnemonic,
				QuantityConcept = material.QuantityConcept.Mnemonic,
				VersionKey = versionKey
			};

			return View(model);
		}
	}
}