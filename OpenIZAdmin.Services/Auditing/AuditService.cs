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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZAdmin.Core.Auditing.Services;
using OpenIZAdmin.Services.Core;

namespace OpenIZAdmin.Services.Auditing
{
	/// <summary>
	/// Represents an auditor service.
	/// </summary>
	/// <seealso cref="IAuditService" />
	/// <seealso cref="OpenIZAdmin.Services.Core.AmiServiceBase" />
	public class AuditService : AmiServiceBase, IAuditService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AuditService"/> class.
		/// </summary>
		/// <param name="client">The client.</param>
		public AuditService(AmiServiceClient client) : base(client)
		{
		}

		/// <summary>
		/// Sends the audit.
		/// </summary>
		/// <param name="audit">The audit.</param>
		public void SendAudit(AuditData audit)
		{
			this.SendAudits(new List<AuditData>
			{
				audit
			});
		}

		/// <summary>
		/// Sends the audits.
		/// </summary>
		/// <param name="audits">The audits.</param>
		public void SendAudits(List<AuditData> audits)
		{
			try
			{
				ThreadPool.QueueUserWorkItem(o =>
				{
					var auditInfo = new AuditInfo
					{
						ProcessId = Process.GetCurrentProcess().Id,
						Audit = audits
					};

					this.Client.SubmitAudit(auditInfo);
				});
			}
			catch (Exception e)
			{
				Trace.TraceError($"Unable to send audit: {e}");
			}
		}
	}
}
