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
 * Date: 2017-3-19
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;

namespace OpenIZAdmin.Logging
{
	/// <summary>
	/// Represents a roll over log trace listener.
	/// </summary>
	/// <seealso cref="System.Diagnostics.TraceListener" />
	public class RollOverTextWriterTraceListener : TraceListener
	{
		/// <summary>
		/// The lock object.
		/// </summary>
		private readonly object lockObject = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="RollOverTextWriterTraceListener"/> class.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		public RollOverTextWriterTraceListener(string fileName)
		{
			this.FileName = fileName;

			if (Path.IsPathRooted(fileName))
			{
				this.FileName = fileName;
			}
			else
			{
				this.FileName = Path.Combine(Path.GetDirectoryName(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, HttpRuntime.BinDirectory)), Path.GetFileName(this.FileName));
			}
		}

		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		public string FileName { get; }

		/// <summary>
		/// Generates the filename.
		/// </summary>
		/// <returns>System.String.</returns>
		private string GenerateFilename()
		{
			return Path.Combine(Path.GetDirectoryName(this.FileName), Path.GetFileNameWithoutExtension(this.FileName) + "_" + DateTime.Today.ToString("yyyyMMdd") + Path.GetExtension(this.FileName));
		}

		/// <summary>
		/// Gets the web entry assembly.
		/// </summary>
		/// <returns>Assembly.</returns>
		private Assembly GetWebEntryAssembly()
		{
			if (HttpContext.Current == null || HttpContext.Current.ApplicationInstance == null)
			{
				return null;
			}

			var type = HttpContext.Current.ApplicationInstance.GetType();

			while (type != null && type.Namespace == "ASP")
			{
				type = type.BaseType;
			}

			return type?.Assembly;
		}

		/// <summary>
		/// Writes the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public override void Write(string value)
		{
			lock (this.lockObject)
			{
				using (var fs = File.Open(GenerateFilename(), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
				{
					fs.Seek(0, SeekOrigin.End);

					using (StreamWriter sw = new StreamWriter(fs))
						sw.Write("{0} : {1}", DateTime.Now, value);
				}
			}
		}

		/// <summary>
		/// Writes the line.
		/// </summary>
		/// <param name="value">The value.</param>
		public override void WriteLine(string value)
		{
			lock (this.lockObject)
			{
				using (FileStream fs = File.Open(GenerateFilename(), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
				{
					fs.Seek(0, SeekOrigin.End);
					using (StreamWriter sw = new StreamWriter(fs))
						sw.WriteLine("{0} : {1}", DateTime.Now, value);
				}
			}
		}
	}
}