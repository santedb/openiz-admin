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
 * User: Nityan
 * Date: 2017-7-10
 */

using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZAdmin.Core.Auditing.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;

namespace OpenIZAdmin.Core.Auditing.Core
{
	/// <summary>
	/// Represents an HTTP context audit helper.
	/// </summary>
	/// <seealso cref="CoreAuditServiceBase" />
	public class HttpContextAuditService : CoreAuditServiceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HttpContextAuditService" /> class.
		/// </summary>
		public HttpContextAuditService()
		{
			this.Context = HttpContext.Current;
		}

		/// <summary>
		/// True if the HTTP request message is sensitive
		/// </summary>
		protected bool IsRequestSensitive { get; set; }

		/// <summary>
		/// Audits the generic error.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <param name="exception">The exception.</param>
		public override void AuditGenericError(OutcomeIndicator outcomeIndicator, AuditCode eventTypeCode, EventIdentifierType eventIdentifierType, Exception exception)
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

			AuditService.SendAudit(audit);
		}

		/// <summary>
		/// Audits the generic error.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <param name="exception">The exception.</param>
		public override void AuditGenericError(OutcomeIndicator outcomeIndicator, EventTypeCode eventTypeCode, EventIdentifierType eventIdentifierType, Exception exception)
		{
			this.AuditGenericError(outcomeIndicator, CreateAuditCode(eventTypeCode), eventIdentifierType, exception);
		}

		/// <summary>
		/// Creates the base audit.
		/// </summary>
		/// <param name="actionType">Type of the action.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <returns>Returns the created base audit data.</returns>
		protected override AuditData CreateBaseAudit(ActionType actionType, AuditCode eventTypeCode, EventIdentifierType eventIdentifierType, OutcomeIndicator outcomeIndicator)
		{
			var audit = base.CreateBaseAudit(actionType, eventTypeCode, eventIdentifierType, outcomeIndicator);

			var remoteIp = GetLocalIPAddress();

			try
			{
				// attempt to get the remote IP address
				remoteIp = this.Context.Request.ServerVariables["REMOTE_ADDR"];
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve remote IP address for auditing purposes: {e}");
			}

			var userIdentifier = string.Empty;

			try
			{
				userIdentifier = this.Context.Request.Url.Host;
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to retrieve request host URL for auditing purposes: {e}");
			}

			// add the receiver
			audit.Actors.Add(new AuditActorData
			{
				UserName = Environment.UserName,
				UserIdentifier = userIdentifier,
				NetworkAccessPointId = Dns.GetHostName(),
				NetworkAccessPointType = NetworkAccessPointType.MachineName,
				AlternativeUserId = Process.GetCurrentProcess().Id.ToString(),
				ActorRoleCode = new List<AuditCode>
				{
					new AuditCode("110152", "DCM")
				}
			});

			// add the sender
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

			// add the user if this is an authenticated request
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
				// add the anonymous actor if the request isn't authenticated
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

			try
			{
				if (outcomeIndicator != OutcomeIndicator.Success)
				{
					// add the object detail
					using (var memoryStream = new MemoryStream())
					{
						var detail = new ObjectDataExtension
						{
							Key = "HTTPMessage"
						};

						using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
						{
							streamWriter.WriteLine("<?xml version=\"1.0\"?><Request><![CDATA[");

							streamWriter.WriteLine("{0} {1} HTTP/1.1", this.Context.Request.HttpMethod, this.Context.Request.Url);

							for (var i = 0; i < this.Context.Request.Headers.Keys.Count; i++)
							{
								streamWriter.WriteLine("{0} : {1}", this.Context.Request.Headers.Keys[i], this.Context.Request.Headers[i]);
							}

							// Only output if request is not sensitive
							if (!this.IsRequestSensitive)
							{
								using (var sr = new StreamReader(this.Context.Request.InputStream))
								{
									streamWriter.WriteLine("\r\n{0}", sr.ReadToEnd());
								}
							}
							else
								streamWriter.WriteLine("*********** SENSITIVE REQUEST REDACTED ***********");

							streamWriter.WriteLine("]]></Request>");
							streamWriter.Flush();

							detail.Value = memoryStream.GetBuffer().Take((int)memoryStream.Length).ToArray();
						}

						var auditableObject = new AuditableObject
						{
							IDTypeCode = AuditableObjectIdType.Uri,
							ObjectId = this.Context.Request.Url.ToString(),
							Role = AuditableObjectRole.Query,
							Type = AuditableObjectType.SystemObject
						};

						auditableObject.ObjectData.Add(detail);

						audit.AuditableObjects.Add(auditableObject);
					}
				}
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to add object detail to audit message: {e}");
			}

			return audit;
		}

		// courtesy of https://stackoverflow.com/questions/6803073/get-local-ip-address
		/// <summary>
		/// Gets the local ip address.
		/// </summary>
		/// <returns>Returns the IP address as a string instance.</returns>
		private static string GetLocalIPAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());

			return (from ip in host.AddressList where ip.AddressFamily == AddressFamily.InterNetwork select ip.ToString()).FirstOrDefault();
		}

		/// <summary>
		/// Converts an <see cref="object" /> to a <see cref="byte" /> array instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns>Returns the converted <see cref="byte" /> array instance.</returns>
		private static byte[] ObjectToByteArray(object instance)
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