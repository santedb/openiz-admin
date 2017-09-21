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

using Microsoft.AspNet.Identity;
using OpenIZ.Core.Alert.Alerting;
using OpenIZ.Core.Model.AMI.Alerting;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AlertModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
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
			return View(new CreateAlertModel());
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
				if (model.IsSystemAlert)
				{
					model.To = Constants.SystemUserId;
					model.Priority = (int)AlertMessageFlags.System;
				}

				var alertMessageInfo = this.ToAlertMessageInfo(model, User);

				this.AmiClient.CreateAlert(alertMessageInfo);

				this.TempData["success"] = Locale.AlertCreatedSuccessfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to create alert: {e}");
			}

			model.ToList = new List<SelectListItem>();

			this.TempData["error"] = Locale.UnableToCreateAlert;

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
					TempData["error"] = Locale.AlertNotFound;
					return RedirectToAction("Index");
				}

				alert.AlertMessage.Flags = AlertMessageFlags.Acknowledged;
				alert.AlertMessage.ObsoletionTime = DateTimeOffset.Now;
				alert.AlertMessage.ObsoletedBy = new OpenIZ.Core.Model.Security.SecurityUser
				{
					Key = Guid.Parse(User.Identity.GetUserId())
				};

				this.AmiClient.UpdateAlert(alert.Id.ToString(), alert);

				TempData["success"] = Locale.AlertUpdatedSuccessfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to delete alert: {e}");
			}

			TempData["error"] = Locale.UnableToUpdateAlert;

			return RedirectToAction("Index");
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
				models.AddRange(this.GetAlerts(true).Select(a => new AlertViewModel(a)));

				return View(models.OrderByDescending(x => x.Time).ThenBy(a => a.Flags));
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve alerts: {e}");
			}

			TempData["error"] = Locale.UnableToRetrieveAlerts;

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
				count = this.GetAlerts().Count(a => a.AlertMessage.Flags != AlertMessageFlags.Acknowledged && a.AlertMessage.ObsoletionTime == null);

				return Content(count.ToString());
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve alerts: {e}");
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
					TempData["error"] = Locale.AlertNotFound;
					return RedirectToAction("Index", "Home");
				}

				alert.AlertMessage.Flags = AlertMessageFlags.Acknowledged;

				this.AmiClient.UpdateAlert(alert.Id.ToString(), alert);

				TempData["success"] = Locale.AlertUpdatedSuccessfully;

				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to read alert: {e}");
			}

			TempData["error"] = Locale.UnableToUpdateAlert;

			return RedirectToAction("Index");
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
					TempData["error"] = Locale.AlertNotFound;

					return RedirectToAction("Index");
				}

				var viewModel = new AlertViewModel(alert);

				return View(viewModel);
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to view alert: {e}");
			}

			TempData["error"] = Locale.AlertNotFound;

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Gets the alerts.
		/// </summary>
		/// <param name="all">if set to <c>true</c> [all].</param>
		/// <returns>Returns a list of alerts for the current user, including alerts marked for "everyone".</returns>
		private IEnumerable<AlertMessageInfo> GetAlerts(bool all = false)
		{
			var username = this.User.Identity.GetUserName();

			var alerts = this.AmiClient.GetAlerts(a => a.To.Contains("everyone")).CollectionItem;
			var userAlerts = this.AmiClient.GetAlerts(a => a.To == username).CollectionItem;

			if (!all)
			{
				alerts = alerts.Where(a => a.AlertMessage.ObsoletionTime == null && a.AlertMessage.Flags != AlertMessageFlags.Acknowledged).ToList();
				userAlerts = userAlerts.Where(a => a.AlertMessage.ObsoletionTime == null && a.AlertMessage.Flags != AlertMessageFlags.Acknowledged).ToList();
			}

			return alerts.Union(userAlerts).Where(a => a.AlertMessage.ObsoletionTime == null).ToList();
		}

		/// <summary>
		/// Converts an alert model to an alert message info.
		/// </summary>
		/// <param name="model">The create alert model.</param>
		/// <param name="user">The <see cref="IPrincipal"/> instance.</param>
		/// <returns>Returns the converted alert message info.</returns>
		private AlertMessageInfo ToAlertMessageInfo(CreateAlertModel model, IPrincipal user)
		{
			var alertMessageInfo = new AlertMessageInfo
			{
				AlertMessage = new AlertMessage
				{
					Body = model.Message,
					CreatedBy = new SecurityUser
					{
						Key = Guid.Parse(user.Identity.GetUserId())
					},
					Flags = (AlertMessageFlags)model.Priority
				}
			};

			alertMessageInfo.AlertMessage.From = user.Identity.GetUserName();

			var securityUser = this.AmiClient.GetUser(model.To).User;

			alertMessageInfo.AlertMessage.RcptTo = new List<SecurityUser>
			{
				securityUser
			};

			switch (alertMessageInfo.AlertMessage.Flags)
			{
				case AlertMessageFlags.System:
					alertMessageInfo.AlertMessage.To = "everyone";
					break;

				default:
					alertMessageInfo.AlertMessage.To = securityUser.UserName;
					break;
			}

			alertMessageInfo.AlertMessage.Subject = model.Subject;
			alertMessageInfo.AlertMessage.TimeStamp = DateTimeOffset.Now;

			return alertMessageInfo;
		}
	}
}