using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Models.ApplicationModels;
using OpenIZAdmin.Models.ApplicationModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using OpenIZAdmin.Models.PolicyModels.ViewModels;

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
            try
            {
                var result = client.GetApplication(id);

                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.TraceError("Unable to retrieve application: {0}", e.StackTrace);
#endif
                Trace.TraceError("Unable to retrieve application: {0}", e.Message);
            }

            return null;
        }

        /// <summary>
        /// Queries for a specific device by device key
        /// </summary>
        /// <param name="client">The AMI service client</param>        
        /// /// <param name="key">The application identifier key </param>        
        /// <returns>Returns application object, null if not found</returns>
        public static SecurityApplicationInfo GetApplication(AmiServiceClient client, Guid? key)
        {
            try
            {
                var result = client.GetApplications(r => r.Id == key);

                if (result != null)
                {
                    return result.CollectionItem.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.TraceError("Unable to retrieve application: {0}", e.StackTrace);
#endif
                Trace.TraceError("Unable to retrieve application: {0}", e.Message);
            }

            return null;
        }

        /// <summary>
        /// Gets the policy objects that have been selected to be added to a device
        /// </summary>
        /// <param name="client">The Ami Service Client.</param> 
        /// <param name="pList">The string list with selected policy names.</param>         
        /// <returns>Returns a list of application SecurityPolicy objects</returns>
        internal static List<SecurityPolicy> GetNewPolicies(AmiServiceClient client, IEnumerable<string> pList)
        {
            List<SecurityPolicy> policyList = new List<SecurityPolicy>();

            try
            {
                foreach (string name in pList)
                {
                    if (IsValidString(name))
                    {
                        //SecurityPolicyInfo result = DeviceUtil.GetPolicy(client, name);
                        var result = client.GetPolicies(r => r.Name == name);
                        if (result.CollectionItem.Count != 0)
                        {
                            SecurityPolicyInfo infoResult = result.CollectionItem.FirstOrDefault();
                            if (infoResult.Policy != null)
                            {
                                policyList.Add(infoResult.Policy);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.TraceError("Unable to retrieve policies: {0}", e.StackTrace);
#endif
                Trace.TraceError("Unable to retrieve policies: {0}", e.Message);
            }

            return policyList;
        }

        /// <summary>
        /// Converts a <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityApplicationInfo"/> to a <see cref="OpenIZAdmin.Models.ApplicationModels.ViewModels.ApplicationViewModel"/>.
        /// </summary>
        /// <param name="appInfo">The SecurityApplicationInfo to convert.</param>
        /// <returns>Returns a ApplicationViewModel model.</returns>
        public static ApplicationViewModel ToApplicationViewModel(SecurityApplicationInfo appInfo)
        {
            ApplicationViewModel viewModel = new ApplicationViewModel();

            viewModel.Id = appInfo.Id.Value;
            viewModel.ApplicationName = appInfo.Name;
            viewModel.ApplicationSecret = appInfo.ApplicationSecret;
            viewModel.CreationTime = appInfo.Application.CreationTime.DateTime;
            viewModel.HasPolicies = IsPolicy(appInfo.Policies);
            viewModel.IsObsolete = IsObsolete(appInfo.Application.ObsoletionTime);         

            if(appInfo.Policies != null)
                viewModel.Policies = appInfo.Policies.Select(p => PolicyUtil.ToPolicyViewModel(p)).OrderBy(q => q.Name).ToList();
            else
                viewModel.Policies = new List<PolicyViewModel>();

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
            EditApplicationModel viewModel = new EditApplicationModel();

            viewModel.Id = appInfo.Id.Value;
            viewModel.ApplicationName = appInfo.Name;
            viewModel.ApplicationSecret = appInfo.ApplicationSecret;
            viewModel.CreationTime = appInfo.Application.CreationTime.DateTime;
            //viewModel.HasPolicies = IsPolicy(appInfo.Policies);

            if (appInfo.Policies != null && appInfo.Policies.Count() > 0)
                viewModel.ApplicationPolicies = appInfo.Policies.Select(p => PolicyUtil.ToPolicyViewModel(p)).OrderBy(q => q.Name).ToList();
            else
                viewModel.ApplicationPolicies = new List<PolicyViewModel>();            

            //viewModel.PoliciesList.Add(new SelectListItem { Text = "", Value = "" });
            //viewModel.PoliciesList.AddRange(DeviceUtil.GetAllPolicies(client).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }));

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
        /// <param name="addPolicies">The property that contains the selected policies to add to the device.</param>
        /// <returns>Returns a security device info object.</returns>
        public static SecurityApplicationInfo ToSecurityApplicationInfo(EditApplicationModel model, SecurityApplicationInfo appInfo, List<SecurityPolicy> addPolicies)
        {
            appInfo.Application.Key = model.Id;
            appInfo.Id = model.Id;
            appInfo.Application.Name = model.ApplicationName;
            appInfo.Name = model.ApplicationName;
            appInfo.Application.ApplicationSecret = model.ApplicationSecret;
            appInfo.ApplicationSecret = model.ApplicationSecret;

            appInfo.Application.Policies = model.Policies ?? new List<SecurityPolicyInstance>();
            //add the new policies
            foreach (var policy in addPolicies.Select(p => new SecurityPolicyInstance(p, (PolicyGrantType)2)))
            {
                appInfo.Application.Policies.Add(policy);
            }

            return appInfo;
        }

        /// <summary>
        /// Checks if an application has policies. Used with boolean property in Model for UI purposes
        /// </summary>
        /// <param name="pList">A list with the policies associated with the application</param>        
        /// <returns>Returns true if policies exist, false if no policies exist</returns>
        private static bool IsPolicy(List<SecurityPolicyInstance> pList)
        {
            if (pList != null && pList.Count() > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if a device is active or inactive
        /// </summary>
        /// <param name="date">A DateTimeOffset object</param>        
        /// <returns>Returns true if active, false if inactive</returns>
        private static bool IsActiveStatus(DateTimeOffset? date)
        {
            if (date != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if an application is active or inactive
        /// </summary>
        /// <param name="date">A DateTimeOffset object</param>        
        /// <returns>Returns true if active, false if inactive</returns>
        private static bool IsObsolete(DateTimeOffset? date)
        {
            if (date == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Verifies a valid string parameter
        /// </summary>
        /// <param name="key">The string to validate</param>        
        /// <returns>Returns true if valid, false if empty or whitespace</returns>
        public static bool IsValidString(string key)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
                return true;
            else
                return false;
        }
    }
}