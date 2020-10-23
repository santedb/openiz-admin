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
 * User: khannan
 * Date: 2017-3-27
 */

using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;
using OpenIZAdmin.Services.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OpenIZAdmin.Models.IntegrationModels
{
    /// <summary>
    /// Represents an entity relationship view model.
    /// </summary>
    public class ActParticipationViewModel : IdentifiedViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActParticipationViewModel"/> class.
        /// </summary>
        public ActParticipationViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActParticipationViewModel"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public ActParticipationViewModel(Guid id) : base(id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActParticipationViewModel" /> class.
        /// </summary>
        /// <param name="participation">The entity relationship.</param>
        /// <param name="isInverse">if set to <c>true</c> the relationship is inverse, which means that the target of the relationship is the entity instead of the source.</param>
        public ActParticipationViewModel(ActParticipation participation)
        {
            this.Id = participation.Key.Value;
            this.Quantity = participation.Quantity;

            this.ParticipationTypeName = participation.ParticipationRole != null ? string.Join(", ", participation.ParticipationRole.ConceptNames.Select(c => c.Name)) : Constants.NotApplicable;

            this.ActName = participation.SourceEntity != null ? string.Join(" ", participation.SourceEntity.TypeConcept?.ConceptNames.Select(c => c.Name)) : Constants.NotApplicable;
            this.ActTypeConcept = participation.SourceEntity?.TypeConcept != null ? string.Join(", ", participation.SourceEntity.TypeConcept.ConceptNames.Select(c => c.Name)) : Constants.NotApplicable;

            this.PlayerId = participation.PlayerEntityKey;

            this.PlayerName = participation.PlayerEntity != null ? string.Join(" ", participation.PlayerEntity.Names.SelectMany(n => n.Component).Select(c => c.Value)) : Constants.NotApplicable;
            this.PlayerTypeConcept = participation.PlayerEntity?.TypeConcept != null ? string.Join(", ", participation.PlayerEntity.TypeConcept.ConceptNames.Select(c => c.Name)) : Constants.NotApplicable;
            this.PlayerType = participation.PlayerEntity?.Type;
            this.PlayerIssues = participation.PlayerEntity?.Extensions?.Any(o => o.ExtensionTypeKey == Constants.DetectedIssueExtensionTypeKey) == true;
        }


        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        [Display(Name = "Quantity", ResourceType = typeof(Locale))]
        public int? Quantity { get; set; }

        /// <summary>
        /// Gets or sets the name of the relationship type.
        /// </summary>
        /// <value>The name of the relationship type.</value>
        [Display(Name = "RelationshipType", ResourceType = typeof(Locale))]
        public string ParticipationTypeName { get; set; }

        /// <summary>
        /// Gets or sets the name of the source.
        /// </summary>
        /// <value>The name of the source.</value>
        [Display(Name = "Name", ResourceType = typeof(Locale))]
        public string ActName { get; set; }

        /// <summary>
        /// Gets or sets the source type concept.
        /// </summary>
        /// <value>The source type concept.</value>
        [Display(Name = "Type", ResourceType = typeof(Locale))]
        public string ActTypeConcept { get; set; }

        /// <summary>
        /// Gets or sets the player identifier
        /// </summary>
        public Guid? PlayerId { get; }

        /// <summary>
        /// Gets or sets the name of the target.
        /// </summary>
        /// <value>The name of the target.</value>
        [Display(Name = "Name", ResourceType = typeof(Locale))]
        public string PlayerName { get; set; }

        /// <summary>
        /// Gets or sets the target type concept.
        /// </summary>
        /// <value>The target type concept.</value>
        [Display(Name = "Type", ResourceType = typeof(Locale))]
        public string PlayerTypeConcept { get; set; }

        /// <summary>
        /// Gets or sets the type of player
        /// </summary>
        public string PlayerType { get; }

        /// <summary>
        /// True if the player has detected issues
        /// </summary>
        public bool PlayerIssues { get; set; }
    }
}