using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Diagnostics;
using OpenIZ.Core.Model.Entities;
using System;
using System.ComponentModel.DataAnnotations;

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

		[Display(Name = "AttachBugInfo", ResourceType = typeof(Localization.Locale))]
		public bool AttachBugInfo { get; set; }

		/// <summary>
		/// Gets or sets the description of the role.
		/// </summary>
		[Display(Name = "StepsToReproduce", ResourceType = typeof(Localization.Locale))]
		[StringLength(4000, ErrorMessageResourceName = "StepsToReproduceTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
		public string BugDetails { get; set; }

		[Required]
		public Guid Id { get; set; }

		public string Reporter { get; set; }

		public bool Success { get; set; }

		public string TransactionMessage { get; set; }

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