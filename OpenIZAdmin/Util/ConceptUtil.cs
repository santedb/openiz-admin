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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Messaging.IMSI.Client;

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
        /// <param name="imsiClient">The <see cref="ImsiServiceClient"/> instance.</param>
        /// <param name="id">The uniquie identifier of the concept instance to retrieve.</param>
        /// <returns>Returns an instance of a Concept.</returns>
        public static Concept GetConcept(ImsiServiceClient imsiClient, Guid? id)
        {
            var bundle = imsiClient.Query<Concept>(c => c.Key == id && c.ObsoletionTime == null);

            bundle.Reconstitute();

            var concept = bundle.Item.OfType<Concept>().FirstOrDefault(c => c.Key == id && c.ObsoletionTime == null);

            if (concept == null) return null;

            if (concept.ClassKey.HasValue && concept.ClassKey.Value != Guid.Empty)
            {
                concept.Class = imsiClient.Get<ConceptClass>(concept.ClassKey.Value, null) as ConceptClass;
            }

            return concept;
        }


        /// <summary>
        /// Gets a list of Reference Terms.
        /// </summary>
        /// <param name="imsiClient">The <see cref="ImsiServiceClient"/> instance.</param>        
        /// <returns>Returns an IEnumerable of Concept Reference Terms.</returns>
        public static IEnumerable<ReferenceTerm> GetConceptReferenceTerms(ImsiServiceClient imsiClient, Concept concept)
        {
            var referenceTermQuery = new List<KeyValuePair<string, object>>();

            foreach (var conceptReferenceTerm in concept.ReferenceTerms)
            {
                referenceTermQuery.AddRange(QueryExpressionBuilder.BuildQuery<ReferenceTerm>(c => c.Key == conceptReferenceTerm.ReferenceTerm.Key));
            }

            return imsiClient.Query<ReferenceTerm>(QueryExpressionParser.BuildLinqExpression<ReferenceTerm>(new NameValueCollection(referenceTermQuery.ToArray()))).Item.OfType<ReferenceTerm>();            
        }


        /// <summary>
        /// Gets a list of Concept Class types.
        /// </summary>
        /// <param name="imsiClient">The <see cref="ImsiServiceClient"/> instance.</param>        
        /// <returns>Returns an IEnumerable of Concept Class types.</returns>
        public static IEnumerable<ConceptClass> GetConceptClasses(ImsiServiceClient imsiClient)
        {
            var classesBundle = imsiClient.Query<ConceptClass>(c => c.ObsoletionTime == null);
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


}
}