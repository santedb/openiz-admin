/*
 * Copyright 2016-2017 Mohawk College of Applied Arts and Technology
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
 * User: Nityan
 * Date: 2016-9-5
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIZAdmin
{
	/// <summary>
	/// Represents constants for the application.
	/// </summary>
	public static class Constants
	{
		/// <summary>
		/// The OAuth endpoint name.
		/// </summary>
		public const string ACS = "ACS";

		/// <summary>
		/// The AMI endpoint name.
		/// </summary>
		public const string AMI = "AMI";

        /// <summary>
		/// The content type of application/xml.
		/// </summary>
		public const string ApplicationXml = "application/xml";
        
        /// <summary>
        /// The User Role type Clinical Staff.
        /// </summary>
        public const string CLINICAL_STAFF = "CLINICAL_STAFF";

        /// <summary>
        /// The IMSI endpoint name.
        /// </summary>
        public const string IMSI = "IMSI";        
        
        /// <summary>
        /// The User Role type Middle Level Officer.
        /// </summary>
        //public const string MIDDLE_LEVEL_OFFICER = "Middle Level Officer";

        /// <summary>
        /// The Mobile Phone Guid associated with a Phone Type 
        /// </summary>
        public const string MOBILE_PHONE_TYPE_GUID = "e161f90e-5939-430e-861a-f8e885cc353d";       

        /// <summary>
        /// The DateTime requested format with timestamp
        /// </summary>
        public const string DATETIME_FORMAT_STRING_WITH_TIMESTAMP = "dd/MM/yyyy hh:mm:ss tt";

        /// <summary>
        /// The DateTime requested format for a View
        /// </summary>
        public const string DATETIME_FORMAT_VIEW = "{0:dd/MM/yyyy}";

        /// <summary>
        /// The DateTime requested format with timestamp for a View
        /// </summary>
        public const string DATETIME_FORMAT_VIEW_WITH_TIMESTAMP = "{0:dd/MM/yyyy hh:mm:ss tt}";        

        /// <summary>
        /// The date format requested to be displayed with timestamp and UTC offset for a View
        /// </summary>        
        public const string DATETIME_FORMAT_VIEW_WITH_TIMESTAMP_UTC_OFFSET = "{0:dd/MM/yyyy  hh:mm:ss tt  zzz}";

        /// <summary>
        /// The User Role type Vaccinator.
        /// </summary>
        //public const string VACCINATOR = "Vaccinator";
    }
}
