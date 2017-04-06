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
            this.TwoLetterCountryCodeList = new List<string>() { Locale.EN };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageModel"/> class.
        /// </summary>
        /// <param name="concept">The concept.</param>
        public LanguageModel(Concept concept) : this()
        {
            this.ConceptClass = concept.Class.Name;            
            this.ConceptId = concept.Key.Value;
            this.Languages = concept.ConceptNames.Select(k => new Language(k.Language, k.Name)).ToList();
            TwoLetterCountryCode = Locale.EN;
            DisplayName = string.Empty;
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
            DisplayName = displayName;
            TwoLetterCountryCode = code;
            ConceptId = conceptId;
            TwoLetterCountryCodeList = new List<string>() { code };
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
            TwoLetterCountryCode = code;
            TwoLetterCountryCodeList = new List<string>() { code };
        }

        /// <summary>
        /// Gets or sets the display name of the language.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the two letter language code of the language.
        /// </summary>
        public string TwoLetterCountryCode { get; set; }

        /// <summary>
        /// Gets the Entity Identifier related to the Language entry
        /// </summary>
        public Guid? EntityId { get; }

       

        /// <summary>
        /// Gets or sets the concept class.
        /// </summary>
        /// <value>The concept class.</value>
        //[Display(Name = "ConceptClass", ResourceType = typeof(Localization.Locale))]
        //[Required(ErrorMessageResourceName = "ConceptClassRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string ConceptClass { get; set; }

        public Guid? ConceptId { get; set; }

        /// <summary>
        /// Gets or sets the concept class list.
        /// </summary>
        /// <value>The concept class list.</value>
        //public List<SelectListItem> ConceptClassList { get; set; }        

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        [Display(Name = "Language", ResourceType = typeof(Localization.Locale))]
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
        /// Gets or sets the mnemonic.
        /// </summary>
        /// <value>The mnemonic.</value>
        //[Display(Name = "Mnemonic", ResourceType = typeof(Localization.Locale))]
        //[Required(ErrorMessageResourceName = "MnemonicRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        //[StringLength(255, ErrorMessageResourceName = "MnemonicTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(255, ErrorMessageResourceName = "NameLength255", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> TwoLetterCountryCodeList { get; set; }

        /// <summary>
        /// Converts an <see cref="CreateConceptModel"/> instance to a <see cref="Concept"/> instance.
        /// </summary>
        /// <returns>Returns a concept instance.</returns>
        public Concept ToConcept()
        {
            return new Concept
            {
                Class = new ConceptClass
                {
                    Key = Guid.Parse(this.ConceptClass)
                },
                ConceptNames = new List<ConceptName>
                      {
                          new ConceptName
                          {
                              Language = this.Language,
                              Name = this.Name
                          }
                      },
                Key = Guid.NewGuid(),
                Mnemonic = this.Mnemonic,
            };
        }
    }
}