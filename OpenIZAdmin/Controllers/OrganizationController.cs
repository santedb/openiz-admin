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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elmah;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.OrganizationModels;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing organizations.
	/// </summary>
	[TokenAuthorize]
	public class OrganizationController : BaseController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OrganizationController"/> class.
		/// </summary>
		public OrganizationController()
		{
		}

		/// <summary>
		/// Displays the create view.
		/// </summary>
		/// <returns>Returns the create view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			var industryConceptSet = this.ImsiClient.Get<ConceptSet>(ConceptSetKeys.IndustryCode, null) as ConceptSet;

			var model = new CreateOrganizationModel
			{
				IndustryConcepts = industryConceptSet?.Concepts.ToSelectList().ToList()
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

					TempData["success"] = Locale.Organization + " " + Locale.Created + " " + Locale.Successfully;

					return RedirectToAction("ViewOrganization", new { id = organization.Key });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			var industryConceptSet = this.ImsiClient.Get<ConceptSet>(ConceptSetKeys.IndustryCode, null) as ConceptSet;

			model.IndustryConcepts = industryConceptSet.Concepts.ToSelectList(c => c.Key == Guid.Parse(model.IndustryConcept)).ToList();

			TempData["error"] = Locale.UnableToCreate + " " + Locale.Organization;

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var organization = this.ImsiClient.Get<Organization>(id, null) as Organization;

				if (organization == null)
				{
					TempData["error"] = Locale.Organization + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				this.ImsiClient.Obsolete<Organization>(organization);

				TempData["success"] = Locale.Organization + " " + Locale.Deleted + " " + Locale.Successfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToDelete + " " + Locale.Organization;

			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Edit(Guid id)
		{
			try
			{
				var bundle = this.ImsiClient.Query<Organization>(m => m.Key == id && m.ClassConceptKey == EntityClassKeys.Organization && m.ObsoletionTime == null, 0, null, true);

				bundle.Reconstitute();

				var organization = bundle.Item.OfType<Organization>().FirstOrDefault(m => m.Key == id && m.ClassConceptKey == EntityClassKeys.Organization && m.ObsoletionTime == null);

				if (organization == null)
				{
					TempData["error"] = Locale.Organization + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var industryConceptSet = this.ImsiClient.Get<ConceptSet>(ConceptSetKeys.IndustryCode, null) as ConceptSet;

				var model = new EditOrganizationModel(organization)
				{
					IndustryConcepts = industryConceptSet?.Concepts.ToSelectList().ToList()
				};

				return View(model);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.Organization + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditOrganizationModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var bundle = this.ImsiClient.Query<Organization>(m => m.Key == model.Id && m.ClassConceptKey == EntityClassKeys.Organization && m.ObsoletionTime == null, 0, null, true);

					bundle.Reconstitute();

					var organization = bundle.Item.OfType<Organization>().FirstOrDefault(m => m.Key == model.Id && m.ClassConceptKey == EntityClassKeys.Organization && m.ObsoletionTime == null);

					if (organization == null)
					{
						TempData["error"] = Locale.Organization + " " + Locale.NotFound;

						return RedirectToAction("Index");
					}

					var updatedOrganization = this.ImsiClient.Update<Organization>(model.ToOrganization(organization));

					TempData["success"] = Locale.Organization + " " + Locale.Updated + " " + Locale.Successfully;

					return RedirectToAction("ViewOrganization", new { id = updatedOrganization.Key });
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			var industryConceptSet = this.ImsiClient.Get<ConceptSet>(ConceptSetKeys.IndustryCode, null) as ConceptSet;

			model.IndustryConcepts = industryConceptSet?.Concepts.ToSelectList().ToList();

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Organization;

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
			return View();
		}

		/// <summary>
		/// Searches for a organization.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of organizations which match the search term.</returns>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			IEnumerable<OrganizationSearchResultViewModel> results = new List<OrganizationSearchResultViewModel>();

			if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
			{
				var bundle = this.ImsiClient.Query<Organization>(m => m.Names.Any(n => n.Component.Any(c => c.Value.Contains(searchTerm))) && m.ClassConceptKey == EntityClassKeys.Organization && m.ObsoletionTime == null);

				TempData["searchTerm"] = searchTerm;
				return PartialView("_OrganizationSearchResultsPartial", bundle.Item.OfType<Organization>().Select(o => new OrganizationSearchResultViewModel(o)));
			}

			TempData["error"] = Locale.Organization + " " + Locale.NotFound;
			TempData["searchTerm"] = searchTerm;

			return PartialView("_OrganizationSearchResultsPartial", results);
		}

		/// <summary>
		/// View for organization.
		/// </summary>
		/// <param name="id">The id of the organization.</param>
		/// <returns>Returns the view for a organization.</returns>
		[HttpGet]
		public ActionResult ViewOrganization(Guid id)
		{
			try
			{
				var bundle = this.ImsiClient.Query<Organization>(m => m.Key == id && m.ObsoletionTime == null, 0, null, true);

				bundle.Reconstitute();

				var organization = bundle.Item.OfType<Organization>().FirstOrDefault(m => m.Key == id && m.ObsoletionTime == null);

				if (organization == null)
				{
					TempData["error"] = Locale.Organization + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				for (var i = 0; i < organization.Relationships.Count(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.WarrantedProduct && r.TargetEntity == null && r.TargetEntityKey.HasValue); i++)
				{
					organization.Relationships[i].TargetEntity = this.ImsiClient.Get<ManufacturedMaterial>(organization.Relationships[i].TargetEntityKey.Value, null) as ManufacturedMaterial;
				}

				return View(new OrganizationViewModel(organization));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.Organization + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}
	}
}