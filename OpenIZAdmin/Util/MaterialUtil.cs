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
 * Date: 2016-8-1
 */

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Models.MaterialModels;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing materials.
	/// </summary>
	public static class MaterialUtil
	{
		/// <summary>
		/// Converts a <see cref="Material"/> instance to a <see cref="EditMaterialModel"/> instance.
		/// </summary>
		/// <param name="imsiClient">The Imsi service client.</param>
		/// <param name="material">The material to convert.</param>
		/// <returns>Returns a EditMaterialModel view model.</returns>
		public static EditMaterialModel ToEditMaterialModel(ImsiServiceClient imsiClient, Material material)
		{
			var model = new EditMaterialModel
			{
				Name = string.Join(" ", material.Names.SelectMany(m => m.Component).Select(c => c.Value))
			};

			var formConcepts = imsiClient.Query<Concept>(m => m.ClassKey == ConceptClassKeys.Form && m.ObsoletionTime == null);
			model.FormConcepts.AddRange(formConcepts.Item.OfType<Concept>().Select(c => new SelectListItem { Text = c.Mnemonic, Selected = material.FormConceptKey == c.Key, Value = c.Key?.ToString() }));

			var quantityConcepts = imsiClient.Query<Concept>(m => m.ClassKey == ConceptClassKeys.UnitOfMeasure && m.ObsoletionTime == null);
			model.QuantityConcepts.AddRange(quantityConcepts.Item.OfType<Concept>().Select(c => new SelectListItem { Text = c.Mnemonic, Selected = material.QuantityConceptKey == c.Key, Value = c.Key?.ToString() }));

			return model;
		}
	}
}