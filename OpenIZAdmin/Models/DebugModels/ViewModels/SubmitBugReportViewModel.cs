using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Models.DebugModels.ViewModels
{
    public class SubmitBugReportViewModel
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="SubmitBugReportViewModel"/> class.
		/// </summary>
		public SubmitBugReportViewModel()
        {
        }
        
        public bool AttachBugInfo { get; set; }

        /// <summary>
        /// Gets or sets the description of the role.
        /// </summary>        
        [Display(Name = "StepsToReproduce", ResourceType = typeof(Localization.Locale))]
        [StringLength(4000, ErrorMessageResourceName = "DescriptionTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string BugDetails { get; set; }

        [Required]
        public Guid Key { get; set; }

        public string Reporter { get; set; }

        public bool Success { get; set; }

        public string TransactionMessage { get; set; }
    }
}