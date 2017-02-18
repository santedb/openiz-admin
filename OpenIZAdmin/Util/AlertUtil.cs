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
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Models.AlertModels;
using System;
using System.Collections.Generic;
using System.Security.Principal;

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
	}
}