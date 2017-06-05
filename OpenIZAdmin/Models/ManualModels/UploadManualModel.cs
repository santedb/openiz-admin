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
 * User: khannan
 * Date: 2017-6-4
 */

using OpenIZAdmin.Localization;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace OpenIZAdmin.Models.ManualModels
{
	/// <summary>
	/// Represents a model to upload a manual.
	/// </summary>
	public class UploadManualModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UploadManualModel"/> class.
		/// </summary>
		public UploadManualModel()
		{
		}

		/// <summary>
		/// Gets or sets the file content.
		/// </summary>
		[Display(Name = "File", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "FileRequired", ErrorMessageResourceType = typeof(Locale))]
		public HttpPostedFileBase File { get; set; }
	}
}