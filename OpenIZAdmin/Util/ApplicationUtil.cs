using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Models.ApplicationModels;
using OpenIZAdmin.Models.ApplicationModels.ViewModels;
using OpenIZAdmin.Models.PolicyModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing applications.
	/// </summary>
	public static class ApplicationUtil
	{
		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityApplicationInfo"/> to a <see cref="OpenIZAdmin.Models.ApplicationModels.EditApplicationModel"/>
		/// </summary>
		/// <param name="client">The Ami Service Client.</param>
		/// /// <param name="appInfo">The SecurityApplicationInfo object to convert to a EditApplicationModel.</param>
		/// <returns>Returns a edit application object.</returns>
		public static EditApplicationModel ToEditApplicationModel(AmiServiceClient client, SecurityApplicationInfo appInfo)
		{
			var viewModel = new EditApplicationModel
			{
				Id = appInfo.Id.Value,
				ApplicationName = appInfo.Name,
				CreationTime = appInfo.Application.CreationTime.DateTime,
				ApplicationPolicies = appInfo.Policies.Select(p => new PolicyViewModel(p)).OrderBy(q => q.Name).ToList()
			};

			if (viewModel.ApplicationPolicies.Any())
			{
				viewModel.Policies = viewModel.ApplicationPolicies.Select(p => p.Key.ToString()).ToList();
			}

			viewModel.PoliciesList.Add(new SelectListItem { Text = "", Value = "" });
			viewModel.PoliciesList.AddRange(CommonUtil.GetAllPolicies(client).Select(r => new SelectListItem { Text = r.Name, Value = r.Key.ToString() }));

			return viewModel;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZAdmin.Models.ApplicationModels.EditApplicationModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityApplicationInfo"/>
		/// </summary>
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
				appInfo.Policies.AddRange(policyList.Select(p => new SecurityPolicyInstance(p, PolicyGrantType.Grant)));
			}

			return appInfo;
		}
	}
}