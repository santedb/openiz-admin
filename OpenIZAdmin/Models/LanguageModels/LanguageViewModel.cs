using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Models.ConceptModels;
using OpenIZAdmin.Models.Core;

namespace OpenIZAdmin.Models.LanguageModels
{
    /// <summary>
    /// Represents a view model for a language search result.
    /// </summary>
    public class LanguageViewModel : LanguageModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageViewModel"/> class.
        /// </summary>
        public LanguageViewModel()
        {
            Languages = new List<Language>();
            LanguageList = new List<SelectListItem>();            
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="LanguageViewModel"/> class.
		/// </summary>
		/// <param name="concept">The concept.</param>
		public LanguageViewModel(Concept concept) : this()
		{
            ConceptClass = concept.Class?.Name;            
            ConceptId = concept.Key ?? Guid.Empty;
		    ConceptVersionKey = concept.VersionKey;
            Languages = concept.ConceptNames.Select(k => new Language(k.Language, k.Name)).ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Language"/> class
        /// with a specified code and display name.
        /// </summary>
        /// <param name="code">The language code.</param>
        /// <param name="displayName">The language display name.</param>        
        /// <param name="concept">The Concept instance</param>
        public LanguageViewModel(string code, string displayName, Concept concept) : this(concept)
        {
            DisplayName = displayName;            
            Name = displayName;
            TwoLetterCountryCode = code;
            Language = code;
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
        [Display(Name = "Mnemonic", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "MnemonicRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(255, ErrorMessageResourceName = "MnemonicTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Mnemonic { get; set; }       

        /// <summary>
        /// Gets or sets the two character language code
        /// </summary>
        public List<string> TwoLetterCountryCodeList { get; set; }       
    }
}