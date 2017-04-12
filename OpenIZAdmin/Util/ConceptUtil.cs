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
 * User: Andrew
 * Date: 2017-4-7
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.ConceptSetModels;
using OpenIZAdmin.Models.ReferenceTermModels;

namespace OpenIZAdmin.Util
{
    /// <summary>
    /// Provides a utility for managing Concepts and Concept Sets.
    /// </summary>
    public static class ConceptUtil
    {
        /// <summary>
        /// Gets a Concept instance
        /// </summary>
        /// <param name="imsiServiceClient">The <see cref="ImsiServiceClient"/> instance.</param>
        /// <param name="mnemonic">The mnemonic to validate.</param>
        /// <returns>Returns an boolean if the mnemonic is unique</returns>
        public static bool CheckUniqueConceptMnemonic(ImsiServiceClient imsiServiceClient, string mnemonic)
        {
            var conceptBundle = imsiServiceClient.Query<Concept>(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);

            conceptBundle.Reconstitute();

            var concept = conceptBundle.Item.OfType<Concept>().FirstOrDefault(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);            
            
            return concept == null;
        }

        /// <summary>
        /// Gets a Concept instance
        /// </summary>
        /// <param name="imsiServiceClient">The <see cref="ImsiServiceClient"/> instance.</param>
        /// <param name="mnemonic">The mnemonic to validate.</param>
        /// <returns>Returns an boolean if the mnemonic is unique</returns>
        public static bool CheckUniqueConceptSetMnemonic(ImsiServiceClient imsiServiceClient, string mnemonic)
        {            
            var conceptSetBundle = imsiServiceClient.Query<ConceptSet>(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);

            conceptSetBundle.Reconstitute();

            var conceptSet = conceptSetBundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);

            return conceptSet == null;
        }

        /// <summary>
        /// Gets a Concept instance
        /// </summary>
        /// <param name="imsiServiceClient">The <see cref="ImsiServiceClient"/> instance.</param>
        /// <param name="id">The uniquie identifier of the concept instance to retrieve.</param>
        /// <param name="versionId">The version identifier (Guid) of the concept instance</param>
        /// <returns>Returns an instance of a Concept.</returns>
        public static Concept GetConcept(ImsiServiceClient imsiServiceClient, Guid? id, Guid? versionId)
        {
            Bundle bundle;
            if (versionId == null)
            {
                bundle = imsiServiceClient.Query<Concept>(c => c.Key == id && c.ObsoletionTime == null);
            }
            else
            {
                bundle = imsiServiceClient.Query<Concept>(c => c.Key == id && c.VersionKey == versionId);
            }
            
            bundle.Reconstitute();

            var concept = bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Key == id && c.ObsoletionTime == null);

            if (concept == null) return null;

            if (concept.ClassKey.HasValue && concept.ClassKey.Value != Guid.Empty)
            {
                concept.Class = imsiServiceClient.Get<ConceptClass>(concept.ClassKey.Value, null) as ConceptClass;
            }

