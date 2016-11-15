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
using System;
using System.Collections.Generic;
using OpenIZ.Core.Model.AMI.Alerting;
using System.Linq;
using System.Web;
using OpenIZAdmin.Models.AlertModels.ViewModels;
using OpenIZAdmin.Models.AlertModels;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Represents a utility for alerts.
	/// </summary>
	public static class AlertUtil
	{
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

			switch (model.Flags)
			{
				case AlertMessageFlags.Alert:
					alertMessageInfo.AlertMessage.Flags = OpenIZ.Core.Alert.Alerting.AlertMessageFlags.Alert;
					break;
				case AlertMessageFlags.HighPriority:
					alertMessageInfo.AlertMessage.Flags = OpenIZ.Core.Alert.Alerting.AlertMessageFlags.HighPriority;
					break;
				case AlertMessageFlags.System:
					alertMessageInfo.AlertMessage.Flags = OpenIZ.Core.Alert.Alerting.AlertMessageFlags.System;
					break;
				case AlertMessageFlags.Acknowledged:
					alertMessageInfo.AlertMessage.Flags = OpenIZ.Core.Alert.Alerting.AlertMessageFlags.Acknowledged;
					break;
				case AlertMessageFlags.Transient:
					alertMessageInfo.AlertMessage.Flags = OpenIZ.Core.Alert.Alerting.AlertMessageFlags.Transient;
					break;
				case AlertMessageFlags.None:
					alertMessageInfo.AlertMessage.Flags = OpenIZ.Core.Alert.Alerting.AlertMessageFlags.None;
					break;
				default:
					break;
			}

			alertMessageInfo.AlertMessage.From = model.From;
			alertMessageInfo.AlertMessage.Subject = model.Subject;
			alertMessageInfo.AlertMessage.TimeStamp = model.Time;
			alertMessageInfo.AlertMessage.To = model.To;

			alertMessageInfo.Id = model.Id;

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

			switch (alertMessageInfo.AlertMessage.Flags)
			{
				case OpenIZ.Core.Alert.Alerting.AlertMessageFlags.None:
					viewModel.Flags = AlertMessageFlags.None;
					break;
				case OpenIZ.Core.Alert.Alerting.AlertMessageFlags.Alert:
					viewModel.Flags = AlertMessageFlags.Alert;
					break;
				case OpenIZ.Core.Alert.Alerting.AlertMessageFlags.Acknowledged:
					viewModel.Flags = AlertMessageFlags.Acknowledged;
					break;
				case OpenIZ.Core.Alert.Alerting.AlertMessageFlags.HighPriority:
					viewModel.Flags = AlertMessageFlags.HighPriority;
					break;
				case OpenIZ.Core.Alert.Alerting.AlertMessageFlags.System:
					viewModel.Flags = AlertMessageFlags.System;
					break;
				case OpenIZ.Core.Alert.Alerting.AlertMessageFlags.Transient:
					viewModel.Flags = AlertMessageFlags.Transient;
					break;
				case OpenIZ.Core.Alert.Alerting.AlertMessageFlags.HighPriorityAlert:
					viewModel.Flags = AlertMessageFlags.HighPriorityAlert;
					break;
				default:
					break;
			}

			viewModel.From = alertMessageInfo.AlertMessage.From;
			viewModel.Id = alertMessageInfo.Id;
			viewModel.Subject = alertMessageInfo.AlertMessage.Subject;
			viewModel.Time = alertMessageInfo.AlertMessage.CreationTime.DateTime;
			viewModel.To = alertMessageInfo.AlertMessage.To;

			return viewModel;
		}
	}
}