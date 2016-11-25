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
			Concept concept = new Concept();

			concept.ConceptNames = new List<ConceptName>();

			concept.ConceptNames.Add(new ConceptName
			{
				Language = model.Language,
				Name = model.Name
			});
            var list = model.ConceptClassList;
            concept.Class = new ConceptClass()
            {
                Key = Guid.Parse(model.ConceptClass)
            };
			concept.Mnemonic = model.Mnemonic;
			return concept;
		}

        public static ConceptViewModel ToConceptViewModel(Concept concept)
        {
            ConceptViewModel viewModel = new ConceptViewModel();


            viewModel.Name = new List<string>();
            viewModel.Languages = new List<string>();
            for (var i = 0; i<concept.ConceptNames.Count; i++)
            {
                viewModel.Name.Add(concept.ConceptNames[i].Name);
                viewModel.Languages.Add(concept.ConceptNames[i].Language);
            }
            viewModel.Class = concept.Class.Name;
            viewModel.Mnemonic = concept.Mnemonic;
            viewModel.Key = concept.Key.Value;
            viewModel.CreationTime = concept.CreationTime.DateTime;

            return viewModel;
        }

        public static EditConceptModel ToEditConceptModel(Concept concept)
        {
            EditConceptModel viewModel = new EditConceptModel();


            viewModel.Name = new List<string>();
            viewModel.Languages = new List<string>();
            for (var i = 0; i < concept.ConceptNames.Count; i++)
            {
                viewModel.Name.Add(concept.ConceptNames[i].Name);
                viewModel.Languages.Add(concept.ConceptNames[i].Language);
            }
            if (!viewModel.Languages.Contains("en")){
                viewModel.Languages.Add("en");
                viewModel.Name.Add("");
            }
            if (!viewModel.Languages.Contains("sw")){
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