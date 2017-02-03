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
		/// Queries for a specific device by device key
		/// </summary>
		/// <param name="client">The AMI service client</param>
		/// /// <param name="id">The application identifier key </param>
		/// <returns>Returns application object, null if not found</returns>
		public static SecurityApplicationInfo GetApplication(AmiServiceClient client, string id)
		{
			return client.GetApplication(id);
		}

		/// <summary>
		/// Queries for a specific device by device key
		/// </summary>
		/// <param name="client">The AMI service client</param>
		/// /// <param name="key">The application identifier key </param>
		/// <returns>Returns application object, null if not found</returns>
		public static SecurityApplicationInfo GetApplication(AmiServiceClient client, Guid? key)
		{
			return client.GetApplication(key.ToString());
		}

		/// <summary>
		/// Queries for a specific device by device key
		/// </summary>
		/// <param name="client">The AMI service client</param>
		/// /// <param name="id">The application identifier key </param>
		/// <returns>Returns application object, null if not found</returns>
		public static SecurityPolicy GetPolicy(AmiServiceClient client, string id)
		{
			Guid appKey = Guid.Empty;
			if (CommonUtil.IsValidString(id) && Guid.TryParse(id, out appKey))
			{
				var policyInfo = PolicyUtil.GetPolicy(client, appKey);

				if (policyInfo != null && policyInfo.Policy != null)
				{
					return policyInfo.Policy;
				}
			}

			return null;
		}

		/// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityApplicationInfo"/> to a <see cref="OpenIZAdmin.Models.ApplicationModels.ViewModels.ApplicationViewModel"/>.
		/// </summary>
		/// <param name="appInfo">The SecurityApplicationInfo to convert.</param>
		/// <returns>Returns a ApplicationViewModel model.</returns>
		public static ApplicationViewModel ToApplicationViewModel(SecurityApplicationInfo appInfo)
		{
			var viewModel = new ApplicationViewModel
			{
				Id = appInfo.Id.Value,
				ApplicationName = appInfo.Name,
				CreationTime = appInfo.Application.CreationTime.DateTime,
				HasPolicies = CommonUtil.IsPolicy(appInfo.Policies),
				IsObsolete = appInfo.Application.ObsoletionTime != null,
				Policies = appInfo.Policies?.Select(PolicyUtil.ToPolicyViewModel).OrderBy(q => q.Name).ToList() ?? new List<PolicyViewModel>()
			};

			return viewModel;
		}

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
				ApplicationPolicies = appInfo.Policies?.Select(PolicyUtil.ToPolicyViewModel).OrderBy(q => q.Name).ToList() ?? new List<PolicyViewModel>()
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
		/// Converts a <see cref="OpenIZAdmin.Models.ApplicationModels.CreateApplicationModel"/> to a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityApplicationInfo"/>.
		/// </summary>
		/// <param name="model">The CreateApplicationModel to convert.</param>
		/// <returns>Returns a security application object.</returns>
		public static SecurityApplicationInfo ToSecurityApplication(CreateApplicationModel model)
		{
			SecurityApplicationInfo appInfo = new SecurityApplicationInfo();
			appInfo.Application = new SecurityApplication();
			Guid appGuid = Guid.NewGuid();

			appInfo.Application.Key = appGuid;
			appInfo.Application.Name = model.ApplicationName;
			appInfo.Application.ApplicationSecret = model.ApplicationSecret;

			appInfo.Id = appGuid;
			appInfo.Name = model.ApplicationName;
			appInfo.ApplicationSecret = model.ApplicationSecret;

			return appInfo;
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