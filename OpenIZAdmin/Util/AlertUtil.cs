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
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.AlertModels;
using OpenIZAdmin.Models.AlertModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using OpenIZ.Messaging.AMI.Client;

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
			var priorities = new List<SelectListItem>
			{
				new SelectListItem
				{
					Text = Locale.Alert, Value = ((int)AlertMessageFlags.Alert).ToString()
				},
				new SelectListItem
				{
					Text = Locale.HighPriority, Value = ((int)AlertMessageFlags.HighPriority).ToString()
				},
				new SelectListItem
				{
					Text = Locale.System, Value = ((int)AlertMessageFlags.System).ToString()
				}
			};

			// HACK

			return priorities;
		}

		/// <summary>
		/// Converts an alert model to an alert message info.
		/// </summary>
		/// <returns>Returns the converted alert message info.</returns>
		public static AlertMessageInfo ToAlertMessageInfo(AlertViewModel model)
		{
			var alertMessageInfo = new AlertMessageInfo
			{
				AlertMessage = new AlertMessage
				{
					Body = model.Body,
					CreatedBy = new SecurityUser
					{
						Key = model.CreatedBy
					},
					Flags = model.Flags,
					From = model.From
				},
				Id = model.Id
			};

			alertMessageInfo.AlertMessage.Subject = model.Subject;
			alertMessageInfo.AlertMessage.TimeStamp = model.Time;
			alertMessageInfo.AlertMessage.To = model.To;

			return alertMessageInfo;
		}

		/// <summary>
		/// Converts an alert model to an alert message info.
		/// </summary>
		/// <param name="client">The <see cref="AmiServiceClient"/> instance.</param>
		/// <param name="model">The create alert model.</param>
		/// <param name="user">The <see cref="IPrincipal"/> instance.</param>
		/// <returns>Returns the converted alert message info.</returns>
		public static AlertMessageInfo ToAlertMessageInfo(AmiServiceClient client, CreateAlertModel model, IPrincipal user)
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

			var securityUser = client.GetUser(model.To).User;

			alertMessageInfo.AlertMessage.RcptTo = new List<SecurityUser>
			{
				securityUser
			};

			alertMessageInfo.AlertMessage.To = securityUser.UserName;

			return alertMessageInfo;
		}

		/// <summary>
		/// Converts an alert message info to an alert view model.
		/// </summary>
		/// <param name="alertMessageInfo"></param>
		/// <returns></returns>
		public static AlertViewModel ToAlertViewModel(AlertMessageInfo alertMessageInfo)
		{
			var viewModel = new AlertViewModel
			{
				Body = alertMessageInfo.AlertMessage.Body,
				CreatedBy = alertMessageInfo.AlertMessage.CreatedBy?.Key.Value ?? Guid.Empty,
				Flags = alertMessageInfo.AlertMessage.Flags,
				From = alertMessageInfo.AlertMessage.From,
				Id = alertMessageInfo.Id,
				Subject = alertMessageInfo.AlertMessage.Subject,
				Time = alertMessageInfo.AlertMessage.CreationTime.DateTime,
				To = alertMessageInfo.AlertMessage.To
			};

			return viewModel;
		}
	}
}