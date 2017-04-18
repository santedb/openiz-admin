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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.CodeSystem
{
    /// <summary>
	/// Represents an edit code system model.
	/// </summary>
    public class EditCodeSystemViewModel : CodeSystemModel
    {
        /// <summary>
		/// Converts an <see cref="CreateCodeSystemViewModel"/> instance to a <see cref="OpenIZAdmin.Models.CodeSystem"/> instance.
		/// </summary>
		/// <returns>Returns a concept instance.</returns>
		public EditCodeSystemViewModel(OpenIZ.Core.Model.DataTypes.CodeSystem codeSystem)
        {

            Id = codeSystem.Key;
            Name = codeSystem.Name;
            Oid = codeSystem.Oid;
                //Domain = this.Domain,
            Url = this.Url;
            Version = codeSystem.VersionText;
            Description = codeSystem.Description;

        }

        /// <summary>
		/// Converts an <see cref="EditCodeSystemViewModel"/> instance to a <see cref="CodeSystem"/> instance.
		/// </summary>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns the converted concept instance.</returns>
		public OpenIZ.Core.Model.DataTypes.CodeSystem ToCodeSystem(OpenIZ.Core.Model.DataTypes.CodeSystem codeSystem)
        {                        
            codeSystem.Name = this.Name;
            codeSystem.Oid = this.Oid;
            //Domain = this.Domain,
            codeSystem.Url = this.Url;
            codeSystem.VersionText = this.Version;
            codeSystem.Description = this.Description;

            return codeSystem;
        }
    }
}