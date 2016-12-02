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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Util
{
	/// <summary> 
	/// Provides a utility for managing concept sets.
	/// </summary>
	public static class ConceptSetUtil
	{

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.DataTypes"/> to a <see cref="OpenIZAdmin.Models.ConceptModels.ViewModels.ConceptSetViewModel"/>.
		/// </summary>
		/// <param name="conceptSet">The concept set object to convert.</param>        
		/// <returns>Returns a ConceptSetViewModel.</returns>
		public static ConceptSetViewModel ToConceptSetViewModel(ConceptSet conceptSet)
		{
            var viewModel = new ConceptSetViewModel();

            viewModel.Oid = conceptSet.Oid;
			viewModel.Mnemonic = conceptSet.Mnemonic;
			viewModel.Key = conceptSet.Key.Value;
			viewModel.CreationTime = conceptSet.CreationTime.DateTime;
            viewModel.Concepts = conceptSet.Concepts;
			return viewModel;
		}

        /// <summary>
        /// Converts a <see cref="OpenIZ.Core.Model.DataTypes"/> to a <see cref="OpenIZAdmin.Models.ConceptModels.ViewModels.ConceptSetViewModel"/>.
        /// </summary>
        /// <param name="conceptSet">The concept set object to convert.</param>        
        /// <returns>Returns a ConceptSetViewModel.</returns>
        public static EditConceptSetModel ToEditConceptSetModel(ConceptSet conceptSet)
        {
            var viewModel = new EditConceptSetModel();

            viewModel.Oid = conceptSet.Oid;
            viewModel.Mnemonic = conceptSet.Mnemonic;
            viewModel.Key = conceptSet.Key.Value;
            viewModel.CreationTime = conceptSet.CreationTime.DateTime;
            viewModel.Concepts = conceptSet.Concepts;
            viewModel.ConceptDeletion = new List<bool>();
            for (var i = 0; i < conceptSet.Concepts.Count(); i++)
            {
                viewModel.ConceptDeletion.Add(false);
            }
            return viewModel;
        }

    }
}