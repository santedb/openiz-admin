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
using OpenIZAdmin.Models.ConceptModels.ViewModels;

namespace OpenIZAdmin.Models.ConceptSetModels.ViewModels
{
	/// <summary>
	/// Represents a view model for a concept set.
	/// </summary>
	public class ConceptSetViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptSetViewModel"/> class.
		/// </summary>
		public ConceptSetViewModel()
		{
			this.Concepts = new List<ConceptViewModel>();
		}

		/// <summary>
		/// Gets or sets a list of concept of the concept set.
		/// </summary>
		public List<ConceptViewModel> Concepts { get; set; }

		/// <summary>
		/// Gets or sets the creation time of the concept set.
		/// </summary>
		[Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the key of the concept.
		/// </summary>
		public Guid Key { get; set; }

		/// <summary>
		/// Gets or sets the mnemonic of the concept.
		/// </summary>
		public string Mnemonic { get; set; }

		/// <summary>
		/// Gets or sets the name of the concept set.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Get or sets the OID of the concept set.
		/// </summary>
		public string Oid { get; set; }

		/// <summary>
		/// Get or sets the URL of the concept set.
		/// </summary>
		public string Url { get; set; }
	}
}