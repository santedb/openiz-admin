using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Diagnostics;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Models.DebugModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenIZAdmin.Models.DebugModels;

namespace OpenIZAdmin.Util
{
    public static class HomeUtil
    {
		/// <summary>
		/// Converts a <see cref="SubmitBugReportModel"/> to a <see cref="SecurityPolicyInfo"/>.
		/// </summary>
		/// <param name="model">The bug report model.</param>
		/// <returns>Returns a DiagnosticReport object.</returns>
		public static DiagnosticReport ToDiagnosticReport(ImsiServiceClient imsiClient, SubmitBugReportModel model)
        {
            var report = new DiagnosticReport();

            var userEntity = UserUtil.GetUserEntityBySecurityUserKey(imsiClient, model.Key);
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