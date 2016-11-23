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

namespace OpenIZAdmin.Util
{
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
        /// Converts a <see cref="OpenIZ.Core.Model.Security.SecurityApplication"/> to a <see cref="OpenIZAdmin.Models.ApplicationModels.ViewModels.ApplicationViewModel"/>.
        /// </summary>
        /// <param name="appInfo">The security application info to convert.</param>
        /// <returns>Returns a DeviceViewModel model.</returns>
        public static ApplicationViewModel ToApplicationViewModel(SecurityApplicationInfo appInfo)
        {
            ApplicationViewModel viewModel = new ApplicationViewModel();

            viewModel.Id = appInfo.Id.Value;
            viewModel.ApplicationName = appInfo.Name;
            viewModel.CreationTime = appInfo.Application.CreationTime.DateTime;
            viewModel.HasPolicies = IsPolicy(appInfo.Policies);

            return viewModel;
        }


        /// <summary>
        /// Converts a <see cref="OpenIZ.Core.Model.AMI.Auth"/> to a <see cref="OpenIZAdmin.Models.ApplicationModels.EditApplicationModel"/>
        /// </summary>
        /// <param name="client">The Ami Service Client.</param>
        /// /// <param name="device">The device object to convert to a EditDeviceModel.</param>
        /// <returns>Returns a EditDeviceModel object.</returns>
        public static EditApplicationModel ToEditApplicationModel(AmiServiceClient client, SecurityApplicationInfo appInfo)
        {
            EditApplicationModel viewModel = new EditApplicationModel();

            //viewModel.Device = device;
            //viewModel.CreationTime = device.CreationTime.DateTime;
            //viewModel.Id = device.Key.Value;
            //viewModel.DeviceSecret = device.DeviceSecret;
            //viewModel.Name = device.Name;
            //viewModel.UpdatedTime = device.UpdatedTime?.DateTime;

            //viewModel.DevicePolicies = (device.Policies != null) ? GetDevicePolicies(device.Policies) : null;                 

            //viewModel.PoliciesList.Add(new SelectListItem { Text = "", Value = "" });
            //viewModel.PoliciesList.AddRange(DeviceUtil.GetAllPolicies(client).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }));

            return viewModel;
        }

        /// <summary>
        /// Converts a <see cref="OpenIZAdmin.Models.ApplicationModels.CreateApplicationModel"/> to a <see cref="OpenIZ.Core.Model.Security.SecurityApplication"/>.
        /// </summary>
        /// <param name="model">The create device model to convert.</param>
        /// <returns>Returns a security device.</returns>
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
        /// Checks if an applications has policies
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