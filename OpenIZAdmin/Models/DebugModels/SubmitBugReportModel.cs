using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Diagnostics;
using OpenIZ.Core.Model.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Models.DebugModels
{
	/// <summary>
	/// Represents a submit bug report model.
	/// </summary>
	public class SubmitBugReportModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SubmitBugReportModel"/> class.
		/// </summary>
		public SubmitBugReportModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SubmitBugReportModel"/> class
		/// with a specific <see cref="SecurityUserInfo"/> instance.
		/// </summary>
		/// <param name="securityUserInfo">The <see cref="SecurityUserInfo"/> instance.</param>
		public SubmitBugReportModel(SecurityUserInfo securityUserInfo)
		{
			this.AttachBugInfo = true;
			this.Id = securityUserInfo.UserId.Value;
			this.Reporter = securityUserInfo.UserName;
			this.Success = false;
		}

		/// <summary>
		/// Gets or sets whether to attach the metadata for the issue
		/// </summary>
		[Display(Name = "AttachBugInfo", ResourceType = typeof(Localization.Locale))]
		public bool AttachBugInfo { get; set; }

		/// <summary>
		/// Gets or sets the description of the role.
		/// </summary>
		[Display(Name = "StepsToReproduce", ResourceType = typeof(Localization.Locale))]
		[StringLength(4000, ErrorMessageResourceName = "StepsToReproduceTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "StepsToReproduceRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
        public string BugDetails { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[Required]
		public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the reporter.
        /// </summary>
        /// <value>The reporter.</value>       
        [Display(Name = "Reporter", ResourceType = typeof(Localization.Locale))] 
        public string Reporter { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SubmitBugReportModel"/> is success.
		/// </summary>
		/// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
		public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the transaction message.
        /// </summary>
        /// <value>The transaction message.</value>
        //[StringLength(256, ErrorMessageResourceName = "TransactionLength256", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string TransactionMessage { get; set; }

		/// <summary>
		/// Converts a <see cref="SubmitBugReportModel"/> instance to a <see cref="DiagnosticReport"/> instance.
		/// </summary>
		/// <param name="submitter">The submitter.</param>
		/// <returns>Returns a diagnostic report instance.</returns>
		public DiagnosticReport ToDiagnosticReport(UserEntity submitter)
		{
			return new DiagnosticReport
			{
				ApplicationInfo = new DiagnosticApplicationInfo(typeof(MvcApplication).Assembly),
				Submitter = submitter,
				Note = this.BugDetails
			};
		}
	}
}