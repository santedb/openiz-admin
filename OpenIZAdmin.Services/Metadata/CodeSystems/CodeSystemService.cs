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
 * Date: 2017-9-7
 */

using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Services.Metadata.CodeSystems
{
	/// <summary>
	/// Represents a code system service.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Services.Core.AmiServiceBase" />
	/// <seealso cref="ICodeSystemService" />
	public class CodeSystemService : AmiServiceBase, ICodeSystemService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodeSystemService"/> class.
		/// </summary>
		/// <param name="client">The client.</param>
		public CodeSystemService(AmiServiceClient client) : base(client)
		{
		}

		/// <summary>
		/// Creates the code system.
		/// </summary>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns the created code system.</returns>
		public CodeSystem CreateCodeSystem(CodeSystem codeSystem)
		{
			return this.Client.CreateCodeSystem(codeSystem);
		}

		/// <summary>
		/// Gets the code system.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the code system for the given key or null if no code system is found.</returns>
		public CodeSystem GetCodeSystem(Guid key)
		{
			return this.Client.GetCodeSystem(key.ToString());
		}

		/// <summary>
		/// Gets the code systems by domain.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <returns>Returns a list of code systems which match the given domain.</returns>
		public IEnumerable<CodeSystem> GetCodeSystemsByDomain(string domain)
		{
			return this.Client.GetCodeSystems(c => c.Authority == domain).CollectionItem.Where(c => c.Authority == domain);
		}

		/// <summary>
		/// Gets the code systems by oid.
		/// </summary>
		/// <param name="oid">The oid.</param>
		/// <returns>Returns a list of code systems which match the given OID.</returns>
		public IEnumerable<CodeSystem> GetCodeSystemsByOid(string oid)
		{
			return this.Client.GetCodeSystems(c => c.Oid == oid).CollectionItem.Where(c => c.Oid == oid);
		}

		/// <summary>
		/// Gets the code systems by URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns>Returns a list of code systems which match the given URL.</returns>
		public IEnumerable<CodeSystem> GetCodeSystemsByUrl(string url)
		{
			return this.Client.GetCodeSystems(c => c.Url == url).CollectionItem.Where(c => c.Url == url);
		}

		/// <summary>
		/// Determines whether the domain is a duplicate domain.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <returns><c>true</c> if the domain already exists; otherwise, <c>false</c>.</returns>
		public bool IsDuplicateDomain(string domain)
		{
			return this.GetCodeSystemsByDomain(domain).Any();
		}

		/// <summary>
		/// Determines whether the OID is a duplicate OID.
		/// </summary>
		/// <param name="oid">The oid.</param>
		/// <returns><c>true</c> if the OID already exists; otherwise, <c>false</c>.</returns>
		public bool IsDuplicateOid(string oid)
		{
			return this.GetCodeSystemsByOid(oid).Any();
		}

		/// <summary>
		/// Determines whether the URL is a duplicate URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns><c>true</c> if the URL already exists; otherwise, <c>false</c>.</returns>
		public bool IsDuplicateUrl(string url)
		{
			return this.GetCodeSystemsByUrl(url).Any();
		}

		/// <summary>
		/// Searches for a code system using a given search term.
		/// </summary>
		/// <param name="searchTerm">The search term.</param>
		/// <returns>Returns a list of code systems which match the given search term.</returns>
		public IEnumerable<CodeSystem> Search(string searchTerm)
		{
			var codeSystems = new List<CodeSystem>();

			codeSystems.AddRange(searchTerm == "*" ? this.Client.GetCodeSystems(c => c.ObsoletionTime == null).CollectionItem : this.Client.GetCodeSystems(c => c.Name.Contains(searchTerm)).CollectionItem);

			return codeSystems;
		}

		/// <summary>
		/// Updates the code system.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns the updated code system.</returns>
		public CodeSystem UpdateCodeSystem(Guid key, CodeSystem codeSystem)
		{
			return this.Client.UpdateCodeSystem(key.ToString(), codeSystem);
		}
	}
}