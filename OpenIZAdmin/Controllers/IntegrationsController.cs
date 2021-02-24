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
 * Date: 2016-7-23
 */

using Newtonsoft.Json;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Services;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.ConceptNameModels;
using OpenIZAdmin.Models.IntegrationModels;
using OpenIZAdmin.Models.ReferenceTermModels;
using OpenIZAdmin.Services.Acts;
using OpenIZAdmin.Services.Entities;
using OpenIZAdmin.Services.EntityRelationships;
using OpenIZAdmin.Services.Metadata.Concepts;
using OpenIZAdmin.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        private const int COL_UUID = 1;
        private const int COL_NAME = 2;
        private const int COL_PARENT = 3;
        private const int COL_YEAR = 4;
        private const int COL_POPULATION = 5;

        /// <summary>
        /// Template file cache
        /// </summary>
        private static String m_templateFile = Path.Combine(Path.GetTempPath(), "facility_template.csv");


        /// <summary>
        /// The entity relationship service
        /// </summary>
        private readonly IEntityRelationshipService entityRelationshipService;

        /// <summary>
        /// The entity service.
        /// </summary>
        private readonly IEntityService entityService;

        /// <summary>
        /// Concept service
        /// </summary>
        private readonly IConceptService conceptService;

        /// <summary>
        /// Act service
        /// </summary>
        private readonly IActService actService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptController"/> class.
        /// </summary>
        public IntegrationsController(IEntityService entityService, IConceptService conceptService, IActService actService, IEntityRelationshipService entityRelationshipService)
        {
            this.entityService = entityService;
            this.conceptService = conceptService;
            this.actService = actService;
            this.entityRelationshipService = entityRelationshipService;

        }

        /// <summary>
        /// Displays the index view.
        /// </summary>
        /// <returns>Returns the index view.</returns>
        public ActionResult Index()
        {

            var model = new IntegrationControlModel();
            try
            {
                var cacts = this.actService.Query<Act>(o => o.ClassConceptKey == ActClassKeys.ControlAct, 0, 15, true);
                model.ImportActs = cacts.Select(o => new ControlActViewModel(o, this.conceptService)).ToList();
                var er = this.entityService.GetEntityRelationshipsByType(EntityRelationshipTypeKeys.Duplicate);

                model.Duplicates = er.Take(50).Select(o =>
                {
                    // is the source or target missing and populate
                    if (o.SourceEntity == null)
                    {
                        o.SourceEntity = this.entityService.Get<Place>(o.SourceEntityKey.Value);
                        o.SourceEntity.TypeConcept = this.conceptService.GetConcept(o.SourceEntity.TypeConceptKey);
                        o.SourceEntity.ClassConcept = this.conceptService.GetConcept(o.SourceEntity.ClassConceptKey);
                    }
                    if (o.TargetEntity == null)
                    {
                        o.TargetEntity = this.entityService.Get<Place>(o.TargetEntityKey.Value);
                        o.TargetEntity.TypeConcept = this.conceptService.GetConcept(o.TargetEntity.TypeConceptKey);
                        o.TargetEntity.ClassConcept = this.conceptService.GetConcept(o.TargetEntity.ClassConceptKey);
                    }
                    return new Models.EntityRelationshipModels.EntityRelationshipViewModel(o);
                }).ToList();
            }
            catch(Exception e)
            {
                Trace.TraceError($"Unable to fetch acts: {e}");
                TempData["error"] = e.Message;
            }
            return View(model);
        }


        /// <summary>
        /// Merge place
        /// </summary>
        /// TODO: Refactor to common place
        public ActionResult MergePlace(Guid duplicateRelationshipId)
        {
            try
            {
                var duplicateRel = this.entityRelationshipService.Get(duplicateRelationshipId);
                if (duplicateRel.RelationshipTypeKey != EntityRelationshipTypeKeys.Duplicate)
                    throw new InvalidOperationException("Cannot perform a merge on a non duplicate relationship");

                // Load the source and target
                Place source = this.entityService.Get<Place>(duplicateRel.SourceEntityKey.Value),
                    target = this.entityService.Get<Place>(duplicateRel.TargetEntityKey.Value);

                // Copy the fields from source to target
                target.Addresses = source.Addresses.Select(o => new EntityAddress()
                {
                    Component = o.Component.Select(c => new EntityAddressComponent(c.ComponentTypeKey.Value, c.Value)).ToList(),
                    AddressUseKey = o.AddressUseKey
                }).ToList();
                target.Identifiers = source.Identifiers.Select(o => new EntityIdentifier(o.Authority, o.Value)).ToList();
                target.Names = source.Names.Select(o => new EntityName()
                {
                    Component = o.Component.Select(c => new EntityNameComponent(c.Value)).ToList(),
                    NameUseKey = o.NameUseKey
                }).ToList();

                // Parent change?
                EntityRelationship sourceParent = source.Relationships.FirstOrDefault(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent),
                    targetParent = source.Relationships.FirstOrDefault(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent);
                if (sourceParent != null)
                    targetParent.TargetEntityKey = sourceParent?.TargetEntityKey ?? targetParent.TargetEntityKey;

                // Served areas?
                var sourceServedAreas = source.Relationships.Where(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);
                if (!sourceServedAreas.Any()) // Try reverse
                    sourceServedAreas = this.entityRelationshipService.GetEntityRelationshipsByTarget(source.Key.Value, EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);
                var targetServedAreas = target.Relationships.Where(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);
                if (!targetServedAreas.Any()) // Try reverse
                    targetServedAreas = this.entityRelationshipService.GetEntityRelationshipsByTarget(target.Key.Value, EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);

                target.TypeConceptKey = source.TypeConceptKey;
                target.ClassConceptKey = source.ClassConceptKey;

                target.Relationships.AddRange(sourceServedAreas.Where(s => targetServedAreas.Any(t => t.SourceEntityKey == s.SourceEntityKey || t.TargetEntityKey == s.TargetEntityKey))
                .Select(r => new EntityRelationship()
                {
                    SourceEntityKey = r.SourceEntityKey == source.Key ? target.Key : r.SourceEntityKey,
                    TargetEntityKey = r.TargetEntityKey == source.Key ? target.Key : r.TargetEntityKey,
                    RelationshipTypeKey = EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation
                }));

                target.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Replaces, source.Key));
                source.StatusConceptKey = StatusKeys.Nullified;

                this.entityService.Update(target);
                this.entityService.Obsolete(source);

                this.TempData["success"] = Locale.DuplicateMergeSuccess;

                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                Trace.TraceError($"Unable to retrieve place: { e }");
                this.TempData["error"] = Locale.UnexpectedErrorMessage;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Merge place
        /// </summary>
        /// TODO: Refactor to common place
        public ActionResult MergeIgnore(Guid duplicateRelationshipId)
        {
            try
            {
                var duplicateRel = this.entityRelationshipService.Get(duplicateRelationshipId);
                if (duplicateRel.RelationshipTypeKey != EntityRelationshipTypeKeys.Duplicate)
                    throw new InvalidOperationException("Cannot perform a merge on a non duplicate relationship");
                this.entityRelationshipService.Delete(duplicateRelationshipId);
               
                this.TempData["success"] = Locale.DuplicateMergeSuccess;

                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                Trace.TraceError($"Unable to retrieve place: { e }");
                this.TempData["error"] = Locale.UnexpectedErrorMessage;
            }

            return RedirectToAction("Index");
        }


        /// <summary>
        /// View the specified import event
        /// </summary>
        /// <param name="id">The identifier of the import event</param>
        /// <returns>The import detail view</returns>
        public ActionResult View(Guid id)
        {
            try
            {
                var act = this.actService.Get<Act>(id);

                // Load additional details
                var loadCache = new Dictionary<Guid, IdentifiedData>();
                foreach (var itm in act.Participations)
                {
                    // Fix role
                    if (itm.ParticipationRole == null)
                    {
                        if (!loadCache.TryGetValue(itm.ParticipationRoleKey.Value, out IdentifiedData concept))
                        {
                            concept = this.conceptService.GetConcept(itm.ParticipationRoleKey);
                            loadCache.Add(itm.ParticipationRoleKey.Value, concept);
                        }
                        itm.ParticipationRole = concept as Concept;
                    }

                    // Fix target
                    if(itm.PlayerEntity == null)
                        itm.PlayerEntity = this.entityService.Get<Entity>(itm.PlayerEntityKey.Value);

                    if(itm.PlayerEntity.TypeConcept == null && itm.PlayerEntity.TypeConceptKey.HasValue)
                    {
                        if (!loadCache.TryGetValue(itm.PlayerEntity.TypeConceptKey.Value, out IdentifiedData concept))
                        {
                            concept = this.conceptService.GetConcept(itm.PlayerEntity.TypeConceptKey);
                            loadCache.Add(itm.PlayerEntity.TypeConceptKey.Value, concept);
                        }
                        itm.PlayerEntity.TypeConcept = concept as Concept;
                    }
                }

                var model = new ControlActViewModel(act, this.conceptService);

                return View(model);

            }
            catch (Exception e)
            {
                Trace.TraceError($"Unable to fetch model: {e}");
                TempData["error"] = e.Message;
            }
            return this.Index();
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

                    using (var ms = System.IO.File.Create(m_templateFile))
                    {
                        using (var tw = new StreamWriter(ms))
                        {
                            tw.WriteLine("hfrid,id,name,parent,year,target");
                            var mre = new ManualResetEvent(false);
                            bool complete = false;

                            // The original author of these services made them non-thread safe, so we have to block here
                            object lockBox = new object();
                            // Read async from server
                            ConcurrentQueue<Place> processStack = new ConcurrentQueue<Place>();
                            ThreadPool.QueueUserWorkItem(p =>
                            {
                                try
                                {
                                    ConcurrentQueue<Place> toProcess = (ConcurrentQueue<Place>)p;
                                    Guid queryId = Guid.NewGuid();
                                    var results = this.entityService.Query<Place>(o => o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation && o.TypeConceptKey != facilityRegionKey && o.TypeConceptKey != facilityDistrictKey, 0, 200, queryId, new string[] { });
                                    while (results.Count() > 0)
                                    {
                                        foreach (var r in results) toProcess.Enqueue(r);
                                        mre.Set();
                                        ofs += results.Count();
                                        Trace.TraceInformation("Fetching records for building import target population list {0}", ofs);
                                        lock (lockBox)
                                            results = this.entityService.Query<Place>(o => o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation, ofs, 500, queryId, new string[] { });
                                    }
                                }
                                catch(Exception e)
                                {
                                    Trace.TraceError("Error fetching places: {0}", e);
                                }
                                finally
                                {
                                    complete = true;
                                }
                            }, processStack);

                            while(!complete)
                            {
                                mre.WaitOne();
                                while(processStack.TryDequeue(out Place itm))
                                {
                                    var parentRel = itm.Relationships.FirstOrDefault(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent);
                                    Place parent = null;
                                    if (parentRel != null && !parentCache.TryGetValue(parentRel.TargetEntityKey.Value, out parent))
                                    {
                                        lock (lockBox)
                                        {
                                            parent = entityService.Get<Place>(parentRel.TargetEntityKey.Value, null);
                                        }
                                        parentCache.Add(parentRel.TargetEntityKey.Value, parent);
                                    }
                                    tw.WriteLine($"{itm.Identifiers?.FirstOrDefault(o => o.Authority?.DomainName == "TZ_HFR_IDNUM")?.Value},{itm.Identifiers?.FirstOrDefault(o => o.Value.StartsWith("urn:uuid"))?.Value},{itm.Names?.FirstOrDefault()?.Component.FirstOrDefault()?.Value},{parent?.Names?.FirstOrDefault()?.Component.FirstOrDefault()?.Value},{DateTime.Now.Year},0");
                                }
                                mre.Reset();
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
                                        if (!Int32.TryParse(lineData[COL_POPULATION], out int population) || !Int32.TryParse(lineData[COL_YEAR], out int year))
                                            throw new FormatException($"Values {lineData[COL_POPULATION]} or {lineData[COL_YEAR]} were not in the correct format");
                                        var extensionValue = JsonConvert.SerializeObject(new { value = population, year = year });

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
                                Trace.TraceError($"Unable to upload Setting: {e}");
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