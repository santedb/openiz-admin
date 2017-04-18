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
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.ConceptSetModels
{
	/// <summary>
	/// Represents an edit concept set model.
	/// </summary>
	public sealed class EditConceptSetModel : ConceptSetModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditConceptSetModel"/> class.
		/// </summary>
		public EditConceptSetModel()
		{
			Concepts = new List<ConceptViewModel>();
			ConceptList = new List<SelectListItem>();
			AddConcepts = new List<string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditConceptSetModel"/> class
		/// with a specific <see cref="ConceptSet"/> instance.
		/// </summary>
		/// <param name="conceptSet">The <see cref="ConceptSet"/> instance.</param>
		public EditConceptSetModel(ConceptSet conceptSet) : this()
		{
			Concepts = conceptSet.Concepts.Select(c => new ConceptViewModel(c, conceptSet.Key ?? Guid.Empty)).ToList();
			CreatedBy = conceptSet.CreatedBy?.UserName;
			CreationTime = conceptSet.CreationTime.DateTime;
			Id = conceptSet.Key ?? Guid.Empty;
			Mnemonic = conceptSet.Mnemonic;
			Name = conceptSet.Name;
			Oid = conceptSet.Oid;
			Url = conceptSet.Url;
		}

		///// <summary>
		///// Gets or sets the Mnemonic of the Concept
		///// </summary>
		//public string ConceptMnemonic { get; set; }

		/// <summary>
		/// Gets or sets the list of Concepts to add
		/// </summary>
		public List<string> AddConcepts { get; set; }

		/// <summary>
		/// Gets or sets the concept list from the search parameters from the ajax search method
		/// </summary>
		public List<SelectListItem> ConceptList { get; set; }

		/// <summary>
		/// Gets or sets the name of the concept.
		/// </summary>
		/// <value>The name of the concept.</value>
		public string ConceptName { get; set; }

		/// <summary>
		/// Gets or sets the created by.
		/// </summary>
		/// <value>The created by.</value>
		[Display(Name = "Created By")]
		public string CreatedBy { get; set; }

		/// <summary>
		/// Gets or sets the mnemonic of the concept.
		/// </summary>
		[Display(Name = "Mnemonic", ResourceType = typeof(Locale))]
		[Required]
		public override string Mnemonic { get; set; }

		/// <summary>
		/// Gets or sets the name of the concept set.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		[Required]
		public override string Name { get; set; }

		/// <summary>
		/// Gets or sets the oid.
		/// </summary>
		/// <value>The oid.</value>
		[Display(Name = "Oid", ResourceType = typeof(Locale))]
		[Required]
		public override string Oid { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		[Display(Name = "Url", ResourceType = typeof(Locale))]
		[Required]
		public override string Url { get; set; }

		/// <summary>
		/// Converts an <see cref="EditConceptSetModel"/> instance to a <see cref="ConceptSet"/> instance.
		/// </summary>
		/// <returns>Returns the converted concept set.</returns>
		public ConceptSet ToConceptSet(ConceptSet conceptSet)
		{
			conceptSet.Mnemonic = this.Mnemonic;
			conceptSet.Name = this.Name;
			conceptSet.Oid = this.Oid;
			conceptSet.Url = this.Url;

			if (!this.AddConcepts.Any()) return conceptSet;

			foreach (var concept in this.AddConcepts)
			{
				Guid id;
				if (Guid.TryParse(concept, out id))
				{
					conceptSet.ConceptsXml.Add(id);
				}
			}

			return conceptSet;
		}
	}
}