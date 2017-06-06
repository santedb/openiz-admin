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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.DAL;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Domain;
using OpenIZAdmin.Models.ManualModels;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing user manuals.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Controllers.BaseController" />
	[TokenAuthorize]
	public class ManualController : BaseController
	{
		/// <summary>
		/// The unit of work.
		/// </summary>
		private readonly IUnitOfWork unitOfWork;

		/// <summary>
		/// Initializes a new instance of the <see cref="ManualController"/> class.
		/// </summary>
		public ManualController() : this(new EntityUnitOfWork(new ApplicationDbContext()))
	    {
		    
	    }

		/// <summary>
		/// Initializes a new instance of the <see cref="ManualController"/> class.
		/// </summary>
		/// <param name="unitOfWork">The unit of work.</param>
		public ManualController(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		/// <summary>
		/// Deletes a manual with the given id.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var manual = this.unitOfWork.ManualRepository.FindById(id);

				this.unitOfWork.ManualRepository.Delete(manual);
				this.unitOfWork.Save();

				var file = Directory.GetFiles(this.Server.MapPath("~/Manuals"), "*.pdf", SearchOption.TopDirectoryOnly).Select(f => new FileInfo(f)).FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.FullName) == manual.Name);

				// delete the file from the file system
				System.IO.File.Delete(Path.GetFullPath(file.FullName));

			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to delete manual: {e}");
				this.TempData["error"] = Locale.UnableToDeleteManual;
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Downloads a manual with the given id.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the manual for the given id.</returns>
		[HttpGet]
		public ActionResult Download(Guid id)
		{
			try
			{
				var manual = this.unitOfWork.ManualRepository.FindById(id);

				if (manual == null)
				{
					this.TempData["error"] = Locale.ManualNotFound;
					return RedirectToAction("Index");
				}

				var file = Directory.GetFiles(this.Server.MapPath("~/Manuals"), "*.pdf", SearchOption.TopDirectoryOnly).Select(f => new FileInfo(f)).FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.FullName) == manual.Name);

				if (file == null)
				{
					this.TempData["error"] = Locale.ManualNotFound;
					return RedirectToAction("Index");
				}

				return File(System.IO.File.ReadAllBytes(file.FullName), "application/pdf", file.Name);

			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to download manual: {e}");
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			var model = new List<ManualIndexViewModel>();

			try
			{
				var manuals = unitOfWork.ManualRepository.AsQueryable();

				foreach (var manual in manuals)
				{
					var file = Directory.GetFiles(this.Server.MapPath("~/Manuals")).Select(f => new FileInfo(f)).FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.FullName) == manual.Name);

					// skip files which are on the file system, but not tracked in the database
					if (file == null)
					{
						continue;
					}

					model.Add(new ManualIndexViewModel(manual)
					{
						// HACK
						DownloadLink = this.Request.Url.GetLeftPart(UriPartial.Authority) + this.Request.RawUrl.Replace("Index", "Download/") + manual.Id
					});
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve manuals: {e}");
				this.TempData["error"] = Locale.UnableToRetrieveManuals;
			}

			return View(model);
		}

		/// <summary>
		/// Tries to validate a PDF.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns><c>true</c> if the PDF is valid, <c>false</c> otherwise.</returns>
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
		/// Uploads this instance.
		/// </summary>
		/// <returns>ActionResult.</returns>
		[HttpGet]
		public ActionResult Upload()
		{
			return View();
		}

		/// <summary>
		/// Uploads the specified model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Upload(UploadManualModel model)
		{
			try
			{
				var id = Guid.NewGuid();

				if (this.ModelState.IsValid && this.IsValidPdf(model.File.InputStream))
				{
					var path = Path.Combine(this.Server.MapPath("~/Manuals"), Path.GetFileName(model.File.FileName));

					model.File.SaveAs(path);

					this.unitOfWork.ManualRepository.Add(new Manual(id, Path.GetFileNameWithoutExtension(model.File.FileName), path));
					this.unitOfWork.Save();

					this.TempData["success"] = Locale.ManualUploadedSuccessfully;

					return RedirectToAction("Index");
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to upload manual: {e}");
				
			}

			this.TempData["error"] = Locale.UnableToUploadManual;

			return View(model);
		}
	}
}