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
 * User: Andrew
 * Date: 2017-5-16
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.ReferenceTermModels;

namespace OpenIZAdmin.Models.AssigningAuthorityModels
{
	/// <summary>
	/// Represents a model of an authority scope.
	/// </summary>
	public class AuthorityScopeViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorityScopeViewModel"/> class.
		/// </summary>
		public AuthorityScopeViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptViewModel"/> class.
		/// </summary>
		/// <param name="concept">The concept.</param>
		public AuthorityScopeViewModel(Concept concept)
		{
			this.Class = concept.Class?.Name;
			this.AssigingAuthorityId = Guid.Empty;
			this.Id = concept.Key ?? Guid.Empty;
			this.Mnemonic = concept.Mnemonic;
			this.Name = string.Join(", ", concept.ConceptNames.Select(c => c.Name));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptViewModel"/> class.
		/// </summary>
		/// <param name="concept">The concept.</param>
		/// <param name="assigningAuthorityId">The guid of the Assigning Authority that the Concept is associated with</param>
		public AuthorityScopeViewModel(Concept concept, Guid assigningAuthorityId) : this(concept)
		{
			this.AssigingAuthorityId = assigningAuthorityId;
		}

		/// <summary>
		/// Gets or sets the class of the concept.
		/// </summary>
		[Display(Name = "ConceptClass", ResourceType = typeof(Locale))]
		public string Class { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the mnemonic.
		/// </summary>
		/// <value>The mnemonic.</value>
		[Display(Name = "Mnemonic", ResourceType =  typeof(Locale))]
		public string Mnemonic { get; set; }

		/// <summary>
		/// Gets or sets the names.
		/// </summary>
		/// <value>The names.</value>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Concept Set identifier associated with the Concept instance
		/// </summary>
		public Guid? AssigingAuthorityId { get; set; }
	}
}