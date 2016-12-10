using OpenIZAdmin.Models.PolicyModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
			this.Policies = new List<string>();
			this.PoliciesList = new List<SelectListItem>();
		}

		//policies added by the user
		[Display(Name = "AddPolicies", ResourceType = typeof(Localization.Locale))]
		public List<string> AddPolicies { get; set; }

		[Display(Name = "ApplicationName", ResourceType = typeof(Localization.Locale))]
		public string ApplicationName { get; set; }

		public IEnumerable<PolicyViewModel> ApplicationPolicies { get; set; }

		[Display(Name = "ApplicationSecret", ResourceType = typeof(Localization.Locale))]
		public string ApplicationSecret { get; set; }

		[Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
		public DateTime CreationTime { get; set; }

		[Display(Name = "HasPolicies", ResourceType = typeof(Localization.Locale))]
		public bool HasPolicies { get; set; }

		[Required]
		public Guid Id { get; set; }

		[Display(Name = "Policies", ResourceType = typeof(Localization.Locale))]
		public List<string> Policies { get; set; }

		//policies autopopulate
		public List<SelectListItem> PoliciesList { get; set; }
	}
}