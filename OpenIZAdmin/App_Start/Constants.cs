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
using System.Configuration;

namespace OpenIZAdmin
{
	/// <summary>
	/// Represents constants for the application.
	/// </summary>
	public static class Constants
	{
		/// <summary>
		/// The OAUTH endpoint name.
		/// </summary>
		public const string Acs = "ACS";

		/// <summary>
		/// The AMI endpoint name.
		/// </summary>
		public const string Ami = "AMI";

		/// <summary>
		/// The content type of application/xml.
		/// </summary>
		public const string ApplicationXml = "application/xml";

		/// <summary>
		/// The User Role type Clinical Staff.
		/// </summary>
		public const string ClinicalStaff = "CLINICAL_STAFF";

		/// <summary>
		/// The DateTime requested format with timestamp
		/// </summary>
		public const string DateTimeFormatStringWithTimestamp = "dd/MM/yyyy hh:mm:ss tt";

		/// <summary>
		/// The DateTime requested format with timestamp for a View
		/// </summary>
		public const string DateTimeFormatViewWithTimestamp = "{0:dd/MM/yyyy hh:mm:ss tt}";

		/// <summary>
		/// The imported data tag.
		/// </summary>
		public const string ImportedDataTag = "http://openiz.org/tags/contrib/importedData";

		/// <summary>
		/// The IMSI endpoint name.
		/// </summary>
		public const string Imsi = "IMSI";

		/// <summary>
		/// The constant for N/A (not applicable).
		/// </summary>
		public const string NotApplicable = "N/A";

		/// <summary>
		/// The OpenIZ grant type claim type.
		/// </summary>
		public const string OpenIZGrantTypeClaimType = "http://openiz.org/claims/grant";

		/// <summary>
		/// Regular expression string for password validation
		/// </summary>
		public const string RegExPassword = @"^[^\s\\/:*;\.\)\(]+$";

		//reference: https://gist.github.com/muya/ce5a18a3f119cc4ac286
		/// <summary>
		/// Regular expression string for username validation
		/// </summary>        
		public const string RegExPhoneNumberTanzania = @"^(?:\+?(\d{3}))?[-. ]?(\d{2,4})[-. ]?(\d{3})[-. ]?(\d{2,4})$";

		//modified from  - http://stackoverflow.com/questions/16699007/regular-expression-to-match-standard-10-digit-phone-number
		//^(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$

		/// <summary>
		/// Regular expression string for basic string validation
		/// </summary>
		//public const string RegExBasicString = @"^[^ !@%\\/:*;\.\)\(]+$";
		public const string RegExBasicString = @"^[^*@&<>]+$";

		/// <summary>
		/// Regular expression string for html tag validation
		/// </summary>        
		public const string RegExBasicHtmlString = @"^[^<>]+$";

		/// <summary>
		/// Regular expression string for select2 limited string validation
		/// </summary>        
		public const string RegExSelect2StringInput = @"/^[\w()+\-\[\]{}]+$/g";

		/// <summary>
		/// Regular expression string for oid validation
		/// </summary>
		public const string RegExOidValidation = @"^(?=^| )\d+(\.\d+)*(?=$| )|(?=^| )\.\d+(?=$| )$";

		/// <summary>
		/// Regular expression string for username validation
		/// </summary>
		public const string RegExUsername = "^[a-zA-Z0-9]+$";

		/// <summary>
		/// The RISI endpoint name.
		/// </summary>
		public const string Risi = "RISI";

		/// <summary>
		/// The system user identifier.
		/// </summary>
		public const string SystemUserId = "fadca076-3690-4a6e-af9e-f1cd68e8c7e8";

		/// <summary>
		/// The target population extension type key.
		/// </summary>
		public static readonly Guid TargetPopulationExtensionTypeKey = Guid.Parse(ConfigurationManager.AppSettings["TargetPopulationKey"]);

		/// <summary>
		/// The target population URL.
		/// </summary>
		public static readonly string TargetPopulationUrl = ConfigurationManager.AppSettings["TargetPopulationUrl"];

		/// <summary>
		/// The target population URL.
		/// </summary>
		public const string OpenIzGrantedPolicyClaim = "http://openiz.org/claims/grant";

        /// <summary>
        /// The concept set mnemonic for TelecomAddressUse.
        /// </summary>
        public const string TelecomAddressUse = "TelecomAddressUse";


        /// <summary>
        /// Access administrative function
        /// </summary>
        public const string UnrestrictedAll = "1.3.6.1.4.1.33349.3.1.5.9.2";

        /// <summary>
        /// Access administrative function
        /// </summary>
        public const string UnrestrictedAdministration = "1.3.6.1.4.1.33349.3.1.5.9.2.0";

