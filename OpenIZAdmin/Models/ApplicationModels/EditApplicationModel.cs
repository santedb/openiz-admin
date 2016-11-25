using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Models.PolicyModels.ViewModels;
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
            //this.PoliciesList = new List<SelectListItem>();
        }

        //policies added by the user
        [Display(Name = "AddPolicies", ResourceType = typeof(Localization.Locale))]
        //[Required(ErrorMessageResourceName = "RolesRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        public IEnumerable<string> AddPoliciesList { get; set; }

        public IEnumerable<PolicyViewModel> ApplicationPolicies { get; set; }

        [Display(Name = "ApplicationId", ResourceType = typeof(Localization.Locale))]
        public string ApplicationId { get; set; }

        [Display(Name = "ApplicationName", ResourceType = typeof(Localization.Locale))]
        public string ApplicationName { get; set; }

        [Display(Name = "ApplicationSecret", ResourceType = typeof(Localization.Locale))]
        public string ApplicationSecret { get; set; }

        [Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
        public DateTime CreationTime { get; set; }

        //public string EntityVersionId { get; set; }

        //[Display(Name = "HasPolicies", ResourceType = typeof(Localization.Locale))]
        //public bool HasPolicies { get; set; }

        [Required]
        public Guid Id { get; set; }        

        //policies autopopulate
        //public List<SelectListItem> PoliciesList { get; set; }

        public List<SecurityPolicyInstance> Policies { get; set; }

        /// <summary>
        /// Gets or sets a list of policies associated with the application.
        /// </summary>
        //public List<Guid> Policies { get; set; }

        /// <summary>
        /// Gets or sets a list of policies assocated with the application.
        /// </summary>
        public List<SelectListItem> PolicyList { get; set; }
    }
}