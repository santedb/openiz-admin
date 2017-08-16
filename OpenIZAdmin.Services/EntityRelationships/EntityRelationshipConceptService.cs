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
 * Date: 2017-8-15
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Services.Core;

namespace OpenIZAdmin.Services.EntityRelationships
{
	/// <summary>
	/// Represents an entity relationship concept service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.ImsiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.EntityRelationships.IEntityRelationshipConceptService" />
	public class EntityRelationshipConceptService : ImsiServiceBase, IEntityRelationshipConceptService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityRelationshipConceptService"/> class.
		/// </summary>
		/// <param name="client">The client.</param>
		public EntityRelationshipConceptService(ImsiServiceClient client) : base(client)
		{
		}

		/// <summary>
		/// Gets a list of concepts which relate materials to manufactured materials.
		/// </summary>
		/// <returns>Returns a list of concepts which relate materials to manufactured materials.</returns>
		public IEnumerable<Concept> GetMaterialManufacturedMaterialRelationshipConcepts()
		{
			return new List<Concept>
			{
				new Concept
				{
					Key = EntityRelationshipTypeKeys.Instance
				}
			};
		}
	}
}
