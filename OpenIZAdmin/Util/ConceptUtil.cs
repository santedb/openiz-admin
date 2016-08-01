﻿/*
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
	public static class ConceptUtil
	{
		public static void PopulateLanguageList(ref CreateConceptModel model)
		{
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

			model.LanguageList.AddRange(cultures.Select(c => new SelectListItem { Text = c.TwoLetterISOLanguageName, Value = c.TwoLetterISOLanguageName }));
		}

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