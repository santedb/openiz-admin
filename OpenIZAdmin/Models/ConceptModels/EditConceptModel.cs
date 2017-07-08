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
 * Date: 2016-11-15
 */

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptNameModels;
using OpenIZAdmin.Models.Core;
using OpenIZAdmin.Models.ReferenceTermModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.ConceptModels
{
	/// <summary>
	/// Represents a model to edit a concept.
	/// </summary>
	public sealed class EditConceptModel : ConceptModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditConceptModel"/> class.
		/// </summary>
		public EditConceptModel()
		{
			this.ConceptClassList = new List<SelectListItem>
			{
				new SelectListItem { Text = string.Empty, Value = string.Empty }
			};

			this.LanguageList = new List<SelectListItem>
			{
				new SelectListItem { Text = string.Empty, Value = string.Empty }
			};

			this.ReferenceTerms = new List<ReferenceTermViewModel>();
			this.Languages = new List<ConceptNameViewModel>();
			this.ReferenceTermList = new List<SelectListItem>();

			this.RelationshipTypeList = new List<SelectListItem>
			{
				new SelectListItem {Text = string.Empty, Value = string.Empty},
				new SelectListItem {Text = Locale.InverseOf, Value = ConceptRelationshipTypeKeys.InverseOf.ToString()},
				new SelectListItem {Text = Locale.MemberOf, Value = ConceptRelationshipTypeKeys.MemberOf.ToString()},
				new SelectListItem {Text = Locale.NegationOf, Value = ConceptRelationshipTypeKeys.NegationOf.ToString()},
				new SelectListItem {Text = Locale.SameAs, Value = ConceptRelationshipTypeKeys.SameAs.ToString()}
			};
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditConceptModel"/> class.
		/// </summary>
		/// <param name="concept">The concept.</param>
		public EditConceptModel(Concept concept) : this()
		{
			this.CreationTime = concept.CreationTime.DateTime;
			this.Id = concept.Key.Value;
			this.IsObsolete = concept.StatusConceptKey == StatusKeys.Obsolete;
			this.IsSystemConcept = concept.IsSystemConcept;
			this.Mnemonic = concept.Mnemonic;
			this.Name = string.Join(" ", concept.ConceptNames.Select(c => c.Name));
			this.Languages = concept.ConceptNames.Select(k => new ConceptNameViewModel(k.Language, k.Name, concept)).ToList();
			this.VersionKey = concept.VersionKey;
		}

		/// <summary>
		/// Gets or sets the reference term to add
		/// </summary>
		[Display(Name = "AddReferenceTerms", ResourceType = typeof(Locale))]
		public string AddReferenceTerm { get; set; }

		/// <summary>
		/// Gets or sets the concept class.
		/// </summary>
		/// <value>The concept class.</value>
		[Display(Name = "ConceptClass", ResourceType = typeof(Locale))]
		public string ConceptClass { get; set; }

		/// <summary>
		/// Gets or sets the concept class list.
		/// </summary>
		/// <value>The concept class list.</value>
		public List<SelectListItem> ConceptClassList { get; set; }

		/// <summary>
		/// Gets or sets the language list.
		/// </summary>
		/// <value>The language list.</value>
		public List<SelectListItem> LanguageList { get; set; }

		/// <summary>
		/// Gets or sets the Language list for the Language ISO 2 digit code and the associated display name of the Concept.
		/// </summary>
		[Display(Name = "ConceptNames", ResourceType = typeof(Locale))]
		public List<ConceptNameViewModel> Languages { get; set; }

		/// <summary>
		/// Gets or sets the concept list from the search parameters from the ajax search method
		/// </summary>
		public List<SelectListItem> ReferenceTermList { get; set; }

		/// <summary>
		/// Gets or sets the relationship.
		/// </summary>
		/// <value>The relationship.</value>
		[Display(Name = "Relationship", ResourceType = typeof(Locale))]
		public string RelationshipType { get; set; }

		/// <summary>
		/// Gets or sets the concept relationship list.
		/// </summary>
		/// <value>The concept relationship list.</value>
		public List<SelectListItem> RelationshipTypeList { get; set; }

		/// <summary>
		/// Checks if a reference term and relationship have been selected
		/// </summary>
		/// <returns>Returns true if a reference term is to be added, false to ignore the action.</returns>
		public bool HasAddReferenceTerm()
		{
			if (string.IsNullOrWhiteSpace(AddReferenceTerm) && string.IsNullOrWhiteSpace(RelationshipType)) return false;

			return !string.IsNullOrWhiteSpace(AddReferenceTerm) || !string.IsNullOrWhiteSpace(RelationshipType);
		}

		/// <summary>
		/// Converts an <see cref="EditConceptModel"/> instance to a <see cref="Concept"/> instance.
		/// </summary>
		/// <param name="imsiServiceClient">The ImsiServiceClient instance.</param>
		/// <param name="concept">The concept.</param>
		/// <returns>Returns the converted concept instance.</returns>
		public Concept ToEditConceptModel(ImsiServiceClient imsiServiceClient, Concept concept)
		{
			concept.CreationTime = DateTimeOffset.Now;
			concept.VersionKey = null;

			if (!string.Equals(this.ConceptClass, concept.ClassKey.ToString()))
			{
				concept.Class = new ConceptClass
				{
					Key = Guid.Parse(this.ConceptClass)
				};
			}

			concept.Mnemonic = this.Mnemonic;

			if (string.IsNullOrWhiteSpace(AddReferenceTerm) || string.IsNullOrWhiteSpace(RelationshipType)) return concept;

			Guid id, relationshipKey;

			if (Guid.TryParse(AddReferenceTerm, out id) && Guid.TryParse(RelationshipType, out relationshipKey))
			{
				var term = imsiServiceClient.Get<ReferenceTerm>(id, null) as ReferenceTerm;
				if (term != null) concept.ReferenceTerms.Add(new ConceptReferenceTerm(term.Key, relationshipKey));
			}

			return concept;
		}
	}
}