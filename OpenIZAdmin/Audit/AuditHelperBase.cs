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
 * Date: 2017-6-25
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using MARC.HI.EHRS.SVC.Auditing.Data;
using Microsoft.AspNet.Identity;
using OpenIZ.Core.Http;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Audit;
using OpenIZAdmin.Models.Domain;
using OpenIZAdmin.Services.Http;

namespace OpenIZAdmin.Audit
{
	/// <summary>
	/// Represents an audit helper.
	/// </summary>
	public abstract class AuditHelperBase
	{
		/// <summary>
		/// The security audit code constant.
		/// </summary>
		private const string SecurityAuditCode = "SecurityAuditCode";

		/// <summary>
		/// The credentials.
		/// </summary>
		private readonly Credentials credentials;

		/// <summary>
		/// Initializes a new instance of the <see cref="AuditHelperBase" /> class.
		/// </summary>
		/// <param name="credentials">The credentials.</param>
		/// <exception cref="System.ArgumentNullException">credentials</exception>
		protected AuditHelperBase(Credentials credentials)
		{
			if (credentials == null)
			{
				throw new ArgumentNullException(nameof(credentials), Locale.ValueCannotBeNull);
			}

			this.credentials = credentials;
		}

		/// <summary>
		/// Audits the generic error.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <param name="exception">The exception.</param>
		public abstract void AuditGenericError(OutcomeIndicator outcomeIndicator, EventTypeCode eventTypeCode, EventIdentifierType eventIdentifierType, Exception exception);

		/// <summary>
		/// Creates the audit code.
		/// </summary>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <returns>Returns the created audit code.</returns>
		private static AuditCode CreateAuditCode(EventTypeCode eventTypeCode)
		{
			var typeCode = typeof(EventTypeCode).GetRuntimeField(eventTypeCode.ToString()).GetCustomAttribute<XmlEnumAttribute>();
			return new AuditCode(typeCode.Name, SecurityAuditCode);
		}

		/// <summary>
		/// Creates the base audit.
		/// </summary>
		/// <param name="actionType">Type of the action.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <returns>Returns the created base audit data.</returns>
		protected virtual AuditData CreateBaseAudit(ActionType actionType, EventTypeCode eventTypeCode, EventIdentifierType eventIdentifierType)
		{
			return new AuditData
			{
				ActionCode = actionType,
				EventTypeCode = CreateAuditCode(eventTypeCode),
				EventIdentifier = eventIdentifierType,
				Timestamp = DateTime.Now
			};
		}

		/// <summary>
		/// Creates the base audit.
		/// </summary>
		/// <param name="actionType">Type of the action.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <returns>Returns the created base audit data.</returns>
		protected virtual AuditData CreateBaseAudit(ActionType actionType, EventTypeCode eventTypeCode, EventIdentifierType eventIdentifierType, OutcomeIndicator outcomeIndicator)
		{
			var audit = CreateBaseAudit(actionType, eventTypeCode, eventIdentifierType);

			audit.Outcome = outcomeIndicator;

			return audit;
		}

		/// <summary>
		/// Gets the device identifier.
		/// </summary>
		/// <returns>Returns the device identifier.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate device identifier</exception>
		protected static string GetDeviceIdentifier()
		{
			var realm = MvcApplication.MemoryCache.Get("Realm") as Realm;

			if (realm == null)
			{
				realm = RealmConfig.GetCurrentRealm();

				MvcApplication.MemoryCache.Set("Realm", realm, MvcApplication.CacheItemPolicy);
			}

			if (string.IsNullOrEmpty(realm.DeviceId) || string.IsNullOrWhiteSpace(realm.DeviceId))
			{
				throw new InvalidOperationException("Unable to locate device identifier");
			}

			return realm.DeviceId;
		}

		/// <summary>
		/// Sends the audit.
		/// </summary>
		/// <param name="audit">The audit.</param>
		protected void SendAudit(AuditData audit)
		{
			this.SendAudit(new List<AuditData> { audit });
		}

		/// <summary>
		/// Sends the audit.
		/// </summary>
		/// <param name="audits">The audits.</param>
		private void SendAudit(List<AuditData> audits)
		{
			try
			{
				using (var client = new AmiServiceClient(new RestClientService(Constants.Ami)))
				{
					client.Client.Credentials = this.credentials;

					var auditInfo = new AuditInfo
					{
						ProcessId = Process.GetCurrentProcess().Id,
						Audit = audits
					};


					client.SubmitAudit(auditInfo);
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Error contacting AMI: {e}");
			}
		}
	}
}