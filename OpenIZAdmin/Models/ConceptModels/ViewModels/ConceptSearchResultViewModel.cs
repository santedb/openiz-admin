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
	public class ConceptSearchResultViewModel
	{
		public ConceptSearchResultViewModel()
		{
			this.ConceptNames = new List<string>();
		}

		public ConceptSearchResultViewModel(Concept concept)
		{
			this.ConceptNames = concept.ConceptNames.Select(c => c.Name).ToList();
			this.ConceptSearchType = ConceptSearchType.Concept;
			this.CreationTime = concept.CreationTime.DateTime;
			this.Key = concept.Key.GetValueOrDefault();
			this.Mnemonic = concept.Mnemonic;
			this.VersionKey = concept.VersionKey;
		}

		public ConceptSearchResultViewModel(ConceptSet conceptSet)
		{
			this.ConceptSearchType = ConceptSearchType.ConceptSet;
			this.ConceptNames = new List<string> { conceptSet.Name };
			this.CreationTime = conceptSet.CreationTime.DateTime;
			this.Key = conceptSet.Key.GetValueOrDefault();
			this.Mnemonic = conceptSet.Mnemonic;
		}

		public List<string> ConceptNames { get; set; }

		public ConceptSearchType ConceptSearchType { get; set; }

		public DateTime CreationTime { get; set; }

		public Guid Key { get; set; }

		public string Mnemonic { get; set; }

		public Guid? VersionKey { get; set; }
	}
}