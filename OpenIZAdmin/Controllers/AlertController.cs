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
 * Date: 2016-11-14
 */

using Microsoft.AspNet.Identity;
using OpenIZ.Core.Alert.Alerting;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AlertModels;
using OpenIZAdmin.Models.AlertModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing alerts.
	/// </summary>
	[TokenAuthorize]
	public class AlertController : BaseController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AlertController"/> class.
		/// </summary>
		public AlertController()
		{
		}

		[HttpGet]
		public ActionResult CreateAlert()
		{
			CreateAlertModel model = new CreateAlertModel();

			model.PriorityList = AlertUtil.CreatePrioritySelectList();

			return PartialView("_CreateAlertPartial", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateAlert(CreateAlertModel model)
		{
			if (ModelState.IsValid)
			{
				var systemUser = UserUtil.GetSystemUser(this.AmiClient);

				var alertMessageInfo = AlertUtil.ToAlertMessageInfo(model, User);

				this.AmiClient.CreateAlert(alertMessageInfo);

				return Redirect(Request.Url.AbsoluteUri);
			}

			model.PriorityList = AlertUtil.CreatePrioritySelectList();

			return PartialView("_CreateAlertPartial", model);
		}

		/// <summary>
		/// Deletes an alert.
		/// </summary>
		/// <param name="id">The id of the alert to be deleted.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteAlert(Guid id)
		{
			var alert = this.AmiClient.GetAlerts(a => a.Key == id).CollectionItem.FirstOrDefault();

			if (alert == null)
			{
				TempData["error"] = Locale.Alert + " " + Locale.NotFound;
				return View("_NotFound", model: "Not Found");
			}

			alert.AlertMessage.ObsoletionTime = DateTimeOffset.Now;
			alert.AlertMessage.ObsoletedBy = new OpenIZ.Core.Model.Security.SecurityUser
			{
				Key = Guid.Parse(User.Identity.GetUserId())
			};

			this.AmiClient.UpdateAlert(alert.Id.ToString(), alert);

			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// Gets a list of alerts.
		/// </summary>
		/// <returns>Returns a list of alerts.</returns>
		[HttpGet]
		public ActionResult GetAlerts()
		{
			List<AlertViewModel> models = new List<AlertViewModel>();
			var username = User.Identity.GetUserName();
			var alerts = this.AmiClient.GetAlerts(a => a.From == "SYSTEM");

			models.AddRange(alerts.CollectionItem.Where(a => a.AlertMessage.Flags != AlertMessageFlags.Acknowledged).Select(a => AlertUtil.ToAlertViewModel(a)));

			return PartialView("_AlertsPartial", models.OrderBy(x => x.Flags).ThenByDescending(a => a.Time));
		}

		/// <summary>
		/// Gets a list of new alerts.
		/// </summary>
		/// <returns>Returns a list of alerts.</returns>
		[HttpGet]
		public ContentResult NewAlerts()
		{
			var results = this.AmiClient.GetAlerts(a => a.From == "SYSTEM");

			int count = results.CollectionItem.Count(a => a.AlertMessage.Flags != AlertMessageFlags.Acknowledged);

			return Content(count.ToString());
		}

		/// <summary>
		/// Marks an alert as read.
		/// </summary>
		/// <param name="id">The id of the alert to be marked as read.</param>
		/// <returns>Returns the list of alerts.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ReadAlert(Guid id)
		{
			var alert = this.AmiClient.GetAlerts(a => a.Key == id).CollectionItem.FirstOrDefault();

			if (alert == null)
			{
				TempData["error"] = Locale.Alert + " " + Locale.NotFound;
				return RedirectToAction("Index", "Home");
			}

			alert.AlertMessage.Flags = AlertMessageFlags.Acknowledged;

			this.AmiClient.UpdateAlert(alert.Id.ToString(), alert);

			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// Displays the alert view.
		/// </summary>
		/// <param name="id">The id of the alert to be viewed.</param>
		/// <returns>Returns the alert view.</returns>
		[HttpGet]
		public ActionResult ViewAlert(string id)
		{
			Guid key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				return Redirect(Request.UrlReferrer.ToString());
			}

			AlertViewModel viewModel = new AlertViewModel();

			var alert = this.AmiClient.GetAlerts(a => a.Key == key).CollectionItem.FirstOrDefault();

			if (alert == null)
			{
				return Redirect(Request.UrlReferrer.ToString());
			}

			viewModel = AlertUtil.ToAlertViewModel(alert);

			return View(viewModel);
		}
	}
}