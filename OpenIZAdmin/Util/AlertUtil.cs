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
using OpenIZ.Core.Model.AMI.Alerting;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AlertModels;
using OpenIZAdmin.Models.AlertModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Represents a utility for alerts.
	/// </summary>
	public static class AlertUtil
	{
		/// <summary>
		/// Gets a list of select list items for priorities.
		/// </summary>
		/// <returns>Returns a list of select list items.</returns>
		public static List<SelectListItem> CreatePrioritySelectList()
		{
			var selectList = new List<SelectListItem>();

			// HACK
			selectList.Add(new SelectListItem { Text = Locale.Alert, Value = ((int)AlertMessageFlags.Alert).ToString() });
			selectList.Add(new SelectListItem { Text = Locale.HighPriority, Value = ((int)AlertMessageFlags.HighPriority).ToString() });
			selectList.Add(new SelectListItem { Text = Locale.System, Value = ((int)AlertMessageFlags.System).ToString() });

			return selectList;
		}

		/// <summary>
		/// Converts an alert model to an alert message info.
		/// </summary>
		/// <returns>Returns the converted alert message info.</returns>
		public static AlertMessageInfo ToAlertMessageInfo(AlertViewModel model)
		{
			AlertMessageInfo alertMessageInfo = new AlertMessageInfo();

			alertMessageInfo.AlertMessage = new OpenIZ.Core.Alert.Alerting.AlertMessage();

			alertMessageInfo.AlertMessage.Body = model.Body;
			alertMessageInfo.AlertMessage.CreatedBy = new OpenIZ.Core.Model.Security.SecurityUser
			{
				Key = model.CreatedBy
			};

			alertMessageInfo.AlertMessage.Flags = model.Flags;
			alertMessageInfo.AlertMessage.From = model.From;
			alertMessageInfo.Id = model.Id;
			alertMessageInfo.AlertMessage.Subject = model.Subject;
			alertMessageInfo.AlertMessage.TimeStamp = model.Time;
			alertMessageInfo.AlertMessage.To = model.To;

			return alertMessageInfo;
		}

		/// <summary>
		/// Converts an alert model to an alert message info.
		/// </summary>
		/// <returns>Returns the converted alert message info.</returns>
		public static AlertMessageInfo ToAlertMessageInfo(CreateAlertModel model, IPrincipal user)
		{
			var alertMessageInfo = new AlertMessageInfo
			{
				AlertMessage = new AlertMessage
				{
					Body = model.Message,
					CreatedBy = new SecurityUser
					{
						Key = Guid.Parse(user.Identity.GetUserId()),
					},
					Flags = (AlertMessageFlags)model.Priority
				}
			};

			switch (alertMessageInfo.AlertMessage.Flags)
			{
				case AlertMessageFlags.System:
					alertMessageInfo.AlertMessage.From = "SYSTEM";
					break;

				default:
					alertMessageInfo.AlertMessage.From = user.Identity.GetUserName();
					break;
			}

			alertMessageInfo.AlertMessage.Subject = model.Subject;
			alertMessageInfo.AlertMessage.TimeStamp = DateTimeOffset.Now;
			alertMessageInfo.AlertMessage.To = model.To;

			return alertMessageInfo;
		}

		/// <summary>
		/// Converts an alert message info to an alert view model.
		/// </summary>
		/// <param name="alertMessageInfo"></param>
		/// <returns></returns>
		public static AlertViewModel ToAlertViewModel(AlertMessageInfo alertMessageInfo)
		{
			AlertViewModel viewModel = new AlertViewModel();

			viewModel.Body = alertMessageInfo.AlertMessage.Body;
			viewModel.CreatedBy = alertMessageInfo.AlertMessage.CreatedBy?.Key.Value ?? Guid.Empty;
			viewModel.Flags = alertMessageInfo.AlertMessage.Flags;
			viewModel.From = alertMessageInfo.AlertMessage.From;
			viewModel.Id = alertMessageInfo.Id;
			viewModel.Subject = alertMessageInfo.AlertMessage.Subject;
			viewModel.Time = alertMessageInfo.AlertMessage.CreationTime.DateTime;
			viewModel.To = alertMessageInfo.AlertMessage.To;

			return viewModel;
		}
	}
}