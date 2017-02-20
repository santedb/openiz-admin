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
using System.Linq;
using System.Web.Mvc;
using OpenIZAdmin.Extensions;

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
			var formConcepts = this.ImsiClient.Query<Concept>(m => m.ClassKey == ConceptClassKeys.Form && m.ObsoletionTime == null).Item.OfType<Concept>();
			var quantityConcepts = this.ImsiClient.Query<Concept>(m => m.ClassKey == ConceptClassKeys.UnitOfMeasure && m.ObsoletionTime == null).Item.OfType<Concept>();

			var model = new CreateMaterialModel
			{
				FormConcepts = formConcepts.ToSelectList().ToList(),
				QuantityConcepts = quantityConcepts.ToSelectList().ToList()
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

					return RedirectToAction("ViewMaterial", new { id = material.Key });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			var formConcepts = this.ImsiClient.Query<Concept>(m => m.ClassKey == ConceptClassKeys.Form && m.ObsoletionTime == null).Item.OfType<Concept>();
			var quantityConcepts = this.ImsiClient.Query<Concept>(m => m.ClassKey == ConceptClassKeys.UnitOfMeasure && m.ObsoletionTime == null).Item.OfType<Concept>();

			model.FormConcepts = formConcepts.ToSelectList().ToList();
			model.QuantityConcepts = quantityConcepts.ToSelectList().ToList();

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
		/// <returns>Returns the edited material.</returns>
		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			try
			{
				var bundle = this.ImsiClient.Query<Material>(m => m.Key == id && m.ClassConceptKey == EntityClassKeys.Material && m.ObsoletionTime == null, 0, null, true);

				bundle.Reconstitute();

				var material = bundle.Item.OfType<Material>().FirstOrDefault(m => m.Key == id && m.ClassConceptKey == EntityClassKeys.Material && m.ObsoletionTime == null);

				if (material == null)
				{
					TempData["error"] = Locale.Material + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var formConcepts = this.ImsiClient.Query<Concept>(m => m.ClassKey == ConceptClassKeys.Form && m.ObsoletionTime == null).Item.OfType<Concept>();
				var quantityConcepts = this.ImsiClient.Query<Concept>(m => m.ClassKey == ConceptClassKeys.UnitOfMeasure && m.ObsoletionTime == null).Item.OfType<Concept>();

				var model = new EditMaterialModel(material)
				{
					FormConcepts = formConcepts.ToSelectList(c => c.Key == material.FormConceptKey).ToList(),
					QuantityConcepts = quantityConcepts.ToSelectList(c => c.Key == material.QuantityConceptKey).ToList()
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
					var bundle = this.ImsiClient.Query<Material>(m => m.Key == model.Id && m.ObsoletionTime == null, 0, null, true);

					bundle.Reconstitute();

					var material = bundle.Item.OfType<Material>().FirstOrDefault(m => m.Key == model.Id && m.ObsoletionTime == null);

					if (material == null)
					{
						TempData["error"] = Locale.Material + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					var updatedMaterial = this.ImsiClient.Update<Material>(model.ToMaterial(material));

					TempData["success"] = Locale.Material + " " + Locale.Updated + " " + Locale.Successfully;

					return RedirectToAction("ViewMaterial", new { id = updatedMaterial.Key });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			//var formConcepts = this.ImsiClient.Query<Concept>(m => m.ClassKey == ConceptClassKeys.Form && m.ObsoletionTime == null).Item.OfType<Concept>();
			//var quantityConcepts = this.ImsiClient.Query<Concept>(m => m.ClassKey == ConceptClassKeys.UnitOfMeasure && m.ObsoletionTime == null).Item.OfType<Concept>();

			//model.FormConcepts = formConcepts.ToSelectList(c => c.Key == material.FormConceptKey).ToList();
			//model.QuantityConcepts = quantityConcepts.ToSelectList(c => c.Key == material.QuantityConceptKey).ToList();

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
			IEnumerable<MaterialSearchResultViewModel> results = new List<MaterialSearchResultViewModel>();

			if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
			{
				var bundle = this.ImsiClient.Query<Material>(m => m.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && m.ClassConceptKey == EntityClassKeys.Material && m.ObsoletionTime == null);

				TempData["searchTerm"] = searchTerm;
				return PartialView("_MaterialSearchResultsPartial", bundle.Item.OfType<Material>().Where(m => m.ClassConceptKey == EntityClassKeys.Material && m.ObsoletionTime == null).Select(m => new MaterialSearchResultViewModel(m)));
			}

			TempData["error"] = Locale.Material + " " + Locale.NotFound;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_MaterialSearchResultsPartial", results);
		}

		/// <summary>
		/// View for material.
		/// </summary>
		/// <param name="id">The id of the material.</param>
		/// <returns>Returns the view for a material.</returns>
		[HttpGet]
		public ActionResult ViewMaterial(Guid id)
		{
			try
			{
				var bundle = this.ImsiClient.Query<Material>(m => m.Key == id && m.ClassConceptKey == EntityClassKeys.Material && m.ObsoletionTime == null, 0, null, true);

				bundle.Reconstitute();

				var material = bundle.Item.OfType<Material>().FirstOrDefault(m => m.Key == id && m.ClassConceptKey == EntityClassKeys.Material && m.ObsoletionTime == null);

				if (material == null)
				{
					TempData["error"] = Locale.Material + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				if (material.FormConcept == null && material.FormConceptKey.HasValue && material.FormConceptKey != Guid.Empty)
				{
					material.FormConcept = this.ImsiClient.Get<Concept>(material.FormConceptKey.Value, null) as Concept;
				}

				if (material.QuantityConcept == null && material.QuantityConceptKey.HasValue && material.QuantityConceptKey != Guid.Empty)
				{
					material.QuantityConcept = this.ImsiClient.Get<Concept>(material.QuantityConceptKey.Value, null) as Concept;
				}

				return View(new MaterialViewModel(material));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.Material + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}
	}
}