            return concept;
        }

        /// <summary>
        /// Gets a Concept instance
        /// </summary>
        /// <param name="imsiServiceClient">The <see cref="ImsiServiceClient"/> instance.</param>
        /// <param name="id">The uniquie identifier of the concept instance to retrieve.</param>
        /// <returns>Returns an instance of a Concept.</returns>
        public static ConceptSet GetConceptSet(ImsiServiceClient imsiServiceClient, Guid? id)
        {
            var bundle = imsiServiceClient.Query<ConceptSet>(c => c.Key == id && c.ObsoletionTime == null);

            bundle.Reconstitute();

            return bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Key == id && c.ObsoletionTime == null);            
        }

        /// <summary>
        /// Gets a list of Reference Terms.
        /// </summary>
        /// <param name="imsiServiceClient">The <see cref="ImsiServiceClient"/> instance.</param>
        /// <param name="concept">The <see cref="Concept"/> instance.</param>
        /// <returns>Returns an IEnumerable of Concept Reference Terms.</returns>
        public static ReferenceTerm GetConceptReferenceTerm(ImsiServiceClient imsiServiceClient, Concept concept, Guid? refTermId)
        {
            ReferenceTerm term = null;
            var referenceTermQuery = new List<KeyValuePair<string, object>>();

            foreach (var conceptReferenceTerm in concept.ReferenceTerms)
            {
                referenceTermQuery.AddRange(QueryExpressionBuilder.BuildQuery<ReferenceTerm>(c => c.Key == conceptReferenceTerm.ReferenceTerm.Key));
            }

            var list = imsiServiceClient.Query<ReferenceTerm>(QueryExpressionParser.BuildLinqExpression<ReferenceTerm>(new NameValueCollection(referenceTermQuery.ToArray()))).Item.OfType<ReferenceTerm>().ToList();
            var index = list.FindIndex(c => c.Key == refTermId);
            if(index >-1 ) term = list[index];

            return term;
        }


        /// <summary>
        /// Gets a list of Reference Terms.
        /// </summary>
        /// <param name="imsiServiceClient">The <see cref="ImsiServiceClient"/> instance.</param>
        /// <param name="concept">The <see cref="Concept"/> instance.</param>
        /// <returns>Returns an IEnumerable of Concept Reference Terms.</returns>
        public static IEnumerable<ReferenceTerm> GetConceptReferenceTerms(ImsiServiceClient imsiServiceClient, Concept concept)
        {
            var referenceTermQuery = new List<KeyValuePair<string, object>>();

            foreach (var conceptReferenceTerm in concept.ReferenceTerms)
            {
                referenceTermQuery.AddRange(QueryExpressionBuilder.BuildQuery<ReferenceTerm>(c => c.Key == conceptReferenceTerm.ReferenceTerm.Key));
            }

            return imsiServiceClient.Query<ReferenceTerm>(QueryExpressionParser.BuildLinqExpression<ReferenceTerm>(new NameValueCollection(referenceTermQuery.ToArray()))).Item.OfType<ReferenceTerm>();            
        }

        /// <summary>
        /// Gets the Concept Class that matches the Concept Class Name
        /// </summary>
        /// <param name="imsiServiceClient">The <see cref="ImsiServiceClient"/> instance.</param>
        /// <param name="concept">The Concept instance</param>
        /// <returns></returns>
        public static List<ReferenceTermViewModel> GetConceptReferenceTermsList(ImsiServiceClient imsiServiceClient, Concept concept)
        {
            var refTermList = new List<ReferenceTermViewModel>();
            try
            {
                var referenceTerms = GetConceptReferenceTerms(imsiServiceClient, concept).ToList();

                if (referenceTerms.Any())
                {
                    refTermList = new List<ReferenceTermViewModel>(referenceTerms.Select(r => new ReferenceTermViewModel(r)));
                    //{
                    //    Mnemonic = r.Mnemonic,
                    //    Name = string.Join(" ", r.DisplayNames.Select(d => d.Name)),
                    //    Id = r.Key ?? Guid.Empty
                    //}));
                }
            }
            catch (Exception e)
            {
                refTermList = new List<ReferenceTermViewModel>();
                Console.WriteLine(e);
            }

            return refTermList;
        }


        /// <summary>
        /// Gets a list of Concept Classes.
        /// </summary>
        /// <param name="imsiServiceClient">The <see cref="ImsiServiceClient"/> instance.</param>        
        /// <returns>Returns an IEnumerable of Concept Class types.</returns>
        public static IEnumerable<ConceptClass> GetConceptClasses(ImsiServiceClient imsiServiceClient)
        {
            var classesBundle = imsiServiceClient.Query<ConceptClass>(c => c.ObsoletionTime == null);
            classesBundle.Reconstitute();
            return classesBundle.Item.OfType<ConceptClass>();
        }


        /// <summary>
        /// Gets the Concept Class that matches the Concept Class Name
        /// </summary>
        /// <param name="imsiServiceClient">The <see cref="ImsiServiceClient"/> instance.</param>
        /// <param name="conceptClassName">The name of the Concept Class</param>
        /// <returns></returns>
        public static ConceptClass GetConceptClass(ImsiServiceClient imsiServiceClient, string conceptClassName)
        {
            ConceptClass conceptClass;
            try
            {
                var conceptClassBundle = imsiServiceClient.Query<ConceptClass>(c => c.ObsoletionTime == null);

                conceptClassBundle.Reconstitute();

                var conceptClasses = conceptClassBundle.Item.OfType<ConceptClass>().Select(c => new ConceptClass { Mnemonic = c.Mnemonic, Name = c.Name, Key = c.Key }).ToList();

                conceptClass = conceptClasses.FirstOrDefault(c => c.Name == conceptClassName);
            }
            catch (Exception e)
            {
                conceptClass = null;
                Console.WriteLine(e);                
            }

            return conceptClass;

        }


        /// <summary>
        /// Converts a <see cref="EditConceptModel"/> to a <see cref="Concept"/>
        /// </summary>        
        /// <param name="model"> The EditConceptModel model with the changes</param>
        /// <param name="concept">The target Concept to apply the update to</param>
        /// <returns>The updated Concept instance</returns>
        public static Concept ToEditConceptInfo(EditConceptModel model, Concept concept)
        {
            if (!string.Equals(model.ConceptClass, concept.ClassKey.ToString()))
            {                
                concept.Class = new ConceptClass
                {
                    Key = Guid.Parse(model.ConceptClass)
                };
            }

           
            concept.Mnemonic = model.Mnemonic;

            return concept;
        }

        /// <summary>
        /// Converts a <see cref="EditConceptSetModel"/> to a <see cref="Concept"/>
        /// </summary>        
        /// <param name="model"> The EditConceptSetModel model with the changes</param>
        /// <param name="conceptSet">The target Concept Set to apply the update to</param>
        /// <returns>The updated Concept instance</returns>
        public static ConceptSet ToEditConceptSetInfo(EditConceptSetModel model, ConceptSet conceptSet)
        {            
            conceptSet.Mnemonic = model.Mnemonic;
            conceptSet.Name = model.Name;
            conceptSet.Oid = model.Oid;
            conceptSet.Url = model.Url;

            if (!model.AddConcepts.Any()) return conceptSet;

            foreach (var concept in model.AddConcepts)
            {
                Guid id;
                if (Guid.TryParse(concept, out id))
                {
                    conceptSet.ConceptsXml.Add(id);
                }
            }

            return conceptSet;
        }

       

        


        


    }
}