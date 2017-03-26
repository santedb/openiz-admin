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
 * Date: 2017-3-13
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using Quartz;
using Quartz.Impl;

namespace OpenIZAdmin
{
	/// <summary>
	/// Represents a quartz scheduling configuration.
	/// </summary>
	public static class QuartzConfig
	{
		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public static void Initialize()
		{
			Trace.TraceInformation("Initializing Quartz configuration");

			var scheduler = StdSchedulerFactory.GetDefaultScheduler();

			scheduler.Start();

			var types = typeof(MvcApplication).Assembly.ExportedTypes.Where(t => t.Namespace == "OpenIZAdmin.Scheduler" && !t.IsAbstract);

			foreach (var type in types)
			{
				Trace.TraceInformation($"Adding type: { type } to Quartz scheduler");

				var jobDetail = JobBuilder.Create(type).Build();

				var trigger = TriggerBuilder.Create().StartNow().WithSimpleSchedule(x => x.WithIntervalInMinutes(10).RepeatForever()).Build();

				scheduler.ScheduleJob(jobDetail, trigger);
			}
		}
	}
}