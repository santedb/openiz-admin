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
 * Date: 2016-7-23
 */
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Models.ConceptModels.ViewModels
{
	public class ConceptViewModel
	{
		public ConceptViewModel()
		{

		}

		public ConceptViewModel(Concept concept)
		{
			this.CreationTime = concept.CreationTime.DateTime;
			this.IsReadOnly = concept.IsSystemConcept;
			this.Language = concept.ConceptNames.Select(c => c.Language).FirstOrDefault();
			this.Mnemonic = concept.Mnemonic;
			this.Name = concept.ConceptNames.Select(c => c.Name).Aggregate((a, b) => (a + ", " + b));
		}

		public ConceptViewModel(ConceptSet conceptSet)
		{
			this.CreationTime = conceptSet.CreationTime.DateTime;
			this.IsReadOnly = conceptSet.Concepts.Select(c => c.IsSystemConcept).All(c => c);
			this.Language = "N/A";
			this.Mnemonic = conceptSet.Mnemonic;
			this.Name = conceptSet.Name;
		}

		public DateTime CreationTime { get; set; }

		public bool IsReadOnly { get; set; }

		public string Language { get; set; }

		public string Mnemonic { get; set; }

		public string Name { get; set; }
	}
}