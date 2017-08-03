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
 * User: Nityan
 * Date: 2016-7-8
 */


using OpenIZ.Core.Applets.Model;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AppletModels;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Serialization;
using OpenIZ.Core.Model.AMI.Applet;
using OpenIZAdmin.Core.Extensions;

namespace OpenIZAdmin.Controllers
{
    /// <summary>
    /// Provides operations for managing applets.
    /// </summary>
    [TokenAuthorize(Constants.UnrestrictedMetadata)]
	public class AppletController : BaseController
	{
		/// <summary>
		/// Deletes the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(string id)
		{
			try
			{
				if (id.HasTrailingForwardSlash())
				{
					id = id.RemoveTrailingForwardSlash();
				}

				this.AmiClient.DeleteApplet(id);

				this.TempData["success"] = Locale.AppletDeletedSuccessfully;
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to delete applet: {e}");
				this.TempData["error"] = Locale.UnableToDeleteApplet;
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Downloads an applet.
		/// </summary>
		/// <param name="id">The id of the applet to download.</param>
		/// <returns>Returns the applet.</returns>
		[HttpGet]
		public ActionResult Download(string id)
		{
			try
			{
				if (id.HasTrailingForwardSlash())
				{
					id = id.RemoveTrailingForwardSlash();
				}

				var stream = this.AmiClient.DownloadApplet(id);

				return File(stream, "application/pak", id + ".pak");
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to download applet: {e}");
				this.TempData["error"] = Locale.UnableToDownloadApplet;
			}

			return this.RedirectToRequestOrHome();
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			var applets = this.AmiClient.GetApplets().CollectionItem.Select(a => new AppletViewModel(a)).ToList();            

			return View(applets);
		}

		/// <summary>
		/// Displays the update applet view.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns an <see cref="ActionResult" /> instance.</returns>
		[HttpGet]
		public ActionResult Update(string id)
		{
			if (this.IsValidId(id))
			{
				if (id.HasTrailingForwardSlash())
				{
					id = id.RemoveTrailingForwardSlash();
				}

				var applet = this.AmiClient.GetApplet(id);

				if (applet == null)
				{
					TempData["error"] = Locale.AppletNotFound;

					return RedirectToAction("Index");
				}

				var model = new UploadAppletModel
				{
					AppletViewModel = new AppletViewModel(applet),
					Id = id
				};

				return View(model);
			}

			TempData["error"] = Locale.AppletNotFound;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Updates the Applet with the details submitted.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Returns an <see cref="ActionResult" /> instance.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Update(UploadAppletModel model)
		{
			AppletPackage package = null;

			try
			{
				if (ModelState.IsValid)
				{
					var fileInfo = new FileInfo(model.File.FileName);

					switch (fileInfo.Extension)
					{
						case ".pak":

							try
							{
								using (var stream = new GZipStream(model.File.InputStream, CompressionMode.Decompress))
								{
									var serializer = new XmlSerializer(typeof(AppletPackage));
									package = (AppletPackage)serializer.Deserialize(stream);
								}
							}
							catch (Exception e)
							{
								Trace.TraceError($"Unable to decode applet: {e}");
								ModelState.AddModelError(nameof(model.File), Locale.UnableToUploadApplet);
							}

							break;

						default:
							ModelState.AddModelError(nameof(model.File), Locale.UnableToUploadApplet);
							break;
					}

					// ensure that the model state wasn't invalidated when attempting to serialize the applet file
					if (ModelState.IsValid)
					{
						this.AmiClient.UpdateApplet(package.Meta.Id, package);

						TempData["success"] = Locale.AppletUploadedSuccessfully;

						return RedirectToAction("Index");
					}
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to update applet: {e}");
			}

			if (model.Id.HasTrailingForwardSlash())
			{
				model.Id = model.Id.RemoveTrailingForwardSlash();
			}

			var applet = this.AmiClient.GetApplet(model.Id);

			if (applet == null)
			{
				TempData["error"] = Locale.AppletNotFound;

				return RedirectToAction("Index");
			}

			model.AppletViewModel = new AppletViewModel(applet);

			TempData["error"] = Locale.UnableToUploadApplet;

			return View(model);
		}

		/// <summary>
		/// Displays the upload view.
		/// </summary>
		/// <returns>Returns the upload view.</returns>
		[HttpGet]
		public ActionResult Upload()
		{
			return View();
		}

		/// <summary>
		/// Uploads an applet.
		/// </summary>
		/// <param name="model">The model containing the applet.</param>
		/// <returns>Returns the upload view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Upload(UploadAppletModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var fileInfo = new FileInfo(model.File.FileName);

					switch (fileInfo.Extension)
					{
						case ".pak":

							try
							{
								AppletPackage package;
								using (var stream = new GZipStream(model.File.InputStream, CompressionMode.Decompress))
								{
									var serializer = new XmlSerializer(typeof(AppletPackage));
									package = (AppletPackage)serializer.Deserialize(stream);
								}

								this.AmiClient.CreateApplet(package);

								TempData["success"] = Locale.AppletUploadedSuccessfully;

								return RedirectToAction("Index");
							}
							catch (Exception e)
							{
								Trace.TraceError($"Unable to upload applet: {e}");
								ModelState.AddModelError(nameof(model.File), Locale.UnableToUploadApplet);
							}

							break;

						default:
							ModelState.AddModelError(nameof(model.File), Locale.UnableToUploadApplet);
							break;
					}
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to upload applet: {e}");
			}

			TempData["error"] = Locale.UnableToUploadApplet;

			return View(model);
		}

		/// <summary>
		/// Retrieves the selected role
		/// </summary>
		/// <param name="id">The identifier of the role object</param>
		/// <returns>Returns the ViewRole view.</returns>
		[HttpGet]
		public ActionResult ViewApplet(string id)
		{
			if (this.IsValidId(id))
			{
				if (id.HasTrailingForwardSlash())
				{
					id = id.RemoveTrailingForwardSlash();
				}

				var applet = this.AmiClient.GetApplet(id);

				if (applet == null)
				{
					TempData["error"] = Locale.AppletNotFound;

					return RedirectToAction("Index");
				}

				return View(new AppletViewModel(applet));
			}

			TempData["error"] = Locale.AppletNotFound;

			return RedirectToAction("Index");
		}
	}
}