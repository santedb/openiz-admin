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
 * Date: 2016-7-8
 */

using System;

namespace OpenIZAdmin.Models.AppletModels.ViewModels
{
	/// <summary>
	/// Represents an applet.
	/// </summary>
	public class AppletViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AppletViewModel"/> class.
		/// </summary>
		public AppletViewModel() : this(null, null, null, null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppletViewModel"/> class
		/// with a group, id, name, and version.
		/// </summary>
		/// <param name="group">The group of the applet.</param>
		/// <param name="id">The id of the applet.</param>
		/// <param name="name">The name of the applet.</param>
		/// <param name="version">The version of the applet.</param>
		public AppletViewModel(string author, string group, string id, string name, string version)
		{
			this.Author = author;
			this.Group = group;
			this.Id = id;
			this.Name = name;
			this.Version = version;
		}

		/// <summary>
		/// Gets or sets the author of the applet.
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// Gets or sets the group of the applet.
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// Gets or sets the id of the applet.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the applet.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the version of the applet.
		/// </summary>
		public string Version { get; set; }
	}
}