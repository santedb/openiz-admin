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
 * User: Andrew
 * Date: 2017-4-7
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Models.ConceptModels;

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
        /// <param name="id">The uniquie identifier of the concept instance to retrieve.</param>
        /// <returns>Returns an instance of a Concept.</returns>
        public static Concept GetConcept(ImsiServiceClient imsiServiceClient, Guid? id)
        {
            var bundle = imsiServiceClient.Query<Concept>(c => c.Key == id && c.ObsoletionTime == null);

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
        /// Gets a Concept instance
        /// </summary>
        /// <param name="imsiServiceClient">The <see cref="ImsiServiceClient"/> instance.</param>
        /// <param name="mnemonic">The mnemonic to validate.</param>
        /// <returns>Returns an boolean if the mnemonic is unique</returns>
        public static bool CheckUniqueMnemonic(ImsiServiceClient imsiServiceClient, string mnemonic)
        {
            var bundle = imsiServiceClient.Query<Concept>(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);

            bundle.Reconstitute();

            var concept = bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Mnemonic == mnemonic && c.ObsoletionTime == null);

            return concept == null;
        }


    }
}