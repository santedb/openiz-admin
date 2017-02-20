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
 * Date: 2017-2-11
 */

using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Messaging.IMSI.Client;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing users.
	/// </summary>
	public static class AccountUtil
	{
		/// <summary>
		/// Gets a list of phone types.
		/// </summary>
		/// <param name="imsiClient">The <see cref="ImsiServiceClient"/> instance.</param>
		/// <returns>Returns a select list of phone types.</returns>
		public static List<SelectListItem> GetPhoneTypeList(ImsiServiceClient imsiClient)
		{
			var phoneTypes = new List<SelectListItem>
			{
				new SelectListItem { Text = string.Empty, Value = string.Empty }
			};

			var bundle = imsiClient.Query<ConceptSet>(c => c.Mnemonic == Constants.TelecomAddressUse);

			var telecoms = bundle.Item.OfType<ConceptSet>().FirstOrDefault(c => c.Mnemonic == Constants.TelecomAddressUse);

			if (telecoms != null)
			{
				phoneTypes.AddRange(from con
									in telecoms.Concepts
									let name = string.Join(" ", con.ConceptNames.Select(n => n.Name).Select(c => c.ToString()))
									select new SelectListItem { Text = name, Value = con.Key.ToString() });
			}

			return phoneTypes;
		}
	}
}