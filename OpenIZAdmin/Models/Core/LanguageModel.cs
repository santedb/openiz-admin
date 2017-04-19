using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.Core
{
    /// <summary>
    /// Provides the model for managing languages.
    /// </summary>
    public abstract class LanguageModel
    {        
        /// <summary>
        /// Gets or sets the Guid identifier of the Concept
        /// </summary>
        public Guid? ConceptId { get; set; }

        /// <summary>
        /// Gets or sets the Guid version identifier of the Concept
        /// </summary>
        public Guid? ConceptVersionKey { get; set; }

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
        [Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Language { get; set; }

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
        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(255, ErrorMessageResourceName = "NameLength255", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Name { get; set; }    

        /// <summary>
        /// Gets or sets the two letter language code of the language.
        /// </summary>
        [Display(Name = "Language", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string TwoLetterCountryCode { get; set; }
    }
}