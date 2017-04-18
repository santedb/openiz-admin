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
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.CodeSystem
{
    /// <summary>
	/// Represents a model to view a code system.
	/// </summary>
    public class CodeSystemViewModel : CodeSystemModel
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="CodeSystemViewModel"/> class.
		/// </summary>
        public CodeSystemViewModel()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeSystemViewModel"/> class
        /// with a specific <see cref="CodeSystem"/> instance.
        /// </summary>
        /// <param name="codeSystem">The <see cref="OpenIZ.Core.Model.DataTypes.CodeSystem"/> instance.</param>
        public CodeSystemViewModel(OpenIZ.Core.Model.DataTypes.CodeSystem codeSystem) : this()
        {
            Name = codeSystem.Name;
            Oid = codeSystem.Oid;
            Description = codeSystem.Description;
            //Domain = codeSystem.Authority; ??????????????????????????
            Url = codeSystem.Url;
            Version = codeSystem.VersionText;
        }


    }
}