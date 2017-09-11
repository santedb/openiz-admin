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
 * User: Nityan
 * Date: 2017-9-9
 */
using System;
using System.Collections.Generic;
using System.Linq;
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
		public AssigningAuthorityInfo GetAssigningAuthority(Guid key)
		{
			return this.Client.GetAssigningAuthorities(a => a.Key == key).CollectionItem.FirstOrDefault(a => a.Id == key);
		}

		/// <summary>
		/// Gets the assigning authorities by OID.
		/// </summary>
		/// <param name="oid">The oid.</param>
		/// <returns>Returns a list of assigning authorities whose OID matches the given OID.</returns>
		public IEnumerable<AssigningAuthorityInfo> GetAssigningAuthoritiesByOid(string oid)
		{
			return this.Client.GetAssigningAuthorities(a => a.Oid == oid).CollectionItem.Where(a => a.AssigningAuthority.Oid == oid);
		}

		/// <summary>
		/// Gets the assigning authorities by domain.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <returns>Returns a list of assigning authorities whose domain matches the given domain.</returns>
		public IEnumerable<AssigningAuthorityInfo> GetAssigningAuthoritiesByDomain(string domain)
		{
			return this.Client.GetAssigningAuthorities(a => a.DomainName == domain).CollectionItem.Where(a => a.AssigningAuthority.DomainName == domain);
		}

		/// <summary>
		/// Searches the specified search term.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of assigning authorities which match the given search term.</returns>
		public IEnumerable<AssigningAuthorityInfo> Search(string searchTerm)
		{
			var results = new List<AssigningAuthorityInfo>();

			if (searchTerm == "*")
			{
				results.AddRange(this.Client.GetAssigningAuthorities(a => a.ObsoletionTime == null).CollectionItem.Where(a => a.AssigningAuthority.ObsoletionTime == null));
			}
			else
			{
				Guid key;

				if (!Guid.TryParse(searchTerm, out key))
				{
					results.AddRange(this.Client.GetAssigningAuthorities(c => c.Name.Contains(searchTerm)).CollectionItem.Where(a => a.AssigningAuthority.Name.Contains(searchTerm)));
				}
				else
				{
					results.Add(this.GetAssigningAuthority(key));
				}
			}

			return results;
		}

		/// <summary>
		/// Updates the assigning authority.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="assigningAuthorityInfo">The assigning authority information.</param>
		/// <returns>Returns the updated assigning authority.</returns>
		public AssigningAuthorityInfo UpdateAssigningAuthority(Guid key, AssigningAuthorityInfo assigningAuthorityInfo)
		{
			return this.Client.UpdateAssigningAuthority(key.ToString(), assigningAuthorityInfo);
		}
	}
}
