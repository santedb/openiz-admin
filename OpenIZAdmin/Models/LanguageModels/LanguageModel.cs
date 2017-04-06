using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.LanguageModels
{
    public class LanguageModel
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="LanguageModel"/> class.
		/// </summary>
		public LanguageModel()
        {
            this.Languages = new List<Language>();
            this.LanguageList = new List<SelectListItem>();
            //this.ConceptClassList = new List<SelectListItem>();
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="LanguageModel"/> class.
		/// </summary>
		/// <param name="concept">The concept.</param>
		public LanguageModel(Concept concept) : this()
		{
            this.ConceptClass = concept.Class.Name;
            //this.CreationTime = concept.CreationTime.DateTime;
            this.ConceptId = concept.Key.Value;
            this.Languages = concept.ConceptNames.Select(k => new Language(k.Language, k.Name)).ToList();
        }

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
        public List<SelectListItem> ConceptClassList { get; set; }

        public string DisplayName { get; set; }

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
        [Display(Name = "Mnemonic", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "MnemonicRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(255, ErrorMessageResourceName = "MnemonicTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(255, ErrorMessageResourceName = "NameLength255", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Name { get; set; }

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