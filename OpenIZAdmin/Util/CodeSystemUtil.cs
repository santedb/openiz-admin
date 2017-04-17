using System;
using System.Collections.Generic;
using System.Linq;
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
 * Date: 2017-4-17
 */

using System.Web;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Models.CodeSystem;

namespace OpenIZAdmin.Util
{
    /// <summary>
    /// Provides a utility for managing Code Systems.
    /// </summary>
    public static class CodeSystemUtil
    {
        /// <summary>
        /// Gets a list of Reference Terms.
        /// </summary>
        /// <param name="imsiServiceClient">The <see cref="ImsiServiceClient"/> instance.</param>
        /// <param name="id">The code system identifier.</param>
        /// <returns>Returns an IEnumerable of Concept Reference Terms.</returns>
        public static CodeSystem GetCodeSystem(ImsiServiceClient imsiServiceClient , Guid? id)
        {

            var bundle = imsiServiceClient.Query<CodeSystem>(c => c.Key == id && c.ObsoletionTime == null);
            bundle.Reconstitute();            
            return bundle.Item.OfType<CodeSystem>().FirstOrDefault(c => c.Key == id && c.ObsoletionTime == null);                        
        }
    }
}