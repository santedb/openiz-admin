using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Diagnostics;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Models.DebugModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Util
{
    public static class HomeUtil
    {
        /// <summary>
		/// Converts a <see cref="SubmitBugReportViewModel"/> to a <see cref="SecurityPolicyInfo"/>.
		/// </summary>
		/// <param name="userInfo">The current user info.</param>
		/// <returns>Returns a SubmitBugReportViewModel model.</returns>
		public static SubmitBugReportViewModel ToSubmitBugReport(SecurityUserInfo userInfo)
        {
            var viewModel = new SubmitBugReportViewModel
            {
                Reporter = userInfo.UserName,
                AttachBugInfo = true,
                Key = (Guid)userInfo.UserId                
            };
            
            return viewModel;
        }


        /// <summary>
		/// Converts a <see cref="SubmitBugReportViewModel"/> to a <see cref="SecurityPolicyInfo"/>.
		/// </summary>
		/// <param name="model">The bug report model.</param>
		/// <returns>Returns a DiagnosticReport object.</returns>
		public static DiagnosticReport ToDiagnosticReport(ImsiServiceClient imsiClient, SubmitBugReportViewModel model)
        {
            var report = new DiagnosticReport();

            var userEntity = UserUtil.GetUserEntity(imsiClient, model.Key);
            if(userEntity != null)
            {
                report.CreatedBy = userEntity.SecurityUser;
                report.Submitter = userEntity;
                report.Note = model.BugDetails;
                DiagnosticApplicationInfo info = new DiagnosticApplicationInfo(typeof(MvcApplication).Assembly);
                report.ApplicationInfo = info;

                return report;
            }
            else
            {
                return null;
            }            
        }        
    }
}