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
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AlertModels;
using OpenIZAdmin.Models.AlertModels.ViewModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing alerts.
	/// </summary>
	[TokenAuthorize]
	public class AlertController : Controller
	{
		/// <summary>
		/// The internal reference to the <see cref="AmiServiceClient"/> instance.
		/// </summary>
		private AmiServiceClient client;

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
			model.ToList = new List<SelectListItem>();

			return PartialView("_CreateAlertPartial", model);
		}

		public ActionResult CreateAlert(CreateAlertModel model)
		{
			if (ModelState.IsValid)
			{
				var alertMessageInfo = AlertUtil.ToAlertMessageInfo(model, Guid.Parse(User.Identity.GetUserId()));

				this.client.CreateAlert(alertMessageInfo);

				return Redirect(Request.Url.AbsoluteUri);
			}

			model.PriorityList = AlertUtil.CreatePrioritySelectList();
			model.ToList = new List<SelectListItem>();

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
			var alert = this.client.GetAlerts(a => a.Key == id).CollectionItem.FirstOrDefault();

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

			this.client.UpdateAlert(alert.Id.ToString(), alert);

			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected override void Dispose(bool disposing)
		{
			this.client?.Dispose();
			base.Dispose(disposing);
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
			var alerts = this.client.GetAlerts(a => a.RcptTo.Any(r => r.UserName == username) || a.From == "SYSTEM");

			models.AddRange(alerts.CollectionItem.Select(a => AlertUtil.ToAlertViewModel(a)));
			return PartialView("_AlertsPartial", models.OrderBy(x => x.Flags).ThenByDescending(a => a.Time));
		}

		/// <summary>
		/// Gets a list of new alerts.
		/// </summary>
		/// <returns>Returns a list of alerts.</returns>
		[HttpGet]
		public ContentResult NewAlerts()
		{
			int count = 0;

			try
			{
				//this.client.CreateAlert(new OpenIZ.Core.Model.AMI.Alerting.AlertMessageInfo
				//{
				//	AlertMessage = new OpenIZ.Core.Alert.Alerting.AlertMessage
				//	{
				//		Body = "Test body",
				//		CreatedBy = new OpenIZ.Core.Model.Security.SecurityUser
				//		{
				//			Key = Guid.Parse(User.Identity.GetUserId())
				//		},
				//		Flags = OpenIZ.Core.Alert.Alerting.AlertMessageFlags.Alert,
				//		From = "Test from",
				//		Subject = "Test subject",
				//		TimeStamp = DateTimeOffset.Now,
				//		To = "Test to"
				//	}
				//});

				var username = User.Identity.GetUserName();
				var results = this.client.GetAlerts(a => a.RcptTo.Any(r => r.UserName == username) || a.From == "SYSTEM");

				count = results.CollectionItem.Count(a => a.AlertMessage.Flags != AlertMessageFlags.Acknowledged);
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to retrieve alerts: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to retrieve alerts: {0}", e.Message);
			}

			return Content(count.ToString());
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var restClient = new RestClientService(Constants.AMI);

			restClient.Accept = "application/xml";
			restClient.Credentials = new AmiCredentials(this.User, HttpContext.Request);

			this.client = new AmiServiceClient(restClient);

			base.OnActionExecuting(filterContext);
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
			try
			{
				var alert = this.client.GetAlerts(a => a.Key == id).CollectionItem.FirstOrDefault();

				if (alert == null)
				{
                    TempData["error"] = Locale.Alert + " " + Locale.NotFound;
					return RedirectToAction("Index", "Home");
				}

				alert.AlertMessage.Flags = OpenIZ.Core.Alert.Alerting.AlertMessageFlags.Acknowledged;

				this.client.UpdateAlert(alert.Id.ToString(), alert);
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to update alert: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to update alert: {0}", e.Message);
			}

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

			try
			{
				var alert = this.client.GetAlerts(a => a.Key == key).CollectionItem.FirstOrDefault();

				if (alert == null)
				{
					return Redirect(Request.UrlReferrer.ToString());
				}

				viewModel = AlertUtil.ToAlertViewModel(alert);
			}
			catch (Exception e)
			{
#if DEBUG
				Trace.TraceError("Unable to find alert: {0}", e.StackTrace);
#endif
				Trace.TraceError("Unable to find alert: {0}", e.Message);

				return Redirect(Request.UrlReferrer.ToString());
			}

			return View(viewModel);
		}
	}
}