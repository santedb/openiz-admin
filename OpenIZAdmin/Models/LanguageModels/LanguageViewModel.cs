using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Models.ConceptModels;

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
            this.Languages = new List<Language>();
            this.LanguageList = new List<SelectListItem>();            
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="LanguageViewModel"/> class.
		/// </summary>
		/// <param name="concept">The concept.</param>
		public LanguageViewModel(Concept concept) : this()
		{
            this.ConceptClass = concept.Class.Name;            
            this.ConceptId = concept.Key.Value;
            this.Languages = concept.ConceptNames.Select(k => new Language(k.Language, k.Name)).ToList();
        }

        /// <summary>
        /// Gets or sets the concept class.
        /// </summary>
        /// <value>The concept class.</value>        
        public string ConceptClass { get; set; }

        //public Guid? ConceptId { get; set; }

        /// <summary>
        /// Gets or sets the concept class list.
        /// </summary>
        /// <value>The concept class list.</value>
        public List<SelectListItem> ConceptClassList { get; set; }

        //public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        [Display(Name = "Language", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "LanguageRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        public override string Language { get; set; }       

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
        public override string Name { get; set; }

        /// <summary>
        /// Gets or sets the two character language code
        /// </summary>
        public List<string> TwoLetterCountryCodeList { get; set; }

        ///// <summary>
        ///// Converts an <see cref="CreateConceptModel"/> instance to a <see cref="Concept"/> instance.
        ///// </summary>
        ///// <returns>Returns a concept instance.</returns>
        //public Concept ToConcept()
        //{
        //    return new Concept
        //    {
        //        Class = new ConceptClass
        //        {
        //            Key = Guid.Parse(this.ConceptClass)
        //        },
        //        ConceptNames = new List<ConceptName>
        //        {
        //            new ConceptName
        //            {
        //                Language = this.Language,
        //                Name = this.Name
        //            }
        //        },
        //        Key = Guid.NewGuid(),
        //        Mnemonic = this.Mnemonic,
        //    };
        //}
    }
}