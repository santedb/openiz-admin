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
using OpenIZAdmin.Models.Core;

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
			CreationTime = conceptSet.CreationTime.DateTime;
			Id = conceptSet.Key ?? Guid.Empty;
			Mnemonic = conceptSet.Mnemonic;
			Name = conceptSet.Name;
			Oid = conceptSet.Oid;
			Url = conceptSet.Url;
		}

		/// <summary>
		/// Gets or sets the list of Concepts to add
		/// </summary>
		/// <value>The add concepts.</value>
		[Display(Name = "AddConcepts", ResourceType = typeof(Locale))]
		public List<string> AddConcepts { get; set; }

		/// <summary>
		/// Gets or sets the concept list from the search parameters from the ajax search method
		/// </summary>
		/// <value>The concept list.</value>
		public List<SelectListItem> ConceptList { get; set; }

		/// <summary>
		/// Gets or sets the mnemonic of the concept.
		/// </summary>
		[Display(Name = "Mnemonic", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "MnemonicRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(64, ErrorMessageResourceName = "MnemonicLength64", ErrorMessageResourceType = typeof(Localization.Locale))]
        public override string Mnemonic { get; set; }

		/// <summary>
		/// Gets or sets the name of the concept set.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(64, ErrorMessageResourceName = "NameLength50", ErrorMessageResourceType = typeof(Localization.Locale))]
        public override string Name { get; set; }

		/// <summary>
		/// Gets or sets the oid.
		/// </summary>
		/// <value>The oid.</value>
		[Display(Name = "Oid", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "OidRequired", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(64, ErrorMessageResourceName = "OidLength64", ErrorMessageResourceType = typeof(Locale))]
        public override string Oid { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		[Display(Name = "Url", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "UrlRequired", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(64, ErrorMessageResourceName = "UrlLength256", ErrorMessageResourceType = typeof(Locale))]
        public override string Url { get; set; }

		/// <summary>
		/// Converts an <see cref="EditConceptSetModel" /> instance to a <see cref="ConceptSet" /> instance.
		/// </summary>
		/// <param name="conceptSet">The concept set.</param>
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

        /// <summary>
        /// Checks of the selected concept is already in the concept set list
        /// </summary>
        /// <returns>Returns true if the selected concept exists, false if not found</returns>
        public bool HasSelectedConcept(ConceptSet conceptSet)
        {
            return AddConcepts.Any() && conceptSet.Concepts.Any(c => c.Key.ToString().Equals(AddConcepts[0]));
        }
    }
}