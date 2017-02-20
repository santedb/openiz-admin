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

using Elmah;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Model.AMI.Applet;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AppletModels;
using OpenIZAdmin.Util;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing applets.
	/// </summary>
	[TokenAuthorize]
	public class AppletController : BaseController
	{
		/// <summary>
		/// Downloads an applet.
		/// </summary>
		/// <param name="id">The id of the applet to download.</param>
		/// <returns>Returns the applet.</returns>
		[HttpGet]
		public ActionResult Download(string id)
		{
			if (id.HasTrailingForwardSlash())
			{
				id = id.RemoveTrailingForwardSlash();
			}

			var applet = this.AmiClient.GetApplet(id);

			var stream = new MemoryStream();

			using (var gzipStream = new GZipStream(stream, CompressionMode.Compress))
			{
				var package = applet.AppletManifest.CreatePackage();
				var serializer = new XmlSerializer(typeof(AppletPackage));

				serializer.Serialize(gzipStream, package);
			}

			return File(stream.ToArray(), "application/pak", applet.AppletManifest.Info.Id + applet.FileExtension);
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
			var fileInfo = new FileInfo(model.File.FileName);

			if (ModelState.IsValid)
			{
				AppletManifest manifest = null;

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

							using (var stream = new MemoryStream(package.Manifest))
							{
								manifest = AppletManifest.Load(stream);
							}
						}
						catch (Exception e)
						{
							ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
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
					var manifestInfo = new AppletManifestInfo(manifest)
					{
						FileExtension = fileInfo.Extension
					};

					this.AmiClient.CreateApplet(manifestInfo);

					TempData["success"] = Locale.Applet + " " + Locale.Uploaded + " " + Locale.Successfully;

					return RedirectToAction("Index");
				}
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
			if (CommonUtil.IsValidString(id))
			{
				if (id.HasTrailingForwardSlash())
				{
					id = id.RemoveTrailingForwardSlash();
				}

				var applet = this.AmiClient.GetApplet(id);

				if (applet == null)
				{
					TempData["error"] = Locale.Applet + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				return View(new AppletViewModel(applet));
			}

			TempData["error"] = Locale.Applet + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}
	}
}