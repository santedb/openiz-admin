﻿/*
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
 * Date: 2016-7-23
 */

using Newtonsoft.Json;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.ConceptNameModels;
using OpenIZAdmin.Models.IntegrationModels;
using OpenIZAdmin.Models.ReferenceTermModels;
using OpenIZAdmin.Services.Entities;
using OpenIZAdmin.Services.Metadata.Concepts;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
    /// <summary>
    /// Provides operations for managing concepts.
    /// </summary>
    [TokenAuthorize(Constants.UnrestrictedAdministration)]
    public class IntegrationsController : MetadataController
    {

        private readonly Guid TargetPopulationKey = Guid.Parse("f9552ed8-66aa-4644-b6a8-108ad54f2476");

        private const int COL_UUID = 0;
        private const int COL_NAME = 1;
        private const int COL_PARENT = 2;
        private const int COL_YEAR = 3;
        private const int COL_POPULATION = 4;

        /// <summary>
        /// Template file cache
        /// </summary>
        private static String m_templateFile = Path.Combine(Path.GetTempPath(), "facility_template.csv");


        /// <summary>
        /// The entity service.
        /// </summary>
        private readonly IEntityService entityService;

        /// <summary>
        /// Concept service
        /// </summary>
        private readonly IConceptService conceptService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptController"/> class.
        /// </summary>
        public IntegrationsController(IEntityService entityService, IConceptService conceptService)
        {
            this.entityService = entityService;
            this.conceptService = conceptService;

        }

        /// <summary>
        /// Displays the index view.
        /// </summary>
        /// <returns>Returns the index view.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get the template for the CSV file
        /// </summary>
        /// <returns></returns>
        public FileStreamResult GetTemplate()
        {
            try
            {

                Dictionary<Guid, Place> parentCache = new Dictionary<Guid, Place>();
                var targetPopExtensionKey = Guid.Parse("f9552ed8-66aa-4644-b6a8-108ad54f2476");
                Guid facilityRegionKey = conceptService.GetConcept("Facility-Region").Key.Value;
                Guid facilityDistrictKey = conceptService.GetConcept("Facility-District").Key.Value;
                var ofs = 0;

                if (!System.IO.File.Exists(m_templateFile) && new FileInfo(m_templateFile).LastWriteTime < DateTime.Now.AddDays(-2))
                {
                    Guid queryId = Guid.NewGuid();
                    var results = this.entityService.Query<Place>(o => o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation && o.TypeConceptKey != facilityRegionKey && o.TypeConceptKey != facilityDistrictKey, 0, 200, queryId, new string[] { });

                    using (var ms = System.IO.File.Create(m_templateFile))
                    {
                        using (var tw = new StreamWriter(ms))
                        {
                            tw.WriteLine("id,name,parent,year,target");
                            while (results.Count() > 0)
                            {
                                foreach (var itm in results)
                                {
                                    var parentRel = itm.Relationships.FirstOrDefault(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent);
                                    Place parent = null;
                                    if (parentRel != null && !parentCache.TryGetValue(parentRel.TargetEntityKey.Value, out parent))
                                    {
                                        parent = entityService.Get<Place>(parentRel.TargetEntityKey.Value, null);
                                        parentCache.Add(parentRel.TargetEntityKey.Value, parent);
                                    }

                                    tw.WriteLine($"{itm.Identifiers?.FirstOrDefault(o => o.Value.StartsWith("urn:uuid"))?.Value},{itm.Names?.FirstOrDefault()?.Component.FirstOrDefault()?.Value},{parent?.Names?.FirstOrDefault()?.Component.FirstOrDefault()?.Value},{DateTime.Now.Year},0");
                                }
                                ofs += results.Count();
                                results = this.entityService.Query<Place>(o => o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation, ofs, 500, queryId, new string[] { });
                            }
                        }
                    }
                }

                this.Response.AddHeader("Content-Disposition", "attachment;filename=population_template.csv");
                return new FileStreamResult(System.IO.File.OpenRead(m_templateFile), "text/csv");
            }
            catch (Exception e)
            {
                Trace.TraceError($"Unable to create template for target population place: {e}");
                throw;
            }

        }

        /// <summary>
		/// Uploads an applet.
		/// </summary>
		/// <param name="model">The model containing the applet.</param>
		/// <returns>Returns the upload view.</returns>
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportPopulation(IntegrationControlModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var fileInfo = new FileInfo(model.TargetPopulationFile.FileName);

                    switch (fileInfo.Extension)
                    {
                        case ".csv":

                            try
                            {
                                
                                using(var tr = new StreamReader(model.TargetPopulationFile.InputStream))
                                {

                                    while(!tr.EndOfStream)
                                    {
                                        var lineData = tr.ReadLine().Split(',');
                                        if(lineData[COL_UUID] == "id") continue; // skip line for headers

                                        var facilityRegId = lineData[COL_UUID];
                                        // Get the target place
                                        var placeResult = this.entityService.Query<Place>(o => o.Identifiers.Any(id => id.Value == facilityRegId), 0, 2, new string[] { "extension" });
                                        if (placeResult.Count() != 1)
                                            throw new InvalidOperationException($"Place with identifier {lineData[COL_UUID]} is ambiguous or not found");

                                        // Does this place have a target population?
                                        var place = placeResult.First();
                                        var extension = place.Extensions.FirstOrDefault(o => o.ExtensionTypeKey == TargetPopulationKey);

                                        // Generate the token
                                        var extensionValue = JsonConvert.SerializeObject(new { value = Int32.Parse(lineData[COL_POPULATION]), year = Int32.Parse(lineData[COL_YEAR]) });

                                        if (extension == null)
                                            place.Extensions.Add(new EntityExtension(TargetPopulationKey, System.Text.Encoding.UTF8.GetBytes(extensionValue)));
                                        else extension.ExtensionValueXml = System.Text.Encoding.UTF8.GetBytes(extensionValue);

                                        // Update the place
                                        entityService.Update(place);
                                    }
                                }

                                TempData["success"] = Locale.ImportCompletedSuccessfully;

                                return RedirectToAction("Index");
                            }
                            catch (Exception e)
                            {
                                Trace.TraceError($"Unable to upload applet: {e}");
                                ModelState.AddModelError(nameof(model.TargetPopulationFile), Locale.UnableToImportPopulation);
                                ModelState.AddModelError(nameof(model.TargetPopulationFile), e.Message);
                            }

                            break;

                        default:
                            ModelState.AddModelError(nameof(model.TargetPopulationFile), Locale.UnableToImportPopulation);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError($"Unable to upload applet: {e}");
            }

            TempData["error"] = Locale.UnableToImportPopulation;

            return View("Index", model);
        }

    }
}