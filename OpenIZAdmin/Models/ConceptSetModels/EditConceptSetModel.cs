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
 * Date: 2016-7-23
 */

using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OpenIZAdmin.Models.ConceptSetModels
{
	/// <summary>
	/// Represents an edit concept set model.
	/// </summary>
	public class EditConceptSetModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditConceptSetModel"/> class.
		/// </summary>
		public EditConceptSetModel()
		{
			this.Concepts = new List<Concept>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditConceptSetModel"/> class
		/// with a specific <see cref="ConceptSet"/> instance.
		/// </summary>
		/// <param name="conceptSet">The <see cref="ConceptSet"/> instance.</param>
		public EditConceptSetModel(ConceptSet conceptSet)
		{
			this.Concepts = conceptSet.Concepts;
			this.CreatedBy = conceptSet.CreatedBy?.UserName;
			this.CreationTime = conceptSet.CreationTime.DateTime;
			this.Key = conceptSet.Key.Value;
			this.Mnemonic = conceptSet.Mnemonic;
			this.Name = conceptSet.Name;
			this.Oid = conceptSet.Oid;
			this.Url = conceptSet.Url;
			this.ConceptDeletion.AddRange(this.Concepts.Select(c => false));
		}

		public List<bool> ConceptDeletion { get; set; }

		public string ConceptMnemonic { get; set; }

		public string ConceptName { get; set; }

		public List<Concept> Concepts { get; set; }

		public Guid ConceptToAdd { get; set; }

		[Display(Name = "Created By")]
		public string CreatedBy { get; set; }

		[Display(Name = "Creation Time")]
		public DateTime CreationTime { get; set; }

		public Guid Key { get; set; }

		[Required]
		public string Mnemonic { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Oid { get; set; }

		public List<Concept> SearchedConcepts { get; set; }

		[Required]
		public string Url { get; set; }

		public ConceptSet ToConceptSet()
		{
			return new ConceptSet
			{
				CreationTime = this.CreationTime,
				Key = this.Key,
				Mnemonic = this.Mnemonic,
				Name = this.Name,
				Oid = this.Oid,
				Url = this.Url
			};
		}
	}
}