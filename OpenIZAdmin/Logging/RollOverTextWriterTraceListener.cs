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
 * User: Nityan
 * Date: 2016-7-10
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace OpenIZAdmin.Logging
{
	/// <summary>
	/// Timed Trace listener
	/// </summary>
	public class RollOverTextWriterTraceListener : TraceListener
	{
		private Object s_lockObject = new object();

		private string _fileName;
		private DateTime _currentDate;

		/// <summary>
		/// Filename
		/// </summary>
		public String FileName { get { return _fileName; } }

		public RollOverTextWriterTraceListener(string fileName)
		{
			// Pass in the path of the logfile (ie. C:\Logs\MyAppLog.log)
			// The logfile will actually be created with a yyyymmdd format appended to the filename
			_fileName = fileName;

			if (!Path.IsPathRooted(fileName))
			{
				string entryAssemblyLocation = this.GetWebEntryAssembly().Location;
				_fileName = Path.Combine(Path.GetDirectoryName(entryAssemblyLocation), Path.GetFileName(_fileName));
			}
		}

		public override void Write(string value)
		{
			lock (s_lockObject)
			{
				using (FileStream fs = File.Open(this.GenerateFilename(), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
				{
					fs.Seek(0, SeekOrigin.End);

					using (StreamWriter sw = new StreamWriter(fs))
						sw.Write("{0} : {1}", DateTime.Now, value);
				}
			}
		}

		public override void WriteLine(string value)
		{
			lock (s_lockObject)
			{
				using (FileStream fs = File.Open(this.GenerateFilename(), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
				{
					fs.Seek(0, SeekOrigin.End);
					using (StreamWriter sw = new StreamWriter(fs))
						sw.WriteLine("{0} : {1}", DateTime.Now, value);
				}
			}
		}

		private string GenerateFilename()
		{
			_currentDate = System.DateTime.Today;
			return Path.Combine(Path.GetDirectoryName(_fileName), Path.GetFileNameWithoutExtension(_fileName) + "_" +
			   _currentDate.ToString("yyyyMMdd") + Path.GetExtension(_fileName));
		}

		private Assembly GetWebEntryAssembly()
		{
			if (System.Web.HttpContext.Current == null ||
				System.Web.HttpContext.Current.ApplicationInstance == null)
			{
				return null;
			}

			var type = System.Web.HttpContext.Current.ApplicationInstance.GetType();
			while (type != null && type.Namespace == "ASP")
			{
				type = type.BaseType;
			}

			return type == null ? null : type.Assembly;
		}
	}
}