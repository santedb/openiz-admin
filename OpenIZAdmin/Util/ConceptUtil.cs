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
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.ConceptModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing concepts.
	/// </summary>
	public static class ConceptUtil
	{
		/// <summary>
		/// Converts a <see cref="Concept"/> to a <see cref="CreateConceptModel"/>.
		/// </summary>
		/// <param name="model">The create concept model to convert.</param>
		/// <returns>Returns a concept.</returns>
		public static Concept ToConcept(CreateConceptModel model)
		{
			return new Concept
			{
				Class = new ConceptClass
				{
					Key = Guid.Parse(model.ConceptClass)
				},
				ConceptNames = new List<ConceptName>
				{
					new ConceptName
					{
						Language = model.Language,
						Name = model.Name
					}
				},
				Key = Guid.NewGuid(),
				Mnemonic = model.Mnemonic,
			};
		}

		/// <summary>
		/// Converts a <see cref="Concept"/> to a <see cref="ConceptViewModel"/>.
		/// </summary>
		/// <param name="concept">The concept object to convert.</param>
		/// <returns>Returns a ConceptViewModel.</returns>
		public static ConceptViewModel ToConceptViewModel(Concept concept)
		{
			var viewModel = new ConceptViewModel
			{
				Class = concept.Class?.Name,
				CreationTime = concept.CreationTime.DateTime,
				Key = concept.Key.Value,
				Mnemonic = concept.Mnemonic,
				VersionKey = concept.VersionKey.GetValueOrDefault(Guid.Empty)
			};

			concept.ConceptNames.ForEach(c =>
			{
				viewModel.Names.Add(c.Name);
				viewModel.Languages.Add(c.Language);
			});

			return viewModel;
		}

		/// <summary>
		/// Converts a <see cref="Concept"/> to a <see cref="EditConceptModel"/>.
		/// </summary>
		/// <param name="concept">The concept object to convert.</param>
		/// <returns>Returns a EditConceptModel.</returns>
		public static EditConceptModel ToEditConceptModel(Concept concept)
		{
			var viewModel = new EditConceptModel
			{
				Name = concept.ConceptNames.Select(c => c.Name).ToList(),
				Languages = concept.ConceptNames.Select(c => c.Language).ToList()
			};

			if (!viewModel.Languages.Contains("en"))
			{
				viewModel.Languages.Add("en");
				viewModel.Name.Add("");
			}

			if (!viewModel.Languages.Contains("sw"))
			{
				viewModel.Languages.Add("sw");
				viewModel.Name.Add("");
			}

			viewModel.Mnemonic = concept.Mnemonic;
			viewModel.Key = concept.Key.Value;
			viewModel.CreationTime = concept.CreationTime.DateTime;

			return viewModel;
		}
	}
}