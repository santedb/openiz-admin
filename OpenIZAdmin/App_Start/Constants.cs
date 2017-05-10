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
using System.Security.Policy;

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
        /// Regular expression string for password validation
        /// </summary>
        //public const string RegExPassword = @"^[^ !@%\\/:*;\.\)\(]+$";
        public const string RegExPassword = @"^[^\s\\/:*;\.\)\(]+$";

        /// <summary>
        /// Regular expression string for username validation
        /// </summary>
        public const string RegExPhoneNumber = @"^(?:\d{8}|00\d{10}|\+\d{2}\d{8})$";

        /// <summary>
        /// Regular expression string for username validation
        /// </summary>
        public const string RegExPhoneNumberAlternate = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";

        //reference: https://gist.github.com/muya/ce5a18a3f119cc4ac286
        /// <summary>
        /// Regular expression string for username validation
        /// </summary>
        //public const string RegExPhoneNumberTanzania = @"/(\+?255|0|^){1}[-. ]?([7]{1}[1]{1}[2-9]{1}|[6]{1}[57]{1}[2-9]{1})[0-9]{6}\z/";
        //public const string RegExPhoneNumberTanzania = @"/(\[+]?[0-9]{3})?[-. ]?([7]{1}[1]{1}[2-9]{1}|[6]{1}[57]{1}[2-9]{1})[0-9]{6}\z/";
	    public const string RegExPhoneNumberTanzania = @"^(?:\+?(\d{3}))?[-. ]?(\d{2,4})[-. ]?(\d{3})[-. ]?(\d{2,4})$";

        //modified from  - http://stackoverflow.com/questions/16699007/regular-expression-to-match-standard-10-digit-phone-number
        //^(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$

        /// <summary>
        /// Regular expression string for username validation
        /// </summary>
        public const string RegExUsername = @"^[^ !@#$%&\\/:*;\.\)\(]+$";        

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
		public static readonly Guid TargetPopulationExtensionTypeKey = Guid.Parse("f9552ed8-66aa-4644-b6a8-108ad54f2476");

		/// <summary>
		/// The target population URL.
		/// </summary>
		public const string TargetPopulationUrl = "http://openiz.org/extensions/contrib/bid/targetPopulation";

		/// <summary>
		/// The concept set mnemonic for TelecomAddressUse.
		/// </summary>
		public const string TelecomAddressUse = "TelecomAddressUse";

		/// <summary>
		/// Access administrative function
		/// </summary>
		public const string UnrestrictedAdministration = "1.3.6.1.4.1.33349.3.1.5.9.2.0";
	}
}