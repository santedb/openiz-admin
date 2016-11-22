/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you
 * may not use this file except in compliance with the License. You may
 * obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 *
 * User: khannan
 * Date: 2016-8-14
 */

using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Models.PolicyModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.DeviceModels
{
	public class EditDeviceModel
	{
		public EditDeviceModel()
		{            
            this.PoliciesList = new List<SelectListItem>();            
        }

        //policies added by the user
        [Display(Name = "AddPolicies", ResourceType = typeof(Localization.Locale))]
        //[Required(ErrorMessageResourceName = "RolesRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        public IEnumerable<string> AddPoliciesList { get; set; }

        [Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
        public DateTimeOffset CreationTime { get; set; }

        //holds the original device object
        public SecurityDevice Device { get; set; }

        [Display(Name = "DeviceSecret", ResourceType = typeof(Localization.Locale))]
        public string DeviceSecret { get; set; }

        //public List<SecurityPolicyInstance> DevicePolicies { get; set; }

        //public List<SecurityPolicyInfoViewModel> DevicePoliciesViewModel { get; set; }

        public IEnumerable<PolicyViewModel> DevicePolicies { get; set; }

        [Required]
        public Guid Id { get; set; }

        //public bool IsObsolete { get; set; }        

        [Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Localization.Locale))]
        [StringLength(255, ErrorMessageResourceName = "NameTooLong", ErrorMessageResourceType = typeof(Localization.Locale))]
        public string Name { get; set; }

        //Holds the device policies that are assigned
        public List<SecurityPolicyInstance> Policies { get; set; }   
                     
        //policies autopopulate
        public List<SelectListItem> PoliciesList { get; set; }

        public DateTime? UpdatedTime { get; set; }
    }

    //public class SecurityPolicyInfoViewModel : SecurityPolicyInfo
    //{

    //    public SecurityPolicyInfoViewModel(SecurityPolicyInstance pInstance)
    //    {            
    //        this.PolicyGrantText = Enum.GetName(typeof(PolicyGrantType), pInstance.GrantType);
    //        this.PolicyGrant = (int)pInstance.GrantType;
    //    }

    //    public int PolicyGrant { get; set; }
    //    public string PolicyGrantText { get; set; }
    //}
}