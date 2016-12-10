/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
	public static class AssigningAuthorityUtil
	{
		/// <summary>
		/// Queries for all AssigningAuthorityInfo objects
		/// </summary>
		/// <param name="client">The AMI service client</param>
		/// <returns>Returns IEnumerable AssigningAuthorityViewModel objects.</returns>
		internal static IEnumerable<AssigningAuthorityViewModel> GetAllAssigningAuthorities(AmiServiceClient client)
		{
			IEnumerable<AssigningAuthorityViewModel> viewModels = new List<AssigningAuthorityViewModel>();

			try
			{
				var assigningAuthorities = client.GetAssigningAuthorities(p => p.ObsoletionTime != null);

				viewModels = assigningAuthorities.CollectionItem.Select(p => AssigningAuthorityUtil.ToAssigningAuthorityViewModel(p));
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve policies: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve policies: {0}", e.Message);
			}

			return viewModels;
		}

		/// <summary>
		/// Converts a <see cref="EditAssigningAuthorityModel"/> instance to an <see cref="AssigningAuthorityInfo"/> instance.
		/// </summary>
		/// <param name="model">The edit assigning authority view model.</param>
		/// <returns>Returns the assigning authority info.</returns>
		public static AssigningAuthorityInfo ToAssigningAuthorityInfo(EditAssigningAuthorityModel model)
		{
			var assigningAuthorityInfo = new AssigningAuthorityInfo
			{
				Id = model.Key,
				AssigningAuthority =
				{
					Key = model.Key,
					Url = model.Url,
					DomainName = model.DomainName,
					Description = model.Description,
					Oid = model.Oid,
					Name = model.Name
				}
			};

			return assigningAuthorityInfo;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.AMI.Auth"/> to a <see cref="OpenIZAdmin.Models.AssigningAuthorityModels.ViewModels.AssigningAuthorityViewModel"/>.
		/// </summary>
		/// <param name="assigningAuthority">The AssigningAuthorityInfo object to convert.</param>
		/// <returns>Returns an AssigningAuthorityViewModel.</returns>
		public static AssigningAuthorityViewModel ToAssigningAuthorityViewModel(AssigningAuthorityInfo assigningAuthority)
		{
			AssigningAuthorityViewModel viewModel = new AssigningAuthorityViewModel();

			viewModel.Key = assigningAuthority.Id;
			viewModel.Name = assigningAuthority.AssigningAuthority.Name;
			viewModel.Oid = assigningAuthority.AssigningAuthority.Oid;
			viewModel.Url = assigningAuthority.AssigningAuthority.Url;
			viewModel.DomainName = assigningAuthority.AssigningAuthority.DomainName;
			viewModel.Description = assigningAuthority.AssigningAuthority.Description;
			return viewModel;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.AMI.Auth"/> to a <see cref="OpenIZAdmin.Models.AssigningAuthorityModels.EditAssigningAuthorityModel"/>.
		/// </summary>
		/// <param name="assigningAuthority">The AssigningAuthorityInfo object to convert.</param>
		/// <returns>Returns an EditAssigningAuthorityModel.</returns>
		public static EditAssigningAuthorityModel ToEditAssigningAuthorityModel(AssigningAuthorityInfo assigningAuthority)
		{
			EditAssigningAuthorityModel viewModel = new EditAssigningAuthorityModel();

			viewModel.Key = assigningAuthority.Id;
			viewModel.Name = assigningAuthority.AssigningAuthority.Name;
			viewModel.Oid = assigningAuthority.AssigningAuthority.Oid;
			viewModel.Url = assigningAuthority.AssigningAuthority.Url;
			viewModel.DomainName = assigningAuthority.AssigningAuthority.DomainName;
			viewModel.Description = assigningAuthority.AssigningAuthority.Description;
			return viewModel;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZAdmin.Models.AssigningAuthorityModels.CreateAssigningAuthorityModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth"/>.
		/// </summary>
		/// <param name="model">The CreateAssigningAuthorityModel object to convert.</param>
		/// <returns>Returns an AssigningAuthorityInfo.</returns>
		public static AssigningAuthorityInfo ToCreateAssigningAuthorityModel(CreateAssigningAuthorityModel model)
		{
			AssigningAuthorityInfo assigningAuthority = new AssigningAuthorityInfo();
			assigningAuthority.AssigningAuthority = new AssigningAuthority()
			{
				Name = model.Name,
				Oid = model.Oid,
				DomainName = model.DomainName,
				Description = model.Description,
				Url = model.Url
			};

			return assigningAuthority;
		}
	}
}