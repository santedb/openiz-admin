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
using System.Globalization;
using System.Xml.Serialization;

namespace OpenIZAdmin.Core.Auditing.Model
{
	/// <summary>
	/// Represents an error.
	/// </summary>
	[Serializable]
	[XmlRoot(nameof(Error), Namespace = "http://openiz.org/admin/error")]
	[XmlType(nameof(Error), Namespace = "http://openiz.org/admin/error")]
	public class Error
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Error" /> class.
		/// </summary>
		public Error()
		{
			this.Timestamp = DateTimeOffset.Now;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Error"/> class.
		/// </summary>
		/// <param name="exception">The exception.</param>
		public Error(Exception exception) : this()
		{
			this.Message = exception.Message;
			this.StackTrace = exception.StackTrace;
		}

		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>The message.</value>
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the stack trace.
		/// </summary>
		/// <value>The stack trace.</value>
		public string StackTrace { get; set; }

		/// <summary>
		/// Gets or sets the timestamp.
		/// </summary>
		/// <value>The timestamp.</value>
		[XmlIgnore]
		public DateTimeOffset Timestamp { get; set; }

		/// <summary>
		/// Gets or sets the timestamp in XML format.
		/// </summary>
		[XmlElement]
		public string TimestampXml
		{
			get
			{
				return this.Timestamp == default(DateTimeOffset) ? null : this.Timestamp.ToString("o", CultureInfo.InvariantCulture);
			}
			set
			{
				this.Timestamp = value != null ? DateTimeOffset.ParseExact(value, "o", CultureInfo.InvariantCulture) : default(DateTimeOffset);
			}
		}
	}
}