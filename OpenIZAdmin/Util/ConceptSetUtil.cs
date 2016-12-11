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
 * Date: 2016-8-1
 */

using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Models.ConceptSetModels;
using OpenIZAdmin.Models.ConceptSetModels.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing concept sets.
	/// </summary>
	public static class ConceptSetUtil
	{
		/// <summary>
		/// Converts a <see cref="EditConceptSetModel"/> instance to a <see cref="ConceptSet"/> instance.
		/// </summary>
		/// <param name="model">The <see cref="EditConceptSetModel"/> instance to convert.</param>
		/// <returns>Returns a <see cref="ConceptSet"/> instance.</returns>
		public static ConceptSet ToConceptSet(EditConceptSetModel model)
		{
			var conceptSet = new ConceptSet
			{
				Key = model.Key,
				Mnemonic = model.Mnemonic,
				Name = model.Name,
				Oid = model.Oid,
				Url = model.Url
			};

			return conceptSet;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.DataTypes"/> to a <see cref="ConceptSetViewModel"/>.
		/// </summary>
		/// <param name="conceptSet">The concept set object to convert.</param>
		/// <returns>Returns a ConceptSetViewModel.</returns>
		public static ConceptSetViewModel ToConceptSetViewModel(ConceptSet conceptSet)
		{
			var viewModel = new ConceptSetViewModel
			{
				Concepts = conceptSet.Concepts.Select(ConceptUtil.ToConceptViewModel).ToList(),
				CreationTime = conceptSet.CreationTime.DateTime,
				Key = conceptSet.Key.Value,
				Mnemonic = conceptSet.Mnemonic,
				Name = conceptSet.Name,
				Oid = conceptSet.Oid,
				Url = conceptSet.Url
			};

			return viewModel;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.DataTypes"/> to a <see cref="ConceptSetViewModel"/>.
		/// </summary>
		/// <param name="conceptSet">The concept set object to convert.</param>
		/// <returns>Returns a ConceptSetViewModel.</returns>
		public static EditConceptSetModel ToEditConceptSetModel(ConceptSet conceptSet)
		{
			var viewModel = new EditConceptSetModel
			{
				Oid = conceptSet.Oid,
				Name = conceptSet.Name,
				Url = conceptSet.Url,
				Mnemonic = conceptSet.Mnemonic,
				Key = conceptSet.Key.Value,
				CreationTime = conceptSet.CreationTime.DateTime,
				Concepts = conceptSet.Concepts,
				ConceptDeletion = new List<bool>()
			};

			for (var i = 0; i < conceptSet.Concepts.Count; i++)
			{
				viewModel.ConceptDeletion.Add(false);
			}

			return viewModel;
		}
	}
}