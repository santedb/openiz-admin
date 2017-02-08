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
 * Date: 2016-7-30
 */

using OpenIZ.Core.Model.AMI.DataTypes;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Models.AssigningAuthorityModels;
using OpenIZAdmin.Models.AssigningAuthorityModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Represents a utility for managing assigning authorities.
	/// </summary>
	public static class AssigningAuthorityUtil
	{
		/// <summary>
		/// Queries for all AssigningAuthorityInfo objects
		/// </summary>
		/// <param name="client">The AMI service client</param>
		/// <returns>Returns IEnumerable AssigningAuthorityViewModel objects.</returns>
		internal static IEnumerable<AssigningAuthorityViewModel> GetAllAssigningAuthorities(AmiServiceClient client)
		{
			var assigningAuthorities = client.GetAssigningAuthorities(p => p.ObsoletionTime == null);

			var viewModels = assigningAuthorities.CollectionItem.Select(a => new AssigningAuthorityViewModel(a));

			return viewModels;
		}
	}
}