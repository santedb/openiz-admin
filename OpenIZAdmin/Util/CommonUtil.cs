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
 * User: Andrew
 * Date: 2016-11-29
 */

using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Util
{
    /// <summary>
    /// Provides a utility for common functionality across classes.
    /// </summary>
    public static class CommonUtil
    {
        /// <summary>
        /// Checks if a List has policies
        /// </summary>
        /// <param name="pList">A list of policies</param>        
        /// <returns>Returns true if policies exist, false if no policies exist</returns>
        public static bool HasPolicies(List<SecurityPolicyInstance> pList)
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
        public static bool IsActiveStatus(DateTimeOffset? date)
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
        public static bool IsObsolete(DateTimeOffset? date)
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

        /// <summary>
        /// Checks if a list has policies
        /// </summary>
        /// <param name="pList">A list of policies</param>        
        /// <returns>Returns true if policies exist, false if no policies exist</returns>
        public static bool IsPolicy(List<SecurityPolicyInstance> pList)
        {
            if (pList != null && pList.Count() > 0)
                return true;
            else
                return false;
        }
    }
}