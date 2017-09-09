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
 * User: nitya
 * Date: 2017-9-9
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.AMI.DataTypes;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Services.Core;

namespace OpenIZAdmin.Services.Metadata.AssigningAuthorities
{
	/// <summary>
	/// Represents an assigning authority service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.AmiServiceBase" />
	/// <seealso cref="OpenIZAdmin.Services.Metadata.AssigningAuthorities.IAssigningAuthorityService" />
	public class AssigningAuthorityService : AmiServiceBase, IAssigningAuthorityService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AssigningAuthorityService"/> class.
		/// </summary>
		/// <param name="client">The client.</param>
		public AssigningAuthorityService(AmiServiceClient client) : base(client)
		{
		}

		/// <summary>
		/// Creates the assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityInfo">The assigning authority information.</param>
		/// <returns>Returns the created assigning authority.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public AssigningAuthorityInfo CreateAssigningAuthority(AssigningAuthorityInfo assigningAuthorityInfo)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the assigning authority.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the assigning authority for the given key or null if no assigning authority is found.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public AssigningAuthorityInfo GetAssigningAuthority(Guid key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the assigning authorities by OID.
		/// </summary>
		/// <param name="oid">The oid.</param>
		/// <returns>Returns a list of assigning authorities whose OID matches the given OID.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IEnumerable<AssigningAuthorityInfo> GetAssigningAuthoritiesByOid(string oid)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the assigning authorities by domain.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <returns>Returns a list of assigning authorities whose domain matches the given domain.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IEnumerable<AssigningAuthorityInfo> GetAssigningAuthoritiesByDomain(string domain)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Searches the specified search term.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of assigning authorities which match the given search term.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IEnumerable<AssigningAuthorityInfo> Search(string searchTerm)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Updates the assigning authority.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="assingAssigningAuthorityInfo">The assing assigning authority information.</param>
		/// <returns>Returns the updated assigning authority.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public AssigningAuthorityInfo UpdateAssigningAuthority(Guid key, AssigningAuthorityInfo assingAssigningAuthorityInfo)
		{
			throw new NotImplementedException();
		}
	}
}