        /// <summary>
        /// Policy identifier for allowance of changing passwords
        /// </summary>
        /// TODO: Affix the mohawk college OID for this
        public const string ChangePassword = "1.3.6.1.4.1.33349.3.1.5.9.2.0.1";

        /// <summary>
        /// Whether the user can create roles
        /// </summary>
        public const string CreateRoles = "1.3.6.1.4.1.33349.3.1.5.9.2.0.2";

        /// <summary>
        /// Policy identifier for allowance of altering passwords
        /// </summary>
        public const string AlterRoles = "1.3.6.1.4.1.33349.3.1.5.9.2.0.3";

        /// <summary>
        /// Policy identifier for allowing of creating new identities
        /// </summary>
        public const string CreateIdentity = "1.3.6.1.4.1.33349.3.1.5.9.2.0.4";

        /// <summary>
        /// Policy identifier for allowing of creating new devices
        /// </summary>
        public const string CreateDevice = "1.3.6.1.4.1.33349.3.1.5.9.2.0.5";

        /// <summary>
        /// Policy identifier for allowing of creating new applications
        /// </summary>
        public const string CreateApplication = "1.3.6.1.4.1.33349.3.1.5.9.2.0.6";

        /// <summary>
        /// Administer the concept dictionary
        /// </summary>
        public const string AdministerConceptDictionary = "1.3.6.1.4.1.33349.3.1.5.9.2.0.7";

        /// <summary>
        /// Policy identifier for allowing of creating new identities
        /// </summary>
        public const string AlterIdentity = "1.3.6.1.4.1.33349.3.1.5.9.2.0.8";

        /// <summary>
        /// Allows an identity to alter a policy
        /// </summary>
        public const string AlterPolicy = "1.3.6.1.4.1.33349.3.1.5.9.2.0.9";

        /// <summary>
        /// Administer data warehouse
        /// </summary>
        public const string AdministerWarehouse = "1.3.6.1.4.1.33349.3.1.5.9.2.0.10";

        /// <summary>
        /// Policy identifier for allowance of login
        /// </summary>
        public const string Login = "1.3.6.1.4.1.33349.3.1.5.9.2.1";

        /// <summary>
        /// Login to an interactive session (with user interaction)
        /// </summary>
        public const string LoginAsService = "1.3.6.1.4.1.33349.3.1.5.9.2.1.0";

        /// <summary>
        /// Access clinical data permission 
        /// </summary>
        public const string UnrestrictedClinicalData = "1.3.6.1.4.1.33349.3.1.5.9.2.2";

        /// <summary>
        /// Query clinical data
        /// </summary>
        public const string QueryClinicalData = "1.3.6.1.4.1.33349.3.1.5.9.2.2.0";

        /// <summary>
        /// Write clinical data
        /// </summary>
        public const string WriteClinicalData = "1.3.6.1.4.1.33349.3.1.5.9.2.2.1";

        /// <summary>
        /// Delete clinical data
        /// </summary>
        public const string DeleteClinicalData = "1.3.6.1.4.1.33349.3.1.5.9.2.2.2";

        /// <summary>
        /// Read clinical data
        /// </summary>
        public const string ReadClinicalData = "1.3.6.1.4.1.33349.3.1.5.9.2.2.3";

        /// <summary>
        /// Indicates the user can elevate themselves (Break the glass)
        /// </summary>
        public const string ElevateClinicalData = "1.3.6.1.4.1.33349.3.1.5.9.2.3";


        /// <summary>
        /// Indicates the user can update metadata
        /// </summary>
        public const string UnrestrictedMetadata = "1.3.6.1.4.1.33349.3.1.5.9.2.4";
        /// <summary>
        /// Indicates the user can read metadata
        /// </summary>
        public const string ReadMetadata = "1.3.6.1.4.1.33349.3.1.5.9.2.4.0";

        /// <summary>
        /// Allow a user all access to the warehouse 
        /// </summary>
        public const string UnrestrictedWarehouse = "1.3.6.1.4.1.33349.3.1.5.9.2.5";

        /// <summary>
        /// Allow a user to write data to the warehouse 
        /// </summary>
        public const string WriteWarehouseData = UnrestrictedWarehouse + ".0";

        /// <summary>
        /// Allow a user to write data to the warehouse 
        /// </summary>
        public const string DeleteWarehouseData = UnrestrictedWarehouse + ".1";

        /// <summary>
        /// Allow a user to write data to the warehouse 
        /// </summary>
        public const string ReadWarehouseData = UnrestrictedWarehouse + ".2";

        /// <summary>
        /// Allow a user to write data to the warehouse 
        /// </summary>
        public const string QueryWarehouseData = UnrestrictedWarehouse + ".3";



    }
}