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

using System.Reflection;

namespace OpenIZAdmin.Models.DebugModels.ViewModels
{
	/// <summary>
	/// Represents an assembly info view model.
	/// </summary>
	public class AssemblyInfoViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyInfoViewModel"/> class.
		/// </summary>
		public AssemblyInfoViewModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyInfoViewModel"/> class
		/// with a specific <see cref="Assembly"/> instance.
		/// </summary>
		/// <param name="assembly">The assembly to load.</param>
		public AssemblyInfoViewModel(Assembly assembly)
		{
			this.Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
			this.Title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
			this.Version = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
		}

		/// <summary>
		/// Get or sets the description of the assembly version.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets of sets the title of the assembly version.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the version of the assembly version.
		/// </summary>
		public string Version { get; set; }
	}
}