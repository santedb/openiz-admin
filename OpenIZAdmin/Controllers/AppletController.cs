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

using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Model.AMI.Applet;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AppletModels;
using OpenIZAdmin.Models.AppletModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			var applets = AppletUtil.GetApplets(this.AmiClient);

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
			FileInfo fileInfo = new FileInfo(model.File.FileName);

			if (ModelState.IsValid)
			{
				XmlSerializer serializer;
				AppletManifest manifest = null;

				switch (fileInfo.Extension)
				{
					case ".pak":
						AppletPackage package;

						using (GZipStream stream = new GZipStream(model.File.InputStream, CompressionMode.Decompress))
						{
							serializer = new XmlSerializer(typeof(AppletPackage));
							package = (AppletPackage)serializer.Deserialize(stream);
						}

						using (MemoryStream stream = new MemoryStream(package.Manifest))
						{
							manifest = AppletManifest.Load(stream);
						}

						break;

					default:
						ModelState.AddModelError(nameof(model.File), Locale.GenericErrorMessage);
						break;
				}

				if (ModelState.IsValid)
				{
					AppletManifestInfo manifestInfo = new AppletManifestInfo(manifest);
					manifestInfo.FileExtension = fileInfo.Extension;

					this.AmiClient.CreateApplet(manifestInfo);

					TempData["success"] = Locale.AppletUploadedSuccessfully;

					if (model.UploadAnotherFile)
					{
						ModelState.Clear();
						model.File = null;
						return View(model);
					}

					return RedirectToAction("Index");
				}
			}

			TempData["error"] = Locale.UnableToUploadApplet;

			return View(model);
		}
	}
}