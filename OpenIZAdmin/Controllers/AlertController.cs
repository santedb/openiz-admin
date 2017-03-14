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
 * Date: 2016-11-14
 */

using Elmah;
using Microsoft.AspNet.Identity;
using OpenIZ.Core.Alert.Alerting;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AlertModels;
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

		/// <summary>
		/// Displays the create alert view.
		/// </summary>
		/// <returns>Returns the create alert view.</returns>
		[HttpGet]
		public ActionResult Create()
		{
			var model = new CreateAlertModel();

			return View(model);
		}

		/// <summary>
		/// Creates an alert.
		/// </summary>
		/// <param name="model">The model containing the create alert information.</param>
		/// <returns>Returns the create alert view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateAlertModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var alertMessageInfo = AlertUtil.ToAlertMessageInfo(this.AmiClient, model, User);

					this.AmiClient.CreateAlert(alertMessageInfo);

					TempData["success"] = Locale.Alert + " " + Locale.Created + " " + Locale.Successfully;

					return RedirectToAction("Index", "Home");
				}
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToCreate + " " + Locale.Alert;

			return View(model);
		}

		/// <summary>
		/// Deletes an alert.
		/// </summary>
		/// <param name="id">The id of the alert to be deleted.</param>
		/// <returns>Returns the index view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Guid id)
		{
			try
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

				TempData["success"] = Locale.Alert + " " + Locale.Updated + " " + Locale.Successfully;

				return RedirectToAction("Index", "Home");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Alert;

			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// Gets a list of alerts.
		/// </summary>
		/// <returns>Returns an <see cref="ActionResult"/> instance.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			var models = new List<AlertViewModel>();

			try
			{
				var username = User.Identity.GetUserName();

				var alerts = this.AmiClient.GetAlerts(a => a.To == username && a.ObsoletionTime == null);

				models.AddRange(alerts.CollectionItem.Where(a => a.AlertMessage.Flags != AlertMessageFlags.Acknowledged && a.AlertMessage.ObsoletionTime == null).Select(a => new AlertViewModel(a)));

				return View(models.OrderBy(x => x.Flags).ThenByDescending(a => a.Time));
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToRetrieve + " " + Locale.Alerts;

			return View(models);
		}

		/// <summary>
		/// Gets a list of new alerts.
		/// </summary>
		/// <returns>Returns a list of alerts.</returns>
		[HttpGet]
		public ContentResult NewAlerts()
		{
			var count = 0;

			try
			{
				var username = User.Identity.GetUserName();
				var results = this.AmiClient.GetAlerts(a => a.To == username && a.ObsoletionTime == null);

				count = results.CollectionItem.Count(a => a.AlertMessage.Flags != AlertMessageFlags.Acknowledged && a.AlertMessage.ObsoletionTime == null);

				return Content(count.ToString());
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			return Content(count.ToString());
		}

		/// <summary>
		/// Marks an alert as read.
		/// </summary>
		/// <param name="id">The id of the alert to be marked as read.</param>
		/// <returns>Returns the list of alerts.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Read(Guid id)
		{
			try
			{
				var alert = this.AmiClient.GetAlerts(a => a.Key == id).CollectionItem.FirstOrDefault();

				if (alert == null)
				{
					TempData["error"] = Locale.Alert + " " + Locale.NotFound;
					return RedirectToAction("Index", "Home");
				}

				alert.AlertMessage.Flags = AlertMessageFlags.Acknowledged;

				this.AmiClient.UpdateAlert(alert.Id.ToString(), alert);

				TempData["success"] = Locale.Alert + " " + Locale.Updated + " " + Locale.Successfully;

				return RedirectToAction("Index", "Home");
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.UnableToUpdate + " " + Locale.Alert;

			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// Displays the alert view.
		/// </summary>
		/// <param name="id">The id of the alert to be viewed.</param>
		/// <returns>Returns the alert view.</returns>
		[HttpGet]
		public ActionResult ViewAlert(Guid id)
		{
			try
			{
				var alert = this.AmiClient.GetAlerts(a => a.Key == id).CollectionItem.FirstOrDefault();

				if (alert == null)
				{
					TempData["error"] = Locale.Alert + " " + Locale.NotFound;

					return RedirectToAction("Index");
				}

				var viewModel = new AlertViewModel(alert);

				return View(viewModel);
			}
			catch (Exception e)
			{
				ErrorLog.GetDefault(HttpContext.ApplicationInstance.Context).Log(new Error(e, HttpContext.ApplicationInstance.Context));
			}

			TempData["error"] = Locale.Alert + " " + Locale.NotFound;

			return RedirectToAction("Index");
		}
	}
}