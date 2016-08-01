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
		/// Populates a language list.
		/// </summary>
		/// <param name="model">The model for which to populate the language list.</param>
		public static void PopulateLanguageList(ref CreateConceptModel model)
		{
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

			model.LanguageList.AddRange(cultures.Where(c => c.TwoLetterISOLanguageName.Length == 2).Select(c => c).Distinct().OrderBy(c => c.DisplayName).Select(c => new SelectListItem { Text = c.DisplayName + " (" + c.TwoLetterISOLanguageName + ")", Value = c.TwoLetterISOLanguageName }));
		}

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

			concept.Mnemonic = model.Mnemonic;

			return concept;
		}
	}
}