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
 * Date: 2016-11-21
 */

using System.Collections.Generic;
using System.Reflection;

namespace OpenIZAdmin.Models.DebugModels.ViewModels
{
	/// <summary>
	/// Represents a version view model.
	/// </summary>
	public class VersionViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VersionViewModel"/> class.
		/// </summary>
		public VersionViewModel()
		{
			this.Assemblies = new List<AssemblyInfoViewModel>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="VersionViewModel"/> class
		/// with a specific <see cref="Assembly"/> instance.
		/// </summary>
		/// <param name="assembly">The assembly instance.</param>
		public VersionViewModel(Assembly assembly)
		{
			this.Assemblies = new List<AssemblyInfoViewModel>();
			this.Company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
			this.Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
			this.Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
			this.Product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
			this.Title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
			this.Version = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
		}

		/// <summary>
		/// Gets or sets the loaded assemblies of the application.
		/// </summary>
		public List<AssemblyInfoViewModel> Assemblies { get; set; }

		/// <summary>
		/// Gets or sets the company of the version information.
		/// </summary>
		public string Company { get; set; }

		/// <summary>
		/// Gets or sets the copyright of the version information.
		/// </summary>
		public string Copyright { get; set; }

		/// <summary>
		/// Gets or sets the description of the version information.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the product of the version information.
		/// </summary>
		public string Product { get; set; }

		/// <summary>
		/// Gets or sets the title of the version information.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the current user id.
		/// </summary>
		public string UserKey { get; set; }

		/// <summary>
		/// Gets or sets the version information of the version information.
		/// </summary>
		public string Version { get; set; }
	}
}