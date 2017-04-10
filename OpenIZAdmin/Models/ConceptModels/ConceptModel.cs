using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Models.ConceptModels
{
    /// <summary>
    /// Represents a model of a concept.
    /// </summary>
    public abstract class ConceptModel
    {
        ///// <summary>
        ///// Initializes a new instance of the <see cref="ConceptModel"/> class.
        ///// </summary>
        //public abstract ConceptModel()
        //{
            
        //}

        /// <summary>
        /// Gets or sets the creation time of the concept.
        /// </summary>
        [Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
        public DateTime CreationTime { get; set; }

        /// <summary>
		/// Gets or sets the key of the concept.
		/// </summary>
		public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the readonly property of the Concept
        /// </summary>
        public bool IsSystemConcept { get; set; }

        /// <summary>
		/// Gets or sets the mnemonic of the concept.
		/// </summary>
		[Display(Name = "Mnemonic", ResourceType = typeof(Localization.Locale))]
        public virtual string Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets a list of names associated with the concept.
        /// </summary>
        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        public List<string> Names { get; set; }

        /// <summary>
        /// Gets or sets the list of reference terms associated with the concept.
        /// </summary>
        [Display(Name = "ReferenceTerms", ResourceType = typeof(Localization.Locale))]
        public List<ReferenceTermModel> ReferenceTerms { get; set; }
    }
}