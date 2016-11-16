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
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Contains operations for managing applets.
	/// </summary>
	[TokenAuthorize]
	public class AppletController : Controller
	{
		/// <summary>
		/// The internal reference to the <see cref="AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient client;

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			try
			{
				
			}
			catch (Exception)
			{

				throw;
			}

			List<AppletViewModel> applets = new List<AppletViewModel>
			{
				new AppletViewModel("org.openiz.core", Guid.NewGuid(), "org.openiz.authentication", "0.5.0.0"),
				new AppletViewModel("org.openiz.core", Guid.NewGuid(), "org.openiz.patientAdministration", "0.5.0.0"),
				new AppletViewModel("org.openiz.core", Guid.NewGuid(), "org.openiz.patientEncounters", "0.5.0.0")
			};

			return View(applets);
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var restClient = new RestClientService(Constants.AMI);

			restClient.Accept = "application/xml";
			restClient.Credentials = new AmiCredentials(this.User, HttpContext.Request);

			this.client = new AmiServiceClient(restClient);

			base.OnActionExecuting(filterContext);
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

			TempData["error"] = "Unable to upload applet";

			return View(model);
		}
	}
}