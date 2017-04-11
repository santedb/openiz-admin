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
 * Date: 2017-4-11
 */

using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.IMSI.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZAdmin.Util
{
    /// <summary>
    /// Provides a utility for managing Materials.
    /// </summary>
    public static class MaterialUtil
    {
        /// <summary>
        /// Gets a Concept instance
        /// </summary>
        /// <param name="imsiServiceClient">The <see cref="ImsiServiceClient"/> instance.</param>
        /// <param name="id">The uniquie identifier of the material instance to retrieve.</param>
        /// <param name="versionId">The version identifier (Guid) of the material instance</param>
        /// <returns>Returns an instance of a Concept.</returns>
        public static Material GetMaterial(ImsiServiceClient imsiServiceClient, Guid? id, Guid? versionId)
        {
            var bundle = imsiServiceClient.Query<Material>(m => m.Key == id && m.VersionKey == versionId && m.ClassConceptKey == EntityClassKeys.Material, 0, null, true);

            bundle.Reconstitute();

            var material = bundle.Item.OfType<Material>().FirstOrDefault(m => m.Key == id && m.VersionKey == versionId && m.ClassConceptKey == EntityClassKeys.Material);

            return material;
        }
    }
}