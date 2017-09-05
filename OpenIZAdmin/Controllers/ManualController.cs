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

using OpenIZAdmin.Attributes;
using OpenIZAdmin.DAL.Manuals;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Domain;
using OpenIZAdmin.Models.ManualModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing user manuals.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Controllers.BaseController" />
	[TokenAuthorize(Constants.UnrestrictedMetadata)]
	public class ManualController : Controller
	{
		/// <summary>
		/// The manual service.
		/// </summary>
		private readonly IManualService manualService;

		/// <summary>
		/// Initializes a new instance of the <see cref="ManualController"/> class.
		/// </summary>
		public ManualController(IManualService manualService)
		{
			this.manualService = manualService;
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
				this.manualService.Delete(id);

				this.TempData["success"] = Locale.ManualDeletedSuccessfully;
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
		[AllowAnonymous]
		public ActionResult Download(Guid id)
		{
			try
			{
				var manual = this.manualService.Get(id);

				if (manual == null)
				{
					this.TempData["error"] = Locale.ManualNotFound;
					return RedirectToAction("Index");
				}

				var content = Convert.FromBase64String(manual.Content);

				return new FileContentResult(content, "application/pdf");
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
				foreach (var manual in this.manualService.AsQueryable())
				{
					model.Add(new ManualIndexViewModel(manual)
					{
						// HACK
						DownloadLink = this.Request?.Url?.GetLeftPart(UriPartial.Authority) + this.Request?.RawUrl?.Replace("Index", "Download/") + manual.Id
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

				if (this.ModelState.IsValid && this.manualService.IsValidPdf(model.File.InputStream))
				{
					var path = Path.Combine(this.Server.MapPath("~/Manuals"), Path.GetFileName(model.File.FileName));

					model.File.SaveAs(path);

					var manualContent = Convert.ToBase64String(System.IO.File.ReadAllBytes(path));

					this.manualService.Save(new Manual(id, Path.GetFileNameWithoutExtension(model.File.FileName), manualContent));

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