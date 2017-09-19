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
 * Date: 2017-9-5
 */

using OpenIZAdmin.Models.Domain;
using System;
using System.IO;
using System.Linq;

namespace OpenIZAdmin.DAL.Manuals
{
	/// <summary>
	/// Represents a service to manage manuals.
	/// </summary>
	public interface IManualService
	{
		/// <summary>
		/// Gets the manuals collection as a queryable interface instance.
		/// </summary>
		/// <returns>Returns the list of manuals in the database.</returns>
		IQueryable<Manual> AsQueryable();

		/// <summary>
		/// Deletes the specified manual.
		/// </summary>
		/// <param name="id">The identifier.</param>
		void Delete(Guid id);

		/// <summary>
		/// Gets the specified manual.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the manual with the specified identifier, or null if no manual is found.</returns>
		Manual Get(Guid id);

		/// <summary>
		/// Determines whether [is valid PDF] [the specified stream].
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns><c>true</c> if [is valid PDF] [the specified stream]; otherwise, <c>false</c>.</returns>
		bool IsValidPdf(Stream stream);

		/// <summary>
		/// Saves the specified manual.
		/// </summary>
		/// <param name="manual">The manual.</param>
		/// <returns>Returns the saved manual.</returns>
		Manual Save(Manual manual);
	}
}