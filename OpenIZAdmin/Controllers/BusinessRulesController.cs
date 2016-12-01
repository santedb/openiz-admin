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
 * User: khannan
 * Date: 2016-11-29
 */
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Model.AMI.Applet;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AppletModels;
using OpenIZAdmin.Models.BusinessRuleModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections;
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
	public class BusinessRulesController : BaseController
	{
		/// <summary>
		/// Downloads an applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to download.</param>
		/// <returns>Returns the applet.</returns>
		[HttpGet]
		public ActionResult Download(string appletId)
		{
			var applet = this.AmiClient.GetApplet(appletId);

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
            List<BusinessRuleViewModel> model = new List<BusinessRuleViewModel>();
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


			return View();
		}

		/// <summary>
		/// Views an applet.
		/// </summary>
		/// <param name="id">The applet identifier.</param>
		/// <returns>Returns the applet view.</returns>
		[HttpGet]
		public ActionResult ViewApplet(Guid id)
		{
			return View();
		}
	}
}