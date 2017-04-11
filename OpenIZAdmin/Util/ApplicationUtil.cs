using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Models.ApplicationModels;
using System.Linq;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing applications.
	/// </summary>
	public static class ApplicationUtil
	{
        /// <summary>
        /// Converts a <see cref="OpenIZAdmin.Models.ApplicationModels.EditApplicationModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityApplicationInfo"/>
        /// </summary>
        /// <param name="amiClient">The <see cref="AmiServiceClient"/> instance.</param>
        /// <param name="model">The edit device model to convert.</param>
        /// <param name="appInfo">The device object to apply the changes to.</param>        
        /// <returns>Returns a security device info object.</returns>
        public static SecurityApplicationInfo ToSecurityApplicationInfo(AmiServiceClient amiClient, EditApplicationModel model, SecurityApplicationInfo appInfo)
		{
			appInfo.Application.Key = model.Id;
			appInfo.Id = model.Id;
			appInfo.Application.Name = model.ApplicationName;
			appInfo.Name = model.ApplicationName;

			var policyList = CommonUtil.GetNewPolicies(amiClient, model.Policies);

			if (policyList.Any())
			{
				appInfo.Policies.Clear();
				appInfo.Policies.AddRange(policyList.Select(p => new SecurityPolicyInfo(p)
				{
					Grant = PolicyGrantType.Grant
				}));
			}

			return appInfo;
		}
	}
}