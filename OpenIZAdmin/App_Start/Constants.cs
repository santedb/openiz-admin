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
		/// The administrators role.
		/// </summary>
		public const string Administrators = "ADMINISTRATORS";

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
		/// The IMSI endpoint name.
		/// </summary>
		public const string Imsi = "IMSI";

		/// <summary>
		/// The concept set mnemonic for TelecomAddressUse.
		/// </summary>
		public const string TelecomAddressUse = "TelecomAddressUse";
	}
}