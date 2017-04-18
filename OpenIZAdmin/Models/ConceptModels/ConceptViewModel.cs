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
using OpenIZAdmin.Models.ReferenceTermModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OpenIZAdmin.Models.ConceptModels
{
	/// <summary>
	/// Represents a model to view a concept.
	/// </summary>
	public sealed class ConceptViewModel : ConceptModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptViewModel"/> class.
		/// </summary>
		public ConceptViewModel()
		{
			this.Names = new List<string>();
			this.ReferenceTerms = new List<ReferenceTermViewModel>();
			this.Languages = new List<Language>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptViewModel"/> class.
		/// </summary>
		/// <param name="concept">The concept.</param>
		public ConceptViewModel(Concept concept) : this()
		{
			Class = concept.Class?.Name;
			ConceptSetId = Guid.Empty;
			CreationTime = concept.CreationTime.DateTime;
			Id = concept.Key ?? Guid.Empty;
			IsObsolete = concept.ObsoletionTime != null;
			IsSystemConcept = concept.IsSystemConcept;
			Languages = concept.ConceptNames.Select(k => new Language(k.Language, k.Name)).ToList();
			Mnemonic = concept.Mnemonic;
			Names = concept.ConceptNames.Select(c => c.Name).ToList();
			ConceptNames = (Names.Any()) ? string.Join(", ", Names) : string.Empty;
			ReferenceTerms = new List<ReferenceTermViewModel>();
			VersionKey = concept.VersionKey;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptViewModel"/> class.
		/// </summary>
		/// <param name="concept">The concept.</param>
		/// <param name="conceptSetId">The guid of the Concept Set that the Concept is associated with</param>
		public ConceptViewModel(Concept concept, Guid conceptSetId) : this(concept)
		{
			ConceptSetId = conceptSetId;
		}

		/// <summary>
		/// Gets or sets the class of the concept.
		/// </summary>
		[Display(Name = "ConceptClass", ResourceType = typeof(Localization.Locale))]
		public string Class { get; set; }

		/// <summary>
		/// Gets or sets the Concept Set identifier associated with the Concept instance
		/// </summary>
		public Guid? ConceptSetId { get; set; }

		/// <summary>
		/// Gets or sets the Language list for the Language ISO 2 digit code and the associated display name of the Concept.
		/// </summary>
		[Display(Name = "Languages", ResourceType = typeof(Localization.Locale))]
		public List<Language> Languages { get; set; }
	}
}