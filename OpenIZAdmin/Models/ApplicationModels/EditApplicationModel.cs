using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.ApplicationModels
{
    public class EditApplicationModel
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="EditApplicationModel"/> class.
		/// </summary>
		public EditApplicationModel()
        {

        }

        /// <summary>
        /// Gets or sets the id of the application.
        /// </summary>
        //[Required]
        public string ApplicationId { get; set; }

        /// <summary>
		/// Gets or sets the name of the application.
		/// </summary>
		[Required]
        [Display(Name = "ApplicationName", ResourceType = typeof(Localization.Locale))]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the secret of the application.
        /// </summary>
        [Required]
        [Display(Name = "ApplicationSecret", ResourceType = typeof(Localization.Locale))]
        public string ApplicationSecret { get; set; }

        /// <summary>
        /// Gets or sets a list of policies associated with the application.
        /// </summary>
        public List<Guid> Policies { get; set; }

        /// <summary>
        /// Gets or sets a list of policies assocated with the application.
        /// </summary>
        public List<SelectListItem> PolicyList { get; set; }
    }
}