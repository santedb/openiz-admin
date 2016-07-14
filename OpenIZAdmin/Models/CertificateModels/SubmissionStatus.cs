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
 * User: Nityan
 * Date: 2016-7-10
 */

namespace OpenIZAdmin.Models.CertificateModels
{
	/// <summary>
	/// Represents a submission status.
	/// </summary>
	public enum SubmissionStatus
	{
		/// <summary>
		/// The submission status is not yet complete.
		/// </summary>
		NotComplete = 0x0,

		/// <summary>
		/// The submission status failed.
		/// </summary>
		Failed = 0x1,

		/// <summary>
		/// The submission status is denied.
		/// </summary>
		Denied = 0x2,

		/// <summary>
		/// The submission status is issued.
		/// </summary>
		Issued = 0x3,

		/// <summary>
		/// The submission status is issued separately.
		/// </summary>
		IssuedSeperately = 0x4,

		/// <summary>
		/// The submission status is submitted.
		/// </summary>
		Submission = 0x5,

		/// <summary>
		/// The submission status is revoked.
		/// </summary>
		Revoked = 0x6
	}
}