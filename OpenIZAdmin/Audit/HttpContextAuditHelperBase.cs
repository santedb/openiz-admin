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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Http;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.Audit;

namespace OpenIZAdmin.Audit
{
	/// <summary>
	/// Represents an HTTP context audit helper.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Audit.AuditHelperBase" />
	public abstract class HttpContextAuditHelperBase : AuditHelperBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HttpContextAuditHelperBase" /> class.
		/// </summary>
		/// <param name="credentials">The credentials.</param>
		/// <param name="context">The context.</param>
		/// <exception cref="System.ArgumentNullException">context</exception>
		protected HttpContextAuditHelperBase(Credentials credentials, HttpContext context) : base(credentials)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context), Locale.ValueCannotBeNull);
			}

			this.Context = context;
		}

		/// <summary>
		/// Gets the context.
		/// </summary>
		/// <value>The context.</value>
		protected HttpContext Context { get; }

		/// <summary>
		/// Audits the generic error.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <param name="exception">The exception.</param>
		public override void AuditGenericError(OutcomeIndicator outcomeIndicator, EventTypeCode eventTypeCode, EventIdentifierType eventIdentifierType, Exception exception)
		{
			var audit = this.CreateBaseAudit(ActionType.Execute, eventTypeCode, eventIdentifierType, outcomeIndicator);

			audit.AuditableObjects.Add(new AuditableObject
			{
				IDTypeCode = AuditableObjectIdType.Uri,
				ObjectId = this.Context.Request.Url.ToString(),
				Role = AuditableObjectRole.Resource,
				Type = AuditableObjectType.SystemObject
			});

			if (exception != null)
			{
				var auditableObject = new AuditableObject
				{
					IDTypeCode = AuditableObjectIdType.Custom,
					ObjectId = exception.GetHashCode().ToString(),
					Role = AuditableObjectRole.Resource,
					Type = AuditableObjectType.Other
				};

				auditableObject.ObjectData.Add(new ObjectDataExtension(exception.GetType().Name, ObjectToByteArray(new Error(exception))));

				audit.AuditableObjects.Add(auditableObject);
			}

			this.SendAudit(audit);
		}

		/// <summary>
		/// Creates the base audit.
		/// </summary>
		/// <param name="actionType">Type of the action.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <returns>Returns the created base audit data.</returns>
		protected override AuditData CreateBaseAudit(ActionType actionType, EventTypeCode eventTypeCode, EventIdentifierType eventIdentifierType, OutcomeIndicator outcomeIndicator)
		{
			var audit = base.CreateBaseAudit(actionType, eventTypeCode, eventIdentifierType, outcomeIndicator);

			var remoteIp = this.Context.Request.ServerVariables["REMOTE_ADDR"];

			audit.Actors.Add(new AuditActorData
			{
				UserIdentifier = remoteIp,
				NetworkAccessPointId = remoteIp,
				NetworkAccessPointType = NetworkAccessPointType.IPAddress,
				ActorRoleCode = new List<AuditCode>
				{
					new AuditCode("110153", "DCM")
				},
				UserIsRequestor = true
			});

			if (this.Context.User?.Identity?.IsAuthenticated == true)
			{
				audit.Actors.Add(new AuditActorData
				{
					UserIdentifier = this.Context.User.Identity.Name,
					UserIsRequestor = true,
					NetworkAccessPointId = remoteIp,
					NetworkAccessPointType = NetworkAccessPointType.IPAddress,
					ActorRoleCode = new List<AuditCode>
					{
						new AuditCode("6", "AuditableObjectRole")
					}
				});
			}
			else
			{
				audit.Actors.Add(new AuditActorData
				{
					UserIdentifier = "Anonymous",
					UserIsRequestor = true,
					NetworkAccessPointId = remoteIp,
					NetworkAccessPointType = NetworkAccessPointType.IPAddress,
					ActorRoleCode = new List<AuditCode>
					{
						new AuditCode("6", "AuditableObjectRole")
					}
				});
			}

			return audit;
		}

		/// <summary>
		/// Converts an <see cref="object" /> to a <see cref="byte" /> array instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns>Returns the converted <see cref="byte" /> array instance.</returns>
		public static byte[] ObjectToByteArray(object instance)
		{
			var formatter = new BinaryFormatter();

			using (var memoryStream = new MemoryStream())
			{
				formatter.Serialize(memoryStream, instance);

				return memoryStream.ToArray();
			}
		}
	}
}