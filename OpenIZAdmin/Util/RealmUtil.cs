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
 * Date: 2016-7-31
 */

using OpenIZAdmin.Models;
using OpenIZAdmin.Models.Domain;
using OpenIZAdmin.Models.RealmModels;
using OpenIZAdmin.Models.RealmModels.ViewModels;

namespace OpenIZAdmin.Util
{
	public static class RealmUtil
	{
		/// <summary>
		/// Generates a <see cref="OpenIZAdmin.Models.RealmModels.LeaveRealmModel"/> instance.
		/// </summary>
		/// <param name="realm">The realm for which to generate the <see cref="OpenIZAdmin.Models.RealmModels.LeaveRealmModel"/> from.</param>
		/// <returns>Returns an instance of the <see cref="OpenIZAdmin.Models.RealmModels.LeaveRealmModel"/> class.</returns>
		public static LeaveRealmModel GenerateLeaveRealmModel(Realm realm)
		{
			LeaveRealmModel viewModel = new LeaveRealmModel();

			viewModel.CurrentRealm = new RealmViewModel().Map(realm);
			viewModel.Map(realm);

			return viewModel;
		}

		/// <summary>
		/// Generates a <see cref="OpenIZAdmin.Models.RealmModels.ViewModels.RealmViewModel"/> instance.
		/// </summary>
		/// <param name="realm">The realm for which to generate the <see cref="OpenIZAdmin.Models.RealmModels.ViewModels.RealmViewModel"/> from.</param>
		/// <returns>Returns an instance of the <see cref="OpenIZAdmin.Models.RealmModels.ViewModels.RealmViewModel"/> class.</returns>
		public static RealmViewModel GenerateRealmViewModel(Realm realm)
		{
			RealmViewModel viewModel = new RealmViewModel();

			viewModel.Map(realm);

			return viewModel;
		}
	}
}