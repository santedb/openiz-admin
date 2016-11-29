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
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing concepts.
	/// </summary>
	public static class ConceptUtil
	{
		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.DataTypes.Concept"/> to a <see cref="OpenIZAdmin.Models.ConceptModels.CreateConceptModel"/>.
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
		/// Converts a <see cref="OpenIZ.Core.Model.DataTypes"/> to a <see cref="OpenIZAdmin.Models.ConceptModels.ViewModels.ConceptViewModel"/>.
		/// </summary>
		/// <param name="concept">The concept object to convert.</param>        
		/// <returns>Returns a ConceptViewModel.</returns>
		public static ConceptViewModel ToConceptViewModel(Concept concept)
		{
			var viewModel = new ConceptViewModel
			{
				Name = new List<string>(),
				Languages = new List<string>()
			};

			foreach (var conceptName in concept.ConceptNames)
			{
				viewModel.Name.Add(conceptName.Name);
				viewModel.Languages.Add(conceptName.Language);
			}

			viewModel.Class = concept.Class?.Name;
			viewModel.Mnemonic = concept.Mnemonic;
			viewModel.Key = concept.Key.Value;
			viewModel.CreationTime = concept.CreationTime.DateTime;

			return viewModel;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.DataTypes"/> to a <see cref="OpenIZAdmin.Models.ConceptModels.EditConceptModel"/>.
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

			//viewModel.Class = concept.Class.Name;
			viewModel.Mnemonic = concept.Mnemonic;
			viewModel.Key = concept.Key.Value;
			viewModel.CreationTime = concept.CreationTime.DateTime;

			return viewModel;
		}

	}
}