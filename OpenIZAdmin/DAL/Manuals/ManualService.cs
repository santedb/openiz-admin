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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenIZAdmin.DAL.Manuals
{
	/// <summary>
	/// Represents a service to manage manuals.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.DAL.Manuals.IManualService" />
	public class ManualService : IManualService
	{
		/// <summary>
		/// The unit of work.
		/// </summary>
		private readonly IUnitOfWork unitOfWork;

		/// <summary>
		/// Initializes a new instance of the <see cref="ManualService"/> class.
		/// </summary>
		public ManualService() : this(new EntityUnitOfWork(new ApplicationDbContext()))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ManualService"/> class.
		/// </summary>
		/// <param name="unitOfWork">The unit of work.</param>
		public ManualService(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		/// <summary>
		/// Gets the manuals collection as a queryable interface instance.
		/// </summary>
		/// <returns>Returns the list of manuals in the database.</returns>
		public IQueryable<Manual> AsQueryable()
		{
			return this.unitOfWork.ManualRepository.AsQueryable();
		}

		/// <summary>
		/// Deletes the specified manual.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
		public void Delete(Guid id)
		{
			var manual = this.Get(id);

			if (manual == null)
			{
				throw new KeyNotFoundException($"The manual with the id :{id} was not found in the database.");
			}

			unitOfWork.ManualRepository.Delete(manual);
			unitOfWork.Save();
		}

		/// <summary>
		/// Gets the specified manual.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the manual with the specified identifier, or null if no manual is found.</returns>
		public Manual Get(Guid id)
		{
			return this.unitOfWork.ManualRepository.FindById(id);
		}

		/// <summary>
		/// Determines whether the stream contains a valid PDF document.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns><c>true</c> if the stream contains a valid PDF; otherwise, <c>false</c>.</returns>
		public bool IsValidPdf(Stream stream)
		{
			// courtesy of:
			// https://stackoverflow.com/questions/3108201/detect-if-pdf-file-is-correct-header-pdf

			var status = false;

			try
			{
				var br = new BinaryReader(stream);

				var buffer = br.ReadBytes(1024);

				var enc = new ASCIIEncoding();
				var header = enc.GetString(buffer);

				//%PDF−1.0
				// If you are loading it into a long, this is (0x04034b50).

				if (buffer[0] == 0x25 && buffer[1] == 0x50 && buffer[2] == 0x44 && buffer[3] == 0x46)
				{
					status = header.Contains("%PDF-");
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to verify PDF: {e}");
			}

			return status;
		}

		/// <summary>
		/// Saves the specified manual.
		/// </summary>
		/// <param name="manual">The manual.</param>
		/// <returns>Returns the saved manual.</returns>
		public Manual Save(Manual manual)
		{
			unitOfWork.ManualRepository.Add(manual);
			unitOfWork.Save();

			return manual;
		}
	}
}