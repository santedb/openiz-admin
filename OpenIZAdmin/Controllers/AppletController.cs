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

using OpenIZAdmin.Attributes;
using OpenIZAdmin.Models.AppletModels;
using OpenIZAdmin.Models.AppletModels.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	[TokenAuthorize]
	public class AppletController : Controller
	{
		[HttpGet]
		public ActionResult Index()
		{
			List<AppletViewModel> applets = new List<AppletViewModel>
			{
				new AppletViewModel("org.openiz.core", Guid.NewGuid(), "org.openiz.authentication", "0.5.0.0"),
				new AppletViewModel("org.openiz.core", Guid.NewGuid(), "org.openiz.patientAdministration", "0.5.0.0"),
				new AppletViewModel("org.openiz.core", Guid.NewGuid(), "org.openiz.patientEncounters", "0.5.0.0")
			};

			return View(applets);
		}

		[HttpGet]
		public ActionResult Upload()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Upload(UploadAppletModel model)
		{
			if (ModelState.IsValid)
			{
				string pathToSave = Server.MapPath("~/Applets/");
				string filename = Path.GetFileName(Request.Files[0].FileName);

				Request.Files[0].SaveAs(Path.Combine(pathToSave, Guid.NewGuid().ToString() + "." + filename));

				TempData["success"] = string.Format("Applet {0} uploaded successfully", filename);

				if (model.UploadAnotherFile)
				{
					ModelState.Clear();
					model.File = null;
					return View(model);
				}

				return RedirectToAction("Index");
			}

			TempData["success"] = "Unable to upload applet";

			return View(model);
		}
	}
}