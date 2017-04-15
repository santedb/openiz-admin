using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.ReferenceTermModels
{
    /// <summary>
	/// Represents a view model for a reference term search result.
	/// </summary>
    public class ReferenceTermSearchResultsViewModel : ReferenceTermModel
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="ConceptSearchResultViewModel"/> class.
		/// </summary>
		public ReferenceTermSearchResultsViewModel()
        {
            this.DisplayNames = new List<ReferenceTermName>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTermSearchResultsViewModel"/> class
        /// with a specific <see cref="Concept"/> instance.
        /// </summary>
        /// <param name="referenceTerm">The <see cref="ReferenceTerm"/> instance.</param>
        public ReferenceTermSearchResultsViewModel(ReferenceTerm referenceTerm) : this()
        {
            CreationTime = referenceTerm.CreationTime.DateTime;
            DisplayNames = referenceTerm.DisplayNames;
            Id = referenceTerm.Key ?? Guid.Empty;
            Mnemonic = referenceTerm.Mnemonic;            
            Names = (DisplayNames.Any()) ? string.Join(", ", DisplayNames) : string.Empty;
        }

        /// <summary>
        /// Gets or sets the creation time of the concept.
        /// </summary>
        [Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
        public DateTime CreationTime { get; set; }


        /// <summary>
        /// Gets or sets the concatenated display names of the Reference Term
        /// </summary>
        [Display(Name = "Names", ResourceType = typeof(Localization.Locale))]
        public string Names { get; set; }
    }
}