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
 * Date: 2017-4-13
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Models.ReferenceTermModels;

namespace OpenIZAdmin.Util
{
    /// <summary>
    /// Provides a utility for managing Reference Terms.
    /// </summary>
    public static class ReferenceTermUtil
    {
        /// <summary>
        /// Gets a list of Reference Terms.
        /// </summary>
        /// <param name="imsiServiceClient">The <see cref="ImsiServiceClient"/> instance.</param>
        /// <param name="concept">The <see cref="Concept"/> instance.</param>
        /// <param name="refTermId">The identifier of the reference term</param>
        /// <returns>Returns an IEnumerable of Concept Reference Terms.</returns>
        public static ReferenceTerm GetConceptReferenceTerm(ImsiServiceClient imsiServiceClient, Concept concept, Guid? refTermId)
        {            
            try
            {
                var list = GetConceptReferenceTerms(imsiServiceClient, concept).ToList();
                var index = list.FindIndex(c => c.Key == refTermId);
                if (index > -1)  return list[index];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);                
            }

            return null;
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
                    refTermList = new List<ReferenceTermViewModel>(referenceTerms.Select(r => new ReferenceTermViewModel(r, concept)));                    
                }
            }
            catch (Exception e)
            {
                refTermList = new List<ReferenceTermViewModel>();
                Console.WriteLine(e);
            }

            return refTermList;
        }
    }
}