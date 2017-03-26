using OpenIZ.Core.Model.AMI.Auth;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.PolicyModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

		/// <summary>
		/// Initializes a new instance of the <see cref="EditApplicationModel"/> class.
		/// </summary>
		/// <param name="securityApplicationInfo">The security application information.</param>
		public EditApplicationModel(SecurityApplicationInfo securityApplicationInfo) : this()
		{
			this.ApplicationName = securityApplicationInfo.Application.Name;
			this.ApplicationPolicies = securityApplicationInfo.Policies.Select(p => new PolicyViewModel(p)).OrderBy(p => p.Name).ToList();
			this.CreationTime = securityApplicationInfo.Application.CreationTime.DateTime;
			this.HasPolicies = this.ApplicationPolicies.Any();
			this.Id = securityApplicationInfo.Id.Value;
			this.Policies = this.ApplicationPolicies.Select(p => p.Id.ToString()).ToList();
			this.IsObsolete = securityApplicationInfo.Application.ObsoletionTime != null;
		}

		/// <summary>
		/// Gets or sets the add policies.
		/// </summary>
		/// <value>The add policies.</value>
		[Display(Name = "AddPolicies", ResourceType = typeof(Locale))]
		public List<string> AddPolicies { get; set; }

		/// <summary>
		/// Gets or sets the name of the application.
		/// </summary>
		/// <value>The name of the application.</value>
		[Display(Name = "ApplicationName", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(64, ErrorMessageResourceName = "NameLength64", ErrorMessageResourceType = typeof(Locale))]
		public string ApplicationName { get; set; }

		/// <summary>
		/// Gets or sets the application policies.
		/// </summary>
		/// <value>The application policies.</value>
		public IEnumerable<PolicyViewModel> ApplicationPolicies { get; set; }

		/// <summary>
		/// Gets or sets the creation time.
		/// </summary>
		/// <value>The creation time.</value>
		[Display(Name = "CreationTime", ResourceType = typeof(Locale))]
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance has policies.
		/// </summary>
		/// <value><c>true</c> if this instance has policies; otherwise, <c>false</c>.</value>
		[Display(Name = "HasPolicies", ResourceType = typeof(Locale))]
		public bool HasPolicies { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[Required]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets whether the application is obsolete.
		/// </summary>
		public bool IsObsolete { get; set; }

		/// <summary>
		/// Gets or sets the policies.
		/// </summary>
		/// <value>The policies.</value>
		[Display(Name = "Policies", ResourceType = typeof(Locale))]
		public List<string> Policies { get; set; }

		/// <summary>
		/// Gets or sets the policies list.
		/// </summary>
		/// <value>The policies list.</value>
		public List<SelectListItem> PoliciesList { get; set; }
	}
}