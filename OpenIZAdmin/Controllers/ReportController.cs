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
 * Date: 2017-4-17
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using OpenIZ.Messaging.RISI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.ReportModels;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using OpenIZAdmin.Services.Reports;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Represents a report controller.
	/// </summary>
	[TokenAuthorize(Constants.UnrestrictedWarehouse)]
	public class ReportController : Controller
	{
		/// <summary>
		/// The report service.
		/// </summary>
		private readonly IReportService reportService;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportController"/> class.
		/// </summary>
		public ReportController(IReportService reportService)
		{
			this.reportService = reportService;
		}

		/// <summary>
		/// Downloads a report.
		/// </summary>
		/// <param name="id">The id of the report to download.</param>
		/// <returns>Returns a file content results which represents the report source.</returns>
		public ActionResult Download(Guid id)
		{
			try
			{
				var reportSourceStream = this.reportService.DownloadReportSource(id);

				var contentDisposition = new ContentDisposition
				{
					FileName = "Report-" + Guid.NewGuid() + ".xml",
					Inline = false
				};

				this.Response.AppendHeader("Content-Disposition", contentDisposition.ToString());

				return File(reportSourceStream, MediaTypeNames.Text.Xml);
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnableToDownloadReport;
				Trace.TraceError($"Unable to download report: {e}");
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Displays the index view.
		/// </summary>
		/// <returns>Returns the index view.</returns>
		[HttpGet]
		public ActionResult Index()
		{
			var reports = new List<ReportViewModel>();

			try
			{
				reports.AddRange(this.reportService.GetAllReportDefinitions().Select(r => new ReportViewModel(r)));
			}
			catch (Exception e)
			{
				this.TempData["error"] = Locale.UnableToLoadReports;
				Trace.TraceError($"Unable to download report: {e}");
			}

			return View(reports);
		}
	}
}