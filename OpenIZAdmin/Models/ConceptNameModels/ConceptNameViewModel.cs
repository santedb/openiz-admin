using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.ConceptNameModels
{
    /// <summary>
    /// Represents a view model for a language search result.
    /// </summary>
    public class ConceptNameViewModel : LanguageModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptNameViewModel"/> class.
        /// </summary>
        public ConceptNameViewModel()
        {
            this.Languages = new List<Language>();
            this.LanguageList = new List<SelectListItem>();            
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="ConceptNameViewModel"/> class.
		/// </summary>
		/// <param name="concept">The concept.</param>
		public ConceptNameViewModel(Concept concept) : this()
		{
            this.ConceptClass = concept.Class?.Name;            
            this.ConceptId = concept.Key ?? Guid.Empty;
		    this.ConceptVersionKey = concept.VersionKey;
            this.Languages = concept.ConceptNames.Select(k => new Language(k.Language, k.Name)).ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Language"/> class
        /// with a specified code and display name.
        /// </summary>
        /// <param name="code">The language code.</param>
        /// <param name="displayName">The language display name.</param>        
        /// <param name="concept">The Concept instance</param>
        public ConceptNameViewModel(string code, string displayName, Concept concept) : this(concept)
        {
            this.DisplayName = displayName;            
            this.Name = displayName;
            this.TwoLetterCountryCode = code;
            this.Language = code;
        }

        /// <summary>
        /// Gets or sets the concept class.
        /// </summary>
        /// <value>The concept class.</value>        
        public string ConceptClass { get; set; }        

        /// <summary>
        /// Gets or sets the concept class list.
        /// </summary>
        /// <value>The concept class list.</value>
        public List<SelectListItem> ConceptClassList { get; set; }          

        /// <summary>
        /// Gets or sets the mnemonic.
        /// </summary>
        /// <value>The mnemonic.</value>
        [Display(Name = "Mnemonic", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "MnemonicRequired", ErrorMessageResourceType = typeof(Locale))]
        [StringLength(255, ErrorMessageResourceName = "MnemonicTooLong", ErrorMessageResourceType = typeof(Locale))]
        public string Mnemonic { get; set; }       

        /// <summary>
        /// Gets or sets the two character language code
        /// </summary>
        public List<string> TwoLetterCountryCodeList { get; set; }       
    }
}