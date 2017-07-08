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
 * Date: 2017-5-19
 */

using OpenIZ.Core.Extensions;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.ExtensionTypeModels
{
	/// <summary>
	/// Represents a create extension type model.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Models.Core.IdentifiedEditModel" />
	public class CreateExtensionTypeModel : IdentifiedEditModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateExtensionTypeModel"/> class.
		/// </summary>
		public CreateExtensionTypeModel()
		{
			this.HandlerClasses = new List<SelectListItem>();
			this.SetHandlerClasses();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateExtensionTypeModel"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		public CreateExtensionTypeModel(Guid id) : base(id)
		{
			this.HandlerClasses = new List<SelectListItem>();
			this.SetHandlerClasses();
		}

		/// <summary>
		/// Gets or sets the handler class.
		/// </summary>
		/// <value>The handler class.</value>
		[Display(Name = "HandlerClass", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "HandlerClassRequired", ErrorMessageResourceType = typeof(Locale))]
		public string HandlerClass { get; set; }

		/// <summary>
		/// Gets or sets the handler classes.
		/// </summary>
		/// <value>The handler classes.</value>
		public List<SelectListItem> HandlerClasses { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Display(Name = "Name", ResourceType = typeof(Locale))]
		[Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Locale))]
		[StringLength(128, ErrorMessageResourceName = "NameLength128", ErrorMessageResourceType = typeof(Locale))]
		[RegularExpression(Constants.RegExBasicString, ErrorMessageResourceName = "InvalidStringEntry", ErrorMessageResourceType = typeof(Locale))]
		public string Name { get; set; }

		/// <summary>
		/// Converts this instance to an <see cref="ExtensionType" /> instance.
		/// </summary>
		/// <returns>Returns the created extension type instance.</returns>
		public ExtensionType ToExtensionType()
		{
			return new ExtensionType(this.Name, Type.GetType(this.HandlerClass))
			{
				Key = Guid.NewGuid(),
				IsEnabled = true
			};
		}

		/// <summary>
		/// Sets the handler classes.
		/// </summary>
		private void SetHandlerClasses()
		{
			this.HandlerClasses.Add(new SelectListItem { Text = string.Empty, Value = string.Empty });
			this.HandlerClasses.Add(new SelectListItem { Text = typeof(BooleanExtensionHandler).Name, Value = typeof(BooleanExtensionHandler).AssemblyQualifiedName });
			this.HandlerClasses.Add(new SelectListItem { Text = typeof(DateExtensionHandler).Name, Value = typeof(DateExtensionHandler).AssemblyQualifiedName });
			this.HandlerClasses.Add(new SelectListItem { Text = typeof(DecimalExtensionHandler).Name, Value = typeof(DecimalExtensionHandler).AssemblyQualifiedName });
			this.HandlerClasses.Add(new SelectListItem { Text = typeof(DictionaryExtensionHandler).Name, Value = typeof(DictionaryExtensionHandler).AssemblyQualifiedName });
			this.HandlerClasses.Add(new SelectListItem { Text = typeof(StringExtensionHandler).Name, Value = typeof(StringExtensionHandler).AssemblyQualifiedName });
		}
	}
}