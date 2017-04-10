using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ConceptModels;

namespace OpenIZAdmin.Models.LanguageModels
{
    /// <summary>
    /// Provides the model for managing languages.
    /// </summary>
    public class LanguageModel
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="LanguageModel"/> class.
		/// </summary>
		public LanguageModel()
        {
            this.Languages = new List<Language>();
            this.LanguageList = new List<SelectListItem>();                
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageModel"/> class.
        /// </summary>
        /// <param name="concept">The concept.</param>
        public LanguageModel(Concept concept) : this()
        {            
            this.ConceptId = concept.Key ?? Guid.Empty;
            this.Languages = concept.ConceptNames.Select(k => new Language(k.Language, k.Name)).ToList();            
        }

        /// <summary>
	    /// Initializes a new instance of the <see cref="Language"/> class
	    /// with a specified code and display name.
	    /// </summary>
	    /// <param name="code">The language code.</param>
	    /// <param name="displayName">The language display name.</param>
	    /// <param name="conceptId">The identifier associated with the Concept</param>
	    public LanguageModel(string code, string displayName, Guid? conceptId) : this()
        {
            ConceptId = conceptId;
            DisplayName = displayName;
            Name = displayName;
            TwoLetterCountryCode = code;
            Language = code;
        }

        /// <summary>
	    /// Initializes a new instance of the <see cref="Language"/> class
	    /// with a specified code and display name.
	    /// </summary>
	    /// <param name="code">The language code.</param>
	    /// <param name="displayName">The language display name.</param>
	    /// <param name="concept">The Concept instance</param>
	    public LanguageModel(string code, string displayName, Concept concept) : this(concept)
        {
            DisplayName = displayName;
            Name = displayName;
            TwoLetterCountryCode = code;
            Language = code;
        }

        /// <summary>
        /// Gets or sets the Guid identifier of the Concept
        /// </summary>
        public Guid? ConceptId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the language.
        /// </summary>
        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(255, ErrorMessageResourceName = "NameLength255", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the current language.
        /// </summary>
        /// <value>The language.</value>        
        [Display(Name = "Languages", ResourceType = typeof(Localization.Locale))]
        public virtual string Language { get; set; }

        /// <summary>
        /// Gets or sets the language list.
        /// </summary>
        /// <value>The language list.</value>
        public List<SelectListItem> LanguageList { get; set; }

        /// <summary>
        /// Gets or sets the Language list for the Language ISO 2 digit code and the associated display name of the Concept.
        /// </summary>		
        [Display(Name = "Languages", ResourceType = typeof(Localization.Locale))]
        public List<Language> Languages { get; set; }

        /// <summary>
        /// Gets or sets the current name.
        /// </summary>
        /// <value>The name.</value>                
        public virtual string Name { get; set; }    

        /// <summary>
        /// Gets or sets the two letter language code of the language.
        /// </summary>
        [Display(Name = "Language", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string TwoLetterCountryCode { get; set; }
    }
}